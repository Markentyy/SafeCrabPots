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

        var harmony = new Harmony(ModManifest.UniqueID);
        CrabPotPatch.Apply(harmony);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
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
            tooltip: () => "Off: right-click never picks pots up. BeyondReach: only out-of-reach pots, with the modifier held. Always: vanilla behavior.",
            allowedValues: new[] { "Off", "BeyondReach", "Always" }
        );

        gmcm.AddKeybindList(
            mod: ModManifest,
            getValue: () => Config.RetrieveModifier,
            setValue: value => Config.RetrieveModifier = value,
            name: () => "Retrieve modifier",
            tooltip: () => "Held with right-click to retrieve out-of-reach pots in BeyondReach mode."
        );

        gmcm.AddTextOption(
            mod: ModManifest,
            getValue: () => Config.DismantleTool,
            setValue: value => Config.DismantleTool = value,
            name: () => "Dismantle tool",
            tooltip: () => "Which tool hit removes crab pots. Other tools pass through harmlessly.",
            allowedValues: new[] { "Pickaxe", "Axe", "Any", "None" }
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
