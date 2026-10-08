using System;
using HarmonyLib;
using UnityEngine;

namespace BoneEngine
{
    /// <summary>
    /// After the game's own physics step for a boat: the steerer's take-over check (every client), then the engine
    /// (owner only). A postfix, so vanilla already ran whatever happens here. Never throws: the first error is logged and
    /// switches the engine off for the session (boats are vanilla from there, and the count lines hide).
    /// </summary>
    [HarmonyPatch]
    internal static class EnginePatches
    {
        private static readonly Failsafe s_failsafe = new Failsafe();

        /// <summary>The engine switched itself off after an error; the count lines hide too (a count with no engine misleads).</summary>
        public static bool Off => s_failsafe.Off;

        [HarmonyPatch(typeof(Ship), nameof(Ship.CustomFixedUpdate))]
        [HarmonyPostfix]
        private static void CustomFixedUpdate(Ship __instance, float fixedDeltaTime, ZNetView ___m_nview, Rigidbody ___m_body)
        {
            if (s_failsafe.Off) return;
            try
            {
                if (___m_nview == null || !___m_nview.IsValid() || ___m_body == null) return;
                var state = ShipEngine.State(__instance);
                ShipEngine.MaybeClaim(__instance, ___m_nview, state, fixedDeltaTime);
                if (!___m_nview.IsOwner())
                {
                    state.Pushing = false;    // another game drives the boat: nothing this one can report as running
                    state.Remaining = 0f;     // any switch starts the next owner on a fresh bone (at most one per switch)
                    state.OwnedSeconds = 0f;
                    return;
                }
                state.OwnedSeconds += fixedDeltaTime;
                ShipEngine.Tick(__instance, ___m_body, state, fixedDeltaTime);
            }
            catch (Exception e)
            {
                if (s_failsafe.Trip()) Plugin.Log.LogError($"The engine failed and is off for this session (boats are vanilla): {e}");
            }
        }
    }
}
