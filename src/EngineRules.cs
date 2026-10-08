namespace BoneEngine
{
    /// <summary>
    /// The engine as plain rules (no Unity), so they're unit tested: which gear pushes how hard and in which direction,
    /// when a bone is taken and how it drains, and when the steerer may take the boat over.
    /// </summary>
    internal static class EngineRules
    {
        /// <summary>The fuel: the bone fragments item's shared name (same on every copy, every language).</summary>
        public const string FuelItem = "$item_bonefragments";

        /// <summary>High gear's push per physics tick, as a multiple of the boat's own good-wind full-sail push.</summary>
        public const float EnginePower = 1.2f;

        /// <summary>High-gear seconds one bone lasts on the reference boat; lower gears drain it proportionally slower.</summary>
        public const float BurnSeconds = 15f;

        /// <summary>The reference boat's sail factor (the Longship's m_sailForceFactor): burn follows push, scaled from here.</summary>
        public const float ReferenceSailFactor = 0.05f;

        /// <summary>The boat's owner must have held it this long before the steerer takes it (closes the open-hold race).</summary>
        public const float OwnerStableSeconds = 2f;

        /// <summary>
        /// The hold must have been seen closed this long before the steerer takes the boat: the owner reopens their own
        /// hold with no network round trip, so a window that just closed may already be open again.
        /// </summary>
        public const float HoldClosedBeforeClaimSeconds = 2f;

        /// <summary>
        /// A fresh owner may only take a bone after holding the boat this long: a non-owner's copy of the hold refreshes
        /// once a second, and removing a bone saves this game's whole copy, so loading at once could write a stale hold
        /// over a friend's last chest moves.
        /// </summary>
        public const float OwnedBeforeLoadSeconds = 1.5f;

        /// <summary>How often the steerer checks whether to take the boat.</summary>
        public const float ClaimInterval = 1f;

        /// <summary>The sail gear is the engine gear: rowing and reverse low, half sail medium, full sail high.</summary>
        public static float GearFraction(Ship.Speed gear)
        {
            switch (gear)
            {
                case Ship.Speed.Back:
                case Ship.Speed.Slow: return 1f / 3f;
                case Ship.Speed.Half: return 2f / 3f;
                case Ship.Speed.Full: return 1f;
                default: return 0f;
            }
        }

        /// <summary>+1 forward, -1 in reverse, 0 stopped.</summary>
        public static int Direction(Ship.Speed gear)
        {
            if (gear == Ship.Speed.Back) return -1;
            return gear == Ship.Speed.Stop ? 0 : 1;
        }

        /// <summary>The impulse to add this tick (the game's sail impulse is also sailFactor × mass per tick).</summary>
        public static float Impulse(float sailForceFactor, float fraction, float mass)
        {
            return EnginePower * sailForceFactor * fraction * mass;
        }

        /// <summary>The owner's game runs the engine only for its own steering, or when nobody steers (sails left up).</summary>
        public static bool RunsHere(bool localSteering, bool anyoneSteering)
        {
            return localSteering || !anyoneSteering;
        }

        /// <summary>
        /// How long a freshly loaded bone lasts at high gear on this boat: bigger engine (sail factor), bigger appetite,
        /// so fuel per unit of push is the same everywhere. A boat without a sail factor never pushes; base time for it.
        /// </summary>
        public static float BoneSeconds(float sailForceFactor)
        {
            if (sailForceFactor <= 0f) return BurnSeconds;
            return BurnSeconds * ReferenceSailFactor / sailForceFactor;
        }

        /// <summary>No bone loaded (the remaining time of the loaded one is used up).</summary>
        public static bool NeedsBone(float remaining)
        {
            return remaining <= 0f;
        }

        /// <summary>A boat without a working sail has no engine: nothing pushed, no bones taken, no count shown.</summary>
        public static bool HasEngine(float sailForceFactor)
        {
            return sailForceFactor > 0f;
        }

        /// <summary>
        /// A bone may be taken from the hold: never under an open chest window, never from an empty hold, and never
        /// before this game has owned the boat long enough for its copy of the hold to be fresh.
        /// </summary>
        public static bool CanLoad(bool holdOpen, int bones, float ownedSeconds)
        {
            return !holdOpen && bones > 0 && ownedSeconds >= OwnedBeforeLoadSeconds;
        }

        /// <summary>What's left of the loaded bone after this tick's engine time (dt × gear fraction), floored at 0.</summary>
        public static float Drain(float remaining, float dt, float fraction)
        {
            if (fraction <= 0f || dt <= 0f) return remaining;
            var left = remaining - dt * fraction;
            return left > 0f ? left : 0f;
        }

        /// <summary>
        /// The steerer takes the boat only when not owner, the hold has been seen closed for a while (not merely closed
        /// this instant), and the owner has been stable.
        /// </summary>
        public static bool ShouldClaim(bool steering, bool owner, float holdClosedSeconds, float ownerStableSeconds)
        {
            return steering && !owner && holdClosedSeconds >= HoldClosedBeforeClaimSeconds
                && ownerStableSeconds >= OwnerStableSeconds;
        }

        /// <summary>The rudder's line: the item's name in the player's language, then the count left in the hold.</summary>
        public static string CountLine(string itemName, int count)
        {
            return itemName + ": " + count;
        }

        /// <summary>
        /// The steering panel's text, two short lines (the panel sits at the screen's right edge): the count, then the
        /// engine status, "active" in the game's gold (a TextMeshPro colour tag; goldHex like "#FFD23C") while the
        /// engine pushed this tick, "idle" plain. The bone in the engine isn't in the count, so "active" at 0 says the
        /// boat is on its last bone.
        /// </summary>
        public static string PanelLine(string itemName, int count, bool running, string goldHex)
        {
            var status = running ? "<color=" + goldHex + ">active</color>" : "idle";
            return CountLine(itemName, count) + "\nEngine status: " + status;
        }
    }
}
