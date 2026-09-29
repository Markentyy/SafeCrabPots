using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace SafeCrabPots.Framework;

// Minimal copy of the Generic Mod Config Menu API surface this mod uses.
// Full API: https://github.com/spacechase0/StardewValleyMods/tree/develop/GenericModConfigMenu
public interface IGenericModConfigMenuApi
{
    void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

    void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);

    void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

    void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
}
