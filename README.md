# BoneEngine

[![CI](https://github.com/algo7/BoneEngine/actions/workflows/ci.yml/badge.svg)](https://github.com/algo7/BoneEngine/actions/workflows/ci.yml)
[![Thunderstore](https://img.shields.io/badge/Thunderstore-BoneEngine-blue)](https://thunderstore.io/c/valheim/p/Algo7/BoneEngine/)

A [BepInEx](https://github.com/BepInEx/BepInEx) mod for Valheim: bone fragments in a boat's hold are engine fuel.
Players without the mod only ever see vanilla things.

What it does for players is in [package/README.md](package/README.md), which is also the mod's Thunderstore page.
Changes: [CHANGELOG.md](CHANGELOG.md).

Made with AI assistance.

## How it works

A boat's movement is computed by one player's game, the owner of its network object (`Ship.CustomFixedUpdate` returns
early for everyone else). The boat's cargo chest has no network object of its own: its `Container` points at the
ship's, so hull and hold share one owner, and opening the hold makes the opener the owner (vanilla).

Three Harmony postfixes:

- `Ship.CustomFixedUpdate`: every client checks whether the local player is at the rudder without owning the boat
  and, once the hold has been seen closed and the owner unchanged for two seconds each, claims it
  (`ZNetView.ClaimOwnership`, the game's own hand-over). The owner, when steering or when nobody is, runs the
  engine: with no bone loaded it takes one from the hold through `Inventory.RemoveItem` (the vanilla chest save
  path, so everyone sees the count; never while the hold is open, and only after owning the boat for 1.5 s, so a
  fresh owner's once-a-second mirror of the hold is current before it is written back), then adds an impulse along
  the boat's forward (backward in reverse) scaled by the gear (1/3, 2/3, 1) and the boat's own `m_sailForceFactor`
  while the hull is in the water, and drains the loaded bone by the gear. A bone lasts 15 high-gear seconds on the
  Longship and scales with the same factor on other boats, so burn follows push (Karve 25 s, Drakkar about 9 s).
  Losing ownership clears the loaded bone, so a switch costs at most that bone.
- `ShipControlls.GetHoverText`: the rudder's hover text gains the count.
- `Hud.UpdateShipHud`: the steering panel gains a text line (a copy of the HUD's health text) with the count and
  whether the engine is running.

Nothing is saved: no ZDO keys, no items, no prefabs. "How much of the loaded bone is left" is memory in the owner's
game; any owner change (rudder swap, opening the hold, logout) starts over on a fresh bone, so a switch costs at most
the bone already loaded and tricks with the chest cost bones rather than save them.

## Building

Needs the .NET SDK (8+) and the game's DLLs (a local Valheim install, or the dedicated server's).

```sh
make build      # bin/Release/BoneEngine.dll
make test       # unit tests: the engine rules, and the game members the patches use
make package    # Algo7-BoneEngine-<version>.zip for r2modman's "Import local mod"
```

The version comes from the git tag (MinVer).

## CI / CD

CI builds and tests against the free dedicated server's DLLs (`.github/scripts/valheim-managed.sh`). A `vX.Y.Z` tag
with a matching `## X.Y.Z` section in `CHANGELOG.md` releases to GitHub and Thunderstore after manual approval.

## Layout

| Path | What |
|---|---|
| `src/Plugin.cs` | Entry point, patching |
| `src/EngineRules.cs` | The engine rules (Unity-free, unit tested) |
| `src/ShipEngine.cs` | Per-ship state, the hold, the push, the burn, the take-over |
| `src/EnginePatches.cs`, `src/HoverPatches.cs`, `src/HudPatches.cs` | The three patches |
| `tests/` | Unit tests (`make test`) |
| `package/` | Thunderstore page and icon |

## License

MIT
