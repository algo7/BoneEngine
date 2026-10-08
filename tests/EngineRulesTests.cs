using BoneEngine;

internal static partial class Tests
{
    private static void Test_GearFraction_EveryGear()
    {
        Eq(0f, EngineRules.GearFraction(Ship.Speed.Stop), "stop");
        Eq(1f / 3f, EngineRules.GearFraction(Ship.Speed.Back), "reverse: low");
        Eq(1f / 3f, EngineRules.GearFraction(Ship.Speed.Slow), "rowing: low");
        Eq(2f / 3f, EngineRules.GearFraction(Ship.Speed.Half), "half sail: medium");
        Eq(1f, EngineRules.GearFraction(Ship.Speed.Full), "full sail: high");
    }

    private static void Test_Direction_FollowsTheGear()
    {
        Eq(0, EngineRules.Direction(Ship.Speed.Stop), "stop");
        Eq(-1, EngineRules.Direction(Ship.Speed.Back), "reverse pushes backward");
        Eq(1, EngineRules.Direction(Ship.Speed.Slow), "rowing");
        Eq(1, EngineRules.Direction(Ship.Speed.Half), "half");
        Eq(1, EngineRules.Direction(Ship.Speed.Full), "full");
    }

    private static void Test_Impulse_ScalesWithEverything()
    {
        // Longship: sail factor 0.05, mass 1 → high gear = EnginePower × 0.05 per tick (a second good-wind full sail).
        Eq(EngineRules.EnginePower * 0.05f, EngineRules.Impulse(0.05f, 1f, 1f), "high gear, unit mass");
        Eq(EngineRules.EnginePower * 0.05f * (1f / 3f) * 2000f, EngineRules.Impulse(0.05f, 1f / 3f, 2000f), "low gear, heavy hull");
        Eq(0f, EngineRules.Impulse(0f, 1f, 2000f), "Trailership (sail factor 0): no engine");
        Eq(0f, EngineRules.Impulse(0.05f, 0f, 2000f), "stopped: nothing");
    }

    private static void Test_Drain_ByGearWithFloorZero()
    {
        Eq(EngineRules.BurnSeconds - 1f, EngineRules.Drain(EngineRules.BurnSeconds, 1f, 1f), "full gear: a second per second");
        Eq(EngineRules.BurnSeconds - 0.5f, EngineRules.Drain(EngineRules.BurnSeconds, 1f, 0.5f), "half the fraction, half the drain");
        Eq(0f, EngineRules.Drain(0.3f, 1f, 1f), "never below zero");
        Eq(4f, EngineRules.Drain(4f, 100f, 0f), "stopped: no drain");
    }

    private static void Test_Drain_LowerGearsLastLonger()
    {
        // One bone lasts BurnSeconds at full gear, three times that at low, 1.5 times at medium (0.1 s ticks, float sums).
        var remaining = EngineRules.BurnSeconds;
        var ticks = 0;
        while (remaining > 0f) { remaining = EngineRules.Drain(remaining, 0.1f, 1f); ticks++; }
        True(ticks >= 149 && ticks <= 151, $"full gear: about 150 ticks ({ticks})");
        remaining = EngineRules.BurnSeconds; ticks = 0;
        while (remaining > 0f) { remaining = EngineRules.Drain(remaining, 0.1f, 1f / 3f); ticks++; }
        True(ticks >= 449 && ticks <= 451, $"low gear: about 450 ticks ({ticks})");
        remaining = EngineRules.BurnSeconds; ticks = 0;
        while (remaining > 0f) { remaining = EngineRules.Drain(remaining, 0.1f, 2f / 3f); ticks++; }
        True(ticks >= 224 && ticks <= 226, $"medium gear: about 225 ticks ({ticks})");
    }

    private static void Test_BoneSeconds_FollowsTheEngineStrength()
    {
        // Burn follows the push: the Longship (sail factor 0.05) is the reference; the Karve (0.03) sips, the Drakkar (0.085) gulps.
        Eq(EngineRules.BurnSeconds, EngineRules.BoneSeconds(0.05f), "Longship: the base time");
        Eq(EngineRules.BurnSeconds * 0.05f / 0.03f, EngineRules.BoneSeconds(0.03f), "Karve: 5/3 of the base time");
        Eq(EngineRules.BurnSeconds * 0.05f / 0.085f, EngineRules.BoneSeconds(0.085f), "Drakkar: about 0.59 of the base time");
        Eq(EngineRules.BurnSeconds, EngineRules.BoneSeconds(0f), "no sail factor: the base time (such a boat never pushes anyway)");
    }

    private static void Test_NeedsBone()
    {
        True(EngineRules.NeedsBone(0f), "nothing loaded");
        True(EngineRules.NeedsBone(-1f), "treated as empty");
        False(EngineRules.NeedsBone(0.01f), "a sliver left still runs");
    }

    private static void Test_CanLoad()
    {
        True(EngineRules.CanLoad(holdOpen: false, bones: 1), "closed hold, one bone");
        False(EngineRules.CanLoad(holdOpen: true, bones: 50), "hold open: never");
        False(EngineRules.CanLoad(holdOpen: false, bones: 0), "empty hold: never");
    }

    private static void Test_RunsHere()
    {
        True(EngineRules.RunsHere(localSteering: true, anyoneSteering: true), "I steer");
        True(EngineRules.RunsHere(localSteering: false, anyoneSteering: false), "nobody steers, sails up");
        False(EngineRules.RunsHere(localSteering: false, anyoneSteering: true), "someone else steers: theirs to run");
    }

    private static void Test_ShouldClaim_AllFourConditions()
    {
        True(EngineRules.ShouldClaim(steering: true, owner: false, holdOpen: false, ownerStableSeconds: 2f), "all met");
        True(EngineRules.ShouldClaim(steering: true, owner: false, holdOpen: false, ownerStableSeconds: 60f), "long stable");
        False(EngineRules.ShouldClaim(steering: false, owner: false, holdOpen: false, ownerStableSeconds: 2f), "not steering");
        False(EngineRules.ShouldClaim(steering: true, owner: true, holdOpen: false, ownerStableSeconds: 2f), "already owner");
        False(EngineRules.ShouldClaim(steering: true, owner: false, holdOpen: true, ownerStableSeconds: 2f), "hold open");
        False(EngineRules.ShouldClaim(steering: true, owner: false, holdOpen: false, ownerStableSeconds: 1.9f), "owner just changed");
    }

    private static void Test_CountLine()
    {
        Eq("Bone fragments: 12", EngineRules.CountLine("Bone fragments", 12), "the line");
        Eq("Bone fragments: 0", EngineRules.CountLine("Bone fragments", 0), "empty hold");
    }

    private static void Test_PanelLine_ShowsTheEngineState()
    {
        // The hold count is bones left to burn; the one in the engine isn't in it, so the line says whether it's running.
        Eq("Bone fragments: 12 (engine running)", EngineRules.PanelLine("Bone fragments", 12, running: true), "pushing");
        Eq("Bone fragments: 0 (engine running)", EngineRules.PanelLine("Bone fragments", 0, running: true), "last bone in the engine");
        Eq("Bone fragments: 12 (engine idle)", EngineRules.PanelLine("Bone fragments", 12, running: false), "stopped, or waiting");
    }
}
