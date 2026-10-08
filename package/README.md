# BoneEngine

Bone fragments have a job now. Drop them in your boat's hold and the boat gets an engine: it pushes the way your gear
points, reverse included, and burns a bone every so often while it does. Only the one steering needs the mod.

## How to Use

1. Put bone fragments in the hold (Karve, Longship, Drakkar; the Raft has no hold).
2. Sail. The sail gear is the engine gear: rowing and reverse are the slow burn, half sail a bit more, full sail the
   fast burn. No wind needed.
3. Watch the count under the wind compass while you steer, or look at the rudder when you're not.

## Good to Know

- The engine takes one bone out of the hold and runs on it; the next one goes in when it's used up. So the count is
  what's left to burn, and an active engine at 0 means it's on its last bone. Moored, stopped or empty boats take
  none.
- One bone or a full hold: same push. More bones just run longer.
- Out of bones, the boat is back to sails and oars. Drop more in and it picks up again.
- Browsing your own hold under sail? The engine won't take a new bone until you close it; the one it has keeps
  pushing.

## Sailing Stats

Top speed on open sea, strong wind, full sail, without → with bones aboard:

| Boat | Wind behind or beside | Straight into the wind |
|---|---|---|
| Karve | 4.6 → 7.5 m/s | 2.0 → 6.0 m/s |
| Longship | 5.9 → 9.7 m/s | 2.0 → 7.7 m/s |
| Drakkar | 5.5 → 9.0 m/s | 1.6 → 7.1 m/s |

Into the wind the sail gives nothing, so without bones that number is you rowing.

Bigger boat, bigger appetite: at full sail the Karve burns a bone every 25 seconds, the Longship every 15, the
Drakkar every 9. Half sail stretches each bone 1.5×, rowing and reverse 3×.

The same stretch of open sea, one kilometre (about one island to the next), minutes:seconds and bones spent:

| Boat | Good wind | Good wind + bones | Headwind | Headwind + bones |
|---|---|---|---|---|
| Karve | 3:38 | 2:13 (6 bones) | 8:20 rowing | 2:47 (7 bones) |
| Longship | 2:49 | 1:43 (7 bones) | 8:20 rowing | 2:09 (9 bones) |
| Drakkar | 3:03 | 1:51 (13 bones) | 10:25 rowing | 2:20 (16 bones) |

Why not twice as fast? The sea pushes back with the **square** of your speed: going twice as fast costs four times
the push. The engine adds about one good-wind sail's worth of push on top of whatever your sail is getting, which
works out to roughly half again your top speed with the wind, and the whole push when the wind gives nothing.

## Multiplayer

Everyone sees the faster boat; nothing to install on the server. The engine runs when the one steering has the mod.
A friend without it steers at the usual speed and the bones stay put. Whenever the boat changes hands, by someone
taking the rudder or opening the hold, the engine starts over on a fresh bone, so a switch costs at most the bone it
was already burning. A friend opening the hold pauses the engine until they close it.

## Compatibility

- Built and tested for Valheim 1.0 (Deep North), on Linux; no OS-specific code.
- Works on top of mods that change sail or oar strength (FasterBoats, ShipConfig): the engine adds to what they give.
- Modded boats with a hold get an engine too.

## Uninstalling

Nothing is saved. Your bones are still in the hold; the boat is just a boat again.

## Links

- Source and bug reports: https://github.com/algo7/BoneEngine (issues welcome)
- Changes: the Changelog tab
- Made with AI assistance.
- Built with [BepInEx](https://github.com/BepInEx/BepInEx) and HarmonyX. MIT license.
