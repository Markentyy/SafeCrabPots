using System;
using HarmonyLib;
using SafeCrabPots.Framework;
using StardewModdingAPI;

namespace SafeCrabPots;

/// <summary>SMAPI entry point. Applies Harmony patches and wires the GMCM options page.</summary>
public sealed class ModEntry : Mod
{
    internal static ModEntry Instance { get; private set; } = null!;
    internal static ModConfig Config { get; private set; } = null!;

    public override void Entry(IModHelper helper)
    {
        Instance = this;
        Config = helper.ReadConfig<ModConfig>();

        // Migrate the pre-1.0 mode name to the new one.
        if (Config.RightClickPickup == "BeyondReach")
        {
            Config.RightClickPickup = "Modifier";
            helper.WriteConfig(Config);
        }

        // Subscribe first: a patch failure must never kill GMCM registration again.
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;

        try
        {
            var harmony = new Harmony(ModManifest.UniqueID);
            CrabPotPatch.Apply(harmony);
        }
        catch (Exception ex)
        {
            Monitor.Log($"Failed to apply Harmony patches, some features will be unavailable:\n{ex}", LogLevel.Error);
        }
    }

    private void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
    {
        IGenericModConfigMenuApi gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (gmcm is null)
            return;

        gmcm.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );

        gmcm.AddTextOption(
            mod: ModManifest,
            getValue: () => Config.RightClickPickup,
            setValue: value => Config.RightClickPickup = value,
            name: () => "Right-click pickup",
            tooltip: () => "Off: right-click never picks pots up. Modifier: only with the retrieve key held. Always: vanilla behavior.",
            allowedValues: new[] { "Off", "Modifier", "Always" }
        );

        gmcm.AddKeybindList(
            mod: ModManifest,
            getValue: () => Config.RetrieveModifier,
            setValue: value => Config.RetrieveModifier = value,
            name: () => "Retrieve modifier",
            tooltip: () => "Held with right-click to retrieve empty pots."
        );

        gmcm.AddBoolOption(
            mod: ModManifest,
            getValue: () => Config.AutoRebait,
            setValue: value => Config.AutoRebait = value,
            name: () => "Auto-rebait on harvest",
            tooltip: () => "Harvesting with bait in hand inserts one automatically. Respects the Luremaster profession."
        );
    }
}
