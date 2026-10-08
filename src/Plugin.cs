using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

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
            Log.LogInfo($"{Name} loaded (v{PluginVersion}), HarmonyX {typeof(Harmony).Assembly.GetName().Version}");
        }
    }
}
