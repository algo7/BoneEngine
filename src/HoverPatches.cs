using System;
using HarmonyLib;
using UnityEngine;

namespace BoneEngine
{
    /// <summary>
    /// The rudder's hover text gains "Bone fragments: N" on boats with a hold (the passengers' way to check; the steerer
    /// has the panel line). Not on the grey "too far" text. Never throws (logged once; then the vanilla text).
    /// </summary>
    [HarmonyPatch]
    internal static class HoverPatches
    {
        private static bool s_errorLogged;

        [HarmonyPatch(typeof(ShipControlls), nameof(ShipControlls.GetHoverText))]
        [HarmonyPostfix]
        private static void GetHoverText(ShipControlls __instance, ref string __result)
        {
            try
            {
                var ship = __instance.m_ship;
                var player = Player.m_localPlayer;
                if (ship == null || player == null || __instance.m_attachPoint == null) return;
                if (Vector3.Distance(player.transform.position, __instance.m_attachPoint.position) >= __instance.m_maxUseRange) return;   // vanilla's "too far"
                if (ShipEngine.Hold(ship) == null) return;
                __result += "\n" + EngineRules.CountLine(Localization.instance.Localize(EngineRules.FuelItem), ShipEngine.Bones(ship));
            }
            catch (Exception e)
            {
                if (s_errorLogged) return;
                s_errorLogged = true;
                Plugin.Log.LogWarning($"The rudder's bone count failed (logged once; the rudder text is vanilla from here): {e}");
            }
        }
    }
}
