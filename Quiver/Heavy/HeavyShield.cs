using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace Quiver
{
	partial class Quiver
	{
		private void AddHeavyShield()
		{
			// Create and add a custom item 
			CustomItem CI = new CustomItem("ShieldHeavy", "ShieldKnight");
			ItemManager.Instance.AddItem(CI);

			// Replace vanilla properties of the custom item
			var itemDrop = CI.ItemDrop;
			itemDrop.m_itemData.m_shared.m_name = "Heavy Shield";

            // Create recipe
            RecipeHeavyShield(itemDrop);

		}

		private static void RecipeHeavyShield(ItemDrop itemDrop)
		{
			// Create and add a recipe for the copied item
			Recipe recipe = ScriptableObject.CreateInstance<Recipe>();
			recipe.name = "Recipe_HeavyShield";
			recipe.m_item = itemDrop;
			recipe.m_amount = 1;
			recipe.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>("piece_workbench");
			recipe.m_resources = new Piece.Requirement[]
			{
			new Piece.Requirement()
			{
				m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("Stone"),
				m_amount = 1
			}
			};

			// Since we got the prefabs from the cache, no referencing is needed
			CustomRecipe CR = new CustomRecipe(recipe, fixReference: false, fixRequirementReferences: false);
			ItemManager.Instance.AddRecipe(CR);
		}
	}
}