# Winds of Magic Restore

A [Mount & Blade II: Bannerlord](https://www.taleworlds.com/) mod for the **The Old Realms (TOR)** total conversion. It lets spellcasters regain their **Winds of Magic** (spell mana) *during* battle through configurable, in-combat rewards instead of waiting for the default trickle.

- **Module ID:** `WindsOfMagicRestore`
- **Version:** `v1.0.0`
- **Type:** Singleplayer only
- **Requires:** The Old Realms (`TOR_Core`) and its dependency chain

---

## Contents

- [Quick start](#quick-start)
  - [Commands](#commands)
  - [Build it](#build-it)
  - [Install it](#install-it)
  - [Check that it works](#check-that-it-works)
- [What it does](#what-it-does)
  - [How rewards flow](#how-rewards-flow)
- [Configuration](#configuration)
  - [Kill rewards](#kill-rewards)
  - [Augment kill rewards](#augment-kill-rewards)
  - [Heal rewards](#heal-rewards)
  - [Passive regen](#passive-regen)
  - [Reserved (not implemented yet)](#reserved-not-implemented-yet)
- [Project structure](#project-structure)
- [Technical details](#technical-details)
  - [Building from source](#building-from-source)
- [Troubleshooting](#troubleshooting)

---

## Quick start

### Commands

The mod registers one in-game console command (open the console with the developer console mod / `~` if enabled):

| Command | What it does |
| --- | --- |
| `wom.diagnostics` | Prints an OK / MISSING report for every TOR_Core hook, Harmony patch target, and the MCM settings. Use this first if rewards aren't working — it tells you exactly which integration point failed (usually after a TOR or Bannerlord update). |

There are **no hotkeys**. All behavior is controlled through the in-game settings menu (see [Configuration](#configuration)).

### Build it

From the repo root, run the one-step build script:

```bat
build.bat
```

Or build the project directly:

```bat
dotnet build src\WindsOfMagicRestore\WindsOfMagicRestore.csproj -c Release
```

This produces:

```
build\WindsOfMagicRestore\
├── SubModule.xml
└── bin\Win64_Shipping_Client\
    ├── WindsOfMagicRestore.dll
    └── WindsOfMagicRestore.pdb
```

### Install it

1. Copy the entire `build\WindsOfMagicRestore\` folder into your game's `Modules` directory:

   ```
   <Bannerlord Install>\Modules\WindsOfMagicRestore\
   ```

   The final layout in-game should be:

   ```
   Modules\WindsOfMagicRestore\
   ├── SubModule.xml
   └── bin\Win64_Shipping_Client\
       └── WindsOfMagicRestore.dll
   ```

2. In the Bannerlord launcher, enable the modules in this load order (all dependencies must be **above** this mod):

   1. `Native`, `SandBoxCore`, `Sandbox`, `StoryMode` (base game)
   2. `Bannerlord.Harmony`
   3. `Bannerlord.ButterLib`
   4. `Bannerlord.UIExtenderEx`
   5. `Bannerlord.MBOptionScreen` — **Mod Configuration Menu v5**, required for the settings UI
   6. `TOR_Core` — The Old Realms
   7. `WindsOfMagicRestore`

3. Launch the game.

### Check that it works

1. **In the launcher:** confirm `WindsOfMagicRestore` is checked and has no missing-dependency warnings.
2. **Settings menu:** Main menu → **Options → Mod Options → Winds of Magic Restore**. If the page appears, the mod loaded.
3. **In a battle:** open the console and run `wom.diagnostics`. Every line should read **OK**. Any **MISSING** line points at a broken integration (the matching reward type will be disabled).
4. **Live test:** start a battle with a Winds of Magic user, kill an enemy, and watch the Winds bar rise (with default settings you get `1.0` winds per Tier 1 kill).

If something is wrong, the mod also logs warnings such as:

```
[WindsOfMagicRestore] Could not apply TOR_Core patches; heal and augment rewards may be disabled.
```

---

## What it does

By default, Winds of Magic only recovers slowly, which can leave casters stranded mid-fight. This mod gives you several **opt-in ways to earn winds back during combat**, each independently configurable:

- **Kill rewards** — gain winds whenever **you** land a killing blow, scaled by the victim's troop tier (1–6).
- **Augment kill rewards** — gain winds when a troop you've **buffed** with an augment (or, optionally, a heal) spell lands a killing blow.
- **Heal rewards** — gain winds based on the total HP restored when one of your heal spells finishes.
- **Passive regeneration** — a steady trickle of winds every second while in battle, independent of everything above.

All winds are granted to your main hero (`Hero.MainHero`).

### How rewards flow

```
Battle starts
  └─ tracking is reset

You cast an augment / heal spell
  └─ the cast is registered for buff tracking

You kill an enemy
  └─ tier-based kill reward

A troop you buffed kills an enemy
  └─ augment-kill reward (only while the buff is active)

A heal spell ends
  └─ reward based on total HP healed

Every battle tick
  └─ passive per-second regen
```

---

## Configuration

Open **Options → Mod Options → Winds of Magic Restore** (provided by Mod Configuration Menu v5). Settings are saved as JSON and take effect without a restart.

### Kill rewards

| Setting | Default | Description |
| --- | --- | --- |
| `WindsPerKillTier1` … `WindsPerKillTier6` | Tier 1 = `1.0`, rest = `0` | Winds gained when **you** kill an enemy of that tier. |

### Augment kill rewards

| Setting | Default | Description |
| --- | --- | --- |
| `WindsPerAugmentKillTier1` … `WindsPerAugmentKillTier6` | Tier 1 = `1.0`, rest = `0` | Winds gained when a troop you **augmented** kills an enemy of that tier. |

### Heal rewards

| Setting | Default | Description |
| --- | --- | --- |
| `WindsPerHealBlock` | `1.0` | Winds granted per heal "block" when a heal spell ends. |
| `HealHpPerWind` | `100.0` | HP healed per block. At defaults, healing 100 HP = 1 block = 1 wind. |
| `CountHealSpellsAsAugment` | `false` | If enabled, healed troops count toward **augment-kill** rewards and the heal-end reward is disabled for heal spells. |

### Passive regen

| Setting | Default | Description |
| --- | --- | --- |
| `WindsPerSecond` | `0.05` | Passive winds gained per second during battle. |

### Reserved (not implemented yet)

| Setting | Default | Description |
| --- | --- | --- |
| `WindsOnDamageDealt` | `0` | Placeholder, currently inactive. |
| `WindsPerCampaignTick` | `0` | Placeholder, currently inactive. |

---

## Project structure

```
WindsOfMagic\
├── build.bat                          # One-step Release build + install/setup instructions
├── README.md
└── src\WindsOfMagicRestore\
    ├── WindsOfMagicRestore.csproj     # .NET project (net472)
    ├── SubModule.cs                   # Entry point, applies Harmony patches
    ├── _Module\SubModule.xml          # Bannerlord module manifest
    ├── Behaviors\
    │   └── WindsRestoreBehavior.cs    # Kill rewards, augment-kill rewards, passive regen
    ├── Patches\
    │   ├── CreateSpellSessionPatch.cs # Registers augment/heal casts for buff tracking
    │   └── FinalizeSessionPatch.cs    # Heal rewards when a spell session ends
    ├── Settings\
    │   └── WindsOfMagicRestoreSettings.cs  # MCM settings + reward calculations
    └── Utilities\
        ├── AgentPartyHelper.cs        # Main-party membership checks
        ├── AugmentBuffTracker.cs      # Tracks active player augment/heal buffs
        ├── DiagnosticsCommand.cs      # `wom.diagnostics` console command
        ├── SpellEffectTypeHelper.cs   # Heal/Augment ability-effect type helpers
        └── TorWindsApi.cs             # Reflection bridge to TOR_Core's AddWindsOfMagic
```

---

## Technical details

| Item | Value |
| --- | --- |
| Target framework | .NET Framework 4.7.2 (`net472`) |
| Game platform | `Win64_Shipping_Client` |
| Mod loader | Native Bannerlord submodule (`MBSubModuleBase` + `SubModule.xml`) |
| Multiplayer | Not supported |

**Dependencies (NuGet):**

| Package | Version | Role |
| --- | --- | --- |
| `Bannerlord.ReferenceAssemblies` | `1.3.15.110062` | Game API references (Bannerlord v1.3.15) |
| `Bannerlord.MCM` | `5.11.3` | Mod Configuration Menu settings UI |
| `Lib.Harmony` | `2.2.2` | Runtime patching of TOR_Core methods (Harmony ID `com.windsofmagic.restore`) |

**TOR_Core integration (all resolved via reflection, so a TOR update can break a hook without crashing the game):**

- `Extensions.HeroExtensions.AddWindsOfMagic` — grants winds to the player
- `Extensions.AgentExtensions.BelongsToMainParty` — identifies your troops
- `AbilitySystem.AbilityManagerMissionLogic.CreateSpellSession` / `FinalizeSession` — spell lifecycle hooks
- `AbilitySystem.SpellCasting.SpellCastSession` — reads `TotalHealingDone`, `Caster`, `AbilityTemplate`
- `BattleMechanics.StatusEffect.StatusEffectComponent` / `StatusEffect` — active-buff detection

### Building from source

There is no Visual Studio solution. Build the project directly with the .NET SDK:

```bat
dotnet build src\WindsOfMagicRestore\WindsOfMagicRestore.csproj -c Release
```

The build automatically copies `_Module\SubModule.xml` into the output folder so it's ready to drop into `Modules\`.

---

## Troubleshooting

| Symptom | Check |
| --- | --- |
| No settings page in Mod Options | Enable **Bannerlord.MBOptionScreen** (MCM v5) in the launcher. |
| Winds never increase | Run `wom.diagnostics` in-battle and look for **MISSING** lines. |
| Heal/augment rewards don't fire | A TOR_Core method signature likely changed; the patch couldn't apply (see log warnings). |
| Mod won't load | Confirm load order — every dependency, especially `TOR_Core`, must be enabled and listed above this mod. |
