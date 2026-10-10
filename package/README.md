# BoneEngine

![A boat at full sail leaving a wake, with a red arrow pointing at Bone Fragments: 54 and Engine status: active](https://raw.githubusercontent.com/algo7/BoneEngine/main/images/header.jpg)

Ever been stuck at a beach rowing backward, or in a headwind? Drop bone fragments in your boat's storage and the
boat gets an engine that runs on bones. Supports both forward and reverse. Client-side and vanilla content only.

## How to Use

1. Put bone fragments in the boat's storage (Karve, Longship, Drakkar). The Raft has none.
2. Sail. The engine's power follows the sail setting, and reverse always runs in low: rowing low, half sail in the
   middle, full sail at full. The push works the same in any wind.
3. The count shows under the wind compass while you steer, and on the rudder text when you walk up to it.

   ![Under the wind compass, Bone Fragments with Engine status: idle and with Engine status: active, then the rudder's hover text with Bone Fragments: 55](https://raw.githubusercontent.com/algo7/BoneEngine/main/images/hud.png)

## How Things Behave

- The engine takes one bone at a time. The count is what's left to burn: an active engine at 0 is on its last bone.
- A stopped boat consumes none.
- The amount of bones only determines how long the engine runs. The push is fixed, scaled by the sail setting only.
- The boat's speed reverts to vanilla if the bones run out. Add more and the engine picks up.

## Sailing Stats

### Top speed at full sail (m/s)

| Boat | Tail/side wind (vanilla) | Tail/side wind (engine) | Headwind (vanilla) | Headwind (engine) |
|---|---|---|---|---|
| Karve | 4.6 | 7.5 | 2.0 | 6.0 |
| Longship | 5.9 | 9.7 | 2.0 | 7.7 |
| Drakkar | 5.5 | 9.0 | 1.6 | 7.1 |

The sail doesn't do anything in a headwind on vanilla boats, so they fall back to rowing speed. With bones, keep
full sail up anyway: the engine runs at full power despite the wind.

### How long each bone lasts (in seconds)

Heavier boat uses more fuel.

| Boat | Full sail | Half sail | Rowing or reverse |
|---|---|---|---|
| Karve | 25 | 37.5 | 75 |
| Longship | 15 | 22.5 | 45 |
| Drakkar | 9 | 13 | 26 |

### Time per kilometer

| Boat | Tail/side (vanilla) | Tail/side (engine) | Headwind (vanilla) | Headwind (engine) |
|---|---|---|---|---|
| Karve | 3:38 | 2:13 (6 bones) | 8:20 rowing | 2:47 (7 bones) |
| Longship | 2:49 | 1:43 (7 bones) | 8:20 rowing | 2:09 (9 bones) |
| Drakkar | 3:03 | 1:51 (13 bones) | 10:25 rowing | 2:20 (16 bones) |

### On the world scale

Longship at full sail with tail/side wind.

| Trip | Distance | Share of the world | Vanilla | With engine |
|---|---|---|---|---|
| Island to island | 1 km | a twentieth | 2:49 | 1:43 |
| Spawn to the far biomes (Ashlands, Deep North) | 9 km | just under half | 25:25 | 15:28 |
| One edge of the world to the other | 20 km | the whole way | 56:30 | 34:22 |

These are straight-line distances. Real trips bend around coasts and weather, so expect longer.

## Engine Power

Water holds a boat back roughly in proportion to the square of its speed. In the real world that is the drag equation:

```
drag = 0.5 × water density × drag coefficient × hull area under water × speed to the power of 2
```

Valheim folds everything except the speed into one drag constant per boat:

```
drag = drag constant × speed to the power of 2
```

A boat stops speeding up when its push equals the drag, which puts its top speed at:

```
top speed = square root of (push / drag constant)
```

That is why doubling the push gives about 1.4× the speed, not 2×. The engine pushes about 1.7 times as hard as a
full sail in a tail or side wind, on top of whatever the sail gives. With the wind behind or beside you that comes to about 1.6×
the vanilla top speed. In a headwind the engine is the whole push.

The engine's power is fixed on purpose, for two reasons:

- **Immersion.** The world should still feel big. The far biomes remain a quarter-hour voyage, not a quick hop.
- **Performance.** The game builds the land ahead of you in 64 m squares, and a faster boat makes it build them more
  often. Near busy coasts like the Black Forest, that shows up as stutter on lower-end PCs.

## Multiplayer

Everyone sees the faster boat. Nothing to install on the server. The engine runs in the steerer's game, so the one
at the rudder needs the mod. A friend without it steers at vanilla speed and burns nothing. Changing hands, by
taking the rudder or opening the storage, costs at most the bone being burned. A friend opening the storage pauses
the engine until shortly after they close it.

## Compatibility

- Built and tested for Valheim 1.0 (Deep North), on Linux, with no OS-specific code.
- Works on top of mods that change sail or oar strength (FasterBoats, ShipConfig): the engine's push adds to theirs.

## Uninstalling

Nothing is saved. Your bones stay in the storage, and the boat is just a boat again.

## Links

- Available on [Thunderstore](https://thunderstore.io/c/valheim/p/Algo7/BoneEngine/) and
  [Hexium](https://valheim.hexium.gg/mods/Algo7/BoneEngine)
- Source and bug reports: https://github.com/algo7/BoneEngine (issues welcome)
- Changes: the Changelog tab
- Made with AI assistance.
- Built with [BepInEx](https://github.com/BepInEx/BepInEx) and HarmonyX. MIT license.
