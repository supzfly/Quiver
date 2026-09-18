using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quiver
{
    /// <summary>
    /// Loads custom arrows from JSON configuration
    /// </summary>
    public class ArrowLoader
    {
        public class DamageConfig
        {
            public float pierce = 0;
            public float blunt = 0;
            public float slash = 0;
            public float fire = 0;
            public float frost = 0;
            public float lightning = 0;
            public float poison = 0;
            public float spirit = 0;
            public float chop = 0;      // For axe arrows
            public float pickaxe = 0;   // For pickaxe arrows
        }

        public class RecipeResourceRequirement
        {
            public string itemName;
            public int amount;
        }

        public class RecipeConfig
        {
            public string craftingStation = "forge";
            public List<RecipeResourceRequirement> resources = new List<RecipeResourceRequirement>();
        }

        public class ArrowMetadata
        {
            public string arrowName;               // e.g., "CrystalArrow" - links to json entry
            public string displayName;             // e.g., "Crystal Arrow"
            public string description;
            public string texturePath;             // e.g., "arrow_crystal.png"
            public string craftingStation;         // e.g., "piece_workbench"
        }

        /// <summary>
        /// Hardcoded metadata for each arrow. Only arrows listed here can be configured via JSON.
        /// </summary>
        private static readonly List<ArrowMetadata> ArrowMetadataList = new List<ArrowMetadata>
        {
            new ArrowMetadata
            {
                arrowName = "BoneArrow",
                displayName = "Bone Arrow",
                description = "Jagged bone tears the flesh.",
                texturePath = "arrow_bone.png",
                craftingStation = "piece_workbench"
            },
            new ArrowMetadata
            {
                arrowName = "BlackMetalArrow",
                displayName = "Black Metal Arrow",
                description = "A weighty projectile. Glints in the light with a greenish glow.",
                texturePath = "arrow_blackmetal.png",
                craftingStation = "forge"
            },
            new ArrowMetadata
            {
                arrowName = "CrystalArrow",
                displayName = "Crystal Arrow",
                description = "Tipped with a shard of crystal. It's structure resonates with the power of the Gods.",
                texturePath = "arrow_crystal.png",
                craftingStation = "forge"
            },
            new ArrowMetadata
            {
                arrowName = "SurtlingCoreArrow",
                displayName = "Surtling Core Arrow",
                description = "This arrow throbs with inner heat..",
                texturePath = "arrow_surtling.png",
                craftingStation = "piece_workbench"
            },
            new ArrowMetadata
            {
                arrowName = "FlametalArrow",
                displayName = "Flametal Arrow",
                description = "Made from the core of a meteorite. The tip shimmers with a pure intensity not seen on this earth.",
                texturePath = "arrow_flametal.png",
                craftingStation = "forge"
            },
            new ArrowMetadata
            {
                arrowName = "IronPickaxeArrow",
                displayName = "Iron Pickaxe Arrow",
                description = "A sturdy rock breaking tool. At Range.",
                texturePath = "arrow_pickaxe_iron.png",
                craftingStation = "forge"
            }
        };

        /// <summary>
        /// Look up an arrow's metadata by arrowName. Returns null if not found.
        /// </summary>
        public static ArrowMetadata GetArrowMetadata(string arrowName)
        {
            return ArrowMetadataList.Find(m => m.arrowName == arrowName);
        }

        public class ArrowConfig
        {
            public string arrowName;               // e.g., "CrystalArrow" - links to ArrowMetadata, used for prefabName
            public DamageConfig damageTypes = new DamageConfig();
            public RecipeConfig recipe = new RecipeConfig();
        }

        /// <summary>
        /// Default configuration values for each arrow, used as defaults when binding .cfg config entries.
        /// Format: damageTypes = "type:value,type:value,...", resources = "itemName:amount,itemName:amount,..."
        /// </summary>
        public class ArrowDefaults
        {
            public string arrowName;
            public string damageTypes;
            public string resources;
        }

        public static readonly List<ArrowDefaults> ArrowDefaultsList = new List<ArrowDefaults>
        {
            new ArrowDefaults
            {
                arrowName = "BoneArrow",
                damageTypes = "pierce:26",
                resources = "BoneFragments:5"
            },
            new ArrowDefaults
            {
                arrowName = "BlackMetalArrow",
                damageTypes = "pierce:62,blunt:40",
                resources = "Wood:8,Feathers:2,BlackMetal:1"
            },
            new ArrowDefaults
            {
                arrowName = "CrystalArrow",
                damageTypes = "pierce:82,lightning:40",
                resources = "Wood:8,Feathers:2,Crystal:1"
            },
            new ArrowDefaults
            {
                arrowName = "SurtlingCoreArrow",
                damageTypes = "pierce:32,fire:62",
                resources = "Wood:8,Feathers:2,SurtlingCore:1"
            },
            new ArrowDefaults
            {
                arrowName = "FlametalArrow",
                damageTypes = "pierce:82,fire:120",
                resources = "Wood:8,Feathers:2,FlametalNew:1"
            },
            new ArrowDefaults
            {
                arrowName = "IronPickaxeArrow",
                damageTypes = "pierce:33,pickaxe:33",
                resources = "RoundLog:8,Iron:1"
            }
        };

        /// <summary>
        /// Tracks created items/recipes so config changes can be applied live without restarting the game.
        /// </summary>
        private static readonly Dictionary<string, ItemDrop> CreatedItemDrops = new Dictionary<string, ItemDrop>();
        private static readonly Dictionary<string, Recipe> CreatedRecipes = new Dictionary<string, Recipe>();

        /// <summary>
        /// Create arrows from bound config values
        /// </summary>
        public static void CreateArrowsFromConfigs(List<ArrowConfig> arrows)
        {
            try
            {
                foreach (var arrowConfig in arrows)
                {
                    CreateArrowFromConfig(arrowConfig);
                }

                Jotunn.Logger.LogInfo($"Loaded {arrows.Count} arrows from configuration");
            }
            catch (Exception ex)
            {
                Jotunn.Logger.LogError($"Error loading arrows from configuration: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Parse a damage types string in the format "type:value,type:value,..." into a DamageConfig
        /// </summary>
        public static DamageConfig ParseDamageTypesString(string damageTypesString)
        {
            DamageConfig damageTypes = new DamageConfig();
            if (string.IsNullOrEmpty(damageTypesString))
            {
                return damageTypes;
            }

            string[] pairs = damageTypesString.Split(',');
            foreach (string pair in pairs)
            {
                string[] parts = pair.Split(':');
                if (parts.Length != 2) continue;

                string type = parts[0].Trim().ToLowerInvariant();
                if (!float.TryParse(parts[1].Trim(), out float value)) continue;

                switch (type)
                {
                    case "pierce": damageTypes.pierce = value; break;
                    case "blunt": damageTypes.blunt = value; break;
                    case "slash": damageTypes.slash = value; break;
                    case "fire": damageTypes.fire = value; break;
                    case "frost": damageTypes.frost = value; break;
                    case "lightning": damageTypes.lightning = value; break;
                    case "poison": damageTypes.poison = value; break;
                    case "spirit": damageTypes.spirit = value; break;
                    case "chop": damageTypes.chop = value; break;
                    case "pickaxe": damageTypes.pickaxe = value; break;
                }
            }

            return damageTypes;
        }

        /// <summary>
        /// Parse a resources string in the format "itemName:amount,itemName:amount,..." into a list of RecipeResourceRequirement
        /// </summary>
        public static List<RecipeResourceRequirement> ParseResourcesString(string resourcesString)
        {
            List<RecipeResourceRequirement> resources = new List<RecipeResourceRequirement>();
            if (string.IsNullOrEmpty(resourcesString))
            {
                return resources;
            }

            string[] pairs = resourcesString.Split(',');
            foreach (string pair in pairs)
            {
                string[] parts = pair.Split(':');
                if (parts.Length != 2) continue;

                string itemName = parts[0].Trim();
                if (!int.TryParse(parts[1].Trim(), out int amount)) continue;

                resources.Add(new RecipeResourceRequirement { itemName = itemName, amount = amount });
            }

            return resources;
        }

        /// <summary>
        /// Create a single arrow from configuration
        /// </summary>
        private static void CreateArrowFromConfig(ArrowConfig config)
        {
            try
            {

                ArrowMetadata metadata = GetArrowMetadata(config.arrowName);
                if (metadata == null)
                {
                    Jotunn.Logger.LogWarning($"No matching arrowName '{config.arrowName}' found. Skipping.");
                    return;
                }

                // Use arrowName as prefabName
                string prefabName = config.arrowName;

                // Determine baseItemName based on damage types
                string baseItemName = DeterminBaseItemName(config.damageTypes);

                // Create and add a custom item cloned from vanilla item
                CustomItem CI = new CustomItem(prefabName, baseItemName);
                ItemManager.Instance.AddItem(CI);

                // Replace vanilla properties of the custom item
                var itemDrop = CI.ItemDrop;
                itemDrop.m_itemData.m_shared.m_name = metadata.displayName;
                itemDrop.m_itemData.m_shared.m_description = metadata.description;
                itemDrop.m_itemData.m_shared.m_weight = DeterminWeight(config.damageTypes);

                ApplyDamages(ref itemDrop.m_itemData.m_shared.m_damages, config.damageTypes);

                // Load and set texture
                if (!string.IsNullOrEmpty(metadata.texturePath))
                {
                    Texture2D texture2D = QUtility.LoadTextureFromAssets(metadata.texturePath);
                    if (texture2D != null)
                    {
                        Sprite icon = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
                        itemDrop.m_itemData.m_shared.m_icons[0] = icon;
                    }
                }

                // Use crafting station from metadata
                config.recipe.craftingStation = metadata.craftingStation;

                // Cache for live config refresh
                CreatedItemDrops[prefabName] = itemDrop;

                // Create recipe
                CreateRecipeFromConfig(itemDrop, config.recipe, prefabName);
            }
            catch (Exception ex)
            {
                Jotunn.Logger.LogError($"Failed to create arrow {config.arrowName}: {ex.Message}");
            }
        }

        /// <summary>
        /// Determine the base item name based on damage types
        /// Default: ArrowFlint (pierce damage)
        /// Blunt damage: ArrowObsidian
        /// Fire damage: ArrowFire
        /// Lightning damage: ArrowSilver
        /// </summary>
        private static string DeterminBaseItemName(DamageConfig damageTypes)
        {
            // Check priority order: blunt > fire > lightning > default (pierce)
            if (damageTypes.blunt > 0)
                return "ArrowObsidian";
            if (damageTypes.fire > 0)
                return "ArrowFire";
            if (damageTypes.lightning > 0)
                return "ArrowSilver";

            return "ArrowFlint";  // Default for pierce damage
        }

        /// <summary>
        /// Determine weight based on damage types
        /// Default: 0.1
        /// If has blunt damage: 0.2
        /// </summary>
        private static float DeterminWeight(DamageConfig damageTypes)
        {
            return damageTypes.blunt > 0 ? 0.2f : 0.1f;
        }

        /// <summary>
        /// Applies a DamageConfig's values onto an existing HitData.DamageTypes instance.
        /// </summary>
        private static void ApplyDamages(ref HitData.DamageTypes target, DamageConfig damageTypes)
        {
            target.m_pierce = damageTypes.pierce;
            target.m_blunt = damageTypes.blunt;
            target.m_slash = damageTypes.slash;
            target.m_fire = damageTypes.fire;
            target.m_frost = damageTypes.frost;
            target.m_lightning = damageTypes.lightning;
            target.m_poison = damageTypes.poison;
            target.m_spirit = damageTypes.spirit;
            target.m_chop = damageTypes.chop;
            target.m_pickaxe = damageTypes.pickaxe;
        }

        /// <summary>
        /// Resolve recipe resource requirements from the prefab cache, skipping and logging any that can't be found.
        /// </summary>
        private static List<Piece.Requirement> BuildRequirements(List<RecipeResourceRequirement> resources)
        {
            List<Piece.Requirement> requirements = new List<Piece.Requirement>();
            foreach (var resource in resources)
            {
                ItemDrop resourceItem = PrefabManager.Cache.GetPrefab<ItemDrop>(resource.itemName);
                if (resourceItem != null)
                {
                    requirements.Add(new Piece.Requirement()
                    {
                        m_resItem = resourceItem,
                        m_amount = resource.amount
                    });
                }
                else
                {
                    Jotunn.Logger.LogWarning($"Recipe resource not found: {resource.itemName}");
                }
            }

            return requirements;
        }

        /// <summary>
        /// Create recipe from configuration
        /// </summary>
        private static void CreateRecipeFromConfig(ItemDrop itemDrop, RecipeConfig recipeConfig, string prefabName)
        {
            Recipe recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.name = "Recipe_" + prefabName;  // Generate recipe name: "Recipe_" + prefabName
            recipe.m_item = itemDrop;
            recipe.m_amount = 20;  // Always create 20 items per recipe
            recipe.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(recipeConfig.craftingStation);
            recipe.m_resources = BuildRequirements(recipeConfig.resources).ToArray();

            // Since we got the prefabs from the cache, no referencing is needed
            CustomRecipe CR = new CustomRecipe(recipe, fixReference: false, fixRequirementReferences: false);
            ItemManager.Instance.AddRecipe(CR);

            // Cache for live config refresh
            CreatedRecipes[prefabName] = recipe;
        }

        /// <summary>
        /// Updates an already-created arrow's damages and recipe requirements in-place, without
        /// requiring a game restart. Call this from a config entry's SettingChanged handler.
        /// </summary>
        public static void RefreshArrowFromConfig(ArrowConfig config)
        {
            try
            {
                string prefabName = config.arrowName;

                ArrowMetadata metadata = GetArrowMetadata(config.arrowName);
                if (metadata == null)
                {
                    Jotunn.Logger.LogWarning($"No matching arrowName '{config.arrowName}' found. Skipping refresh.");
                    return;
                }

                // Update damage values on the existing item
                if (CreatedItemDrops.TryGetValue(prefabName, out ItemDrop itemDrop) && itemDrop != null)
                {
                    itemDrop.m_itemData.m_shared.m_weight = DeterminWeight(config.damageTypes);
                    ApplyDamages(ref itemDrop.m_itemData.m_shared.m_damages, config.damageTypes);
                }
                else
                {
                    Jotunn.Logger.LogWarning($"Could not find created item for '{prefabName}' to refresh.");
                }

                // Update recipe resource requirements on the existing recipe
                if (CreatedRecipes.TryGetValue(prefabName, out Recipe recipe) && recipe != null)
                {
                    config.recipe.craftingStation = metadata.craftingStation;
                    recipe.m_resources = BuildRequirements(config.recipe.resources).ToArray();
                    recipe.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(metadata.craftingStation);
                }
                else
                {
                    Jotunn.Logger.LogWarning($"Could not find created recipe for '{prefabName}' to refresh.");
                }

                Jotunn.Logger.LogInfo($"Refreshed arrow '{prefabName}' from updated configuration");
            }
            catch (Exception ex)
            {
                Jotunn.Logger.LogError($"Failed to refresh arrow {config.arrowName}: {ex.Message}");
            }
        }
    }
}
