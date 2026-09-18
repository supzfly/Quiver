using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace Quiver
{
    partial class Quiver
    {

        private void AddPickaxeIronArrows()
        {
            // Create and add a custom item
            CustomItem CI = new CustomItem("ArrowPickaxeIron", "ArrowFlint");
            ItemManager.Instance.AddItem(CI);

            // Replace vanilla properties of the custom item
            var itemDrop = CI.ItemDrop;
            itemDrop.m_itemData.m_shared.m_name = "Iron Pickaxe Arrow";
            itemDrop.m_itemData.m_shared.m_description = "A sturdy rock breaking tool. At Range.";
            itemDrop.m_itemData.m_shared.m_weight = 0.1f; 
            itemDrop.m_itemData.m_shared.m_damages.m_pierce = 33;
            itemDrop.m_itemData.m_shared.m_damages.m_pickaxe = 33;


            Texture2D texture2D = QUtility.LoadTextureFromAssets("arrow_pickaxe_iron.png");
            Sprite icon = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));

            itemDrop.m_itemData.m_shared.m_icons[0] = icon;

            // Create recipe
            RecipePickaxeIronArrow(itemDrop);


        }

        private static void RecipePickaxeIronArrow(ItemDrop itemDrop)
        {
            // Create and add a recipe for the copied item
            Recipe recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.name = "Recipe_PickaxeIronArrow";
            recipe.m_item = itemDrop;
            recipe.m_amount = 20;
            recipe.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>("forge");
            recipe.m_resources = new Piece.Requirement[]
            {
                new Piece.Requirement()
                {
                    m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("RoundLog"),
                    m_amount = 8
                },
                new Piece.Requirement()
                {
                    m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("Iron"),
                    m_amount = 1
                }
            };

            // Since we got the prefabs from the cache, no referencing is needed
            CustomRecipe CR = new CustomRecipe(recipe, fixReference: false, fixRequirementReferences: false);
            ItemManager.Instance.AddRecipe(CR);
        }
    }
}
