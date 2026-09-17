using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace Quiver
{
    partial class Quiver
    {

        private void AddHolyHandGrendade()
        {
            // Create and add a custom item based on SwordBlackmetal
            CustomItem CI = new CustomItem("HolyHandgrenade", "BombOoze");
            ItemManager.Instance.AddItem(CI);

            // Replace vanilla properties of the custom item
            var itemDrop = CI.ItemDrop;
            itemDrop.m_itemData.m_shared.m_name = "Holy Hand Grenade of Antioch";
            itemDrop.m_itemData.m_shared.m_description = "First shalt thou take out the Holy Pin. Then shalt thou count to three, no more, no less. Three shall be the number thou shalt count, and the number of the counting shall be three. Four shalt thou not count, neither count thou two, excepting that thou then proceed to three. Five is right out.";
            itemDrop.m_itemData.m_shared.m_damages.m_poison = 0;
            itemDrop.m_itemData.m_shared.m_damages.m_blunt = 60;
            itemDrop.m_itemData.m_shared.m_damages.m_lightning = 42;
            itemDrop.m_itemData.m_shared.m_damages.m_fire = 42;
            itemDrop.m_itemData.m_shared.m_damages.m_spirit = 42;

            Texture2D texture2D = QUtility.LoadTextureFromAssets("holy_hand_grenade.png");
            Sprite icon = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));

            itemDrop.m_itemData.m_shared.m_icons[0] = icon;

            // Create recipe
            RecipeHolyHandGrendade(itemDrop);


        }

        private static void RecipeHolyHandGrendade(ItemDrop itemDrop)
        {
            // Create and add a recipe for the copied item
            Recipe recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.name = "Recipe_HolyHandGrendade";
            recipe.m_item = itemDrop;
            recipe.m_amount = 5;
            recipe.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>("forge");
            recipe.m_resources = new Piece.Requirement[]
            {
            new Piece.Requirement()
            {
                m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("Coins"),
                m_amount = 100
            },
            new Piece.Requirement()
            {
                m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("Silver"),
                m_amount = 1
            },
             new Piece.Requirement()
            {
                m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("Ruby"),
                m_amount = 5
            },  
            new Piece.Requirement()
            {
                m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("Resin"),
                m_amount = 5
            }
            };

            // Since we got the prefabs from the cache, no referencing is needed
            CustomRecipe CR = new CustomRecipe(recipe, fixReference: false, fixRequirementReferences: false);
            ItemManager.Instance.AddRecipe(CR);
        }
    }
}
