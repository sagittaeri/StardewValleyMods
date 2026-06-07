using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        public static void ShowMineCartMenuPostfix(GameLocation __instance, string networkId, string excludeDestinationId)
        {
            return;
            mod.Monitor.Log("Showing mine-cart menu", LogLevel.Info);
            Game1.activeClickableMenu?.exitThisMenuNoSound();
            Game1.dialogueUp = false;
            Game1.player.CanMove = true;

            BetterShowMineCartMenu(__instance, networkId, excludeDestinationId);
        }

        private static void BetterShowMineCartMenu(GameLocation gameLocation, string networkId, string excludeDestinationId)
        {
            if (Game1.player.mount != null)
                return;
            Dictionary<string, MinecartNetworkData> dictionary = DataLoader.Minecarts(Game1.content);
            MinecartNetworkData network;
            if (networkId == null || !dictionary.TryGetValue(networkId, out network))
                mod.Monitor.Log($"Can't show minecart menu for unknown network ID '{networkId}'.", LogLevel.Warn);
            else if (!GameStateQuery.CheckConditions(network.UnlockCondition, gameLocation))
                Game1.drawObjectDialogue(TokenParser.ParseText(network.LockedMessage) ??
                                         Game1.content.LoadString("Strings\\Locations:MineCart_OutOfOrder"));
            else
            {
                // Show new dialogue here
                foreach (string key in dictionary.Keys)
                {
                    mod.Monitor.Log(string.Format("Network:{0} / BuyTicketMessage:{1} / ChooseDestinationMessage:{2} / LockedMessage:{3} / UnlockCondition:{4}",
                        key, dictionary[key].BuyTicketMessage, dictionary[key].ChooseDestinationMessage, dictionary[key].LockedMessage, dictionary[key].UnlockCondition), LogLevel.Info);
                    foreach (MinecartDestinationData dest in dictionary[key].Destinations)
                    {
                        mod.Monitor.Log(string.Format("  -- {0}:{1} / Condition:{2} / Price:{3}", dest.DisplayName, dest.TargetLocation, dest.Condition, dest.Price), LogLevel.Info);
                    }
                }
            }
        }
    }
}
