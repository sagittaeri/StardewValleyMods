namespace BetterMinecartMenu
{
    using StardewModdingAPI;

    public class BetterMinecartMenu : Mod
    {
        public BetterMinecartMenuConfig Config { get; set; }
        public bool HasHorseOverhaulMod;
        public bool HasHorseSqueezeMod;
        public bool HasWeightLossMod;

        private static IManifest Manifest { get; set; }

        public override void Entry(IModHelper helper)
        {
            this.HasHorseOverhaulMod = this.Helper.ModRegistry.IsLoaded("Goldenrevolver.HorseOverhaul");
            this.HasHorseSqueezeMod = this.Helper.ModRegistry.IsLoaded("jorgamun.HorseSqueeze");
            this.HasWeightLossMod = this.Helper.ModRegistry.IsLoaded("BadNetCode.PonyWeightLossProgram");

            Manifest = this.ModManifest;
            this.Config = helper.ReadConfig<BetterMinecartMenuConfig>();
            this.Helper.Events.GameLoop.GameLaunched += delegate
            {
                BetterMinecartMenuConfig.SetUpModConfigMenu(this.Config, this);
            };

            Patcher.PatchAll(this);
        }
    }
}
