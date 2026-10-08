using System;
using HarmonyLib;
using UnityEngine;

namespace BoneEngine
{
    /// <summary>
    /// The rudder's hover text gains "Bone fragments: N" on boats with a hold (the passengers' way to check; the steerer
    /// has the panel line). Not on the grey "too far" text, not before the hold's contents exist, not once the engine is
    /// off. Never throws: the first error is logged and switches this line off for the session (vanilla text from there).
    /// </summary>
    [HarmonyPatch]
    internal static class HoverPatches
    {
        private static readonly Failsafe s_failsafe = new Failsafe();

        [HarmonyPatch(typeof(ShipControlls), nameof(ShipControlls.GetHoverText))]
        [HarmonyPostfix]
        private static void GetHoverText(ShipControlls __instance, ref string __result)
        {
            if (s_failsafe.Off || EnginePatches.Off) return;
            try
            {
                var ship = __instance.m_ship;
                var player = Player.m_localPlayer;
                if (ship == null || player == null || __instance.m_attachPoint == null) return;
                if (Vector3.Distance(player.transform.position, __instance.m_attachPoint.position) >= __instance.m_maxUseRange) return;   // vanilla's "too far"
                if (ShipEngine.HoldInventory(ship) == null || !EngineRules.HasEngine(ship.m_sailForceFactor)) return;
                __result += "\n" + EngineRules.CountLine(Localization.instance.Localize(EngineRules.FuelItem), ShipEngine.Bones(ship));
            }
            catch (Exception e)
            {
                if (s_failsafe.Trip()) Plugin.Log.LogWarning($"The rudder's bone count failed and is off for this session (the rudder text is vanilla): {e}");
            }
        }
    }
}
