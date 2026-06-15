using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using PropertyChanged.SourceGenerator;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Minecarts;
using StardewValley.TokenizableStrings;

namespace BetterMinecartMenu;

public partial class BetterMinecartMenuModel : INotifyPropertyChanged
{
    public partial class TabModel : INotifyPropertyChanged
    {
        public string Name;
        public string Tooltip;

        [Notify] public bool active;
        [Notify] public string transform;
        internal string id;
        internal int index;
        internal BetterMinecartMenuModel mainModel;

        public void Clicked()
        {
            if (this.active)
                return;
            this.mainModel.ClickedTab(this.index);
        }
    }

    public partial class DestinationModel
    {
        public string Name;

        internal bool active;
        internal string id;
        internal BetterMinecartMenuModel mainModel;

        public float Opacity
        {
            get
            {
                if (this.active)
                    return 1f;
                return 0.5f;
            }
        }

        public void Clicked()
        {
            if (!this.active && !mod.Config.AllowUnavailable)
                return;
            this.mainModel.ClickedDestination(this.id);
        }
    }

    private static BetterMinecartMenu mod;

    public string currentNetworkId;
    public string currentDestinationId;
    public List<string> networkIds { get; set; } = new();

    [Notify] public int activeTabIndex { get; set; } = 0;
    public ObservableCollection<TabModel>  Tabs { get; set; } = new();
    public ObservableCollection<DestinationModel> Destinations { get; set; } = new();

    [Notify] public string networkName;

    private Dictionary<string, List<DestinationModel>> visibleDestinations = new();

    public BetterMinecartMenuModel(BetterMinecartMenu mod, string currentNetworkId = null, string currentDestinationId = null)
    {
        BetterMinecartMenuModel.mod  = mod;

        this.currentDestinationId = currentDestinationId;
        this.currentNetworkId = this.currentDestinationId != null && mod.AllDestinationNetwork.ContainsKey(this.currentDestinationId) ? mod.AllDestinationNetwork[this.currentDestinationId] : currentNetworkId;
        foreach (string networkId in mod.AllNetworkData.Keys)
        {
            if (!mod.AllNetworkData.ContainsKey(networkId) || !GameStateQuery.CheckConditions(mod.AllNetworkData[networkId].UnlockCondition))
            {
                if (mod.Config.HideUnavailable)
                    continue;
            }

            if (!mod.AllNetworkEdits.ContainsKey(networkId))
            {
                if (!mod.Config.ShowUnknown)
                    continue;
            }

            if (mod.HiddenNetworkData.ContainsKey(networkId))
            {
                if (!mod.Config.ShowHidden)
                    continue;
            }

            this.visibleDestinations[networkId] = new List<DestinationModel>();
            foreach (MinecartDestinationData dest in mod.AllNetworkData[networkId].Destinations)
            {
                bool passedCondition = GameStateQuery.CheckConditions(dest.Condition, Game1.player.currentLocation);
                if (mod.Config.HideUnavailable && !passedCondition)
                    continue;
                this.visibleDestinations[networkId].Add(new DestinationModel()
                {
                    Name = TokenParser.ParseText(dest.DisplayName),
                    active = dest.Id != this.currentDestinationId && passedCondition,
                    id = dest.Id,
                    mainModel = this
                });
            }

            if (this.visibleDestinations[networkId].Count == 0)
            {
                this.visibleDestinations.Remove(networkId);
                continue;
            }

            this.networkIds.Add(networkId);
        }

        int i = 0;
        foreach (string token in mod.Config.NetworkOrder.Split(","))
        {
            if (string.IsNullOrWhiteSpace(token))
                continue;
            string networkId = token.Trim();

            int index = this.networkIds.IndexOf(networkId);
            if (index < 0)
                continue;
            this.networkIds.RemoveAt(index);
            this.networkIds.Insert(i, networkId);
            i++;
        }

        i = 0;
        foreach (string networkId in this.networkIds)
        {
            if (this.currentNetworkId == networkId)
                this.ActiveTabIndex = i;

            this.Tabs.Add(new TabModel()
            {
                Active = this.currentNetworkId == networkId,
                Tooltip = mod.AllNetworkEdits.ContainsKey(networkId) && !string.IsNullOrWhiteSpace(mod.AllNetworkEdits[networkId]?.DisplayName) ? TokenParser.ParseText(mod.AllNetworkEdits[networkId]?.DisplayName) : networkId,
                Name = mod.AllNetworkEdits.ContainsKey(networkId) && !string.IsNullOrWhiteSpace(mod.AllNetworkEdits[networkId]?.TabName) ? TokenParser.ParseText(mod.AllNetworkEdits[networkId]?.TabName) : networkId.Substring(0, 1),
                index = i,
                id = networkId,
                mainModel = this
            });
            i++;
        }

        mod.Monitor.Log($"BetterMinecartMenuModel / currentNetworkId:{currentNetworkId} / currentDestinationId:{currentDestinationId} / resolved:{this.currentNetworkId}", LogLevel.Info);

        this.ClickedTab(this.ActiveTabIndex);
    }

