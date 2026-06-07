using StardewModdingAPI.Events;

namespace SmartHorses
{
    using HarmonyLib;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Characters;
    using StardewValley.Extensions;
    using StardewValley.Locations;
    using StardewValley.Menus;
    using System;
    using System.Numerics;
    using xTile.Dimensions;
    using xTile.Layers;

    internal class InteractPatches
    {
        private static SmartHorses mod;
        private static bool allowInteractWhileRiding = false;
        private static Horse tempHorse = null;
        private static bool noOffset = false;
        private static bool fromDesertBus = false;

        internal static void ApplyPatches(SmartHorses smartHorses, Harmony harmony)
        {
            mod = smartHorses;

            mod.Helper.Events.Player.Warped += OnWarp;

            harmony.Patch(
                original: AccessTools.Method(typeof(BusStop), nameof(BusStop.checkAction)),
                prefix: new HarmonyMethod(typeof(InteractPatches), nameof(GameLocationCheckActionPrefix)));

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkAction)),
               prefix: new HarmonyMethod(typeof(InteractPatches), nameof(GameLocationCheckActionPrefix)));

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkAction)),
               postfix: new HarmonyMethod(typeof(InteractPatches), nameof(GameLocationCheckActionPostfix)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.isRidingHorse)),
               prefix: new HarmonyMethod(typeof(InteractPatches), nameof(IsRidingHorsePrefix)));

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.ShowMineCartMenu)),
               prefix: new HarmonyMethod(typeof(InteractPatches), nameof(ShowMineCartMenuPrefix)));

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.ShowMineCartMenu)),
               postfix: new HarmonyMethod(typeof(InteractPatches), nameof(ShowMineCartMenuPostfix)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Horse), nameof(Horse.checkAction)),
               prefix: new HarmonyMethod(typeof(InteractPatches), nameof(HorseCheckActionPrefix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(Horse), nameof(Horse.GetBoundingBox)),
                postfix: new HarmonyMethod(typeof(InteractPatches), nameof(HorseGetBoundingBoxPostfix)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Game1), "onFadedBackInComplete"),
               postfix: new HarmonyMethod(typeof(InteractPatches), nameof(OnFadedBackInCompletePostfix)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Horse), nameof(Horse.dismount)),
               prefix: new HarmonyMethod(typeof(InteractPatches), nameof(DismountPrefix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(Desert), "doorOpenAfterReturn"),
                postfix: new HarmonyMethod(typeof(InteractPatches), nameof(DoorOpenAfterReturnPostfix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(Desert), "playerReachedBusDoor"),
                prefix: new HarmonyMethod(typeof(InteractPatches), nameof(PlayerReachedBusDoorPrefix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(BusStop), "doorOpenAfterReturn"),
                postfix: new HarmonyMethod(typeof(InteractPatches), nameof(DoorOpenAfterReturnPostfix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(BusStop), "playerReachedBusDoor"),
                prefix: new HarmonyMethod(typeof(InteractPatches), nameof(PlayerReachedBusDoorPrefix)));
        }

        private static void OnWarp(object sender, WarpedEventArgs e)
        {
            if (Game1.player.CanMove)
                PutHorseInFrontOfPlayer(Game1.player, true);
            else
                PutHorseInFrontOfPlayer(Game1.player, false);
        }

        public static void ForciblyWarpHorseToPlayer(Horse horse, int delay = 0, bool mountNow = false)
        {
            // mod.Monitor.Log(string.Format("ForciblyWarpHorseToPlayer: {0} / tempHorse:{1} / fromDesertBus:{2}", Game1.currentLocation.Name, tempHorse, fromDesertBus), LogLevel.Info);
            DelayedAction.functionAfterDelay(() =>
            {
                // mod.Monitor.Log(string.Format("Remount horse: {0}/{1}", Game1.player.currentLocation, Game1.player.Position), LogLevel.Info);
                Game1.warpCharacter(horse, Game1.player.currentLocation, Game1.player.Tile);
                PutHorseInFrontOfPlayer(Game1.player, mountNow);
            }, delay);
        }

        public static void PutHorseInFrontOfPlayer(Farmer who, bool mount = false)
        {
            if (Game1.player.mount != null)
                return;

            if (!Game1.displayFarmer)
                return;

            // 1. Safety Check: Ignore events if the world isn't fully loaded yet
            if (!Context.IsWorldReady)
                return;

            // 2. Safety Check: Skip if a narrative event or cutscene is actively taking control
            if (Game1.CurrentEvent != null || Game1.eventUp)
                return;

            // 3. Safety Check: Skip if the player is currently inside a minigame (like Journey of the Prairie King)
            if (Game1.currentMinigame != null)
                return;

            // 4. Safety Check: Skip if a temporary festival map layout is active
            if (Game1.isFestival())
                return;

            Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int)Game1.player.Tile.X * 64, (int)Game1.player.Tile.Y * 64, 64, 64);
            rectangle.Inflate(128, 128);
            foreach (NPC character in Game1.player.currentLocation.characters)
            {
                if (character != null && !character.IsMonster && character is Horse && ((Horse)character).GetBoundingBox().Intersects(rectangle))
                {
                    // Move nearby horse 1 tile in front of farmer after changing maps (prevents players from accidentally changing map back when mounting a horse at the edge of map)
                    if (noOffset)
                        character.setTileLocation(Game1.player.Tile);
                    else
                        character.setTileLocation(Game1.player.Tile + Utility.DirectionsTileVectors[Game1.player.FacingDirection]);
                    noOffset = false;
                    if (mount && mod.Config.MountNearbyHorseWhenChangingMap)
                    {
                        // Auto mount nearby horse after changing maps
                        bool result = ((Horse)character).checkAction(Game1.player, Game1.player.currentLocation);
                        if (result && Game1.player.FarmerSprite.IsPlayingBasicAnimation(Game1.player.FacingDirection, carrying: false) || Game1.player.FarmerSprite.IsPlayingBasicAnimation(Game1.player.FacingDirection, carrying: true))
                            Game1.player.faceGeneralDirection(character.getStandingPosition(), 0, opposite: false, useTileCalculations: false);
                    }
                    return;
                }
            }
        }

        // Needed to fix bug where if player uses "Desert - Bus Stop" bus the game soft-locks
        [HarmonyBefore("Goldenrevolver.HorseOverhaul")]
        public static bool PlayerReachedBusDoorPrefix(GameLocation __instance, Character c, GameLocation l)
        {
            if (Game1.player.isRidingHorse())
            {
                fromDesertBus = true;
                tempHorse = Game1.player.mount;
                Game1.player.mount.dismount();
                tempHorse.Position = new Microsoft.Xna.Framework.Vector2();
            }
            return true;
        }
        public static void DoorOpenAfterReturnPostfix(GameLocation __instance, int extraInfo)
        {
            if (fromDesertBus && tempHorse != null)
            {
                ForciblyWarpHorseToPlayer(tempHorse, 0, true);
                tempHorse = null;
                fromDesertBus = false;
            }
        }

        public static bool GameLocationCheckActionPrefix(GameLocation __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            if (who == null || who.mount == null)
                return true;

            if (mod.Config.UseMinecartsWhileRiding)
            {
                // Town minecart needs a special fix since it doesn't use the usual system
                if (__instance.Name == "Town")
                {
                    Layer layer = __instance.map.GetLayer("Buildings");
                    int tileIndexAt = layer.GetTileIndexAt(tileLocation, "Landscape");
                    if (tileIndexAt == 958 || (uint)(tileIndexAt - 1080) <= 1u)
                    {
                        if (!Game1.isFestival())
                        {
                            __instance.setTileProperty(
                                (int)tileLocation.X,
                                (int)tileLocation.Y,
                                "Buildings",
                                "Action",
                                "MinecartTransport Default Town");
                        }
                        else
                        {
                            __instance.removeTileProperty(
                                (int)tileLocation.X,
                                (int)tileLocation.Y,
                                "Buildings",
                                "Action");
                        }
                    }
                }
                //string actionPropertyValue = __instance.doesTileHaveProperty(
                //    (int)tileLocation.X,
                //    (int)tileLocation.Y,
                //    "Action",
                //    "Buildings"
                //);
                //bool isMineCart = false;
                //if (!string.IsNullOrEmpty(actionPropertyValue) && actionPropertyValue.StartsWith("MinecartTransport"))
                //    isMineCart = true;
                //Console.WriteLine("LOC:{0}   PROP:{1}   MINECART?:{2}", __instance.Name, actionPropertyValue, isMineCart);
            }

            if (mod.Config.InteractWhileRiding)
            {
                bool isChair = false;
                // Prevent chairs from being used while mounted
                foreach (MapSeat mapSeat in __instance.mapSeats)
                {
                    if (mapSeat.OccupiesTile(tileLocation.X, tileLocation.Y) && !mapSeat.IsBlocked(__instance))
                    {
                        isChair = true;
                        break;
                    }
                }
                allowInteractWhileRiding = !isChair;
            }
            return true;
        }

        public static void GameLocationCheckActionPostfix(GameLocation __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            allowInteractWhileRiding = false;
        }

        public static bool IsRidingHorsePrefix(Farmer __instance, ref bool __result)
        {
            // Forces GameLocation.checkAction to interact with the world as if unmounted
            if (allowInteractWhileRiding && mod.Config.InteractWhileRiding)
            {
                __result = false;
                return false;
            }
            return true;
        }

        public static bool ShowMineCartMenuPrefix(GameLocation __instance, string networkId, string excludeDestinationId)
        {
            if (mod.Config.UseMinecartsWhileRiding)
            {
                if (allowInteractWhileRiding)
                    allowInteractWhileRiding = false;
                // Allows players to use mine carts while mounted, and the horse warps with the player
                tempHorse = Game1.player.mount;
                Game1.player.mount = null;
            }
            return true;
        }

        public static void ShowMineCartMenuPostfix(GameLocation __instance, string networkId, string excludeDestinationId)
        {
            if (tempHorse != null)
            {
                Game1.player.mount = tempHorse;
                tempHorse = null;
            }
            return;
        }
        public static bool HorseCheckActionPrefix(GameLocation __instance, Farmer who, GameLocation l, ref bool __result)
        {
            if (mod.Config.OnlyDismountOnToolActionInput)
            {
                // Prevent players from dismounting on right click i.e. must use left click or use tool button to dismount
                // This allows players to stay on the horse if they tried to interact with a chest/forage/tree but missed for example
                if (who != null && who.mount != null && !Game1.didPlayerJustLeftClick())
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }

        public static bool DismountPrefix(Horse __instance)
        {
            // Console.WriteLine("DISMOUNTING HORSE LOC:{0}   GAME LOC:{1}   PLAYER LOC:{2}", Game1.player.mount.currentLocation.Name, Game1.currentLocation.Name, Game1.player.currentLocation.Name);
            // Console.WriteLine("---DisplayFarmer:{0}   IsWorldReady:{1}   EventUp:{2}   Minigame:{3}   Festival:{4}", Game1.displayFarmer, Context.IsWorldReady, Game1.CurrentEvent != null || Game1.eventUp, Game1.currentMinigame, Game1.isFestival());

            // Special case for when opening a shop menu and previewing the farm
            if (!Game1.displayFarmer && Context.IsWorldReady && !Game1.eventUp && Game1.currentMinigame == null && !Game1.isFestival())
            {
                __instance.currentLocation = Game1.player.currentLocation;
                noOffset = true;
            }
            return true;
        }

        public static void HorseGetBoundingBoxPostfix(Horse __instance, ref Microsoft.Xna.Framework.Rectangle __result)
        {
            if (mod.Config.ThinHorse && !mod.HasHorseSqueezeMod && !mod.HasWeightLossMod && !mod.HasHorseOverhaulMod)
            {
                // Must make the horse thinner so that it doesn't get stuck in doorways after entering buildings etc
                // Note: tghis is a very simple implementation, and there's jankiness when mounting and dismounting. Horse Overhaul or Pony Weight Loss Program is recommended in addition to this mod
                if (__instance.Sprite == null)
                    __result = Microsoft.Xna.Framework.Rectangle.Empty;
                __result = new Microsoft.Xna.Framework.Rectangle((int)__instance.position.X + 32, (int)__instance.position.Y + 16, 48, 32);
            }
        }

        public static void OnFadedBackInCompletePostfix(Game1 __instance)
        {
            // mod.Monitor.Log(string.Format("onFadedBackInComplete: {0} / canMove: {1} / freezePause: {2} / noMovementPause: {3} / movementPause: {4}", Game1.currentLocation.Name, Game1.player.canMove, Game1.player.freezePause, Game1.player.noMovementPause, Game1.player.movementPause), LogLevel.Info);
            PutHorseInFrontOfPlayer(Game1.player, true);
        }
    }
}
