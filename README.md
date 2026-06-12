# Winds of Magic Restore

A [Mount & Blade II: Bannerlord](https://www.taleworlds.com/) mod for the **The Old Realms (TOR)** total conversion. It lets spellcasters regain their **Winds of Magic** (spell mana) *during* battle through configurable, in-combat rewards instead of waiting for the default trickle.

- **Module ID:** `WindsOfMagicRestore`
- **Version:** `v1.0.0`
- **Type:** Singleplayer only
- **Requires:** The Old Realms (`TOR_Core`) and its dependency chain

---

## Contents

- [Quick start](#quick-start)
  - [Prerequisites](#prerequisites)
  - [Build it](#build-it)
  - [Install it](#install-it)
  - [Update an existing install](#update-an-existing-install)
  - [Check that it works](#check-that-it-works)
  - [Commands](#commands)
- [What it does](#what-it-does)
  - [How rewards flow](#how-rewards-flow)
  - [Which kills count](#which-kills-count)
- [Configuration](#configuration)
- [Project structure](#project-structure)
- [Technical details](#technical-details)
- [Troubleshooting](#troubleshooting)

---

## Quick start

### Prerequisites

Before building, make sure you have:

| Requirement | Notes |
| --- | --- |
| [.NET SDK](https://dotnet.microsoft.com/download) | Any recent SDK (6, 7, or 8) is fine. The mod targets **.NET Framework 4.7.2**; the SDK builds it without a separate install. |
| Windows | Bannerlord modding target. Build on Windows (or WSL with `dotnet` available). |
| Internet on first build | NuGet restores `Bannerlord.ReferenceAssemblies`, `Bannerlord.MCM`, and `Lib.Harmony` automatically. |

Check the SDK is available:

```bat
dotnet --version
```

You do **not** need Visual Studio. There is no `.sln` file.

### Build it

Open a terminal in the **repo root** (the folder that contains `build.bat`).

**Windows (recommended):**

```bat
build.bat
```

**Or build directly:**

```bat
dotnet build src\WindsOfMagicRestore\WindsOfMagicRestore.csproj -c Release
```

**Git Bash / WSL** (if `build.bat` is not found):

```bash
dotnet build src/WindsOfMagicRestore/WindsOfMagicRestore.csproj -c Release
```

A successful build prints `Сборка успешно завершена` / `Build succeeded` and writes:

```
build\WindsOfMagicRestore\
├── SubModule.xml
└── bin\Win64_Shipping_Client\
    ├── WindsOfMagicRestore.dll
    └── WindsOfMagicRestore.pdb
```

Only `SubModule.xml` and `WindsOfMagicRestore.dll` are required in-game. The `.pdb` is optional (useful for debugging).

**Common build failures:**

| Problem | Fix |
| --- | --- |
| `'dotnet' is not recognized` | Install the .NET SDK and reopen the terminal. |
| NuGet restore errors | Check internet, then run `dotnet restore src\WindsOfMagicRestore\WindsOfMagicRestore.csproj`. |
| Build OK but no `build\` folder | You may be in the wrong directory. `cd` to the repo root first. |

Always use **Release** (`-c Release`). Debug builds output to the same path and are not meant for play.

### Install it

1. **Close Bannerlord** if it is running (the launcher can lock the DLL).

2. Copy the **entire** `build\WindsOfMagicRestore\` folder into your game `Modules` directory:

   ```
   <Bannerlord Install>\Modules\WindsOfMagicRestore\
   ```

   Example Steam path:

   ```
   C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\Modules\WindsOfMagicRestore\
   ```

3. Confirm the layout matches this exactly:

   ```
   Modules\WindsOfMagicRestore\
   ├── SubModule.xml
   └── bin\Win64_Shipping_Client\
       └── WindsOfMagicRestore.dll
   ```

   Wrong layouts that **will not work**:
   - DLL sitting next to `SubModule.xml` (missing `bin\Win64_Shipping_Client\`)
   - Only copying the DLL without `SubModule.xml`
   - Nested folder like `Modules\WindsOfMagicRestore\WindsOfMagicRestore\...`

4. In the Bannerlord launcher, enable modules in this order (dependencies **above** this mod):

   1. `Native`, `SandBoxCore`, `Sandbox`, `StoryMode`
   2. `Bannerlord.Harmony`
   3. `Bannerlord.ButterLib`
   4. `Bannerlord.UIExtenderEx`
   5. `Bannerlord.MBOptionScreen` — **Mod Configuration Menu v5** (settings UI)
   6. `TOR_Core`
   7. `WindsOfMagicRestore`

5. Launch the game.

### Update an existing install

After rebuilding:

1. Close Bannerlord.
2. Replace `Modules\WindsOfMagicRestore\` with the new `build\WindsOfMagicRestore\` folder (or overwrite at least `WindsOfMagicRestore.dll`).
3. Relaunch. **No restart required** for code changes, but the game must reload the DLL.

If behaviour looks unchanged after an update, you are usually still running an old DLL — double-check the file timestamp on `WindsOfMagicRestore.dll`.

### Check that it works

Work through these in order:

1. **Launcher** — `WindsOfMagicRestore` is enabled with no missing-dependency warnings.
2. **Settings** — Main menu → **Options → Mod Options → Winds of Magic Restore**. If this page exists, the mod loaded and MCM is working.
3. **Diagnostics** — In a battle, open the developer console and run:
   ```
   wom.diagnostics
   ```
   Every line should be **OK**. Any **MISSING** line means that feature is disabled (often after a TOR_Core update).
4. **API smoke test** — Still in battle, run:
   ```
   wom.add 5
   ```
   The winds bar should increase by 5. If this works, TOR integration is fine and any remaining issue is reward logic or settings.
5. **Kill test** — Kill an enemy (melee or spell). With defaults you get **1.0** winds per kill (all tiers). Passive regen (`0.05`/sec) is a slower sanity check.

If something fails, see [Troubleshooting](#troubleshooting).

### Commands

Requires the developer console (vanilla hotkey `~` when enabled, or a console mod):

| Command | What it does |
| --- | --- |
| `wom.diagnostics` | OK / MISSING report for TOR hooks, Harmony patch targets, and MCM. Run this first when rewards stop working. |
| `wom.add <amount>` | Adds winds directly (e.g. `wom.add 5`). Use to verify TOR wind gain works in the current battle. |

There are **no hotkeys**. All reward amounts are configured in Mod Options.

---

## What it does

By default, Winds of Magic only recovers slowly, which can leave casters stranded mid-fight. This mod gives you several **independently configurable** ways to earn winds back during combat:

- **Kill rewards** — winds when **you** get kill credit (melee, ranged, direct spell hits, bombardment zones, hex/DOT ticks).
- **Augment kill rewards** — winds when a troop you **buffed** kills an enemy while the buff is active.
- **Heal rewards** — winds when a heal spell ends, based on total HP restored.
- **Passive regeneration** — steady per-second trickle during battle.

All winds are granted to your main hero (`Hero.MainHero`).

### How rewards flow

```
Battle starts
  └─ tracking is reset

You cast an augment / heal spell
  └─ cast is registered for buff tracking

You kill an enemy
  └─ tier-based kill reward

A troop you buffed kills an enemy
  └─ augment-kill reward (while buff is active)

A heal spell ends
  └─ reward based on total HP healed

Every battle tick
  └─ passive per-second regen
```

### Which kills count

| Kill type | Supported |
| --- | --- |
| Melee / ranged | Yes |
| Direct spell damage | Yes |
| Bombardment / area damage zones | Yes (via TOR spell sessions) |
| Hex and other DOT status effects | Yes (including ticks after the cast ends) |
| Friendly fire | No |
| Kills with no player involvement | No |

Kill credit uses TOR's `BookSpellKill` hook for spells plus agent-removal fallback for edge cases (e.g. late DOT ticks).

---

## Configuration

Open **Options → Mod Options → Winds of Magic Restore** (MCM v5). Settings are JSON on disk and apply without restarting the game.

### Kill rewards

| Setting | Default | Description |
| --- | --- | --- |
| **Winds per kill** | `1.0` | Same reward for every enemy tier (shown when per-tier mode is off). |
| **Different amount per tier** | Off | Enable to set Tier 1–6 individually. |
| Tier 1 … Tier 6 | `1.0` / `0` | Per-tier values (only when toggle is on). |

### Augment kill rewards

| Setting | Default | Description |
| --- | --- | --- |
| **Winds per augment kill** | `1.0` | Same reward for all tiers when per-tier mode is off. |
| **Different amount per tier** | Off | Per-tier augment-kill values. |

### Heal rewards

| Setting | Default | Description |
| --- | --- | --- |
| **Winds per heal block** | `1.0` | Winds per heal block when a heal spell ends. |
| **HP per block** | `100.0` | HP healed per block (100 HP = 1 wind at defaults). |
| **Heals count as augments** | Off | Healed troops also qualify for augment-kill rewards. Heal-end rewards still apply. |

### Passive regen

| Setting | Default | Description |
| --- | --- | --- |
| **Winds per second** | `0.05` | Passive in-battle regen. |

### Reserved (not implemented yet)

| Setting | Default |
| --- | --- |
| Winds on damage dealt | `0` |
| Winds per campaign tick | `0` |

**Settings tip:** If you enabled **Different amount per tier** and only set Tier 1, higher-tier enemies give **0** winds. For one value for all tiers, leave that toggle **off** and use **Winds per kill**.

---

## Project structure

```
├── build.bat
├── README.md
├── build\WindsOfMagicRestore\          # install this folder
└── src\WindsOfMagicRestore\
    ├── WindsOfMagicRestore.csproj
    ├── SubModule.cs
    ├── _Module\SubModule.xml
    ├── Behaviors\WindsRestoreBehavior.cs
    ├── Patches\
    │   ├── BookSpellKillPatch.cs       # spell / DOT / area kill rewards
    │   ├── CreateSpellSessionPatch.cs
    │   └── FinalizeSessionPatch.cs
    ├── Settings\WindsOfMagicRestoreSettings.cs
    └── Utilities\
        ├── KillCreditHelper.cs
        ├── KillRewardService.cs
        ├── TorWindsApi.cs
        └── ...
```

---

## Technical details

| Item | Value |
| --- | --- |
| Target framework | .NET Framework 4.7.2 (`net472`) |
| Game platform | `Win64_Shipping_Client` |
| Harmony ID | `com.windsofmagic.restore` |

**NuGet dependencies:**

| Package | Version |
| --- | --- |
| `Bannerlord.ReferenceAssemblies` | `1.3.15.110062` |
| `Bannerlord.MCM` | `5.11.3` |
| `Lib.Harmony` | `2.2.2` |

**TOR_Core integration** (reflection — a TOR update can break a hook without crashing):

- `HeroExtensions.AddWindsOfMagic` / `AddCustomResource` — grant winds
- `AgentExtensions.BelongsToMainParty` — party troop checks
- `AbilityManagerMissionLogic` — `CreateSpellSession`, `FinalizeSession`, `BookSpellKill`
- `SpellCastSession` — caster, healing totals
- `StatusEffectComponent` / `StatusEffect` — augment/heal buff tracking

---

## Troubleshooting

| Symptom | What to do |
| --- | --- |
| Build fails | Install .NET SDK; run from repo root; try `dotnet restore` then rebuild. |
| Mod missing in launcher | Copy full `build\WindsOfMagicRestore\` folder; check `SubModule.xml` is present. |
| No Mod Options page | Enable **Bannerlord.MBOptionScreen** (MCM v5) above this mod. |
| `wom.diagnostics` shows MISSING | TOR_Core likely updated; that hook needs a mod update. Kill rewards need `AddWindsOfMagic` + `BookSpellKill` OK. |
| `wom.add` works, kills do not | Check **Winds per kill** is above 0. If per-tier mode is on, check the victim's tier. Confirm you are at **below max winds** (TOR caps gains). |
| `wom.add` does not work | TOR_Core not loaded, wrong load order, or API changed — fix MISSING diagnostics first. |
| Spell kills fail, melee works | Rebuild and reinstall; ensure `BookSpellKill patch target` is OK in diagnostics. |
| Heal/augment rewards fail | `CreateSpellSession` / `FinalizeSession` or status-effect hooks MISSING — see log warning about TOR patches. |
| Changes after rebuild have no effect | Game was open during copy, or wrong `Modules` path — check DLL timestamp. |
| Log warning about patches | `[WindsOfMagicRestore] Could not apply TOR_Core patches; heal and augment rewards may be disabled.` — kill rewards may still work if `BookSpellKill` patched. |

Log file (if needed): `%USERPROFILE%\Documents\Mount and Blade II Bannerlord\logs\rgl_log_*.txt` — search for `WindsOfMagicRestore`.
