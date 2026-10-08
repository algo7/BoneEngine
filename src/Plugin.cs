using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace BoneEngine
{
    [BepInPlugin(Guid, Name, PluginVersion)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string Guid = "algo7.boneengine";
        public const string Name = "BoneEngine";
        public const string PluginVersion = PluginInfo.Version; // from the git tag, generated at build time (MinVer)

        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            if (Application.isBatchMode)
            {
                Log.LogInfo($"{Name} loaded (v{PluginVersion}): dedicated server, nothing to do");
                return;
            }
            var harmony = new Harmony(Guid);
            if (!TryPatch(harmony, typeof(EnginePatches), "engine hook"))
            {
                Log.LogError("The engine is off: boats are vanilla");
                return;
            }
            if (!TryPatch(harmony, typeof(HoverPatches), "rudder text hook")) Log.LogWarning("No bone count on the rudder (the engine still works)");
            if (!TryPatch(harmony, typeof(HudPatches), "steering panel hook")) Log.LogWarning("No bone count on the steering panel (the engine still works)");
            Log.LogInfo($"{Name} loaded (v{PluginVersion})");
        }

        /// <summary>True when patched; false (logged) on failure.</summary>
        private static bool TryPatch(Harmony harmony, Type patches, string what)
        {
            try
            {
                harmony.PatchAll(patches);
                return true;
            }
            catch (Exception e)
            {
                Log.LogError($"Could not install the {what}: {e}");
                return false;
            }
        }
    }
}
