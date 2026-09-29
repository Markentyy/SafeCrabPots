using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace SafeCrabPots.Framework;

/// <summary>Harmony patches implementing the safe-handling rules. All branches are defensive:
/// they only act when the pot is in the exact vanilla state they expect, otherwise vanilla runs.</summary>
internal static class CrabPotPatch
{
    /// <summary>Bait category ID (mirrors vanilla CrabPot.performObjectDropInAction).</summary>
    private const int BaitCategory = -21;

    /// <summary>Luremaster profession ID (mirrors vanilla CrabPot.DayUpdate).</summary>
    private const int LuremasterProfession = 11;

    public static void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Method(typeof(CrabPot), nameof(CrabPot.checkForAction)),
            prefix: new HarmonyMethod(typeof(CrabPotPatch), nameof(CheckForAction_Prefix)),
            postfix: new HarmonyMethod(typeof(CrabPotPatch), nameof(CheckForAction_Postfix))
        );
    }

    /// <summary>Intercepts right-clicks that vanilla would turn into accidental pickups.</summary>
    /// <returns>False to swallow the click, true to run vanilla.</returns>
    private static bool CheckForAction_Prefix(CrabPot __instance, Farmer who, bool justCheckingForActivity)
    {
        if (justCheckingForActivity)
            return true;

        // Holding bait: vanilla drop-in owns baiting, don't interfere.
        if (who.ActiveObject?.Category == BaitCategory)
            return true;

        // Catch present (lid open): vanilla harvests; the postfix may auto-rebait.
        if (__instance.tileIndexToShow == 714)
            return true;

        // Baited pot without catch: vanilla has nothing to do here either.
        if (__instance.bait.Value != null)
            return true;

        // Empty, unbaited pot: vanilla would pick it up on click. Apply the mode.
        // (Tools can't remove pots from water, so click removal is the only path.)
        string mode = ModEntry.Config.RightClickPickup;
        if (mode == "Always")
            return true;

        if (mode == "Modifier" || mode == "BeyondReach") // "BeyondReach" is the pre-1.0 name, kept for old configs
        {
            if (IsRetrieveModifierHeld())
                return true; // deliberate retrieval
            Game1.showRedMessage($"Hold {ModEntry.Config.RetrieveModifier} + right-click to retrieve");
            return false;
        }

        return false; // "Off": swallow the pickup
    }

    /// <summary>One-click rebait: after a real harvest, insert bait from hand if configured.</summary>
    private static void CheckForAction_Postfix(CrabPot __instance, Farmer who, bool justCheckingForActivity, bool __result)
    {
        if (justCheckingForActivity || !__result)
            return;
        if (!ModEntry.Config.AutoRebait)
            return;

        // Only a real harvest qualifies: the pot must still be placed, empty and unbaited.
        GameLocation location = __instance.Location;
        if (location == null || !location.objects.ContainsKey(__instance.TileLocation))
            return;
        if (__instance.heldObject.Value != null || __instance.bait.Value != null)
            return;
        if (__instance.tileIndexToShow == 714)
            return;

        // Reuse vanilla baiting so Luremaster, ownership, sounds and lid animation stay vanilla.
        if (who.ActiveObject is not StardewValley.Object held || held.Category != BaitCategory)
            return;
        if (__instance.performObjectDropInAction(held, probe: false, who: who))
            who.reduceActiveItemByOne();
    }

    private static bool IsRetrieveModifierHeld()
    {
        StardewModdingAPI.IInputHelper input = ModEntry.Instance.Helper.Input;
        foreach (StardewModdingAPI.Utilities.Keybind keybind in ModEntry.Config.RetrieveModifier.Keybinds)
        {
            foreach (SButton button in keybind.Buttons)
            {
                if (input.IsDown(button))
                    return true;
            }
        }
        return false;
    }
}
