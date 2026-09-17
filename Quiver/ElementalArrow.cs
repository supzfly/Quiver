using JotunnLib.Entities;
using JotunnLib.Managers;
using UnityEngine;

namespace Quiver
{
	class ElementalArrowPrefab : PrefabConfig
	{
		public ElementalArrowPrefab() : base("ArrowElemental", "ArrowSilver")
		{

		}

		public override void Register()
		{
			ItemDrop item = Prefab.GetComponent<ItemDrop>();
			item.m_itemData.m_shared.m_itemType = ItemDrop.ItemData.ItemType.Ammo;
			item.m_itemData.m_shared.m_name = "Elemental Arrow";
			item.m_itemData.m_shared.m_description = "With these powers combined... I am Captain Planet!!! Wait, no. That's not right.";
			item.m_itemData.m_dropPrefab = Prefab;
			item.m_itemData.m_shared.m_weight = 0.1f;
			item.m_itemData.m_shared.m_maxStackSize = 100;
			item.m_itemData.m_shared.m_variants = 1;

			item.m_itemData.m_shared.m_damages.m_pierce = 52; // based on silver arrow

			item.m_itemData.m_shared.m_damages.m_fire = 22; // fire
			item.m_itemData.m_shared.m_damages.m_spirit = 20; // silver
			item.m_itemData.m_shared.m_damages.m_frost = 52; // frost
			item.m_itemData.m_shared.m_damages.m_poison = 52; // poison

			Texture2D texture2D = QUtility.LoadTextureFromAssets("arrow_elemental.png");
			Sprite icon = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));

			item.m_itemData.m_shared.m_icons[0] = icon;

		}

	}

	class ElementalArrowRecipe : RecipeConfig
	{
		public ElementalArrowRecipe()
		{
			ObjectManager.Instance.RegisterRecipe(new RecipeConfig()
			{
				Name = "Recipe_ElementalArrows",
				Item = "ArrowElemental",
				Amount = 20,
				CraftingStation = "forge",

				Requirements = new PieceRequirementConfig[]
				{	
					new PieceRequirementConfig()
					{
						Item = "ArrowFire",
						Amount = 5
					},
					new PieceRequirementConfig()
					{
						Item = "ArrowFrost",
						Amount = 5
					},
					new PieceRequirementConfig()
					{
						Item = "ArrowPoison",
						Amount = 5
					},
					new PieceRequirementConfig()
					{
						Item = "ArrowSilver",
						Amount = 5
					},
				}
			});
		}
	}
}
