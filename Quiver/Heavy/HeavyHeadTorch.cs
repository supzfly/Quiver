using Jotunn.Entities;
using Jotunn.Managers;
using System;
using UnityEngine;

namespace Quiver
{
	partial class Quiver
	{
		private void AddHeavyHeadTorch()
		{
			// Create and add a custom item
			CustomItem CI = new CustomItem("HelmetDvergerHeavy", "HelmetDverger");
			ItemManager.Instance.AddItem(CI);

			// Replace vanilla properties of the custom item
			var itemDrop = CI.ItemDrop;
			itemDrop.m_itemData.m_shared.m_name = "Heavy Dverger Circlet";
			//itemDrop.m_itemData.m_shared.m_description = "";
			itemDrop.m_itemData.m_shared.m_armor = 150;
			itemDrop.m_itemData.m_shared.m_movementModifier = 0.15f;

			itemDrop.m_itemData.m_shared.m_heatResistanceModifier = 0.20f; // 20% heat resistance

			itemDrop.m_itemData.m_shared.m_eitrRegenModifier = 0.20f; // 10% eitr regen

			// resistance
			int modPoison = Enum.TryParse<NewDamageTypes>("Poison", out NewDamageTypes resultPoison) ? (int)resultPoison : (int)Enum.Parse(typeof(HitData.DamageType), "Poison");
			itemDrop.m_itemData.m_shared.m_damageModifiers.Add(new HitData.DamageModPair() { m_type = (HitData.DamageType)modPoison, m_modifier = (HitData.DamageModifier)Enum.Parse(typeof(HitData.DamageModifier), "Resistant") });

			int modFire = Enum.TryParse<NewDamageTypes>("Fire", out NewDamageTypes resultFire) ? (int)resultFire : (int)Enum.Parse(typeof(HitData.DamageType), "Fire");
			itemDrop.m_itemData.m_shared.m_damageModifiers.Add(new HitData.DamageModPair() { m_type = (HitData.DamageType)modFire, m_modifier = (HitData.DamageModifier)Enum.Parse(typeof(HitData.DamageModifier), "Resistant") });

			int modFrost = Enum.TryParse<NewDamageTypes>("Frost", out NewDamageTypes resultFrost) ? (int)resultFrost : (int)Enum.Parse(typeof(HitData.DamageType), "Frost");
			itemDrop.m_itemData.m_shared.m_damageModifiers.Add(new HitData.DamageModPair() { m_type = (HitData.DamageType)modFrost, m_modifier = (HitData.DamageModifier)Enum.Parse(typeof(HitData.DamageModifier), "Resistant") });

			int modLightning = Enum.TryParse<NewDamageTypes>("Lightning", out NewDamageTypes resultLightning) ? (int)resultLightning : (int)Enum.Parse(typeof(HitData.DamageType), "Lightning");
			itemDrop.m_itemData.m_shared.m_damageModifiers.Add(new HitData.DamageModPair() { m_type = (HitData.DamageType)modLightning, m_modifier = (HitData.DamageModifier)Enum.Parse(typeof(HitData.DamageModifier), "Resistant") });

			// Create recipe
			RecipeHeavyHeadTorch(itemDrop);

		}		

		private static void RecipeHeavyHeadTorch(ItemDrop itemDrop)
		{
			// Create and add a recipe for the copied item
			Recipe recipe = ScriptableObject.CreateInstance<Recipe>();
			recipe.name = "Recipe_HeavyHeadTorch";
			recipe.m_item = itemDrop;
			recipe.m_amount = 1;
			recipe.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>("piece_workbench");
			recipe.m_resources = new Piece.Requirement[]
			{
			new Piece.Requirement()
			{
				m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("HelmetDverger"),
				m_amount = 1
			}
			};

			// Since we got the prefabs from the cache, no referencing is needed
			CustomRecipe CR = new CustomRecipe(recipe, fixReference: false, fixRequirementReferences: false);
			ItemManager.Instance.AddRecipe(CR);
		}
	}
}