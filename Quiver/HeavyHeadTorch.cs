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
			// Create and add a custom item based on SwordBlackmetal
			CustomItem CI = new CustomItem("HelmetDvergerHeavy", "HelmetDverger");
			ItemManager.Instance.AddItem(CI);

			// Replace vanilla properties of the custom item
			var itemDrop = CI.ItemDrop;
			itemDrop.m_itemData.m_shared.m_name = "Heavy Dverger Circlet";
			//itemDrop.m_itemData.m_shared.m_description = "";
			itemDrop.m_itemData.m_shared.m_armor = ArmourValue.Value;
			itemDrop.m_itemData.m_shared.m_movementModifier = MovementValue.Value;

			itemDrop.m_itemData.m_shared.m_heatResistanceModifier = HeatValue.Value; // 0.2f; // 20% heat resistance

			itemDrop.m_itemData.m_shared.m_eitrRegenModifier = EitrValue.Value; // 0.1f; // 10% eitr regen

			// resistance
			int modPoison = Enum.TryParse<NewDamageTypes>("Poison", out NewDamageTypes resultPoison) ? (int)resultPoison : (int)Enum.Parse(typeof(HitData.DamageType), "Poison");
			itemDrop.m_itemData.m_shared.m_damageModifiers.Add(new HitData.DamageModPair() { m_type = (HitData.DamageType)modPoison, m_modifier = (HitData.DamageModifier)Enum.Parse(typeof(HitData.DamageModifier), "Resistant") });

			int modFire = Enum.TryParse<NewDamageTypes>("Fire", out NewDamageTypes resultFire) ? (int)resultFire : (int)Enum.Parse(typeof(HitData.DamageType), "Fire");
			itemDrop.m_itemData.m_shared.m_damageModifiers.Add(new HitData.DamageModPair() { m_type = (HitData.DamageType)modFire, m_modifier = (HitData.DamageModifier)Enum.Parse(typeof(HitData.DamageModifier), "Resistant") });

			int modFrost = Enum.TryParse<NewDamageTypes>("Frost", out NewDamageTypes resultFrost) ? (int)resultFrost : (int)Enum.Parse(typeof(HitData.DamageType), "Frost");
			itemDrop.m_itemData.m_shared.m_damageModifiers.Add(new HitData.DamageModPair() { m_type = (HitData.DamageType)modFrost, m_modifier = (HitData.DamageModifier)Enum.Parse(typeof(HitData.DamageModifier), "Resistant") });

			int modLightning = Enum.TryParse<NewDamageTypes>("Lightning", out NewDamageTypes resultLightning) ? (int)resultLightning : (int)Enum.Parse(typeof(HitData.DamageType), "Lightning");
			itemDrop.m_itemData.m_shared.m_damageModifiers.Add(new HitData.DamageModPair() { m_type = (HitData.DamageType)modLightning, m_modifier = (HitData.DamageModifier)Enum.Parse(typeof(HitData.DamageModifier), "Resistant") });

			// Add this after the other resistances in AddHeavyHeadTorch
			int modHot = Enum.TryParse<NewDamageTypes>("Hot", out NewDamageTypes resultHot) ? (int)resultHot : (int)Enum.Parse(typeof(HitData.DamageType), "Hot");
			itemDrop.m_itemData.m_shared.m_damageModifiers.Add(new HitData.DamageModPair() { m_type = (HitData.DamageType)modHot, m_modifier = (HitData.DamageModifier)Enum.Parse(typeof(HitData.DamageModifier), "Resistant") });

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