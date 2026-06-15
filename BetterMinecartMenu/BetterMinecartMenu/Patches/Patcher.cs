using System.Collections.Generic;
using Force.DeepCloner;
using StardewValley.Extensions;
using StardewValley.TokenizableStrings;

namespace BetterMinecartMenu
{
    using HarmonyLib;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Menus;
    using StardewValley.GameData.Minecarts;
    using System;

    public class Patcher
    {
        private static BetterMinecartMenu mod;

        public static void PatchAll(BetterMinecartMenu betterMinecartMenu)
        {
            mod = betterMinecartMenu;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.ShowMineCartMenu)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(ShowMineCartMenuPostfix)));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to setup required patches\n{0}:", e);
            }
        }

        [HarmonyBefore("Sagittaeri.SmartHorses")]
        public static void ShowMineCartMenuPostfix(GameLocation __instance, string networkId, string excludeDestinationId)
        {
            if (string.IsNullOrWhiteSpace(networkId) || (mod.AllNetworkData.ContainsKey(networkId) && !mod.HiddenNetworkData.ContainsKey(networkId)))
            {
                Game1.activeClickableMenu?.exitThisMenuNoSound();
                Game1.dialogueUp = false;
                Game1.player.CanMove = true;
                BetterShowMineCartMenu(__instance, networkId, excludeDestinationId);
            }
        }

        private static void BetterShowMineCartMenu(GameLocation gameLocation, string currentNetworkId, string excludeDestinationId)
        {
#if DEBUG
            foreach (string networkId in mod.AllNetworkData.Keys)
            {
                MinecartNetworkData minecartNetworkData = mod.AllNetworkData[networkId];
                string networkName = mod.AllNetworkEdits.ContainsKey(networkId) && !string.IsNullOrWhiteSpace(mod.AllNetworkEdits[networkId]?.DisplayName) ? TokenParser.ParseText(mod.AllNetworkEdits[networkId]?.DisplayName) : networkId;
                mod.Monitor.Log($"// {networkId}:{networkName} / Unlock: {minecartNetworkData.UnlockCondition}", LogLevel.Info);
                foreach (MinecartDestinationData destinationData in minecartNetworkData.Destinations)
                    mod.Monitor.Log($"---- {destinationData.Id}:{TokenParser.ParseText(destinationData.DisplayName)} in {destinationData.TargetLocation} / Unlock: {destinationData.Condition}", LogLevel.Info);
            }
#endif
            if (Game1.player.mount != null)
                return;

            MinecartNetworkData networkData;
            if (currentNetworkId == null || !mod.AllNetworkData.TryGetValue(currentNetworkId, out networkData))
                mod.Monitor.Log($"Can't show minecart menu for unknown network ID '{currentNetworkId}'.", LogLevel.Warn);
            else if (!mod.Config.AllowUnavailable && !GameStateQuery.CheckConditions(networkData.UnlockCondition, gameLocation))
                Game1.drawObjectDialogue(TokenParser.ParseText(networkData.LockedMessage) ??
                                         Game1.content.LoadString("Strings\\Locations:MineCart_OutOfOrder"));
            else
            {
                BetterMinecartMenuModel model = new BetterMinecartMenuModel(mod, excludeDestinationId);
                IClickableMenu menu = mod.viewEngine.CreateMenuFromAsset(mod.Config.UseVerticalTabs ? "Mods/Sagittaeri.BetterMinecartMenu/Views/VerticalTabs" : "Mods/Sagittaeri.BetterMinecartMenu/Views/HorizontalTabs", model);
                Game1.activeClickableMenu = menu;
            }
        }
    }
}
