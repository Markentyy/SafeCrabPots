using StardewModdingAPI.Utilities;

namespace SafeCrabPots;

/// <summary>Player-facing settings. Every behavior is behind its own toggle.</summary>
public sealed class ModConfig
{
    /// <summary>Right-click pickup: "Off" (never), "Modifier" (only with the retrieve key held), "Always" (vanilla).</summary>
    public string RightClickPickup { get; set; } = "Modifier";

    /// <summary>Held with right-click to retrieve empty pots.</summary>
    public KeybindList RetrieveModifier { get; set; } = new KeybindList(StardewModdingAPI.SButton.LeftShift);

    /// <summary>When harvesting with bait in hand, insert one automatically.</summary>
    public bool AutoRebait { get; set; } = true;
}
