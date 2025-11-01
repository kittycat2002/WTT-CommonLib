# WTT-CommonLib

A comprehensive modding library for SPT that simplifies adding custom content to Escape from Tarkov. WTT-CommonLib handles both server-side database modifications and client-side resource loading automatically - you just configure your content and call the appropriate services.

## Table of Contents
- [Features](#features)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Available Server Services](#available-server-services)
  - [CustomItemServiceExtended](#customitemserviceextended)
  - [CustomLocaleService](#customlocaleservice)
  - [CustomQuestService](#customquestservice)
  - [CustomQuestZoneService](#customquestzoneservice)
  - [CustomVoiceService](#customvoiceservice)
  - [CustomHeadService](#customheadservice)
  - [CustomClothingService](#customclothingservice)
  - [CustomBotLoadoutService](#custombotloadoutservice)
  - [CustomLootspawnService](#customlootspawnservice)
  - [CustomAssortSchemeService](#customassortschemeservice)
  - [CustomStaticSpawnService](#customstaticspawnservice)
  - [CustomHideoutRecipeService](#customhideoutrecipeservice)
  - [CustomRigLayoutService](#customriglayoutservice)
  - [CustomSlotImageService](#customslotimageservice)
  - [CustomBuffService](#custombuffservice)
  - [CustomProfileService](#customprofileservice)
  - [CustomWeaponPresetService](#customweaponpresetservice)
- [Example Mod Structure](#example-mod-structure)
- [Available Client Services](#available-client-services)
  - [CustomTemplateIdToObjectService](#customtemplateidtoobjectservice) 

## Features

**Simplified Item Creation** - Clone and modify items with JSON configs  
**Quest System** - Create custom quests with zone-based objectives  
**Character Customization** - Add heads, voices, and clothing  
**Bot Configuration** - Customize AI loadouts and equipment  
**Loot Management** - Control item spawns and distributions  
**Hideout Integration** - Add crafting recipes  
**Multi-language Support** - Easy localization system  

## Installation

1. Download WTT-CommonLib from GitHub or SPT Forge
2. Open the .7z file
3. Drag the SPT and BepInEx folders into your main SPT directory (the one that contains EscapeFromTarkov.exe)

**FOR MOD AUTHORS**

4. Reference the SPT/user/mods/WTT-ServerCommonLib/WTTServerCommonLib.dll in your project
5. Inject `WTTServerCommonLib` through the constructor


## Quick Start

Here's a minimal example showing how to use WTT-CommonLib:

```csharp
using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using Range = SemanticVersioning.Range;

namespace YourModName;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.yourname.yourmod";
    public override string Name { get; init; } = "Your Mod Name";
    public override string Author { get; init; } = "Your Name";
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public override Range SptVersion { get; init; } = new("4.0.1");
    public override string License { get; init; } = "MIT";
    public override bool? IsBundleMod { get; init; } = true;
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 2)]
public class YourMod(
    WTTServerCommonLib.WTTServerCommonLib wttCommon
) : IOnLoad
{
    public async Task OnLoad()
    {
        // Use WTT-CommonLib services
        await wttCommon.CustomItemServiceExtended.CreateCustomItems();
        await wttCommon.CustomLocaleService.CreateCustomLocales();
        
        await Task.CompletedTask;
    }
}
```

### Key Points:
- Inject `WTTServerCommonLib.WTTServerCommonLib` through the constructor
- Set `TypePriority = OnLoadOrder.PostDBModLoader + 2` to load after the database
- Utilize any of the public services available

---

## Available Server Services

### CustomItemServiceExtended

**Purpose**: Creates custom items (weapons, armor, consumables, etc.) and integrates them into traders, loot tables, and bot loadouts.

**Usage**:
```csharp
// Use default path (db/CustomItems/)
await wttCommon.CustomItemServiceExtended.CreateCustomItems();

// Or specify custom path
await wttCommon.CustomItemServiceExtended.CreateCustomItems( 
    Path.Join("db", "MyCustomItemFolder"));
```

**Configuration**: Place JSON files in `db/CustomItems/` (or your custom path):

<details>
<summary> Example Item Configuration (Click to expand)</summary>

```json
{
  "6761b213607f9a6f79017aef": {
    "itemTplToClone": "572b7adb24597762ae139821",
    "parentId": "6815465859b8c6ff13f94026",
    "handbookParentId": "5b5f6f8786f77447ed563642",
    "overrideProperties": {
      "ExaminedByDefault": true,
      "Prefab": {
        "path": "Gear_Belts/belt_fannypack.bundle",
        "rcid": ""
      },
      "Width": 2,
      "Height": 2,
      "Weight": 0.46,
      "Grids": [
        {
          "_id": "belt_fannypackgrid",
          "_name": "main",
          "_parent": "6761b213607f9a6f79017aef",
          "_props": {
            "cellsH": 3,
            "cellsV": 2,
            "filters": [
              {
                "Filter": ["54009119af1c881c07000029"]
              }
            ]
          }
        }
      ]
    },
    "locales": {
      "en": {
        "name": "Fanny Pack",
        "shortName": "Fanny Pack",
        "description": "A fanny pack that can be worn at the waist."
      }
    },
    "fleaPriceRoubles": 10900,
    "handbookPriceRoubles": 7250,
    "addtoInventorySlots": ["ArmBand"],
    "addtoHideoutPosterSlots": true,
    "addPosterToMaps": true,
    "posterSpawnProbability": 10,
    "addtoStatuetteSlots": true,
    "addCaliberToAllCloneLocations": true,
    "addtoStaticAmmo": true,
    "staticAmmoProbability": 5,
    "addtoBots": true,
    "addtoSpecialSlots": true,
    "addtoModSlots": true,
    "modSlot": ["mod_muzzle"],
    "addtoHallOfFame": true,
    "hallOfFameSlots": [
      "bigTrophies", 
      "smallTrophies", 
      "dogTags"
    ],
    "addtoGeneratorAsFuel": true,
    "generatorFuelSlotStages": [
      "1",
      "2",
      "3"
    ],
    "addtoEmptyPropSlots": true,
    "emptyPropSlot": {
      "itemToAddTo": "628a66b41d5e41750e314f34",
      "modSlot": "mod_muzzle"
    },
    "addtoStaticLootContainers": true,
    "StaticLootContainers": [
      {
        "ContainerName": "LOOTCONTAINER_DEAD_SCAV",
        "Probability": 54
      }
    ],
    "addtoTraders": true,
    "traders": {
      "RAGMAN": {
        "681ce253b2fd4632d780ca88": {
          "barterSettings": {
            "loyalLevel": 1,
            "unlimitedCount": true,
            "stackObjectsCount": 99
          },
          "barters": [
            {
              "count": 26125,
              "_tpl": "MONEY_ROUBLES"
            }
          ]
        }
      }
    }
  }
}
```
</details>

**Features**:
- Clone existing items and modify properties
- Add to trader inventories with custom barters
- Add to bot loadouts (inherits spawn chances from cloned item)
- Add to loot containers
- Weapon preset support
- Weapon mastery integration
- Hall of Fame integration
- Generator fuel integration
- Hideout poster/statuette integration
- Custom inventory slot placement
- Special slot support
- Caliber-based weapon compatibility
- Mod slot propagation (based on the cloned item's locations)
- Add to slots that are empty by default (e.g., mod_muzzle on Keymount muzzles)

---

### CustomLocaleService

**Purpose**: Handles translations for all your custom content.

**Usage**:
```csharp
await wttCommon.CustomLocaleService.CreateCustomLocales();
// Or specify custom path
await wttCommon.CustomLocaleService.CreateCustomLocales( 
    Path.Join("db", "MyCustomLocalesFolder"));
```

**Configuration**: Create locale files in `db/CustomLocales/`:
- `en.json` - English
- `ru.json` - Russian
- `de.json` - German
- etc.

<details>
<summary> Example Locale File (Click to expand)</summary>

```json
{
  "my_custom_weapon_001 Name": "Custom Assault Rifle",
  "my_custom_weapon_001 ShortName": "CAR",
  "my_custom_weapon_001 Description": "A powerful custom rifle",
  "my_custom_quest_001 name": "Custom Quest Name",
  "my_custom_quest_001 description": "Quest description here"
}
```
</details>

---

### CustomQuestService

**Purpose**: Adds custom quests to the database with support for complex objectives, rewards, time windows, and faction restrictions.

**Usage**:
```csharp
await wttCommon.CustomQuestService.CreateCustomQuests();
// Or specify custom path
await wttCommon.CustomQuestService.CreateCustomQuests( 
    Path.Join("db", "MyCustomQuestsFolder"));
```

**Default Folder Structure**: 

The service expects quests organized by trader in the following structure:

```
db/CustomQuests/
├── QuestTimeData.json          # Optional: Time-limited quest configuration
├── QuestSideData.json          # Optional: Faction-exclusive quest configuration
├── mechanic/                   # Trader folder (can use trader name or ID)
│   ├── Quests/
│   │   └── quest_definitions.json
│   ├── QuestAssort/            # Optional: Quest-unlocked trader items
│   │   └── assort.json
│   ├── Locales/
│   │   ├── en.json
│   │   └── ru.json
│   └── Images/                 # Quest icons
│       └── quest_icon.png
├── prapor/
│   └── ...
└── skier/
    └── ...
```

**Configuration Files**:

**QuestTimeData.json** (Optional - Time-Limited Quests):
```json
{
  "my_quest_id": {
    "StartMonth": 12,
    "StartDay": 1,
    "EndMonth": 12,
    "EndDay": 31
  }
}
```
Quests in this file will only be available during the specified date range. Useful for seasonal/holiday events.

**QuestSideData.json** (Optional - Faction-Exclusive Quests):
```json
{
  "usecOnlyQuests": [
    "quest_id_1",
    "quest_id_2"
  ],
  "bearOnlyQuests": [
    "quest_id_3",
    "quest_id_4"
  ]
}
```
Quests listed here will only be available to the specified PMC faction.

**Quest Assort** (`QuestAssort/*.json`) - Items unlocked after quest completion:
```json
{
  "success": {
    "my_quest_id": "assort_item_id_to_unlock"
  }
}
```

**Locales** (`Locales/*.json`):
```json
{
  "my_quest_id name": "Custom Quest Name",
  "my_quest_id description": "Quest description text",
  "my_quest_id successMessageText": "Quest completion message",
  "my_quest_id startedMessageText": "Quest started message"
}
```

**Images** (`Images/*`): Quest icons referenced by filename (without extension) in the quest definition.

**Features**:
- Time-limited quests (seasonal/date-based via QuestTimeData.json)
- Faction restrictions (BEAR/USEC only via QuestSideData.json)
- Full locale support with fallback to English
- Quest-unlocked trader assortments
- Custom quest icons

**Trader Names**: You can use either trader names (case-insensitive) or trader IDs for folder names:
- `mechanic`, `prapor`, `therapist`, `skier`, `peacekeeper`, `jaeger`, `ragman`, `fence`
- Or their trader IDs: `54cb50c76803fa8b248b4571`, etc.

**Important Notes**:
- Quest .json files **MUST MATCH BSG QUEST MODELS** exactly. Invalid quest data will throw errors and prevent loading.
- QuestTimeData.json and QuestSideData.json are optional and can be placed in the root `CustomQuests/` folder
- If a quest is outside its time window, it will not be loaded into the database
- Images must be in standard formats (.png, .jpg, etc)
- Locales fall back to English if a translation is missing for a specific language

---

### CustomQuestZoneService

**Purpose**: Manages custom quest zones for Visit, PlaceItem, and other location-based objectives.

**Usage**:
```csharp
await wttCommon.CustomQuestZoneService.CreateCustomQuestZones();
// Or specify custom path
await wttCommon.CustomQuestZoneService.CreateCustomQuestZones( 
    Path.Join("db", "MyCustomQuestZonesFolder"));
```

**Configuration**: Place zone files in `db/CustomQuestZones/`

<details>
<summary> Example Zone Configuration (Click to expand)</summary>

```json
{
  "ZoneId": "deadbody_1",
  "ZoneName": "deadbody_1",
  "ZoneLocation": "woods",
  "ZoneType": "placeitem",
  "FlareType": "",
  "Position": {
    "X": "91.6219",
    "Y": "16.7",
    "Z": "-845.9562"
  },
  "Rotation": {
    "X": "0",
    "Y": "0",
    "Z": "0",
    "W": "1"
  },
  "Scale": {
    "X": "1.5",
    "Y": "3.25",
    "Z": "1.75"
  }
}
```
</details>

**In-Game Editor**: Press **F12** in-game → Navigate to **WTT-ClientCommonLib** settings → Create and position zones visually

---

### CustomVoiceService

**Purpose**: Adds custom character voices for players and bots.

**Usage**:
```csharp
await wttCommon.CustomVoiceService.CreateCustomVoices();
// Or specify custom path
await wttCommon.CustomVoiceService.CreateCustomVoices( 
    Path.Join("db", "MyCustomVoicesFolder"));
```

**Configuration**: Create voice config files in `db/CustomVoices/`:

<details>
<summary> Example Voice Configuration (Click to expand)</summary>

```json
{
  "6747aa4495b4845a0f3d9f98": {
    "locales": {
      "en": "Duke"
    },
    "name": "Duke",
    "bundlePath": "voices/Duke/Voices/duke_voice.bundle",
    "addVoiceToPlayer": true
  }
}
```
</details>

**Requirements**: Package voice audio into Unity AssetBundles → Place in `bundles/` folder → Add to `bundles.json`

---

### CustomHeadService

**Purpose**: Adds custom character head models for player customization.

**Usage**:
```csharp
await wttCommon.CustomHeadService.CreateCustomHeads();
// Or specify custom path
await wttCommon.CustomHeadService.CreateCustomHeads( 
    Path.Join("db", "MyCustomHeadsFolder"));
```

**Configuration**: Create head config files in `db/CustomHeads/`:

<details>
<summary> Example Head Configuration (Click to expand)</summary>

```json
{
  "6747aa715be2c2e443264f32": {
    "path": "heads/chrishead.bundle",
    "addHeadToPlayer": true,
    "side": ["Bear", "Usec"],
    "locales": {
      "en": "Chris Redfield"
    }
  }
}
```
</details>

**Requirements**: Package head models into Unity AssetBundles → Place in `bundles/` folder

---

### CustomClothingService

**Purpose**: Adds custom clothing sets (tops, bottoms) for players.

**Usage**:
```csharp
await wttCommon.CustomClothingService.CreateCustomClothing();
// Or specify custom path
await wttCommon.CustomClothingService.CreateCustomClothing( 
    Path.Join("db", "MyCustomClothingFolder"));
```

**Configuration**: Create clothing config files in `db/CustomClothing/`:

<details>
<summary> Example Clothing Configuration (Click to expand)</summary>

```json
{
  "type": "top",
  "suiteId": "6748037e298128d377dfffd0",
  "outfitId": "67480381bd1eb568c78598df",
  "topId": "67480383b253d50226f3becd",
  "handsId": "67480396eda19f232a648533",
  "locales": {
    "en": {
        "name": "Lara's Tattered Tank Top",
        "description": "Women's Upper"
  },
  "topBundlePath": "clothing/lara_top.bundle",
  "handsBundlePath": "clothing/lara_hands.bundle",
  "traderId": "RAGMAN",
  "loyaltyLevel": 1,
  "profileLevel": 1,
  "standing": 0,
  "currencyId": "ROUBLES",
  "price": 150
}
```
</details>

---

### CustomBotLoadoutService

**Purpose**: Customizes AI bot equipment, weapons, and appearance.

**Usage**:
```csharp
await wttCommon.CustomBotLoadoutService.CreateCustomBotLoadouts();
// Or specify custom path
await wttCommon.CustomBotLoadoutService.CreateCustomBotLoadouts( 
    Path.Join("db", "MyCustomBotLoadoutsFolder"));
```

**Configuration**: Create bot loadout files in `db/CustomBotLoadouts/`:

<details>
<summary> Example Bot Loadout (Click to expand)</summary>

```json
{
  "chances": {
    "equipment": {
      "FirstPrimaryWeapon": 100
    },
    "weaponMods": {
      "mod_stock": 100,
      "mod_magazine": 100,
      "mod_tactical_002": 65
    }
  },
  "inventory": {
    "equipment": {
      "FirstPrimaryWeapon": {
        "679a6a534f3d279c99b135b9": 500
      }
    },
    "mods": {
      "679a6a534f3d279c99b135b9": {
        "mod_stock": ["679a6e58085b54fdd56f5d0d"],
        "mod_magazine": ["679a702c47bb7fa666fe618e"]
      }
    },
    "Ammo": {
      "Caliber545x39": {
        "61962b617c6c7b169525f168": 1
      }
    }
  }
}
```
</details>

---

### CustomLootspawnService

**Purpose**: Controls where and how often your custom items spawn as loot on maps. Supports both random loot spawns and guaranteed forced spawns for quest objectives.

**Usage**:
```csharp
await wttCommon.CustomLootspawnService.CreateCustomLootspawns();
// Or specify custom path
await wttCommon.CustomLootspawnService.CreateCustomLootspawns( 
    Path.Join("db", "MyCustomLootspawnsFolder"));
```

**Default Folder Structure**:

```
db/CustomLootspawns/
├── CustomSpawnpoints/           # Random loot spawns (probability-based)
│   ├── woods_spawns.json
│   ├── customs_spawns.json
│   └── factory_spawns.json
└── CustomSpawnpointsForced/     # Guaranteed spawns (for quest items)
    ├── woods_forced.json
    └── customs_forced.json
```

**Configuration Files**:

**Random Loot Spawns** (`CustomSpawnpoints/*.json`):

<details>
<summary> Example CustomSpawnpoints config (Click to expand)</summary>
  
```json
{
  "sandbox": [
        {
            "locationId": "(82.8276, 14.3806, 181.004)",
            "probability": 0.30,
            "template": {
                "Id": "ag43_spawn",
                "IsContainer": false,
                "useGravity": false,
                "randomRotation": false,
                "Position": {
                    "x": 82.8276,
                    "y": 14.3806,
                    "z": 181.004
                },
                "Rotation": {
                    "x": 2.8415,
                    "y": 212.2408,
                    "z": 91.2423
                },
                "IsGroupPosition": false,
                "GroupPositions": [],
                "IsAlwaysSpawn": false,
                "Root": "68cf6c56cb996a3530052b52",
                "Items": [
                    {
                        "composedKey": "-69420",
                        "_id": "68cf6c56cb996a3530052b53",
                        "_tpl": "68cf56067ff6ceab0c2fd49e",
                        "upd": {
                            "SpawnedInSession": true,
                            "Repairable": {
                                "MaxDurability": 100,
                                "Durability": 100
                            },
                            "Foldable": {
                                "Folded": true
                            },
                            "FireMode": {
                                "FireMode": "single"
                            }
                        }
                    },
                    {
                        "_id": "68cf6c52cb996a3530052b4c",
                        "_tpl": "564ca99c4bdc2d16268b4589",
                        "slotId": "mod_magazine",
                        "parentId": "68cf6c56cb996a3530052b53"
                    },
                    {
                        "_id": "68cf6c52cb996a3530052b4d",
                        "_tpl": "68c63ee6dcb5f65309eb4fcc",
                        "slotId": "mod_muzzle",
                        "upd": {
                            "SpawnedInSession": true
                        },
                        "parentId": "68cf6c56cb996a3530052b53"
                    },
                    {
                        "_id": "68cf6c52cb996a3530052b4e",
                        "_tpl": "68c23b3d4a286357245eb641",
                        "slotId": "mod_sight_rear",
                        "upd": {
                            "SpawnedInSession": true,
                            "Sight": {
                                "ScopesCurrentCalibPointIndexes": [
                                    0
                                ],
                                "ScopesSelectedModes": [
                                    0
                                ],
                                "SelectedScope": 0,
                                "ScopeZoomValue": 0
                            }
                        },
                        "parentId": "68cf6c56cb996a3530052b53"
                    },
                    {
                        "_id": "68cf6c52cb996a3530052b4f",
                        "_tpl": "68cf53ddb8f10c637706563c",
                        "slotId": "mod_stock",
                        "upd": {
                            "SpawnedInSession": true
                        },
                        "parentId": "68cf6c56cb996a3530052b53"
                    },
                    {
                        "_id": "68cf6c52cb996a3530052b50",
                        "_tpl": "56dff216d2720bbd668b4568",
                        "slotId": "cartridges",
                        "location": 0,
                        "upd": {
                            "StackObjectsCount": 30
                        },
                        "parentId": "68cf6c52cb996a3530052b4c"
                    }
                ]
            },
            "itemDistribution": [
                {
                    "composedKey": {
                        "key": "-69420"
                    },
                    "relativeProbability": 1
                }
            ]
        }
    ]
}
```
</details>

**Forced Loot Spawns** (`CustomSpawnpointsForced/*.json`) - Always spawns when quest is active:

<details>
<summary> Example CustomSpawnpointsForced config (Click to expand)</summary>
  
```json
{
    "interchange": [     
        {
            "locationId": "(31.7642 38.7517 -22.9169)",
            "probability": 0.25,
            "template": {
                "Id": "quest_item_immortal_poster (1) [8d2f6c4e-9b3a-4e1f-a7d5-2c8b0e9f3a6d]",
                "IsContainer": false,
                "useGravity": false,
                "randomRotation": false,
                "Position": {
                    "x": 31.7642,
                    "y": 38.7517,
                    "z": -23.244
                },
                "Rotation": {
                    "x": 0,
                    "y": 0,
                    "z": 0
                },
                "IsGroupPosition": false,
                "GroupPositions": [],
                "IsAlwaysSpawn": true,
                "Root": "68748750c2bc7bbc4797d713",
                "Items": [
                    {
                        "_id": "68748762bdc2e875d3940b4f",
                        "_tpl": "687464af51ed3be7e4f6f525",
                        "upd": {
                            "StackObjectsCount": 1
                        }
                    }
                ]
            },
            "itemDistribution": [
                {
                    "composedKey": {
                        "key": "687464af51ed3be7e4f6f525"
                    },
                    "relativeProbability": 1
                }
            ]
        }
    ]
}
```
</details>

**Map Names**: Use the following map identifiers (case-sensitive):
- `bigmap` - Customs
- `woods` - Woods
- `factory4_day` / `factory4_night` - Factory
- `interchange` - Interchange
- `lighthouse` - Lighthouse
- `rezervbase` - Reserve
- `shoreline` - Shoreline
- `tarkovstreets` - Streets of Tarkov
- `laboratory` - Labs
- `sandbox` - Ground Zero

**Use Cases**:

- **Quest Items**: Use `CustomSpawnpointsForced/` for items players must find for quests
- **Multiple Locations**: Use `GroupPositions` array to define several possible spawn positions for variety

---

### CustomAssortSchemeService

**Purpose**: Adds complex, fully-assembled items (like pre-modded weapons or armor with plates) to trader inventories with custom barter schemes. This service gives you complete control over item configuration and pricing.

**Usage**:
```csharp
await wttCommon.CustomAssortSchemeService.CreateCustomAssortSchemes();
// Or specify custom path
await wttCommon.CustomAssortSchemeService.CreateCustomAssortSchemes( 
    Path.Join("db", "MyCustomAssortSchemesFolder"));
```

**When to Use This**:
- **Fully-modded weapons** with specific attachments pre-installed
- **Armor with plates** already inserted
- **Complex items** that require nested child items

**Default Folder Structure**:

```
db/CustomAssortSchemes/
├── peacekeeper_assort.json
├── mechanic_assort.json
└── ragman_assort.json
```

**Configuration Structure**:

Each file defines trader assortments with three main sections:

<details>
<summary>Click to expand full configuration example</summary>

```json
{
  "PEACEKEEPER": {
    "items": [
      {
        "_id": "my_custom_weapon_root",
        "_tpl": "5447a9cd4bdc2dbd208b4567",
        "upd": {
          "Repairable": {
            "MaxDurability": 100,
            "Durability": 100
          },
          "FireMode": {
            "FireMode": "fullauto"
          },
          "UnlimitedCount": true,
          "StackObjectsCount": 999999,
          "BuyRestrictionMax": 0
        },
        "parentId": "hideout",
        "slotId": "hideout"
      },
      {
        "_id": "weapon_mod_magazine",
        "_tpl": "55d480c04bdc2d1d4e8b456a",
        "slotId": "mod_magazine",
        "parentId": "my_custom_weapon_root"
      },
      {
        "_id": "weapon_mod_stock",
        "_tpl": "5649be884bdc2d79388b4577",
        "slotId": "mod_stock",
        "parentId": "my_custom_weapon_root"
      }
    ],
    "barter_scheme": {
      "my_custom_weapon_root": [
        [
          {
            "count": 50000,
            "_tpl": "5449016a4bdc2d6f028b456f"
          }
        ]
      ]
    },
    "loyal_level_items": {
      "my_custom_weapon_root": 2
    }
  }
}
```

</details>

---

### CustomStaticSpawnService

**Purpose**: Places persistent 3D objects on maps with advanced quest-based conditions. Supports complex spawn logic including quest status checks, item requirements, boss detection, and linked quest conditions.

**Usage**:
```csharp
await wttCommon.CustomStaticSpawnService.CreateCustomStaticSpawns();
// Or specify custom path
await wttCommon.CustomStaticSpawnService.CreateCustomStaticSpawns( 
    Path.Join("db", "MyCustomStaticSpawnsFolder"));
```

**Default Folder Structure**:

```
db/CustomStaticSpawns/
├── StaticBundles/               # Unity AssetBundles containing prefabs
│   ├── my_objects.bundle
│   └── quest_decorations.bundle
└── CustomSpawnConfigs/          # Configuration files
    ├── interchange_spawns.json
    ├── woods_spawns.json
    └── customs_spawns.json
```

**Configuration Structure**:

<details>
<summary>Example CustomStaticSpawn config (Click to expand)</summary>

```json
[
  {
    "questId": "my_custom_quest_001",
    "locationID": "interchange",
    "bundleName": "my_objects",
    "prefabName": "QuestMarker_001",
    "position": {
      "x": 123.45,
      "y": 15.67,
      "z": -89.01
    },
    "rotation": {
      "x": 0,
      "y": 45,
      "z": 0
    },
    "requiredQuestStatuses": ["Started"],
    "excludedQuestStatuses": ["AvailableForStart"],
    "questMustExist": true,
    "linkedQuestId": null,
    "linkedRequiredStatuses": [],
    "linkedExcludedStatuses": [],
    "linkedQuestMustExist": null,
    "requiredItemInInventory": null,
    "requiredLevel": 10,
    "requiredFaction": "USEC",
    "requiredBossSpawned": null
  },
  {
    "questId": "my_custom_quest_002",
    "locationID": "woods",
    "bundleName": "quest_decorations",
    "prefabName": "TreeMarker_001",
    "position": {
      "x": 200.5,
      "y": 20.0,
      "z": -150.3
    },
    "rotation": {
      "x": 0,
      "y": 0,
      "z": 0
    },
    "requiredQuestStatuses": ["AvailableForStart", "Started"],
    "excludedQuestStatuses": ["Completed"],
    "questMustExist": false,
    "linkedQuestId": "linked_quest_001",
    "linkedRequiredStatuses": ["Completed"],
    "linkedExcludedStatuses": [],
    "linkedQuestMustExist": true,
    "requiredItemInInventory": "some_item_template_id",
    "requiredLevel": null,
    "requiredFaction": null,
    "requiredBossSpawned": "BosBully"
  }
]
```
</details>

**Condition Properties**:

| Property | Type | Description |
|----------|------|-------------|
| `questId` | string | Quest ID that must exist/not exist based on conditions |
| `locationID` | string | Map ID where the object spawns (lowercase, case-sensitive) |
| `bundleName` | string | Name of the AssetBundle containing the prefab |
| `prefabName` | string | Name of the GameObject inside the bundle to spawn |
| `position` | XYZ | World position where object spawns |
| `rotation` | XYZ | Euler angles for object rotation |
| `requiredQuestStatuses` | List[string] | Quest must be in one of these statuses to spawn (e.g., "Started", "Completed") |
| `excludedQuestStatuses` | List[string] | Quest must NOT be in these statuses to spawn |
| `questMustExist` | bool | If true, quest must exist in player profile; if false, must not exist |
| `linkedQuestId` | string | Secondary quest ID for linked quest conditions |
| `linkedRequiredStatuses` | List[string] | Linked quest must be in one of these statuses |
| `linkedExcludedStatuses` | List[string] | Linked quest must NOT be in these statuses |
| `linkedQuestMustExist` | bool | If true, linked quest must exist; if false, must not exist |
| `requiredItemInInventory` | string | Player must have this item template in inventory to spawn object |
| `requiredLevel` | int | Player must be at least this level to see the object |
| `requiredFaction` | string | Player must be this faction: "USEC" or "BEAR" |
| `requiredBossSpawned` | string | Boss with this role must be alive in raid (e.g., "BosBully", "BosKnight") |

**In-Game Object Placement Tool**:

Press **~** in-game to access the debug console:

1. **Spawn Object**: `SpawnObject <bundleName> <prefabName>`
   - Spawns object in front of you
   
2. **Enter Edit Mode**: `EnterEditMode`
   - WASD - Move object
   - Numpad 1-9 - Rotate
   - Shift - 2x speed multiplier
   - Shift+Alt - 4x speed multiplier
   - Enter - Backslash
   - Delete key - Remove selected object
   - Period - Cycle through spawned objects
   - Comma - Cycle through spawned objects

3. **Export Spawns**: `ExportSpawnedObjectInfo`
   - Saves all placed objects to JSON file
   - Outputs to: `WTT-ClientCommonLib-CustomStaticSpawnConfig-Output-{timestamp}.json`

**Example Workflow**:

1. Build Unity prefabs and package as AssetBundles (my_objects.bundle)
2. Place bundle in `db/CustomStaticSpawns/StaticBundles/`
3. Run in-game: `SpawnObject my_objects QuestMarker_001`
4. Position object with edit mode controls
5. Run: `ExportSpawnedObjectInfo` to generate config JSON
6. Move JSON to `db/CustomStaticSpawns/CustomSpawnConfigs/`
7. Call `CreateCustomStaticSpawns()` to load configurations

---

### CustomHideoutRecipeService

**Purpose**: Creates custom crafting recipes for hideout production modules (Workbench, Lavatory, Medstation, etc.).

**Usage**:
```csharp
await wttCommon.CustomHideoutRecipeService.CreateHideoutRecipes();
// Or specify custom path
await wttCommon.CustomHideoutRecipeService.CreateHideoutRecipes( 
    Path.Join("db", "MyCustomHideoutRecipesFolder"));
```

**Configuration**: Place recipe JSON files in `db/CustomHideoutRecipes/`

**Example Recipe**:
```json
{
  "_id": "my_custom_recipe_001",
  "areaType": 10,
  "requirements": [
    {
      "areaType": 10,
      "requiredLevel": 2,
      "type": "Area"
    },
    {
      "templateId": "5c06779c86f77426e00dd782",
      "count": 1,
      "isFunctional": false,
      "isEncoded": false,
      "type": "Item"
    }
  ],
  "productionTime": 3600,
  "needFuelForAllProductionTime": true,
  "locked": false,
  "endProduct": "my_custom_item_001",
  "continuous": false,
  "count": 1,
  "productionLimitCount": 0,
  "isEncoded": false
}
```

**Key Points**:
- Recipe JSON **must match BSG's HideoutProduction model exactly**
- `_id` must be a valid 24-character hex MongoDB ID
- `areaType` determines which hideout module the recipe appears in (10 = Workbench, 2 = Lavatory, 7 = Medstation, etc.)
- `productionTime` is in seconds
- Invalid recipe structure will throw errors and prevent loading

---

### CustomRigLayoutService

**Purpose**: Sends custom rig layouts to the client so it can register them in-game for your items to use.

**Usage**:
```csharp
await wttCommon.CustomRigLayoutService.CreateRigLayouts();
// Or specify custom path
await wttCommon.CustomRigLayoutService.CreateRigLayouts( 
    Path.Join("db", "MyCustomRigLayoutsFolder"));
```

**Requirements**:
- Build your rig layout prefabs into Unity AssetBundles
- Place `.bundle` files in `db/CustomRigLayouts/` inside your mod folder

***

### CustomSlotImageService

**Purpose**: Provides custom inventory slot icons for unique items.

**Usage**:
```csharp
wttCommon.CustomSlotImageService.CreateSlotImages();
// Or specify custom path
wttCommon.CustomSlotImageService.CreateSlotImages( 
    Path.Join("db", "MyCustomSlotImagesFolder"));
```

**Configuration**:
- Place image files (`.png`, `.jpg`, `.jpeg`, `.bmp`) in `db/CustomSlotImages/` inside your mod folder
- Name each file as the slot ID it replaces (filename without extension)
- The slot ID/key will be used for locale entries if needed

***

### CustomBuffService

**Purpose**: Registers custom stimulator buff configurations to the game database, allowing you to create new buff effects for stimulators and other consumables.

**Usage**:
```csharp
// Use default path (db/CustomBuffs/)
await wttCommon.CustomBuffService.CreateCustomBuffs();

// Or specify custom path
await wttCommon.CustomBuffService.CreateCustomBuffs( 
    Path.Join("db", "MyCustomBuffsFolder"));
```

<details>
<summary> Example Buff Configuration (Click to expand)</summary>

```json
{
    "Geeked": [
        {
            "BuffType": "StaminaRate",
            "Chance": 1,
            "Delay": 1,
            "Duration": 180,
            "Value": 200,
            "AbsoluteValue": true,
            "SkillName": ""
        },
        {
            "BuffType": "WeightLimit",
            "Chance": 1,
            "Delay": 1,
            "Duration": 180,
            "Value": 50,
            "AbsoluteValue": true,
            "SkillName": ""
        },
        {
            "BuffType": "DamageModifier",
            "Chance": 1,
            "Delay": 1,
            "Duration": 180,
            "Value": -15,
            "AbsoluteValue": true,
            "SkillName": ""
        },
        {
            "BuffType": "HealthRate",
            "Chance": 1,
            "Delay": 1,
            "Duration": 180,
            "Value": 3,
            "AbsoluteValue": true,
            "SkillName": ""
        }
    ]
}
```
</details>


***


### CustomProfileService

**Purpose**: Registers custom player profile editions to the game database, allowing you to create alternative starting profiles with custom inventory, skills, and appearance configurations for different PMC factions.

**Usage**:
```csharp
// Use default path (config/CustomProfiles/)
await wttCommon.CustomProfileService.AddCustomProfiles();

// Or specify custom path
await wttCommon.CustomProfileService.AddCustomProfiles( 
    Path.Join("config", "MyCustomProfilesFolder"));
```

**Configuration**: Place profile configuration files in `config/CustomProfiles/`:


**Features**:
- Create faction-specific starting profiles (BEAR/USEC)
- Define custom starting inventory and equipment
- Set initial skill levels and health values
- Configure player stats and progression data

**File Naming**:
- File name (without extension) becomes the profile ID

**Important Notes**:
- Profile structures **must match SPT's ProfileSides model exactly** - invalid data will cause errors

***


### CustomWeaponPresetService

**Purpose**: Registers custom weapon presets to the game database, allowing you to create pre-configured weapon builds with specific attachments that players and bots can select and use.

**Usage**:
```csharp
// Use default path (db/CustomWeaponPresets/)
await wttCommon.CustomWeaponPresetService.CreateCustomWeaponPresets();

// Or specify custom path
await wttCommon.CustomWeaponPresetService.CreateCustomWeaponPresets( 
    Path.Join("db", "MyCustomWeaponPresetsFolder"));
```

<details>
<summary> Example Weapon Preset Configuration (Click to expand)</summary>

```json
{
    "5a32808386f774764a3226d9": {
      "_changeWeaponName": true,
      "_id": "5a32808386f774764a3226d9",
      "_items": [
        {
          "_id": "5a2fa9c4c4a282000d72204f",
          "_tpl": "5447a9cd4bdc2dbd208b4567"
        },
        {
          "_id": "5a2fa9c4c4a282000d722051",
          "_tpl": "59db3a1d86f77429e05b4e92",
          "parentId": "5a2fa9c4c4a282000d72204f",
          "slotId": "mod_pistol_grip"
        },
        {
          "_id": "5a32808386f774764a3226ce",
          "_tpl": "59c1383d86f774290a37e0ca",
          "parentId": "5a2fa9c4c4a282000d72204f",
          "slotId": "mod_magazine"
        },
        {
          "_id": "5a32808386f774764a3226d1",
          "_tpl": "59e6920f86f77411d82aa167",
          "parentId": "5a32808386f774764a3226ce",
          "slotId": "cartridges",
          "upd": {
            "StackObjectsCount": 60
          }
        },
        {
          "_id": "5a2fa9c4c4a282000d722053",
          "_tpl": "59bfe68886f7746004266202",
          "parentId": "5a2fa9c4c4a282000d72204f",
          "slotId": "mod_reciever"
        },
        {
          "_id": "5a2fa9c4c4a282000d722058",
          "_tpl": "57adff4f24597737f373b6e6",
          "parentId": "5a2fa9c4c4a282000d722053",
          "slotId": "mod_scope"
        },
        {
          "_id": "5a2fa9c4c4a282000d722059",
          "_tpl": "55d35ee94bdc2d61338b4568",
          "parentId": "5a2fa9c4c4a282000d722053",
          "slotId": "mod_barrel"
        },
        {
          "_id": "5a2fa9c4c4a282000d72205b",
          "_tpl": "59bffc1f86f77435b128b872",
          "parentId": "5a2fa9c4c4a282000d722059",
          "slotId": "mod_muzzle"
        },
        {
          "_id": "5a2fa9c4c4a282000d722060",
          "_tpl": "59bffbb386f77435b379b9c2",
          "parentId": "5a2fa9c4c4a282000d72205b",
          "slotId": "mod_muzzle"
        },
        {
          "_id": "5a2fa9c4c4a282000d72205c",
          "_tpl": "56eabcd4d2720b66698b4574",
          "parentId": "5a2fa9c4c4a282000d722059",
          "slotId": "mod_gas_block"
        },
        {
          "_id": "5a2fa9c4c4a282000d72205a",
          "_tpl": "595cf16b86f77427440c32e2",
          "parentId": "5a2fa9c4c4a282000d722053",
          "slotId": "mod_handguard"
        },
        {
          "_id": "5a32808386f774764a3226d2",
          "_tpl": "5649a2464bdc2d91118b45a8",
          "parentId": "5a2fa9c4c4a282000d72205a",
          "slotId": "mod_scope"
        },
        {
          "_id": "5a32808386f774764a3226d4",
          "_tpl": "58d39d3d86f77445bb794ae7",
          "parentId": "5a32808386f774764a3226d2",
          "slotId": "mod_scope"
        },
        {
          "_id": "5a32808386f774764a3226d8",
          "_tpl": "58d399e486f77442e0016fe7",
          "parentId": "5a32808386f774764a3226d4",
          "slotId": "mod_scope"
        },
        {
          "_id": "5a2fa9c4c4a282000d72205d",
          "_tpl": "59e0bed186f774156f04ce84",
          "parentId": "5a2fa9c4c4a282000d72205a",
          "slotId": "mod_mount_000"
        },
        {
          "_id": "5a32808386f774764a3226d5",
          "_tpl": "59f8a37386f7747af3328f06",
          "parentId": "5a2fa9c4c4a282000d72205d",
          "slotId": "mod_foregrip"
        },
        {
          "_id": "5a2fa9c4c4a282000d72205e",
          "_tpl": "59e0bdb186f774156f04ce82",
          "parentId": "5a2fa9c4c4a282000d72205a",
          "slotId": "mod_mount_002"
        },
        {
          "_id": "5a32808386f774764a3226d6",
          "_tpl": "56def37dd2720bec348b456a",
          "parentId": "5a2fa9c4c4a282000d72205e",
          "slotId": "mod_tactical"
        },
        {
          "_id": "5a2fa9c4c4a282000d72205f",
          "_tpl": "59e0be5d86f7742d48765bd2",
          "parentId": "5a2fa9c4c4a282000d72205a",
          "slotId": "mod_mount_003"
        },
        {
          "_id": "5a32808386f774764a3226d7",
          "_tpl": "544909bb4bdc2d6f028b4577",
          "parentId": "5a2fa9c4c4a282000d72205f",
          "slotId": "mod_tactical"
        },
        {
          "_id": "5a32808386f774764a3226d3",
          "_tpl": "59e0be5d86f7742d48765bd2",
          "parentId": "5a2fa9c4c4a282000d72205a",
          "slotId": "mod_mount_004"
        },
        {
          "_id": "5a32808386f774764a3226cf",
          "_tpl": "5947e98b86f774778f1448bc",
          "parentId": "5a2fa9c4c4a282000d72204f",
          "slotId": "mod_stock"
        },
        {
          "_id": "5a32808386f774764a3226d0",
          "_tpl": "56ea7165d2720b6e518b4583",
          "parentId": "5a2fa9c4c4a282000d72204f",
          "slotId": "mod_charge"
        }
      ],
      "_name": "M4A1 2017 New year",
      "_parent": "5a2fa9c4c4a282000d72204f",
      "_type": "Preset"
    }
}
```
</details>

**Features**:
- Create complete weapon builds with all attachments pre-configured
- Multiple presets per configuration file

**Important Notes**:
- Preset structures **must match SPT's Preset model exactly** - invalid preset data will cause errors
- If you are expecting to have a unique named preset, you must also push a locale for that preset for the name to be applied properly

***

## Example Mod Structure

### Custom Weapon Mod Structure

```
MyWeaponMod/
├── bundles/
│ └── MyCustomWeapon.bundle
├── db/
│ ├── CustomItems/
│ │ └── weapons.json
│ └── CustomAssortSchemes/
│ 	└── praporAssort.json
├── bundles.json
└── MyWeaponMod.dll
```

***

## Available Client Services

### CustomTemplateIdToObjectService

**Purpose**: Allows other mods to register custom item template mappings at runtime, enabling the game to properly instantiate custom item types with their corresponding inventory logic classes.

**Usage**:
```csharp
// Create your new template and item classes

// TEMPLATE TYPE
public class GameBoyModTemplateType(string romName, string cartridgeImage, string accessoryType)
    : CompoundItemTemplateClass
{
    public readonly string RomName = romName;
    public readonly string CartridgeImage = cartridgeImage;
    public readonly string AccessoryType = accessoryType;
}


// ITEM TYPE
public class GameBoyModItemType(string id, GameBoyModTemplateType template) : CompoundItem(id, template)
{
}

// ITEM
public class GameBoyAccessory : GameBoyModItemType
{
    public GameBoyAccessory(string id, GameBoyModTemplateType template) : base(id, template)
    {
        AccessoryType = template.AccessoryType;
        Components.Add(_tag = new TagComponent(this));
    }

    public string AccessoryType { get; }


    public override IEnumerable<EItemInfoButton> ItemInteractionButtons
    {
        get
        {
            foreach (var itemInfoButton in GetBaseInteractions())
            {
                yield return itemInfoButton;
            }
            yield return EItemInfoButton.Install;
            yield return EItemInfoButton.Uninstall;
            if (!string.IsNullOrEmpty(_tag?.Name))
            {
                yield return EItemInfoButton.ResetTag;
            }
        }
    }

// Define your custom template mappings
public static readonly List<TemplateIdToObjectType> CustomMappings =
[
    // Add GameBoyCartridge Template
    new(
        "66f16b85ed966fb78f5563d8", // Template ID
        null,   // Item type
        typeof(GameBoyModTemplateType),         // Template type
        null // Constructor
    ),
    // Add GameBoyCartridge Item
    new(
        "66f17b4cb59dbccbf12990e6", // Template ID
        typeof(GameBoyCartridge),   // Item type
        typeof(GameBoyModTemplateType),         // Template type
        (id, template) => new GameBoyCartridge(id, (GameBoyModTemplateType)template) // Constructor
    ),
];

// Register with CommonLib
CustomTemplateIdToObjectService.AddNewTemplateIdToObjectMapping(CustomMappings);
```

**Mapping Properties**:

| Property | Type | Description |
|----------|------|-------------|
| `TemplateId` | string | Custom item template ID that matches your item template ID in the database |
| `ItemType` | Type | C# class that inherits from `Item` |
| `TemplateType` | Type | C# class that represents the template data structure |
| `Constructor` | Delegate | Function that instantiates your custom item: `(id, parent) => new YourItem(id, parent)` |

**What Gets Registered**:

The service registers your mappings into three internal tables:

- **TypeTable**: Maps template IDs to their `ItemType` classes for inventory object creation
- **TemplateTypeTable**: Maps template IDs to their `TemplateType` classes for data representation
- **ItemConstructors**: Maps template IDs to constructor functions for instantiation

**Example Use Cases**:
- Creating custom templates and item types for belts and cases with unique layouts (SEE PACK N STRAP)
- Creating custom templates and item types for a unique usable item and unique mods for it (SEE KOMRADE KID)

**Important Notes**:
- Both `ItemType` and `TemplateType` must be properly defined classes
- Only register custom items - this won't override vanilla Escape from Tarkov items
- You still must add your new templates and items to the server database/templates/items. This is just a helper to add to the client `TemplateIdToObjectMappingClass`.
***
