using Jotunn.Managers;
using System.Security.Cryptography;
using System.Text;

namespace Quiver
{
    partial class Quiver
    {
        // SHA-256 hash of the expected loadheavy value, so the plaintext isn't readable in source
        private const string LoadHeavyHash = "19c2fe415a020812c18279fcdfd6028498ecbbf1791824dbe20e2946b3f95fe1";

        private static string ComputeSha256(string value)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value ?? string.Empty));
                StringBuilder builder = new StringBuilder(bytes.Length * 2);
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Hook up heavy prefab creation
        private void InitializeHeavy()
        {
            // for me
            if (ComputeSha256(infoResourcesConfig.Value) == LoadHeavyHash) {
                PrefabManager.OnVanillaPrefabsAvailable += LoadHeavyItems;
            }
        }

        // Build the hardcoded heavy prefabs/recipes, applying stats from the !Info DamageTypes setting
        private void LoadHeavyItems()
        {
            try
            {
                HeavyLoader.CreateHeavyItems(infoDamageTypesConfig.Value);
            }
            catch (System.Exception ex)
            {
                Jotunn.Logger.LogError($"Error loading heavy items: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