    public void ClickedTab(int index)
    {
        this.ActiveTabIndex = index;
        foreach (TabModel tab in this.Tabs)
        {
            tab.Active = tab.index == index;
            tab.Transform = tab.index == index ? "translate: 80, 0" :  "translate: 40, 0";
        }
        string clickedNetworkId = this.networkIds[this.ActiveTabIndex];

        this.Destinations.Clear();
        foreach (DestinationModel dest in this.visibleDestinations[clickedNetworkId])
            this.Destinations.Add(dest);
        this.NetworkName = mod.AllNetworkEdits.ContainsKey(clickedNetworkId) && !string.IsNullOrWhiteSpace(mod.AllNetworkEdits[clickedNetworkId]?.DisplayName) ? TokenParser.ParseText(mod.AllNetworkEdits[clickedNetworkId]?.DisplayName) : clickedNetworkId;
    }

    public void ClickedDestination(string id)
    {
        // mod.Monitor.Log($"Clicked {id}:{this.allDestinationData[id]?.DisplayName}", LogLevel.Info);
        Game1.activeClickableMenu.exitThisMenu();

        MinecartDestinationData destination = mod.AllDestinationData[id];
        MinecartNetworkData network = mod.AllNetworkData[mod.AllDestinationNetwork[destination.Id]];
        int price = destination.Price;
        if (price < 1)
        {
            Game1.player.currentLocation.MinecartWarp(destination);
        }
        else
        {
            string numberWithCommas = Utility.getNumberWithCommas(price);
            Game1.player.currentLocation.createQuestionDialogue((destination.BuyTicketMessage ?? network.BuyTicketMessage) != null ? string.Format(TokenParser.ParseText(network.BuyTicketMessage), (object) numberWithCommas) : Game1.content.LoadString("Strings\\Locations:BuyTicket", (object) numberWithCommas), Game1.player.currentLocation.createYesNoResponses(), (GameLocation.afterQuestionBehavior) ((who, whichAnswer) =>
            {
                if (!(whichAnswer == "Yes"))
                    return;
                if (who.Money >= price)
                {
                    who.Money -= price;
                    Game1.player.currentLocation.MinecartWarp(destination);
                }
                else
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
            }));
        }
    }

    public void ClickedClose()
    {
        Game1.activeClickableMenu.exitThisMenu();
    }

    public bool HandleButtonPress(SButton button)
    {
        int pageOffset = button switch
        {
            SButton.LeftTrigger => -1,
            SButton.LeftShoulder => -1,
            SButton.RightTrigger => 1,
            SButton.RightShoulder => 1,
            _ => 0
        };
        if (pageOffset == 1 || pageOffset == -1)
        {
            this.ActiveTabIndex = (this.ActiveTabIndex + pageOffset + this.Tabs.Count) % this.Tabs.Count;
            this.ClickedTab(this.ActiveTabIndex);
            return true;
        }
        return false;
    }
}
