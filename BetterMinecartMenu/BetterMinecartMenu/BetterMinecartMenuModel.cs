using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        [Notify] public Tuple<Texture2D, Rectangle> destTexture;
        [Notify] string buttonOpacity;
        [Notify] string textureOpacity;
        [Notify] string statusLayout;
        [Notify] string statusOpacity;
        [Notify] string statusText;

        internal bool active;
        internal string id;
        internal BetterMinecartMenuModel mainModel;

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
    [Notify] public SpriteFont localisedFont { get; set; }
    [Notify] public float localisedFontScale { get; set; }
    [Notify] public bool localisedFontBold { get; set; }

    private Dictionary<string, List<DestinationModel>> visibleDestinations = new();
    private Tuple<Texture2D, Rectangle> blankTexture;

    public BetterMinecartMenuModel(BetterMinecartMenu mod, string currentNetworkId = null, string currentDestinationId = null)
    {
        BetterMinecartMenuModel.mod  = mod;
        if (mod.mtm != null)
            this.blankTexture = new Tuple<Texture2D, Rectangle>(new Texture2D(Game1.graphics.GraphicsDevice, 2, 1), new Rectangle(0, 0,2,1));

        if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh
            || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja
            )
        {
            this.LocalisedFont = Game1.smallFont;
            this.LocalisedFontScale = 1f;
            this.localisedFontBold = false;
        }
        else if  (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en
                  || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de
                  || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr
                  || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es
                  || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt
                  || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it
                  )
        {
            this.LocalisedFont = Game1.tinyFont;
            this.LocalisedFontScale = 0.5f;
            this.LocalisedFontBold = true;
        }
        else if  (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
        {
            this.LocalisedFont = Game1.dialogueFont;
            this.LocalisedFontScale = 0.5f;
            this.LocalisedFontBold = true;
        }
        else if  (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr)
        {
            this.LocalisedFont = Game1.dialogueFont;
            this.LocalisedFontScale = 0.49f;
            this.LocalisedFontBold = true;
        }
        else if  (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru)
        {
            this.LocalisedFont = Game1.smallFont;
            this.LocalisedFontScale = 0.75f;
            this.LocalisedFontBold = true;
        }
        else if  (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.hu
                  || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.th)
        {
            this.LocalisedFont = Game1.smallFont;
            this.LocalisedFontScale = 0.72f;
            this.LocalisedFontBold = true;
        }
        else
        {
            // unknown language - pick safe defaults, which may be too big but at least it won't look awful
            this.LocalisedFont = Game1.smallFont;
            this.LocalisedFontScale = 1f;
            this.LocalisedFontBold = false;
        }

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
                GameLocation targetLocation = Game1.getLocationFromName(dest.TargetLocation);
                Tuple<Texture2D, Rectangle> destTex = null;
                Texture2D mapTex = null;
                if (mod.Config.MTM_HighRes && mod.Config.MTM_Enable)
                    mapTex = mod.mtm?.GetMapThumbnail(targetLocation, 4096, true, false);
                else
                    mapTex = mod.mtm?.GetMapThumbnail(targetLocation);
                if (mapTex != null && mod.Config.MTM_Enable)
                {
                    var size = targetLocation.Map.Layers[0].LayerSize;
                    int cropW = (int)Math.Round(20 * (mapTex.Width / (float)Math.Max(1, size.Width)));
                    int cropH = (int)Math.Round(10 * (mapTex.Height / (float)Math.Max(1, size.Height)));

                    int px = (int)Math.Round((dest.TargetTile.X + 0.5f) / Math.Max(1, size.Width) * mapTex.Width);
                    int py = (int)Math.Round((dest.TargetTile.Y - 0.5f) / Math.Max(1, size.Height) * mapTex.Height);
                    int cropX = Math.Clamp((int)(px - cropW / 2f), 0, Math.Max(0, mapTex.Width - cropW));
                    int cropY = Math.Clamp((int)(py - cropH / 2f), 0, Math.Max(0, mapTex.Height - cropH));
                    destTex = new Tuple<Texture2D, Rectangle>(mapTex, new Rectangle(cropX, cropY, cropW, cropH));
                }

                string statusText = "";
                if (dest.Id == this.currentDestinationId)
                    statusText = mod.Helper.Translation.Get("config.LabelYouAreHere");
                else if (!passedCondition)
                    statusText = mod.Helper.Translation.Get("config.LabelLocked");
                this.visibleDestinations[networkId].Add(new DestinationModel()
                {
                    Name = TokenParser.ParseText(dest.DisplayName),
                    DestTexture = passedCondition || !mod.Config.MTM_HideUnavailableThumbnail ? destTex : this.blankTexture,
                    ButtonOpacity = passedCondition ? "1.0" : "0.5",
                    TextureOpacity =  dest.Id != this.currentDestinationId && (mod.Config.MTM_HideUnavailableThumbnail || statusText == "") ? "1.0" : "0.5",
                    StatusLayout = mod.mtm == null || !mod.Config.MTM_Enable || statusText == "" ? "0px 0px" : "stretch 60px",
                    StatusOpacity = statusText == "" ? "0" : "1.0",
                    StatusText = statusText,
                    active = statusText == "",
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
        // mod.Monitor.Log($"BetterMinecartMenuModel / currentNetworkId:{currentNetworkId} / currentDestinationId:{currentDestinationId} / resolved:{this.currentNetworkId}", LogLevel.Info);
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
