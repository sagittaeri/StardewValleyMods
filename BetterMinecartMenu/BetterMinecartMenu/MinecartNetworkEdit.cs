using HarmonyLib;
using StardewValley;
using StardewValley.TokenizableStrings;

namespace BetterMinecartMenu;

using System.Collections.Generic;

public class MinecartNetworkEdit
{
    public static readonly string AssetName = "Sagittaeri.BetterMinecartMenu/MinecartNetworkEdits";

    public string DisplayName = "";
    public string TabName = "";
    public bool RemoveNetwork = false;
    public string RedirectNetwork = "";
    public List<string> RemoveDestinations = new();
    public List<string> AppendDestinations = new();
    public List<string> OrderFromTop = new();
    public List<string> OrderFromBottom = new();

    public override string ToString()
    {
        return $"[MNE]({TokenParser.ParseText(this.TabName)}){TokenParser.ParseText(this.DisplayName)} RemoveNetwork:{this.RemoveNetwork} RedirectNetwork:{this.RedirectNetwork} RemoveDestinations:{string.Join(',', this.RemoveDestinations)} AppendDestinations:{string.Join(',', this.AppendDestinations)}";
    }

    public static Dictionary<string, MinecartNetworkEdit> Load()
    {
        return Game1.content.Load<Dictionary<string, MinecartNetworkEdit>>(AssetName);
    }
}
