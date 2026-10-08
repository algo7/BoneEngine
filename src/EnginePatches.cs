using System;
using HarmonyLib;
using UnityEngine;

namespace BoneEngine
{
    /// <summary>
    /// After the game's own physics step for a boat: the steerer's take-over check (every client), then the engine
    /// (owner only). A postfix, so vanilla already ran whatever happens here. Never throws (logged once; vanilla goes on).
    /// </summary>
    [HarmonyPatch]
    internal static class EnginePatches
    {
        private static bool s_errorLogged;

        [HarmonyPatch(typeof(Ship), nameof(Ship.CustomFixedUpdate))]
        [HarmonyPostfix]
        private static void CustomFixedUpdate(Ship __instance, float fixedDeltaTime, ZNetView ___m_nview, Rigidbody ___m_body)
        {
            try
            {
                if (___m_nview == null || !___m_nview.IsValid() || ___m_body == null) return;
                var state = ShipEngine.State(__instance);
                ShipEngine.MaybeClaim(__instance, ___m_nview, state, fixedDeltaTime);
                if (!___m_nview.IsOwner()) return;
                ShipEngine.Tick(__instance, ___m_body, state, fixedDeltaTime);
            }
            catch (Exception e)
            {
                if (s_errorLogged) return;
                s_errorLogged = true;
                Plugin.Log.LogError($"The engine failed (logged once; boats are vanilla from here): {e}");
            }
        }
    }
}
