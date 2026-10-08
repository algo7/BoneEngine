using System;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace BoneEngine
{
    /// <summary>
    /// The steering panel (sail-gear icons, rudder, wind wheel; shown to the steerer only) gains a "Bone fragments: N"
    /// line on boats with a hold. The text is a copy of the HUD's health text (same font), made once per HUD (a new
    /// HUD comes with every world). Updated only when the count changes. On any failure the line is dropped for the
    /// session (logged once); the panel itself is vanilla.
    /// </summary>
    [HarmonyPatch]
    internal static class HudPatches
    {
        /// <summary>Where the line sits under the gear icons (panel pixels at the game's reference scale); tuned in game.</summary>
        private const float LineOffsetY = -70f;
        private const float LineFontSize = 18f;

        private static Hud s_hud;
        private static TMP_Text s_text;
        private static int s_lastCount = -1;
        private static bool s_off;

        [HarmonyPatch(typeof(Hud), "UpdateShipHud")]
        [HarmonyPostfix]
        private static void UpdateShipHud(Hud __instance, Player player)
        {
            if (s_off) return;
            try
            {
                var ship = player != null ? player.GetControlledShip() : null;
                var panel = __instance.m_shipHudRoot;
                var show = ship != null && panel != null && panel.activeSelf && ShipEngine.Hold(ship) != null;
                if (!show)
                {
                    if (s_text != null && s_text.gameObject.activeSelf) s_text.gameObject.SetActive(false);
                    return;
                }
                if (s_text == null || s_hud != __instance)
                {
                    s_hud = __instance;
                    s_text = MakeLine(__instance);
                    s_lastCount = -1;
                    if (s_text == null)
                    {
                        TurnOff("The steering panel has no text to copy (a game update?): no bone count on the panel");
                        return;
                    }
                }
                var bones = ShipEngine.Bones(ship);
                if (bones != s_lastCount)
                {
                    s_lastCount = bones;
                    s_text.text = EngineRules.CountLine(Localization.instance.Localize(EngineRules.FuelItem), bones);
                }
                if (!s_text.gameObject.activeSelf) s_text.gameObject.SetActive(true);
            }
            catch (Exception e)
            {
                TurnOff($"The steering panel's bone count failed; the panel is vanilla from here: {e}");
            }
        }

        /// <summary>A copy of the health text under the panel's controls root, centred below the gear icons.</summary>
        private static TMP_Text MakeLine(Hud hud)
        {
            var template = hud.m_healthText;
            var root = hud.m_shipControlsRoot;
            if (template == null || root == null) return null;
            var line = UnityEngine.Object.Instantiate(template.gameObject, root.transform);
            line.name = "BoneEngineCount";
            var text = line.GetComponent<TMP_Text>();
            if (text == null)
            {
                UnityEngine.Object.Destroy(line);
                return null;
            }
            var rect = (RectTransform)line.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, LineOffsetY);
            rect.sizeDelta = new Vector2(320f, 32f);
            rect.localScale = Vector3.one;
            text.alignment = TextAlignmentOptions.Center;
            text.enableAutoSizing = false;
            text.fontSize = LineFontSize;
            text.color = Color.white;
            text.text = "";
            return text;
        }

        private static void TurnOff(string message)
        {
            s_off = true;
            Plugin.Log.LogWarning(message);
            try
            {
                if (s_text != null) UnityEngine.Object.Destroy(s_text.gameObject);
            }
            catch (Exception)
            {
                // Already logged; the empty line may linger on this HUD.
            }
            s_text = null;
        }
    }
}
