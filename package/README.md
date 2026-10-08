# BoneEngine

Ever rowed yourself off a beach one stroke at a time, or sailed home into a headwind that never turned? Bone
fragments have a job now. Drop them in your boat's storage and the boat gets an engine: it pushes both forward and
in reverse, and burns bones while it does. Only the one steering needs the mod.

## How to Use

1. Put bone fragments in the boat's storage (Karve, Longship, Drakkar). The Raft has none.
2. Sail. The engine's power follows the sail setting, and reverse always runs in low: rowing low, half sail in the
   middle, full sail at full. The push works the same in any wind.
3. The count shows under the wind compass while you steer, and on the rudder text when you walk up to it.

## How Things Behave

- The engine takes one bone at a time. The count is what's left to burn: an active engine at 0 is on its last bone.
- A stopped boat consumes none.
- The amount of bones only determines how long the engine runs. The push is fixed, scaled by the sail setting only.
- The boat's speed reverts to vanilla if the bones run out. Add more and the engine picks up.

## Sailing Stats

Top speed at full sail, in m/s:

| Boat | Tail/side wind (vanilla) | Tail/side wind (engine) | Headwind (vanilla) | Headwind (engine) |
|---|---|---|---|---|
| Karve | 4.6 | 7.5 | 2.0 | 6.0 |
| Longship | 5.9 | 9.7 | 2.0 | 7.7 |
| Drakkar | 5.5 | 9.0 | 1.6 | 7.1 |

Vanilla boats can't sail into a headwind, so the vanilla headwind column is rowing speed. With bones, keep full sail
up anyway: the engine runs at full power despite the wind.

How many seconds each bone lasts (heavier boat uses more fuel):

| Boat | Full sail | Half sail | Rowing or reverse |
|---|---|---|---|
| Karve | 25 | 37.5 | 75 |
| Longship | 15 | 22.5 | 45 |
| Drakkar | 9 | 13 | 26 |

One kilometre of open sea (about one island to the next), minutes:seconds and bones spent:

| Boat | Tail/side (vanilla) | Tail/side (engine) | Headwind (vanilla) | Headwind (engine) |
|---|---|---|---|---|
| Karve | 3:38 | 2:13 (6 bones) | 8:20 rowing | 2:47 (7 bones) |
| Longship | 2:49 | 1:43 (7 bones) | 8:20 rowing | 2:09 (9 bones) |
| Drakkar | 3:03 | 1:51 (13 bones) | 10:25 rowing | 2:20 (16 bones) |

Against the world itself, on a Longship at full sail with a tail or side wind:

| Trip | Distance | Share of the world | Vanilla | With engine |
|---|---|---|---|---|
| Island to island | 1.5 km | about a thirteenth | 4:14 | 2:35 |
| Spawn to the far biomes (Ashlands, Deep North) | 9 km | just under half | 25:25 | 15:28 |
| One edge of the world to the other | 20 km | the whole way | 56:30 | 34:22 |

These are straight-line distances. Real trips bend around coasts and weather, so expect longer.

## Engine Power

Water holds a boat back in proportion to the square of its speed. In the real world that is the drag equation:

**drag = 0.5 × water density × drag coefficient × frontal area × speed to the power of 2**

Valheim folds everything except the speed into one drag constant per boat:

**drag = drag constant × speed to the power of 2**

A boat stops speeding up when its push equals the drag, which puts its top speed at:

**top speed = square root of (push / drag constant)**

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
- Modded boats get an engine when they're built like the vanilla ones, with the storage part of the ship itself and a
  working sail.

## Uninstalling

Nothing is saved. Your bones stay in the storage, and the boat is just a boat again.

## Links

- Source and bug reports: https://github.com/algo7/BoneEngine (issues welcome)
- Changes: the Changelog tab
- Made with AI assistance.
- Built with [BepInEx](https://github.com/BepInEx/BepInEx) and HarmonyX. MIT license.
