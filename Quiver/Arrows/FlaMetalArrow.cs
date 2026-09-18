using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace Quiver
{
	partial class Quiver
	{

		private void AddFlaMetalArrows()
		{
			// Create and add a custom item
			CustomItem CI = new CustomItem("ArrowFlaMetal", "ArrowFire");
			ItemManager.Instance.AddItem(CI);

			// Replace vanilla properties of the custom item
			var itemDrop = CI.ItemDrop;
			itemDrop.m_itemData.m_shared.m_name = "Flametal Arrow";
			itemDrop.m_itemData.m_shared.m_description = "Made from the core of a meteorite. The tip shimmers with a pure intensity not seen on this earth.";
			itemDrop.m_itemData.m_shared.m_weight = 0.1f;
			itemDrop.m_itemData.m_shared.m_damages.m_pierce = 82;
			itemDrop.m_itemData.m_shared.m_damages.m_fire = 120;
			

			Texture2D texture2D = QUtility.LoadTextureFromAssets("arrow_flametal.png");
			Sprite icon = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));

			itemDrop.m_itemData.m_shared.m_icons[0] = icon;

			// Create recipe
			RecipeFlaMetalArrow(itemDrop);


		}

		private static void RecipeFlaMetalArrow(ItemDrop itemDrop)
		{
			// Create and add a recipe for the copied item
			Recipe recipe = ScriptableObject.CreateInstance<Recipe>();
			recipe.name = "Recipe_FlaMetalArrow";
			recipe.m_item = itemDrop;
			recipe.m_amount = 20;
			recipe.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>("forge");
			recipe.m_resources = new Piece.Requirement[]
			{
			new Piece.Requirement()
			{
				m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("Wood"),
				m_amount = 8
			},
			new Piece.Requirement()
			{
				m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("Feathers"),
				m_amount = 2
			},
			new Piece.Requirement()
			{
				m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("FlametalNew"),
				m_amount = 1
			}
			};

			// Since we got the prefabs from the cache, no referencing is needed
			CustomRecipe CR = new CustomRecipe(recipe, fixReference: false, fixRequirementReferences: false);
			ItemManager.Instance.AddRecipe(CR);
		}
	}
}