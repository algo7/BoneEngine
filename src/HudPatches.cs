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
        /// <summary>Gap between the wheel's bottom edge and the text: room for the wind symbol, which orbits the wheel's rim.</summary>
        private const float LineGap = 34f;
        private const float LineFontSize = 20f;

        private static Hud s_hud;
        private static TMP_Text s_text;
        private static int s_lastCount = -1;
        private static bool s_lastRunning;
        private static bool s_off;

        /// <summary>The game's gold readout colour (armour, carry weight), read from the inventory's weight label; a close fallback.</summary>
        private static string s_goldHex;
        private const string FallbackGoldHex = "#FFD23C";

        [HarmonyPatch(typeof(Hud), "UpdateShipHud")]
        [HarmonyPostfix]
        private static void UpdateShipHud(Hud __instance, Player player)
        {
            if (s_off) return;
            try
            {
                var ship = player != null ? player.GetControlledShip() : null;
                var panel = __instance.m_shipHudRoot;
                var show = !EnginePatches.Off && ship != null && panel != null && panel.activeSelf
                    && ShipEngine.HoldInventory(ship) != null && EngineRules.HasEngine(ship.m_sailForceFactor);
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
                var running = ShipEngine.State(ship).Pushing;
                if (bones != s_lastCount || running != s_lastRunning)
                {
                    s_lastCount = bones;
                    s_lastRunning = running;
                    s_text.text = EngineRules.PanelLine(Localization.instance.Localize(EngineRules.FuelItem), bones, running, GoldHex());
                    FitToText(s_text);
                }
                if (!s_text.gameObject.activeSelf) s_text.gameObject.SetActive(true);
            }
            catch (Exception e)
            {
                TurnOff($"The steering panel's bone count failed; the panel is vanilla from here: {e}");
            }
        }

        /// <summary>The gold of the inventory's weight readout, as a "#RRGGBB" tag value; read once, fallback if the GUI isn't there.</summary>
        private static string GoldHex()
        {
            if (s_goldHex != null) return s_goldHex;
            var gui = InventoryGui.instance;
            var label = gui != null ? gui.m_weight : null;
            s_goldHex = label != null ? "#" + ColorUtility.ToHtmlStringRGB(label.color) : FallbackGoldHex;
            return s_goldHex;
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
            // Hang from the wheel's bottom-right corner and grow leftwards: the wheel sits at the screen's right edge.
            rect.pivot = new Vector2(1f, 1f);
            var wheelBottom = wheel.anchoredPosition.y - wheel.rect.height * (1f - wheel.pivot.y);
            var wheelRight = wheel.anchoredPosition.x + wheel.rect.width * (1f - wheel.pivot.x);
            rect.anchoredPosition = new Vector2(wheelRight, wheelBottom - LineGap);
            rect.sizeDelta = new Vector2(420f, 60f);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            // Lines start flush left inside a box that is exactly as wide as the widest line (FitToText), so the first
            // characters line up and the box's right edge stays pinned under the wheel.
            text.alignment = TextAlignmentOptions.TopLeft;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.overflowMode = TextOverflowModes.Overflow;
            text.enableAutoSizing = false;
            text.fontSize = LineFontSize;
            text.color = Color.white;
            text.text = "";
            return text;
        }

        /// <summary>Size the box to its text (widest line, both lines high), keeping the pivot at the top right.</summary>
        private static void FitToText(TMP_Text text)
        {
            var size = text.GetPreferredValues();
            ((RectTransform)text.transform).sizeDelta = new Vector2(size.x + 2f, size.y + 2f);
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
