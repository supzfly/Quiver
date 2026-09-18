using UnityEngine;

namespace Quiver
{
    /// <summary>
    /// Loads stat overrides for the "heavy" prefabs from the zArrows.DamageTypes config setting.
    /// </summary>
    public class HeavyLoader
    {
        public class HeavyConfig
        {
            public int armor = 0;
            public float moveModifier = 0f;
            public float heatResistanceModifier = 0f;
            public float eitrRegenModifier = 0f;
        }

        /// <summary>
        /// Parse a heavy config string in the format "armor:150,move:15,heat:20,eitr:20" into a HeavyConfig.
        /// move, heat and eitr are percentages (e.g. 20 becomes 0.20f).
        /// </summary>
        public static HeavyConfig ParseHeavyConfigString(string heavyConfigString)
        {
            HeavyConfig config = new HeavyConfig();
            if (string.IsNullOrEmpty(heavyConfigString))
            {
                return config;
            }

            string[] pairs = heavyConfigString.Split(',');
            foreach (string pair in pairs)
            {
                string[] parts = pair.Split(':');
                if (parts.Length != 2) continue;

                string type = parts[0].Trim().ToLowerInvariant();
                if (!float.TryParse(parts[1].Trim(), out float value)) continue;

                switch (type)
                {
                    case "armor": config.armor = (int)value; break;
                    case "move": config.moveModifier = value / 100f; break;
                    case "heat": config.heatResistanceModifier = value / 100f; break;
                    case "eitr": config.eitrRegenModifier = value / 100f; break;
                }
            }

            return config;
        }

        /// <summary>
        /// Apply the parsed heavy config values onto an item's shared data.
        /// </summary>
        public static void ApplyHeavyConfig(ItemDrop.ItemData.SharedData shared, HeavyConfig config)
        {
            if (shared == null || config == null)
            {
                return;
            }

            shared.m_armor = config.armor;
            shared.m_movementModifier = config.moveModifier;
            shared.m_heatResistanceModifier = config.heatResistanceModifier;
            shared.m_eitrRegenModifier = config.eitrRegenModifier;
        }

        /// <summary>
        /// Convenience helper: parse the config string and apply it directly to the item's shared data.
        /// </summary>
        public static void ApplyHeavyConfigFromString(ItemDrop.ItemData.SharedData shared, string heavyConfigString)
        {
            ApplyHeavyConfig(shared, ParseHeavyConfigString(heavyConfigString));
        }
    }
}
