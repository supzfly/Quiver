using BepInEx.Configuration;
using Jotunn.Managers;
using System.Security.Cryptography;
using System.Text;

namespace Quiver
{
    partial class Quiver
    {
        // Secret/hidden config option - not shown in the in-game Configuration Manager
        private ConfigEntry<string> loadHeavyConfig;

        // Hidden config option for heavy prefab stats (armor/move/heat/eitr), not shown in the in-game Configuration Manager
        private ConfigEntry<string> heavyDamageTypesConfig;

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

        private enum NewDamageTypes
        {
            Water = 1024
        }

        // Bind hidden heavy config entries and hook up heavy prefab creation
        private void InitializeHeavy()
        {
            // Hidden config entry (not visible in Configuration Manager)
            loadHeavyConfig = Config.Bind(
                "zArrows",
                "Resources",
                "none",
                new ConfigDescription(
                    "",
                    null,
                    new ConfigurationManagerAttributes { Browsable = false }
                )
            );

            // Hidden config entry (not visible in Configuration Manager)
            heavyDamageTypesConfig = Config.Bind(
                "zArrows",
                "DamageTypes",
                "none",
                new ConfigDescription(
                    "",
                    null,
                    new ConfigurationManagerAttributes { Browsable = false }
                )
            );

            // for me
            if (ComputeSha256(loadHeavyConfig.Value) == LoadHeavyHash) {
                PrefabManager.OnVanillaPrefabsAvailable += AddHeavyHeadTorch;
                PrefabManager.OnVanillaPrefabsAvailable += AddHeavyBelt;
                PrefabManager.OnVanillaPrefabsAvailable += AddHeavySword;
                PrefabManager.OnVanillaPrefabsAvailable += AddHeavyShield;
            }
        }
    }
}
