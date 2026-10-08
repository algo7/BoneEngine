# BoneEngine

Bone fragments have a job now. Drop them in your boat's storage and the boat gets an engine: it pushes the way your
gear points, reverse included, and burns a bone every so often while it does. Only the one steering needs the mod.

## How to Use

1. Put bone fragments in the boat's storage (Karve, Longship, Drakkar; the Raft has none).
2. Sail. The sail gear is also the engine gear: rowing and reverse run it low, half sail in the middle, full sail at
   full. The push works the same in any wind.
3. The count shows under the wind compass while you steer, and on the rudder text when you walk up to it.

## Good to Know

- The engine takes one bone out of the storage and runs on it; the next one is taken when that one is used up. The
  count is therefore what's left to burn, and an active engine at 0 means it's running on its last bone. A moored,
  stopped or empty boat takes none.
- One bone or a full storage: the push is the same. More bones only run longer.
- Out of bones, the boat sails and rows as vanilla. Add bones and the engine picks up again.
- While your own storage window is open, the engine takes no new bone; the one it has keeps pushing.

## Sailing Stats

Top speed on open sea, strong wind, full sail, without → with bones aboard:

| Boat | Tail or side wind | Headwind |
|---|---|---|
| Karve | 4.6 → 7.5 m/s | 2.0 → 6.0 m/s |
| Longship | 5.9 → 9.7 m/s | 2.0 → 7.7 m/s |
| Drakkar | 5.5 → 9.0 m/s | 1.6 → 7.1 m/s |

In a headwind the sail gives nothing, so the vanilla number is rowing speed.

Burn rates: at full sail the Karve takes a bone every 25 seconds, the Longship every 15, the Drakkar every 9. The
stronger a boat's engine, the faster it burns. Half sail stretches each bone 1.5×, rowing and reverse 3×.

One kilometre of open sea (about one island to the next), minutes:seconds and bones spent:

| Boat | Tail/side wind | Tail/side + bones | Headwind | Headwind + bones |
|---|---|---|---|---|
| Karve | 3:38 | 2:13 (6 bones) | 8:20 rowing | 2:47 (7 bones) |
| Longship | 2:49 | 1:43 (7 bones) | 8:20 rowing | 2:09 (9 bones) |
| Drakkar | 3:03 | 1:51 (13 bones) | 10:25 rowing | 2:20 (16 bones) |

Against the world itself, on a Longship at full sail with a tail or side wind:

| Trip | Distance | Share of the world | Vanilla | With bones |
|---|---|---|---|---|
| Island to island | 1.5 km | about a thirteenth | 4:14 | 2:35 |
| Spawn to the far biomes (Ashlands, Deep North) | 9 km | just under half | 25:25 | 15:28 |
| One edge of the world to the other | 20 km | the whole way | 56:30 | 34:22 |

These are straight-line distances; real trips bend around coasts and weather, so expect longer.

Why not twice as fast? The sea pushes back with the **square** of your speed: going twice as fast costs four times
the push. The engine adds about one tailwind sail's worth of push on top of whatever your sail is getting, which
works out to roughly half again your top speed with the wind behind you, and the whole push in a headwind.

## Multiplayer

Everyone sees the faster boat; nothing to install on the server. The engine runs in the game of whoever is steering,
so the one at the rudder needs the mod. A friend without it steers at vanilla speed and the bones stay put. Whenever
the boat changes hands, by someone taking the rudder or opening the storage, the engine starts over on a fresh bone,
so a switch costs at most the bone it was already burning. A friend opening the storage pauses the engine until a
short moment after they close it.

## Compatibility

- Built and tested for Valheim 1.0 (Deep North), on Linux; no OS-specific code.
- Works on top of mods that change sail or oar strength (FasterBoats, ShipConfig): the engine's push adds to theirs.
- Modded boats get an engine when they're built like the vanilla ones, with the storage part of the ship itself and a
  working sail.

## Uninstalling

Nothing is saved. Your bones stay in the storage; the boat is just a boat again.

## Links

- Source and bug reports: https://github.com/algo7/BoneEngine (issues welcome)
- Changes: the Changelog tab
- Made with AI assistance.
- Built with [BepInEx](https://github.com/BepInEx/BepInEx) and HarmonyX. MIT license.
