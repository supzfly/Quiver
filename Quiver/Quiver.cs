using BepInEx;
using BepInEx.Configuration;
using Jotunn.Managers;

namespace Quiver
{
	/// <summary>
	/// Changelog
	/// 0.4.3
	/// Implemented Elemental and Chromatic Arrows
	/// 0.5.4
	/// # Changed Chromatic arrow to require 5 x needle arrows rather than 4 needles
	/// # Reduced bone arrow damage to 26 (was 28) (You happy now Jugger?!!!)
	/// # Added custom asset images for the new arrows
	/// 1.5.4
	/// # Rewrote mod for Hearth & home
	/// # Only including basic arrows currently (no elemental/chromatic)
	/// 1.6.4
	/// # Added 
	/// 1.7.2
	/// # Added pickaxe arrows I guess (got lazy and didn't comment at the time)
	/// 1.8.2
	/// # Added axe arrows coz 'why not' that's why
	/// 1.8.3
	/// # Changed Flametal arrows to use new naming
	/// 1.9.0
	/// # Adding heat resistance to headtorch.
	/// # Tidying up config naming - will require a config reset
	/// 1.9.1
	/// # Adding Eitr regen.
	/// </summary>
	[BepInPlugin("fly.quiver", "Quiver", "1.9.1")]
	[BepInDependency(Jotunn.Main.ModGuid)]
	public partial class Quiver : BaseUnityPlugin

	{
		// Configuration values
		private ConfigEntry<string> Password;
		private ConfigEntry<float> MovementValue;
		private ConfigEntry<float> HeatValue;
		private ConfigEntry<float> EitrValue;
		private ConfigEntry<int> ArmourValue;
		//private ConfigEntry<bool> BoolConfig1;

		private enum NewDamageTypes
		{
			Water = 1024
		}

		private void Awake()
		{
			//config
			CreateConfigValues();

			// Add custom items cloned from vanilla items
			PrefabManager.OnVanillaPrefabsAvailable += AddBoneArrows;
			PrefabManager.OnVanillaPrefabsAvailable += AddBlackMetalArrows;
			PrefabManager.OnVanillaPrefabsAvailable += AddCrystalArrows;
			PrefabManager.OnVanillaPrefabsAvailable += AddSurtlingArrows;
			PrefabManager.OnVanillaPrefabsAvailable += AddFlaMetalArrows;
			// mining
			//PrefabManager.OnVanillaPrefabsAvailable += AddPickaxeAntlerArrows;
			//PrefabManager.OnVanillaPrefabsAvailable += AddPickaxeBronzeArrows;
			PrefabManager.OnVanillaPrefabsAvailable += AddPickaxeIronArrows;
			// wood cutting
			//PrefabManager.OnVanillaPrefabsAvailable += AddAxeFlintArrows;
			PrefabManager.OnVanillaPrefabsAvailable += AddAxeIronArrows;
			// lawl
			PrefabManager.OnVanillaPrefabsAvailable += AddHolyHandGrendade;

			// for me
			if (Password.Value == "ysatb") {
				PrefabManager.OnVanillaPrefabsAvailable += AddHeavyHeadTorch;
				PrefabManager.OnVanillaPrefabsAvailable += AddHeavyWishbone;
				PrefabManager.OnVanillaPrefabsAvailable += AddHeavyBelt;
				PrefabManager.OnVanillaPrefabsAvailable += AddCheatSword;
				PrefabManager.OnVanillaPrefabsAvailable += AddCheatShield;
				PrefabManager.OnVanillaPrefabsAvailable += AddHeavyWisplight;
			}

		}

		// Create some sample configuration values
		private void CreateConfigValues()
		{
			Config.SaveOnConfigSet = true;

			// Add client config which can be edited in every local instance independently
			Password = Config.Bind("Client config", "PV", "",
				new ConfigDescription("PV String", 
				null,
				new ConfigurationManagerAttributes() { Browsable = false }));

			MovementValue = Config.Bind("Client config", "MV", 0.15f,
				new ConfigDescription("MV Float", 
				new AcceptableValueRange<float>(0f, 1f), 
				new ConfigurationManagerAttributes() { Browsable = false }));

			HeatValue = Config.Bind("Client config", "HV", 0.20f,
				new ConfigDescription("HV Float",
				new AcceptableValueRange<float>(0f, 1f),
				new ConfigurationManagerAttributes() { Browsable = false }));

			EitrValue = Config.Bind("Client config", "EV", 0.20f,
				new ConfigDescription("EV Float",
				new AcceptableValueRange<float>(0f, 1f),
				new ConfigurationManagerAttributes() { Browsable = false }));

			ArmourValue = Config.Bind("Client config", "AV", 150,
				new ConfigDescription("AV Int",
				null,
				new ConfigurationManagerAttributes() { Browsable = false }));

			//BoolConfig1 = Config.Bind("Client config", "BoolSetting1", false,
			//	new ConfigDescription("A boolean value",
			//	null,
			//	new ConfigurationManagerAttributes() { Browsable = false }));

		}

		// Reading and writing configuration values
		private void ReadAndWriteConfigValues()
		{
			// Reading configuration entry
			//string readValue = Setting1.Value;
			// or
			//float readBoxedValue = (float)Config["Client config", "LocalFloat"].BoxedValue;

			// Writing configuration entry
			//IntegerConfig.Value = 150;
			// or
			//Config["Client config", "LocalBool"].BoxedValue = true;
		}
	}
}
