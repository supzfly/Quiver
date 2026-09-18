using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quiver
{
    /// <summary>
    /// Loads stat overrides for the "heavy" prefabs from the !Info DamageTypes config setting,
    /// and creates the hardcoded heavy prefabs/recipes.
    /// </summary>
    public class HeavyLoader
    {
        public class HeavyConfig
        {
            public int armor = DefaultArmor;
            public float moveModifier = DefaultMoveModifier;
            public float heatResistanceModifier = DefaultHeatResistanceModifier;
            public float eitrRegenModifier = DefaultEitrRegenModifier;
        }

        // Default values applied when a stat is missing from the config string
        private const int DefaultArmor = 150;
        private const float DefaultMoveModifier = 0.15f;
        private const float DefaultHeatResistanceModifier = 0.20f;
        private const float DefaultEitrRegenModifier = 0.20f;

        public class RecipeResourceRequirement
        {
            public string itemName;
            public int amount;
        }

        /// <summary>
        /// Hardcoded definition of a single heavy prefab: source item to clone, display name,
        /// whether it should receive the armor/move/heat/eitr stats from config, elemental
        /// resistances to add, and its crafting recipe.
        /// </summary>
        public class HeavyItemDefinition
        {
            public string prefabName;
            public string baseItemName;
            public string displayName;
            public float? weight;
            public bool applyHeavyStats;
            public string[] resistanceDamageTypes = new string[0];
            public string craftingStation = "piece_workbench";
            public int recipeAmount = 1;
            public List<RecipeResourceRequirement> resources = new List<RecipeResourceRequirement>();
        }

        private enum NewDamageTypes
        {
            Water = 1024
        }

        /// <summary>
        /// Hardcoded list of heavy prefabs to create.
        /// </summary>
        private static readonly List<HeavyItemDefinition> HeavyItemDefinitions = new List<HeavyItemDefinition>
        {
            new HeavyItemDefinition
            {
                prefabName = "BeltStrengthHeavy",
                baseItemName = "BeltStrength",
                displayName = "Heavy Megingjord",
                weight = 0,
                applyHeavyStats = true,
                resistanceDamageTypes = new[] { "Poison", "Fire", "Frost", "Lightning" },
                craftingStation = "piece_workbench",
                resources = new List<RecipeResourceRequirement>
                {
                    new RecipeResourceRequirement { itemName = "BeltStrength", amount = 1 }
                }
            },
            new HeavyItemDefinition
            {
                prefabName = "HelmetDvergerHeavy",
                baseItemName = "HelmetDverger",
                displayName = "Heavy Dverger Circlet",
                applyHeavyStats = true,
                resistanceDamageTypes = new[] { "Poison", "Fire", "Frost", "Lightning" },
                craftingStation = "piece_workbench",
                resources = new List<RecipeResourceRequirement>
                {
                    new RecipeResourceRequirement { itemName = "HelmetDverger", amount = 1 }
                }
            },
            new HeavyItemDefinition
            {
                prefabName = "SwordHeavy",
                baseItemName = "SwordCheat",
                displayName = "Heavy Sword",
                applyHeavyStats = false,
                craftingStation = "piece_workbench",
                resources = new List<RecipeResourceRequirement>
                {
                    new RecipeResourceRequirement { itemName = "Stone", amount = 1 }
                }
            },
            new HeavyItemDefinition
            {
                prefabName = "ShieldHeavy",
                baseItemName = "ShieldKnight",
                displayName = "Heavy Shield",
                applyHeavyStats = false,
                craftingStation = "piece_workbench",
                resources = new List<RecipeResourceRequirement>
                {
                    new RecipeResourceRequirement { itemName = "Stone", amount = 1 }
                }
            }
        };

        /// <summary>
        /// Parse a heavy config string in the format "armor:150,move:15,heat:20,eitr:20" into a HeavyConfig.
        /// move, heat and eitr are percentages (e.g. 20 becomes 0.20f).
        /// </summary>
        public static HeavyConfig ParseHeavyConfigString(string heavyConfigString)
        {
            HeavyConfig config = new HeavyConfig();
            if (string.IsNullOrEmpty(heavyConfigString))
            {
                return config;
            }

            string[] pairs = heavyConfigString.Split(',');
            foreach (string pair in pairs)
            {
                string[] parts = pair.Split(':');
                if (parts.Length != 2) continue;

                string type = parts[0].Trim().ToLowerInvariant();
                if (!float.TryParse(parts[1].Trim(), out float value)) continue;

                switch (type)
                {
                    case "armor": config.armor = (int)value; break;
                    case "move": config.moveModifier = value / 100f; break;
                    case "heat": config.heatResistanceModifier = value / 100f; break;
                    case "eitr": config.eitrRegenModifier = value / 100f; break;
                }
            }

            return config;
        }

        /// <summary>
        /// Apply the parsed heavy config values onto an item's shared data.
        /// </summary>
        public static void ApplyHeavyConfig(ItemDrop.ItemData.SharedData shared, HeavyConfig config)
        {
            if (shared == null || config == null)
            {
                return;
            }

            shared.m_armor = config.armor;
            shared.m_movementModifier = config.moveModifier;
            shared.m_heatResistanceModifier = config.heatResistanceModifier;
            shared.m_eitrRegenModifier = config.eitrRegenModifier;
        }

        /// <summary>
        /// Convenience helper: parse the config string and apply it directly to the item's shared data.
        /// </summary>
        public static void ApplyHeavyConfigFromString(ItemDrop.ItemData.SharedData shared, string heavyConfigString)
        {
            ApplyHeavyConfig(shared, ParseHeavyConfigString(heavyConfigString));
        }

        /// <summary>
        /// Add elemental resistance modifiers for the given damage type names onto the item's shared data.
        /// </summary>
        private static void ApplyResistances(ItemDrop.ItemData.SharedData shared, string[] damageTypeNames)
        {
            foreach (string damageTypeName in damageTypeNames)
            {
                int modType = Enum.TryParse<NewDamageTypes>(damageTypeName, out NewDamageTypes result)
                    ? (int)result
                    : (int)Enum.Parse(typeof(HitData.DamageType), damageTypeName);

                shared.m_damageModifiers.Add(new HitData.DamageModPair()
                {
                    m_type = (HitData.DamageType)modType,
                    m_modifier = (HitData.DamageModifier)Enum.Parse(typeof(HitData.DamageModifier), "Resistant")
                });
            }
        }

        /// <summary>
        /// Create a recipe for the given item using a single crafting station and a flat resource list.
        /// </summary>
        private static void CreateRecipe(ItemDrop itemDrop, HeavyItemDefinition definition)
        {
            Recipe recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.name = "Recipe_" + definition.prefabName;
            recipe.m_item = itemDrop;
            recipe.m_amount = definition.recipeAmount;
            recipe.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(definition.craftingStation);

            List<Piece.Requirement> requirements = new List<Piece.Requirement>();
            foreach (var resource in definition.resources)
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
            recipe.m_resources = requirements.ToArray();

            // Since we got the prefabs from the cache, no referencing is needed
            CustomRecipe CR = new CustomRecipe(recipe, fixReference: false, fixRequirementReferences: false);
            ItemManager.Instance.AddRecipe(CR);
        }

        /// <summary>
        /// Create a single heavy prefab and its recipe from a definition.
        /// </summary>
        private static void CreateHeavyItem(HeavyItemDefinition definition, string heavyConfigString)
        {
            try
            {
                CustomItem CI = new CustomItem(definition.prefabName, definition.baseItemName);
                ItemManager.Instance.AddItem(CI);

                var itemDrop = CI.ItemDrop;
                itemDrop.m_itemData.m_shared.m_name = definition.displayName;

                if (definition.weight.HasValue)
                {
                    itemDrop.m_itemData.m_shared.m_weight = definition.weight.Value;
                }

                if (definition.applyHeavyStats)
                {
                    ApplyHeavyConfigFromString(itemDrop.m_itemData.m_shared, heavyConfigString);
                }

                if (definition.resistanceDamageTypes.Length > 0)
                {
                    ApplyResistances(itemDrop.m_itemData.m_shared, definition.resistanceDamageTypes);
                }

                CreateRecipe(itemDrop, definition);
            }
            catch (Exception ex)
            {
                Jotunn.Logger.LogError($"Failed to create heavy item {definition.prefabName}: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Create all hardcoded heavy prefabs/recipes, applying stats from the given config string.
        /// </summary>
        public static void CreateHeavyItems(string heavyConfigString)
        {
            foreach (var definition in HeavyItemDefinitions)
            {
                CreateHeavyItem(definition, heavyConfigString);
            }
        }
    }
}
