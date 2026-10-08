using System.Runtime.CompilerServices;
using UnityEngine;

namespace BoneEngine
{
    /// <summary>Per-ship memory in the running game only: never written to the world.</summary>
    internal sealed class EngineState
    {
        /// <summary>High-gear seconds left of the loaded bone; 0 = nothing loaded.</summary>
        public float Remaining;

        /// <summary>The engine pushed on the last tick this game ran for the ship (for the panel line).</summary>
        public bool Pushing;

        /// <summary>Our own water-level lookup cache (vanilla keeps the same kind per sample point).</summary>
        public WaterVolume Water;

        public Container Hold;
        public bool HoldLooked;

        public float ClaimTimer;
        public long LastOwner;
        public float OwnerStable;
    }

    /// <summary>
    /// The engine in game: the boat's owner takes bones from the vanilla hold, adds the push and drains the loaded
    /// bone; the steerer takes the boat over when it's safe. State per ship lives here (ConditionalWeakTable: gone
    /// with the ship).
    /// </summary>
    internal static class ShipEngine
    {
        private static readonly ConditionalWeakTable<Ship, EngineState> s_states = new ConditionalWeakTable<Ship, EngineState>();

        public static EngineState State(Ship ship) => s_states.GetValue(ship, _ => new EngineState());

        /// <summary>The boat's hold (the vanilla cargo chest), looked up once per ship; null on the Raft.</summary>
        public static Container Hold(Ship ship)
        {
            var state = State(ship);
            if (!state.HoldLooked)
            {
                state.HoldLooked = true;
                state.Hold = ship.GetComponentInChildren<Container>(true);
            }
            return state.Hold;
        }

        /// <summary>The hold's inventory, or null (no hold, or not built yet: Container.Awake needs the ZDO).</summary>
        public static Inventory HoldInventory(Ship ship)
        {
            var hold = Hold(ship);
            return hold != null ? hold.GetInventory() : null;
        }

        /// <summary>Bone fragments in the hold, any slot, any world level.</summary>
        public static int Bones(Ship ship)
        {
            var inventory = HoldInventory(ship);
            return inventory != null ? inventory.CountItems(EngineRules.FuelItem, -1, false) : 0;
        }

        /// <summary>
        /// Somebody has the hold's window open, as seen by a non-owner (the ZDO mirror the owner writes). On the owner
        /// use the hold's own IsInUse(): the ZDO copy can lag until the owner's next write re-syncs it.
        /// </summary>
        public static bool HoldOpen(ZNetView nview) => nview.GetZDO().GetInt(ZDOVars.s_inUse) == 1;

        /// <summary>Vanilla's water test at the centre of mass (forces only apply below the disable level).</summary>
        private static bool InWater(Ship ship, Rigidbody body, EngineState state)
        {
            var centre = body.worldCenterOfMass;
            var level = Floating.GetWaterLevel(centre, ref state.Water);
            return centre.y - level - ship.m_waterLevelOffset <= ship.m_disableLevel;
        }

        /// <summary>Every client, every tick: the modded steerer takes the boat when EngineRules.ShouldClaim says so.</summary>
        public static void MaybeClaim(Ship ship, ZNetView nview, EngineState state, float dt)
        {
            var owner = nview.GetZDO().GetOwner();
            if (owner != state.LastOwner)
            {
                state.LastOwner = owner;
                state.OwnerStable = 0f;
            }
            else
            {
                state.OwnerStable += dt;
            }
            state.ClaimTimer += dt;
            if (state.ClaimTimer < EngineRules.ClaimInterval) return;
            state.ClaimTimer = 0f;
            var player = Player.m_localPlayer;
            if (player == null || ship.m_shipControlls == null) return;
            var steering = ship.m_shipControlls.GetUser() == player.GetPlayerID();
            if (!EngineRules.ShouldClaim(steering, nview.IsOwner(), HoldOpen(nview), state.OwnerStable)) return;
            nview.ClaimOwnership();
            state.OwnerStable = 0f;
            Plugin.Log.LogDebug($"Took over {ship.name} for the engine (previous owner {owner})");
        }

        /// <summary>
        /// The owner's tick, when this player steers or nobody does: take a bone if none is loaded (never under an open
        /// chest window, never from an empty hold), push in the gear's direction, drain the loaded bone by the gear.
        /// </summary>
        public static void Tick(Ship ship, Rigidbody body, EngineState state, float dt)
        {
            state.Pushing = false;
            var gear = ship.GetSpeedSetting();
            var fraction = EngineRules.GearFraction(gear);
            if (fraction <= 0f) return;                       // stopped: the loaded bone keeps
            var controls = ship.m_shipControlls;
            var player = Player.m_localPlayer;
            var localSteering = controls != null && player != null && controls.GetUser() == player.GetPlayerID();
            var anyoneSteering = controls != null && controls.HaveValidUser();
            if (!EngineRules.RunsHere(localSteering, anyoneSteering)) return;   // someone else's trip: theirs to run
            var hold = Hold(ship);
            var inventory = hold != null ? hold.GetInventory() : null;
            if (inventory == null) return;                    // no hold (Raft), or not built yet
            if (!InWater(ship, body, state)) return;          // beached or airborne: no push, no drain
            if (EngineRules.NeedsBone(state.Remaining))
            {
                var bones = inventory.CountItems(EngineRules.FuelItem, -1, false);
                if (!EngineRules.CanLoad(hold.IsInUse(), bones)) return;   // IsInUse is owner-local and exact here
                inventory.RemoveItem(EngineRules.FuelItem, 1, -1, false);  // vanilla save path: everyone sees the count
                state.Remaining = EngineRules.BurnSeconds;
            }
            var direction = ship.transform.forward * EngineRules.Direction(gear);
            var impulse = EngineRules.Impulse(ship.m_sailForceFactor, fraction, body.mass);
            body.AddForceAtPosition(direction * impulse, body.worldCenterOfMass, ForceMode.Impulse);
            state.Remaining = EngineRules.Drain(state.Remaining, dt, fraction);
            state.Pushing = true;
        }
    }
}
