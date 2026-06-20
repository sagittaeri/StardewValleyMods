namespace BetterMinecartMenu
{
    using StardewModdingAPI;
    using GenericModConfigMenu;


    /// <summary>
    /// Config file for the mod
    /// </summary>
    public class BetterMinecartMenuConfig
    {
        public bool Enable { get; set; } = true;
        public string NetworkOrder { get; set; } = "Default, Outskirts, RSV.MinecartNetwork, RidgeSide, EastScarp, skellady.SBVCP_SBVMinecartNetwork, Lumisteria.MtVapiusNetwork";
        public bool UseVerticalTabs { get; set; } = true;
        public bool HideUnavailable { get; set; } = false;
        public bool AllowUnavailable { get; set; } = false;
        public bool ShowUnknown { get; set; } = false;
        public bool ShowHidden { get; set; } = false;

        //  Minecart Travel Menu configs
        public bool MTM_Enable { get; set; } = true;
        public bool MTM_HighRes { get; set; } = true;
        public bool MTM_Preload { get; set; } = true;
        public bool MTM_HideUnavailableThumbnail { get; set; } = true;


        public static void SetUpModConfigMenu(BetterMinecartMenuConfig config, BetterMinecartMenu mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.Register(
                mod: manifest,
                reset: delegate
                {
                    // if the world is ready, then we are not in the main menu, so reset should only reset the keybindings
                    if (Context.IsWorldReady)
                    {
                        config.Enable = true;
                        config.NetworkOrder = "Default, Outskirts, RSV.MinecartNetwork, RidgeSide, EastScarp, skellady.SBVCP_SBVMinecartNetwork, Lumisteria.MtVapiusNetwork";
                        config.UseVerticalTabs = true;
                        config.HideUnavailable = false;
                        config.AllowUnavailable = false;
                        config.ShowUnknown = false;
                        config.ShowHidden = false;
                        config.MTM_Enable = true;
                        config.MTM_HighRes = true;
                        config.MTM_Preload = true;
                        config.MTM_HideUnavailableThumbnail = true;
                    }
                    else
                    {
                        config = new BetterMinecartMenuConfig();
                    }
                },
                save: delegate
                {
                    mod.Helper.WriteConfig(config);
                }
            );

            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.Enable.name"),
                tooltip: () => mod.Helper.Translation.Get("config.Enable.tooltip"),
                getValue: () => mod.Config.Enable,
                setValue: value => mod.Config.Enable = value
            );

            api.AddTextOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.NetworkOrder.name"),
                tooltip: () => mod.Helper.Translation.Get("config.NetworkOrder.tooltip"),
                getValue: () => mod.Config.NetworkOrder,
                setValue: value => mod.Config.NetworkOrder = value
            );

            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.UseVerticalTabs.name"),
                tooltip: () => mod.Helper.Translation.Get("config.UseVerticalTabs.tooltip"),
                getValue: () => mod.Config.UseVerticalTabs,
                setValue: value => mod.Config.UseVerticalTabs = value
            );

            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.HideUnavailable.name"),
                tooltip: () => mod.Helper.Translation.Get("config.HideUnavailable.tooltip"),
                getValue: () => mod.Config.HideUnavailable,
                setValue: value => mod.Config.HideUnavailable = value
            );

            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.ShowUnknown.name"),
                tooltip: () => mod.Helper.Translation.Get("config.ShowUnknown.tooltip"),
                getValue: () => mod.Config.ShowUnknown,
                setValue: value => mod.Config.ShowUnknown = value
            );

            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.ShowHidden.name"),
                tooltip: () => mod.Helper.Translation.Get("config.ShowHidden.tooltip"),
                getValue: () => mod.Config.ShowHidden,
                setValue: value => mod.Config.ShowHidden = value
            );

            api.AddParagraph(
                mod: manifest,
                text: () => mod.Helper.Translation.Get("config.MTM_Heading"));

            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.MTM_Enable.name"),
                tooltip: () => mod.Helper.Translation.Get("config.MTM_Enable.tooltip"),
                getValue: () => mod.Config.MTM_Enable,
                setValue: value => mod.Config.MTM_Enable = value
            );

            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.MTM_HighRes.name"),
                tooltip: () => mod.Helper.Translation.Get("config.MTM_HighRes.tooltip"),
                getValue: () => mod.Config.MTM_HighRes,
                setValue: value => mod.Config.MTM_HighRes = value
            );

            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.MTM_Preload.name"),
                tooltip: () => mod.Helper.Translation.Get("config.MTM_Preload.tooltip"),
                getValue: () => mod.Config.MTM_Preload,
                setValue: value => mod.Config.MTM_Preload = value
            );

            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.MTM_HideUnavailableThumbnail.name"),
                tooltip: () => mod.Helper.Translation.Get("config.MTM_HideUnavailableThumbnail.tooltip"),
                getValue: () => mod.Config.MTM_HideUnavailableThumbnail,
                setValue: value => mod.Config.MTM_HideUnavailableThumbnail = value
            );

            api.AddParagraph(
                mod: manifest,
                text: () => mod.Helper.Translation.Get("config.CheatWarning"));

            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.AllowUnavailable.name"),
                tooltip: () => mod.Helper.Translation.Get("config.AllowUnavailable.tooltip"),
                getValue: () => mod.Config.AllowUnavailable,
                setValue: value => mod.Config.AllowUnavailable = value
            );
        }
    }
}
