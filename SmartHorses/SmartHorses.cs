namespace SmartHorses
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using StardewValley.Buildings;
    using StardewValley.Characters;
    using StardewValley.Objects;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;


    public class SmartHorses : Mod
    {
        public SmartHorsesConfig Config { get; set; }
        public bool HasHorseOverhaulMod = false;
        public bool HasHorseSqueezeMod = false;
        public bool HasWeightLossMod = false;

        private static IManifest Manifest { get; set; }

        public override void Entry(IModHelper helper)
        {
            this.HasHorseOverhaulMod = this.Helper.ModRegistry.IsLoaded("Goldenrevolver.HorseOverhaul");
            this.HasHorseSqueezeMod = this.Helper.ModRegistry.IsLoaded("jorgamun.HorseSqueeze");
            this.HasWeightLossMod = this.Helper.ModRegistry.IsLoaded("BadNetCode.PonyWeightLossProgram");

            Manifest = this.ModManifest;
            this.Config = helper.ReadConfig<SmartHorsesConfig>();
            this.Helper.Events.GameLoop.GameLaunched += delegate
            {
                SmartHorsesConfig.SetUpModConfigMenu(this.Config, this);
            };

            Patcher.PatchAll(this);
        }
    }
}
