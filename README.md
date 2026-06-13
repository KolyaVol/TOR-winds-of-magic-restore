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
  - [Which damage counts](#which-damage-counts)
  - [On-screen feedback](#on-screen-feedback)
- [Configuration](#configuration)
  - [Damage reward formula](#damage-reward-formula)
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
6. **Damage test** (optional) — Set **Winds per 100 HP dealt** to something visible like `10` under Melee and ranged damage or Spell damage, then hit an enemy. You should see winds tick up proportionally (see [Damage reward formula](#damage-reward-formula)).

If something fails, see [Troubleshooting](#troubleshooting).

### Commands

Requires the developer console (vanilla hotkey `~` when enabled, or a console mod):

| Command | What it does |
| --- | --- |
| `wom.diagnostics` | Full OK / MISSING report for TOR hooks, Harmony patch targets, and MCM. Run this first when rewards stop working. |
| `wom.export` | Saves the same report to `Documents\Mount and Blade II Bannerlord\WindsOfMagicRestore_diagnostics.txt`. |
| `wom.add <amount>` | Adds winds directly (e.g. `wom.add 5`). Use to verify TOR wind gain works in the current battle. |

There are **no hotkeys**. All reward amounts are configured in Mod Options.

---

## What it does

By default, Winds of Magic only recovers slowly, which can leave casters stranded mid-fight. This mod gives you several **independently configurable** ways to earn winds back during combat:

- **Kill rewards** — winds when **you** or a **party companion** gets kill credit (melee, ranged, direct spell hits, bombardment zones, hex/DOT ticks).
- **Buffed unit kill rewards** — winds when a troop you **buffed** kills an enemy while the buff is active.
- **Heal rewards** — winds when a heal spell ends (yours or a companion's), based on total HP restored.
- **Damage rewards** — winds when **you** or a **party companion** deals melee, ranged, or spell damage (scaled per 100 HP; see formula below).
- **Passive regeneration** — steady per-second trickle during battle.

Your main hero always receives winds from your own actions. Companion rewards use **separate MCM values** and can be sent to the main hero only, to the companion only, or given in full to both.

### How rewards flow

```
Battle starts
  └─ tracking is reset; compatibility warning if hooks are missing

You cast an augment / heal spell
  └─ cast is registered for buff tracking

You deal damage (melee, ranged, or spell)
  └─ proportional damage reward per hit

You kill an enemy
  └─ tier-based kill reward

A troop you buffed kills an enemy
  └─ buffed-unit kill reward (while buff is active)

A heal spell ends
  └─ reward based on total HP healed

Every battle tick
  └─ passive per-second regen
```

### Which kills count

| Kill type | Supported |
| --- | --- |
| Melee / ranged (you or companion) | Yes |
| Direct spell damage (you or companion) | Yes |
| Bombardment / area damage zones | Yes (via TOR spell sessions) |
| Hex and other DOT status effects | Yes (including ticks after the cast ends) |
| Party companion kills | Yes | Separate companion kill settings |
| Regular party troop kills | No (buffed-unit kill rewards only) |
| Friendly fire kills | No |
| Kills with no player involvement | No |

Kill credit uses TOR's `BookSpellKill` hook for spells plus agent-removal fallback for edge cases (e.g. late DOT ticks).

### Which damage counts

| Damage type | Supported | Notes |
| --- | --- | --- |
| Melee / ranged (your hits) | Yes | Via TOR `ApplyGeneralDamageModifiers` |
| Melee / ranged (companion hits) | Yes | Separate companion damage settings |
| Spell damage (your casts) | Yes | Via TOR `BookSpellDamage` |
| Spell damage (companion casts) | Yes | Separate companion damage settings |
| Damage to enemies | Yes | Uses **Winds per 100 HP dealt** settings |
| Friendly fire damage | Optional | Separate **Friendly fire winds per 100 HP** settings; default `0` (off) |
| Regular party troop damage | No | Only main hero and party companions count |
| Self-damage | No | Hitting yourself does not grant winds |

Damage rewards apply **per hit** (or per spell damage tick), not only on kill. Melee/ranged and spell damage use separate settings.

### On-screen feedback

When winds are granted in battle, the mod batches small gains and shows a blue message such as **+2.5 Winds of Magic restored** every few seconds. This covers kills, heals, damage, and passive regen.

---

## Configuration

Open **Options → Mod Options → Winds of Magic Restore** (MCM v5). Settings are JSON on disk and apply without restarting the game.

### Kill rewards

| Setting | Default | Description |
| --- | --- | --- |
| **Enable kill rewards** | On | Master toggle for kill-based winds. |
| **Winds per kill** | `1.0` | Same reward for every enemy tier (when per-tier mode is off). |
| **Set different winds gain for every tier** | Off | Enable to set Tier 1–9 individually. |
| Tier 1 … Tier 9 | `1.0` / `0` | Per-tier values (only when toggle is on). |

### Buffed unit kills

| Setting | Default | Description |
| --- | --- | --- |
| **Enable buffed unit kill rewards** | On | Master toggle for augment/buff kill rewards. |
| **Winds per kill** | `1.0` | Same reward for all tiers when per-tier mode is off. |
| **Set different winds gain for every tier** | Off | Per-tier buffed-unit kill values (Tier 1–9). |

### Heal rewards

| Setting | Default | Description |
| --- | --- | --- |
| **Winds per selected amount HP** | `1.0` | Winds paid out when a heal spell ends. |
| **HP block for winds** | `100.0` | HP healed per payout block (100 HP = 1 wind at defaults). |
| **Heals count as augments** | Off | Healed troops also qualify for buffed-unit kill rewards. Heal-end rewards still apply. |

Heal payout uses the same proportional idea as damage: `(HP healed ÷ HP block) × winds per block`. Example: 250 HP healed with defaults → `(250 ÷ 100) × 1.0` = **2.5** winds.

### Passive regen

| Setting | Default | Description |
| --- | --- | --- |
| **Winds per second** | `0.05` | Passive in-battle regen. |

### Melee and ranged damage

| Setting | Default | Description |
| --- | --- | --- |
| **Winds per 100 HP dealt** | `0` | Winds restored per **100 HP** of melee or ranged damage **you** deal to enemies. Set above `0` to enable. |
| **Friendly fire winds per 100 HP** | `0` | Same scale, but for damage dealt to **friendly** units. Off by default. |

### Spell damage

| Setting | Default | Description |
| --- | --- | --- |
| **Winds per 100 HP dealt** | `0` | Winds restored per **100 HP** of spell damage **you** deal to enemies. Set above `0` to enable. |
| **Friendly fire winds per 100 HP** | `0` | Same scale for friendly spell damage. Off by default. |

### Companion kill rewards

| Setting | Default | Description |
| --- | --- | --- |
| **Enable companion kill rewards** | On | Master toggle for companion kill-based winds. |
| **Restore winds to** | Main hero | Who receives companion kill winds (see below). |
| **Winds per kill** | `1.0` | Companion kill reward for all tiers when per-tier mode is off. |
| **Set different winds gain for every tier** | Off | Per-tier companion kill values (Tier 1–9). |

### Companion heal rewards

| Setting | Default | Description |
| --- | --- | --- |
| **Enable companion heal rewards** | On | Grant winds when a companion's heal spell ends. |
| **Restore winds to** | Main hero | Who receives companion heal winds. |
| **Winds per selected amount HP** | `1.0` | Companion heal payout block multiplier. |
| **HP block for winds** | `100.0` | HP healed per companion wind payout block. |

### Companion melee and ranged damage

| Setting | Default | Description |
| --- | --- | --- |
| **Restore winds to** | Main hero | Who receives companion melee/ranged damage winds. Also applies to companion spell damage. |
| **Winds per 100 HP dealt** | `0` | Companion winds per 100 HP of melee/ranged damage. Set above `0` to enable. |
| **Friendly fire winds per 100 HP** | `0` | Same scale for companion melee/ranged friendly fire. |

### Companion spell damage

| Setting | Default | Description |
| --- | --- | --- |
| **Winds per 100 HP dealt** | `0` | Companion winds per 100 HP of spell damage dealt. |
| **Friendly fire winds per 100 HP** | `0` | Same scale for companion spell friendly fire. |

Restore target uses the dropdown under **Companion melee and ranged damage** for all companion damage types.

#### Companion restore targets

| Option | Behavior |
| --- | --- |
| **Main hero** | Full reward goes to the main hero only. |
| **Self** | Full reward goes to the companion only. |
| **Both** | Main hero and companion each receive the full configured amount. |

### Diagnostics

| Setting | Default | Description |
| --- | --- | --- |
| **Warn at battle start** | On | One-time on-screen warning if TOR hooks are missing. Full report: `wom.diagnostics` or `wom.export` in the battle console. |

### Damage reward formula

Damage rewards scale **proportionally** with HP dealt. The block size is fixed at **100 HP** — the setting value is winds earned per 100 damage, not a flat payout per hit.

```
winds gained = (damage dealt ÷ 100) × winds per 100 HP dealt
```

**Example:** You set **Winds per 100 HP dealt** to `10` and deal **25** HP in one hit:

```
(25 ÷ 100) × 10 = 2.5 winds
```

More examples with **Winds per 100 HP dealt = 10**:

| Damage dealt | Winds gained |
| --- | --- |
| 25 HP | 2.5 |
| 50 HP | 5.0 |
| 100 HP | 10.0 |
| 175 HP | 17.5 |

The same formula applies to spell damage and to friendly-fire damage settings (when those are above `0`). Partial amounts add up across many small hits — there is no rounding to whole winds until the game applies the gain.

**Settings tips:**

- If you enabled **Set different winds gain for every tier** and only set Tier 1, higher-tier enemies give **0** winds. For one value for all tiers, leave that toggle **off** and use **Winds per kill**.
- Kill rewards and damage rewards **stack** — a killing blow can grant both the kill payout and winds for the damage on that hit.
- Set damage settings to `0` to disable that reward type entirely.

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
    │   ├── ApplyGeneralDamageModifiersPatch.cs  # melee/ranged damage rewards
    │   ├── BookSpellDamagePatch.cs              # spell damage rewards
    │   ├── BookSpellKillPatch.cs                # spell / DOT / area kill rewards
    │   ├── CreateSpellSessionPatch.cs           # augment/heal cast registration
    │   └── FinalizeSessionPatch.cs              # heal-end rewards
    ├── Settings\
    │   ├── WindsOfMagicRestoreSettings.cs       # MCM UI
    │   ├── WindsOfMagicRestoreSettings.Rewards.cs
    │   ├── CompanionWindsRecipientMode.cs
    │   └── CompanionWindsRecipientOption.cs
    ├── Domain\                                  # reward logic
    │   ├── KillRewardService.cs
    │   ├── DamageRewardService.cs
    │   ├── HealRewardService.cs
    │   └── CompanionWindsGrantService.cs
    ├── Battle\                                  # agent/party identification
    │   ├── KillCreditHelper.cs
    │   ├── CompanionHelper.cs
    │   ├── AgentPartyHelper.cs
    │   └── UnitTierHelper.cs
    ├── Integration\                             # TOR reflection & spell plumbing
    │   ├── TorTypes.cs
    │   ├── TorWindsApi.cs
    │   ├── SpellCastRegistry.cs
    │   ├── SpellSessionResolver.cs
    │   ├── StatusEffectReflection.cs
    │   ├── StatusEffectHelper.cs
    │   ├── AugmentBuffTracker.cs
    │   └── SpellEffectTypeHelper.cs
    └── Infrastructure\                          # logging, diagnostics, UI feedback
        ├── ModLog.cs
        ├── ModGuard.cs
        ├── ModTrace.cs
        ├── ModDiagnostics.cs
        ├── DiagnosticsCommand.cs
        ├── WindsRestoreMessages.cs
        └── KillRewardTracker.cs
```

---

## Technical details

| Item | Value |
| --- | --- |
| Target framework | .NET Framework 4.7.2 (`net472`) |
| Game platform | `Win64_Shipping_Client` |
| Harmony ID | `com.windsofmagic.restore` |
| Harmony patches | 5 (`FinalizeSession`, `CreateSpellSession`, `BookSpellKill`, `BookSpellDamage`, `ApplyGeneralDamageModifiers`) |

**NuGet dependencies:**

| Package | Version |
| --- | --- |
| `Bannerlord.ReferenceAssemblies` | `1.3.15.110062` |
| `Bannerlord.MCM` | `5.11.3` |
| `Lib.Harmony` | `2.2.2` |

**TOR_Core integration** (reflection — a TOR update can break a hook without crashing):

- `HeroExtensions.AddWindsOfMagic` / `AddCustomResource` — grant winds
- `AgentExtensions.BelongsToMainParty` — party troop checks
- `AbilityManagerMissionLogic` — `CreateSpellSession`, `FinalizeSession`, `BookSpellKill`, `BookSpellDamage`
- `TORAgentApplyDamageModel.ApplyGeneralDamageModifiers` — melee/ranged damage tracking
- `SpellCastSession` — caster, healing totals
- `StatusEffectComponent` / `StatusEffect` — augment/heal buff tracking

---

## Troubleshooting

| Symptom | What to do |
| --- | --- |
| Build fails | Install .NET SDK; run from repo root; try `dotnet restore` then rebuild. |
| Mod missing in launcher | Copy full `build\WindsOfMagicRestore\` folder; check `SubModule.xml` is present. |
| No Mod Options page | Enable **Bannerlord.MBOptionScreen** (MCM v5) above this mod. |
| `wom.diagnostics` shows MISSING | TOR_Core likely updated; that hook needs a mod update. See which feature each patch powers in the report. |
| `wom.add` works, kills do not | Check **Enable kill rewards** and **Winds per kill** > 0. If per-tier mode is on, check the victim's tier (1–9). Confirm you are **below max winds** (TOR caps gains). |
| `wom.add` works, damage does not | Set **Winds per 100 HP dealt** above `0`. Check `BookSpellDamage` (spells) or `ApplyGeneralDamageModifiers` (melee/ranged) is OK in diagnostics. Only **your** hits count. |
| `wom.add` does not work | TOR_Core not loaded, wrong load order, or API changed — fix MISSING diagnostics first. |
| Spell kills fail, melee works | Rebuild and reinstall; ensure `BookSpellKill patch target` is OK in diagnostics. |
| Heal/buffed-unit rewards fail | `CreateSpellSession` / `FinalizeSession` or status-effect hooks MISSING — see log warning about TOR patches. |
| Yellow warning at battle start | Compatibility issue detected. Run `wom.diagnostics` or disable **Warn at battle start** in Diagnostics settings. |
| Changes after rebuild have no effect | Game was open during copy, or wrong `Modules` path — check DLL timestamp. |
| Log warning about patches | `[WindsOfMagicRestore] Could not apply TOR_Core patches; ...` — features tied to missing patches are disabled; others may still work. |

Log file (if needed): `%USERPROFILE%\Documents\Mount and Blade II Bannerlord\logs\rgl_log_*.txt` — search for `WindsOfMagicRestore`.
