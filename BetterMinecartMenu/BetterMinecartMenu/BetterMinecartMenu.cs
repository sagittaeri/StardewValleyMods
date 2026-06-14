namespace BetterMinecartMenu
{
    using Force.DeepCloner;
    using System.Collections.Generic;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewValley.TokenizableStrings;
    using StardewValley.Extensions;
    using StardewValley.GameData.Minecarts;
    using StardewValley;
    using StardewUI.Framework;

    public class BetterMinecartMenu : Mod
    {
        public BetterMinecartMenuConfig Config { get; set; }
        public IViewEngine viewEngine;

        private Dictionary<string, MinecartNetworkEdit> allNetworkEdits;
        public Dictionary<string, MinecartNetworkEdit> AllNetworkEdits
        {
            get
            {
                if (this.allNetworkEdits == null)
                    this.CacheMinecartData();
                return this.allNetworkEdits!;
            }
        }

        private Dictionary<string, MinecartNetworkData> allNetworkData;
        public Dictionary<string, MinecartNetworkData> AllNetworkData
        {
            get
            {
                if (this.allNetworkData == null)
                    this.CacheMinecartData();
                return this.allNetworkData!;
            }
        }

        private Dictionary<string, MinecartNetworkData> hiddenNetworkData;
        public Dictionary<string, MinecartNetworkData> HiddenNetworkData
        {
            get
            {
                if (this.hiddenNetworkData == null)
                    this.CacheMinecartData();
                return this.hiddenNetworkData!;
            }
        }

        private Dictionary<string, MinecartDestinationData> allDestinationData;
        public Dictionary<string, MinecartDestinationData> AllDestinationData
        {
            get
            {
                if (this.allDestinationData == null)
                    this.CacheMinecartData();
                return this.allDestinationData!;
            }
        }

        private Dictionary<string, string> allDestinationNetwork;
        public Dictionary<string, string> AllDestinationNetwork
        {
            get
            {
                if (this.allDestinationNetwork == null)
                    this.CacheMinecartData();
                return this.allDestinationNetwork!;
            }
        }


        private static IManifest Manifest { get; set; }

        public override void Entry(IModHelper helper)
        {
            Manifest = this.ModManifest;
            this.Config = helper.ReadConfig<BetterMinecartMenuConfig>();
            this.Helper.Events.GameLoop.GameLaunched += delegate
            {
                BetterMinecartMenuConfig.SetUpModConfigMenu(this.Config, this);
            };

            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.Content.AssetsInvalidated += this.OnAssetsInvalidated;
            helper.Events.GameLoop.GameLaunched += (sender, args) =>
            {
                this.viewEngine = helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI");
                this.viewEngine?.RegisterViews("Mods/Sagittaeri.BetterMinecartMenu/Views", "Views");
#if DEBUG
                this.Monitor.Log("EnableHotReloading", LogLevel.Info);
                this.viewEngine?.EnableHotReloading("/Users/kgtan/Projects/web/StardewValleyMods/BetterMinecartMenu/BetterMinecartMenu");
#endif
            };

            Patcher.PatchAll(this);
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(MinecartNetworkEdit.AssetName))
                // Important: Make sure to initialize a new instance every time
                e.LoadFrom(() => new Dictionary<string, MinecartNetworkEdit>(), AssetLoadPriority.Exclusive);
        }

        private void OnAssetsInvalidated(object sender, AssetsInvalidatedEventArgs e)
        {
            foreach (var name in e.NamesWithoutLocale)
            {
                if (name.IsEquivalentTo(MinecartNetworkEdit.AssetName))
                {
                    // assuming monitor is the IMonitor instance from ModEntry
                    this.Monitor.Log($"Asset {MinecartNetworkEdit.AssetName} invalidated, reloading.", LogLevel.Info);
                    this.allNetworkEdits = null;
                    this.allNetworkData = null;
                    this.allDestinationData = null;
                    this.allDestinationNetwork = null;
                    this.CacheMinecartData();
                }
            }
        }

        private void CacheMinecartData()
        {
            this.allNetworkData = DataLoader.Minecarts(Game1.content);
            this.hiddenNetworkData = new();
            this.allDestinationData = new();
            this.allDestinationNetwork = new();
            this.allNetworkEdits = MinecartNetworkEdit.Load();
            Dictionary<string, List<string>> destinationNetworks  = new();

            foreach (string networkId in this.allNetworkData.Keys)
            {
                MinecartNetworkData minecartNetworkData = this.allNetworkData[networkId];
                foreach (MinecartDestinationData destinationData in minecartNetworkData.Destinations)
                {
                    if (!this.allDestinationData.ContainsKey(destinationData.Id))
                        this.allDestinationData[destinationData.Id] = destinationData;
                    if (!destinationNetworks.ContainsKey(destinationData.Id))
                        destinationNetworks[destinationData.Id] = new List<string>();
                    if (!destinationNetworks[destinationData.Id].Contains(networkId))
                        destinationNetworks[destinationData.Id].Add(networkId);
                }
            }

            // Do the network edits
            if (this.allNetworkEdits != null)
            {
                foreach (string networkId in this.allNetworkEdits.Keys)
                {
                    MinecartNetworkEdit edit = this.allNetworkEdits[networkId];
                    // mod.Monitor.Log($"{networkId}: {edit}", LogLevel.Info);

                    // Remove destinations from this network
                    foreach (string destId in edit.RemoveDestinations)
                    {
                        this.allNetworkData[networkId].Destinations.RemoveWhere(e => e.Id == destId);
                        if (destinationNetworks[destId].Contains(networkId))
                            destinationNetworks[destId].Remove(networkId);
                        if (destinationNetworks[destId].Count == 0)
                            destinationNetworks.Remove(destId);
                    }

                    // Add destinations to this network
                    foreach (string destId in edit.AppendDestinations)
                    {
                        if (!this.allDestinationData.ContainsKey(destId))
                            continue;
                        if (!this.allNetworkData[networkId].Destinations.Contains(this.allDestinationData[destId]))
                            this.allNetworkData[networkId].Destinations.Add(this.allDestinationData[destId]);

                        // Remove added destinations from other networks
                        if (destinationNetworks.TryGetValue(destId, out List<string> originalNetworkIds))
                        {
                            foreach (string originalNetworkId in originalNetworkIds)
                                this.allNetworkData[originalNetworkId].Destinations.RemoveWhere(e => e.Id == destId);
                        }
                        destinationNetworks[destId] = new List<string> { networkId };
                    }

                    // Reorder destinations in the network
                    int i = 0;
                    foreach (string destId in edit.OrderFromTop)
                    {
                        if (!this.allDestinationData.ContainsKey(destId))
                            continue;
                        int index = this.allNetworkData[networkId].Destinations.IndexOf(this.allDestinationData[destId]);
                        if (index < 0)
                            continue;
                        this.allNetworkData[networkId].Destinations.RemoveAt(index);
                        this.allNetworkData[networkId].Destinations.Insert(i++, this.allDestinationData[destId]);
                    }
                    foreach (string destId in edit.OrderFromBottom)
                    {
                        if (!this.allDestinationData.ContainsKey(destId))
                            continue;
                        int index = this.allNetworkData[networkId].Destinations.IndexOf(this.allDestinationData[destId]);
                        if (index < 0)
                            continue;
                        this.allNetworkData[networkId].Destinations.RemoveAt(index);
                        this.allNetworkData[networkId].Destinations.Add(this.allDestinationData[destId]);
                    }

                    // Remove the whole network
                    if (edit.RemoveNetwork)
                        this.hiddenNetworkData[networkId] = this.allNetworkData[networkId];
                }
            }

            // Finally, update the destination network dict
            foreach (string networkId in this.allNetworkData.Keys)
            {
                MinecartNetworkData minecartNetworkData = this.allNetworkData[networkId];
                foreach (MinecartDestinationData destinationData in minecartNetworkData.Destinations)
                    this.allDestinationNetwork[destinationData.Id] = networkId;
            }
        }
    }
}
