namespace SmartHorses
{
    using StardewModdingAPI;
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using System;

    using GenericModConfigMenu;


    /// <summary>
    /// Config file for the mod
    /// </summary>
    public class SmartHorsesConfig
    {
        public bool InteractWhileRiding { get; set; } = true;

        public bool UseMinecartsWhileRiding { get; set; } = true;

        public bool MountNearbyHorseWhenChangingMap { get; set; } = true;

        public bool OnlyDismountOnToolActionInput { get; set; } = true;

        public bool ThinHorse { get; set; } = true;

        public static void SetUpModConfigMenu(SmartHorsesConfig config, SmartHorses mod)
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
                        config.InteractWhileRiding = true;
                        config.UseMinecartsWhileRiding = true;
                        config.MountNearbyHorseWhenChangingMap = true;
                        config.OnlyDismountOnToolActionInput = true;
                        config.ThinHorse = true;
                    }
                    else
                    {
                        config = new SmartHorsesConfig();
                    }
                },
                save: delegate
                {
                    mod.Helper.WriteConfig(config);
                }
            );

            // add some config options
            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.InteractWhileRiding.name"),
                tooltip: () => mod.Helper.Translation.Get("config.InteractWhileRiding.tooltip"),
                getValue: () => mod.Config.InteractWhileRiding,
                setValue: value => mod.Config.InteractWhileRiding = value
            );

            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.UseMinecartsWhileRiding.name"),
                tooltip: () => mod.Helper.Translation.Get("config.UseMinecartsWhileRiding.tooltip"),
                getValue: () => mod.Config.UseMinecartsWhileRiding,
                setValue: value => mod.Config.UseMinecartsWhileRiding = value
            );

            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.MountNearbyHorseWhenChangingMap.name"),
                tooltip: () => mod.Helper.Translation.Get("config.MountNearbyHorseWhenChangingMap.tooltip"),
                getValue: () => mod.Config.MountNearbyHorseWhenChangingMap,
                setValue: value => mod.Config.MountNearbyHorseWhenChangingMap = value
            );

            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.OnlyDismountOnToolActionInput.name"),
                tooltip: () => mod.Helper.Translation.Get("config.OnlyDismountOnToolActionInput.tooltip"),
                getValue: () => mod.Config.OnlyDismountOnToolActionInput,
                setValue: value => mod.Config.OnlyDismountOnToolActionInput = value
            );

            api.AddBoolOption(
                mod: manifest,
                name: () => mod.Helper.Translation.Get("config.ThinHorse.name"),
                tooltip: () => mod.Helper.Translation.Get("config.ThinHorse.tooltip"),
                getValue: () => mod.Config.ThinHorse,
                setValue: value => mod.Config.ThinHorse = value
            );
        }
    }
}
