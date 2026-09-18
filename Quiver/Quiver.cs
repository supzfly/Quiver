using BepInEx;
using BepInEx.Configuration;
using Jotunn.Configs;
using Jotunn.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Quiver
{
    /// <summary>
    /// Changelog
    /// 2.0.0
    /// # Updating for Valheim 1.0
    /// # Converted to use json file for item definitions 
    /// 1.9.1
    /// # Adding Eitr regen.
    /// 1.9.0
    /// # Adding heat resistance to headtorch.
    /// # Tidying up config naming - will require a config reset
    /// 1.8.3
    /// # Changed Flametal arrows to use new naming
    /// 1.8.2
    /// # Added axe arrows coz 'why not' that's why
    /// 1.7.2
    /// # Added pickaxe arrows I guess (got lazy and didn't comment at the time)
    /// 1.6.4
    /// # Added 
    /// 1.5.4
    /// # Rewrote mod for Hearth & home
    /// # Only including basic arrows currently (no elemental/chromatic)
    /// 0.5.4
    /// # Changed Chromatic arrow to require 5 x needle arrows rather than 4 needles
    /// # Reduced bone arrow damage to 26 (was 28) (You happy now Jugger?!!!)
    /// # Added custom asset images for the new arrows
    /// 0.4.3
    /// Implemented Elemental and Chromatic Arrows
    /// </summary>
    [BepInPlugin("fly.quiver", "Quiver", "2.0.0")]
	[BepInDependency(Jotunn.Main.ModGuid)]
	public partial class Quiver : BaseUnityPlugin

	{
		// Bound config entries per arrow: arrowName -> (damageTypes, resources)
		private Dictionary<string, ConfigEntry<string>> arrowDamageTypesConfigs = new Dictionary<string, ConfigEntry<string>>();
		private Dictionary<string, ConfigEntry<string>> arrowResourcesConfigs = new Dictionary<string, ConfigEntry<string>>();

		// Hidden "!Info" Resources entry, reused as the loadheavy secret value
		private ConfigEntry<string> infoResourcesConfig;

		// Hidden "!Info" DamageTypes entry, reused as the heavy prefab stat overrides (armor/move/heat/eitr)
		private ConfigEntry<string> infoDamageTypesConfig;

		private void Awake()
		{
			// Bind config entries for arrows
			CreateConfigValues();

			// Bind heavy config entries and hook up heavy prefab creation
			InitializeHeavy();

			// Load custom items from config
			PrefabManager.OnVanillaPrefabsAvailable += LoadArrowsFromConfig;
		}

		// Bind config entries for each arrow using ArrowLoader's default data
		private void CreateConfigValues()
		{
			Config.SaveOnConfigSet = true;

			// Hidden, read-only entries bound first so their descriptions are written once at the top of the
			// config file instead of being repeated for every arrow's DamageTypes/Resources entries below.
			// Section name is prefixed with '!' so it sorts alphabetically before the arrow sections.
			infoDamageTypesConfig = Config.Bind("!Info", "DamageTypes", "",
				new ConfigDescription(
					"Damage types and values in format 'type:value,type:value,...' (e.g. pierce:26,blunt:40)",
					null,
					new ConfigurationManagerAttributes { Browsable = false }));


			infoResourcesConfig = Config.Bind("!Info", "Resources", "",
				new ConfigDescription(
					"Crafting resources in format 'itemName:amount,itemName:amount,...' (e.g. Wood:8,Feathers:2)",
					null,
					new ConfigurationManagerAttributes { Browsable = false }));

			foreach (var arrowDefaults in ArrowLoader.ArrowDefaultsList)
			{
				string arrowName = arrowDefaults.arrowName;
				var metadata = ArrowLoader.GetArrowMetadata(arrowName);
				string section = metadata != null ? metadata.displayName : arrowName;

				var damageTypesEntry = Config.Bind(section, "DamageTypes", arrowDefaults.damageTypes,
					new ConfigDescription("",
					null,
					new ConfigurationManagerAttributes { IsAdminOnly = true }));

				var resourcesEntry = Config.Bind(section, "Resources", arrowDefaults.resources,
					new ConfigDescription("",
					null,
					new ConfigurationManagerAttributes { IsAdminOnly = true }));

				arrowDamageTypesConfigs[arrowName] = damageTypesEntry;
				arrowResourcesConfigs[arrowName] = resourcesEntry;

				// Live refresh: apply updated values to the already-created arrow without requiring a restart
				damageTypesEntry.SettingChanged += (sender, args) => RefreshArrow(arrowName);
				resourcesEntry.SettingChanged += (sender, args) => RefreshArrow(arrowName);
			}
		}

		// Build an ArrowConfig for the given arrow from its currently bound config entries
		private ArrowLoader.ArrowConfig BuildArrowConfig(string arrowName)
		{
			return new ArrowLoader.ArrowConfig
			{
				arrowName = arrowName,
				damageTypes = ArrowLoader.ParseDamageTypesString(arrowDamageTypesConfigs[arrowName].Value),
				recipe = new ArrowLoader.RecipeConfig
				{
					resources = ArrowLoader.ParseResourcesString(arrowResourcesConfigs[arrowName].Value)
				}
			};
		}

		// Rebuild a single arrow's config from bound entries and push it to ArrowLoader for a live update
		private void RefreshArrow(string arrowName)
		{
			try
			{
				ArrowLoader.RefreshArrowFromConfig(BuildArrowConfig(arrowName));
			}
			catch (System.Exception ex)
			{
				Jotunn.Logger.LogError($"Error refreshing arrow '{arrowName}' from config: {ex.Message}\n{ex.StackTrace}");
			}
		}

		// Build ArrowConfig list from bound config entries and create the arrows
		private void LoadArrowsFromConfig()
		{
			try
			{
				List<ArrowLoader.ArrowConfig> arrows = new List<ArrowLoader.ArrowConfig>();

				foreach (var arrowDefaults in ArrowLoader.ArrowDefaultsList)
				{
					arrows.Add(BuildArrowConfig(arrowDefaults.arrowName));
				}

				ArrowLoader.CreateArrowsFromConfigs(arrows);
			}
			catch (System.Exception ex)
			{
				Jotunn.Logger.LogError($"Error loading Arrows from config: {ex.Message}\n{ex.StackTrace}");
			}
		}
	}
}
