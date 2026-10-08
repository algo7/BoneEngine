using System;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace BoneEngine
{
    /// <summary>
    /// The steering panel (shown to the steerer only) gains a "Bone fragments: N" line under the wind wheel on boats
    /// with a hold. The text is a copy of the HUD's health text (same font), made once per HUD (a new HUD comes with
    /// every world), parented next to the wheel (not under it: the wheel turns with the ship; not under the ship-wheel
    /// marker at the rudder: that root is pinned in world-to-screen space and rotates). Updated only when the count
    /// changes. On any failure the line is dropped for the session (logged once); the panel itself is vanilla.
    /// </summary>
    [HarmonyPatch]
    internal static class HudPatches
    {
        /// <summary>Gap between the wheel's bottom edge and the line (panel pixels at the game's reference scale); tuned in game.</summary>
        private const float LineGap = 10f;
        private const float LineFontSize = 20f;

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

        /// <summary>A copy of the health text, a sibling of the wind wheel, centred just below it.</summary>
        private static TMP_Text MakeLine(Hud hud)
        {
            var template = hud.m_healthText;
            var wheel = hud.m_shipWindIndicatorRoot;
            var parent = wheel != null ? wheel.parent as RectTransform : null;
            if (template == null || parent == null) return null;
            var line = UnityEngine.Object.Instantiate(template.gameObject, parent);
            line.name = "BoneEngineCount";
            var text = line.GetComponent<TMP_Text>();
            if (text == null)
            {
                UnityEngine.Object.Destroy(line);
                return null;
            }
            var rect = (RectTransform)line.transform;
            rect.anchorMin = wheel.anchorMin;
            rect.anchorMax = wheel.anchorMax;
            rect.pivot = new Vector2(0.5f, 1f);
            var wheelBottom = wheel.anchoredPosition.y - wheel.rect.height * (1f - wheel.pivot.y);
            var wheelCentreX = wheel.anchoredPosition.x + wheel.rect.width * (0.5f - wheel.pivot.x);
            rect.anchoredPosition = new Vector2(wheelCentreX, wheelBottom - LineGap);
            rect.sizeDelta = new Vector2(320f, 34f);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
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
