# BoneEngine

Bone fragments have a job now. Drop them in your boat's hold and the boat gets an engine: it pushes the way your gear
points, reverse included, and burns a bone every so often while it does. Only the one steering needs the mod.

## How to Use

1. Put bone fragments in the hold (Karve, Longship, Drakkar; the Raft has no hold).
2. Sail. The sail gear is the engine gear: rowing and reverse are the slow burn, half sail a bit more, full sail the
   fast burn. No wind needed.
3. Watch the count next to the gear icons while you steer, or look at the rudder when you're not.

## Good to Know

- The engine takes one bone out of the hold and runs on it; the next one goes in when it's used up. So the count is
  what's left to burn, and an active engine at 0 means it's on its last bone. Moored, stopped or empty boats take
  none.
- One bone or a full hold: same push. More bones just run longer.
- Bigger boat, bigger appetite: the Karve sips, the Longship drinks, the Drakkar gulps.
- Out of bones, the boat is back to sails and oars. Drop more in and it picks up again.
- While someone has the hold open, the engine finishes the bone it has and waits for the next.

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
