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
            if (!mod.Config.Enable)
                return;

#if DEBUG
            mod.Monitor.Log($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", LogLevel.Info);
            mod.Monitor.Log($">>> ShowMineCartMenuPostfix / NetworkId:{networkId} / Dest:{excludeDestinationId}", LogLevel.Info);
#endif
            // Redirect if exists
            if (!string.IsNullOrWhiteSpace(networkId) && mod.AllNetworkEdits.ContainsKey(networkId) && !string.IsNullOrWhiteSpace(mod.AllNetworkEdits[networkId].RedirectNetwork))
            {
                // mod.Monitor.Log($"ShowMineCartMenuPostfix > Overwrite Redirect / Old:{networkId} / New:{mod.AllNetworkEdits[networkId].RedirectNetwork}", LogLevel.Info);
                networkId = mod.AllNetworkEdits[networkId].RedirectNetwork;
            }

            // If part of hidden network (i.e. train or bus), don't modify anything
            if (string.IsNullOrWhiteSpace(networkId) || !mod.HiddenNetworkData.ContainsKey(networkId))
            {
                // Forcefully search for the nearest excludeDestinationId if not provided or if unknown
                if (excludeDestinationId != null && !mod.AllDestinationNetwork.ContainsKey(excludeDestinationId) &&
                    mod.Config.AutoFindSource)
                    excludeDestinationId = null;
                if (excludeDestinationId == null && mod.Config.AutoFindSource)
                {
                    foreach (MinecartNetworkData minecartNetworkData in mod.AllNetworkData.Values)
                    {
                        foreach (MinecartDestinationData destinationData in minecartNetworkData.Destinations)
                        {
                            if (destinationData.TargetLocation != Game1.player.currentLocation.Name)
                                continue;
                            double distanceSquared =
                                Math.Pow((double)destinationData.TargetTile.X - Game1.player.TilePoint.X, 2.0) +
                                Math.Pow(destinationData.TargetTile.Y - Game1.player.TilePoint.Y, 2.0);
                            if (distanceSquared > 50.0)
                                continue;
                            // mod.Monitor.Log($"ShowMineCartMenuPostfix > Found Nearby Dest / Dest:{destinationData.Id} / Distance2:{distanceSquared} / Player:{Game1.player.TilePoint} / Cart:{destinationData.TargetTile}", LogLevel.Info);
                            excludeDestinationId = destinationData.Id;
                            break;
                        }

                        if (excludeDestinationId != null)
                            break;
                    }
                }

                // Forcefully correct the network ID
                if (excludeDestinationId != null && mod.AllDestinationNetwork.ContainsKey(excludeDestinationId))
                {
                    // mod.Monitor.Log($"ShowMineCartMenuPostfix > Overwrite NetworkID with DestNetwork / Old:{networkId} / New:{mod.AllDestinationNetwork[excludeDestinationId]}", LogLevel.Info);
                    networkId = mod.AllDestinationNetwork[excludeDestinationId];
                }

                if (!string.IsNullOrWhiteSpace(networkId) && mod.AllNetworkData.ContainsKey(networkId) && mod.AllNetworkData[networkId].Destinations.Count == 0)
                {
                    // mod.Monitor.Log($"ShowMineCartMenuPostfix > No destinations, treat as null / Old:{networkId}", LogLevel.Info);
                    networkId = null;
                }
            }

            // If not a bus/train network, suppress the default menu and opens this mod's menu
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
                Game1.drawObjectDialogue(TokenParser.ParseText(networkData.LockedMessage) ?? Game1.content.LoadString("Strings\\Locations:MineCart_OutOfOrder"));
            else
            {
                BetterMinecartMenuModel model = new BetterMinecartMenuModel(mod, currentNetworkId, excludeDestinationId);
                string viewName = mod.Config.UseVerticalTabs ? "VerticalTabs" : "HorizontalTabs";
                if (mod.mtm != null && mod.Config.MTM_Enable)
                    viewName += "-MTM";
                IClickableMenu menu = mod.viewEngine.CreateMenuFromAsset("Mods/Sagittaeri.BetterMinecartMenu/Views/" + viewName, model);
                Game1.activeClickableMenu = menu;
            }
        }
    }
}
