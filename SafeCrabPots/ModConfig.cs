using StardewModdingAPI.Utilities;

namespace SafeCrabPots;

/// <summary>Player-facing settings. Every behavior is behind its own toggle.</summary>
public sealed class ModConfig
{
    /// <summary>Right-click pickup: "Off" (never), "BeyondReach" (only out of tool reach + modifier held), "Always" (vanilla).</summary>
    public string RightClickPickup { get; set; } = "BeyondReach";

    /// <summary>Held with right-click to retrieve out-of-reach pots in BeyondReach mode.</summary>
    public KeybindList RetrieveModifier { get; set; } = new KeybindList(StardewModdingAPI.SButton.LeftShift);

    /// <summary>Which tool hit removes crab pots: "Pickaxe", "Axe", "Any", "None".</summary>
    public string DismantleTool { get; set; } = "Pickaxe";

    /// <summary>When harvesting with bait in hand, insert one automatically.</summary>
    public bool AutoRebait { get; set; } = true;
}
