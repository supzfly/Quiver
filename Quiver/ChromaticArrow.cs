using JotunnLib.Entities;
using JotunnLib.Managers;
using UnityEngine;

namespace Quiver
{
	class ChromaticArrowPrefab : PrefabConfig
	{
		public ChromaticArrowPrefab() : base("ArrowChromatic", "ArrowNeedle")
		{

		}

		public override void Register()
		{
			ItemDrop item = Prefab.GetComponent<ItemDrop>();
			item.m_itemData.m_shared.m_itemType = ItemDrop.ItemData.ItemType.Ammo;
			item.m_itemData.m_shared.m_name = "Chromatic Arrow";
			item.m_itemData.m_shared.m_description = "Super Zeo Megazord Power!";
			item.m_itemData.m_dropPrefab = Prefab;
			item.m_itemData.m_shared.m_weight = 0.2f;
			item.m_itemData.m_shared.m_maxStackSize = 100;
			item.m_itemData.m_shared.m_variants = 1;

			item.m_itemData.m_shared.m_damages.m_pierce = 62; // needle

			item.m_itemData.m_shared.m_damages.m_blunt = 40; // black metal
			item.m_itemData.m_shared.m_damages.m_lightning = 40; // crystal
			item.m_itemData.m_shared.m_damages.m_fire = 22; // fire
			item.m_itemData.m_shared.m_damages.m_spirit = 20; // silver
			item.m_itemData.m_shared.m_damages.m_frost = 52; // frost
			item.m_itemData.m_shared.m_damages.m_poison = 52; // poison

			Texture2D texture2D = QUtility.LoadTextureFromAssets("arrow_chromatic.png");
			Sprite icon = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));

			item.m_itemData.m_shared.m_icons[0] = icon;

		}

	}

	class ChromaticArrowRecipe : RecipeConfig
	{
		public ChromaticArrowRecipe()
		{
			ObjectManager.Instance.RegisterRecipe(new RecipeConfig()
			{
				Name = "Recipe_ChromaticArrows",
				Item = "ArrowChromatic",
				Amount = 20,
				CraftingStation = "forge",

				Requirements = new PieceRequirementConfig[]
				{
					new PieceRequirementConfig()
					{
						Item = "ArrowNeedle",
						Amount = 5
					},
					new PieceRequirementConfig()
					{
						Item = "ArrowElemental",
						Amount = 5
					},
					
					new PieceRequirementConfig()
					{
						Item = "ArrowBlackMetal",
						Amount = 5
					},
					new PieceRequirementConfig()
					{
						Item = "ArrowCrystal",
						Amount = 5
					},
					
				}
			});
		}
	}
}
