using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using InputTools;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace InputTools
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public InputToolsAPI inputTools;
        public Actions actions;
        public ControlStack controlStack;

        public List<SButton> buttonsPressing = new List<SButton>();
        public List<Tuple<SButton, SButton>> buttonPairsPressing = new List<Tuple<SButton, SButton>>();
        public List<Tuple<SButton, SButton>> buttonPairsReleased = new List<Tuple<SButton, SButton>>();
        public int tickButtonPairsReleased;
        public IInputToolsAPI.InputDevice lastInputDevice;
        public Vector2 lastCursorScreenPixels;
        public Vector2 lastTileHighlightPos;
        public Item lastItemHeld;
        public bool isLastPlacementTileFromCursor;
        public bool isFarmerMovedLastTick;
        public bool isCursorMovedLastTick;
        public bool isPlacementTileMovedLastTick;
        public bool isItemChangedLastTick;
        public Vector2 moveAxisLastTick;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;

            this.actions = new Actions(this);
            this.controlStack = new ControlStack(this);
        }

        public override object GetApi()
        {
            this.inputTools = new InputToolsAPI(this);
            return this.inputTools;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            //if (!Context.IsWorldReady)
            //    return;

            this.lastInputDevice = this.inputTools.GetInputDevice(e.Button);

            if (this.buttonsPressing.Count > 0)
            {
                foreach (SButton heldButton in this.buttonsPressing)
                {
                    Tuple<SButton, SButton> buttonPair = new Tuple<SButton, SButton>(heldButton, e.Button);
                    if (!this.buttonPairsPressing.Contains(buttonPair))
                        this.buttonPairsPressing.Add(buttonPair);
                    foreach (string groupID in this.actions.GetActionsFromKeyPair(buttonPair))
                        this.inputTools._Global.OnActionPressed(groupID);
                    this.inputTools._Global.OnButtonPairPressed(buttonPair);
                    //this.Monitor.Log($"{Game1.ticks} ButtonPairPressed {buttonPair}", LogLevel.Debug);
                }
            }

            if (!this.buttonsPressing.Contains(e.Button))
                this.buttonsPressing.Add(e.Button);
            this.inputTools._Global.OnButtonPressed(e.Button);
            foreach (string groupID in this.actions.GetActionsFromKey(e.Button))
                this.inputTools._Global.OnActionPressed(groupID);

            if (this.inputTools.IsConfirmButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this.inputTools._Global.OnConfirmPressed(this.inputTools.IsConfirmButton(e.Button));
            if (this.inputTools.IsCancelButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this.inputTools._Global.OnCancelPressed(this.inputTools.IsCancelButton(e.Button));
            if (this.inputTools.IsAltButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this.inputTools._Global.OnAltPressed(this.inputTools.IsAltButton(e.Button));
            if (this.inputTools.IsMenuButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this.inputTools._Global.OnMenuPressed(this.inputTools.IsMenuButton(e.Button));

            if (this.inputTools.IsMoveRightButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this.inputTools._Global.OnMoveRightPressed(this.inputTools.IsMoveRightButton(e.Button));
            if (this.inputTools.IsMoveDownButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this.inputTools._Global.OnMoveDownPressed(this.inputTools.IsMoveDownButton(e.Button));
            if (this.inputTools.IsMoveLeftButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this.inputTools._Global.OnMoveLeftPressed(this.inputTools.IsMoveLeftButton(e.Button));
            if (this.inputTools.IsMoveUpButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this.inputTools._Global.OnMoveUpPressed(this.inputTools.IsMoveUpButton(e.Button));

            Vector2 moveAxis = this.inputTools._Global.GetMoveAxis();
            if (this.moveAxisLastTick == Vector2.Zero && moveAxis != Vector2.Zero)
                this.inputTools._Global.OnMoveAxisPressed(moveAxis);
            this.moveAxisLastTick = moveAxis;

            //this.Monitor.Log($"{Game1.ticks} ButtonPressed {e.Button}", LogLevel.Debug);
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            //if (!Context.IsWorldReady)
            //    return;

            if (this.buttonPairsPressing.Count > 0)
            {
                this.buttonPairsReleased.Clear();
                this.tickButtonPairsReleased = Game1.ticks;
                foreach (Tuple<SButton, SButton> buttonPair in new List<Tuple<SButton, SButton>>(this.buttonPairsPressing))
                {
                    if (buttonPair.Item1 == e.Button || buttonPair.Item2 == e.Button)
                    {
                        this.buttonPairsReleased.Add(buttonPair);
                        this.buttonPairsPressing.Remove(buttonPair);
                        this.inputTools._Global.OnButtonPairReleased(buttonPair);
                        //this.Monitor.Log($"{Game1.ticks} ButtonPairRemoved {buttonPair}", LogLevel.Debug);
                        foreach (string groupID in this.actions.GetActionsFromKeyPair(buttonPair))
                            this.inputTools._Global.OnActionReleased(groupID);
                    }
                }
            }

            if (this.buttonsPressing.Contains(e.Button))
                this.buttonsPressing.Remove(e.Button);
            this.inputTools._Global.OnButtonReleased(e.Button);
            foreach (string groupID in this.actions.GetActionsFromKey(e.Button))
                this.inputTools._Global.OnActionReleased(groupID);

            if (this.inputTools.IsConfirmButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this.inputTools._Global.OnConfirmReleased(this.inputTools.IsConfirmButton(e.Button));
            if (this.inputTools.IsCancelButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this.inputTools._Global.OnCancelReleased(this.inputTools.IsCancelButton(e.Button));
            if (this.inputTools.IsAltButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this.inputTools._Global.OnAltReleased(this.inputTools.IsAltButton(e.Button));
            if (this.inputTools.IsMenuButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this.inputTools._Global.OnMenuReleased(this.inputTools.IsMenuButton(e.Button));

            if (this.inputTools.IsMoveRightButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this.inputTools._Global.OnMoveRightReleased(this.inputTools.IsMoveRightButton(e.Button));
            if (this.inputTools.IsMoveDownButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this.inputTools._Global.OnMoveDownReleased(this.inputTools.IsMoveDownButton(e.Button));
            if (this.inputTools.IsMoveLeftButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this.inputTools._Global.OnMoveLeftReleased(this.inputTools.IsMoveLeftButton(e.Button));
            if (this.inputTools.IsMoveUpButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this.inputTools._Global.OnMoveUpReleased(this.inputTools.IsMoveUpButton(e.Button));

            Vector2 moveAxis = this.inputTools._Global.GetMoveAxis();
            if (this.moveAxisLastTick != Vector2.Zero && moveAxis == Vector2.Zero)
                this.inputTools._Global.OnMoveAxisReleased(moveAxis);
            this.moveAxisLastTick = Vector2.Zero;

            //this.Monitor.Log($"{Game1.ticks} ButtonReleased {e.Button}", LogLevel.Debug);
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            //if (!Context.IsWorldReady)
            //    return;

            this.inputTools.CurrentInputDevice();

            this.isFarmerMovedLastTick = Game1.player.lastPosition != Game1.player.Position;
            this.isCursorMovedLastTick = this.lastCursorScreenPixels != this.Helper.Input.GetCursorPosition().ScreenPixels;
            this.lastCursorScreenPixels = this.Helper.Input.GetCursorPosition().ScreenPixels;
            this.isPlacementTileMovedLastTick = false;
            this.isItemChangedLastTick = this.lastItemHeld != Game1.player.CurrentItem;
            this.lastItemHeld = Game1.player.CurrentItem;
            if (this.isItemChangedLastTick)
                this.inputTools.OnPlacementItemChanged(Game1.player.CurrentItem);

            bool isKeyboardMoveButtonHeld = this.inputTools._Global.IsMoveButtonHeld(keyboardWASD: true, keyboardArrows: false, controllerThumbstick: false, controllerDPad: false) != IInputToolsAPI.MoveSource.None;
            bool isControllerMoveButtonHeld = this.inputTools._Global.IsMoveButtonHeld(keyboardWASD: false, keyboardArrows: false, controllerThumbstick: true, controllerDPad: true) != IInputToolsAPI.MoveSource.None;
            if ((!Game1.wasMouseVisibleThisFrame && this.isItemChangedLastTick) || (this.isFarmerMovedLastTick && !isKeyboardMoveButtonHeld) || isControllerMoveButtonHeld)
            {
                // If controller last used, placement tile is the grab tile i.e. tile in front of player
                Game1.timerUntilMouseFade = 0;
                this.isPlacementTileMovedLastTick = this.lastTileHighlightPos != this.inputTools.GetPlacementTileWithController();
                if (this.isPlacementTileMovedLastTick)
                {
                    this.lastTileHighlightPos = this.inputTools.GetPlacementTileWithController();
                    this.isLastPlacementTileFromCursor = false;
                    this.inputTools.OnPlacementTileChanged(this.lastTileHighlightPos);
                }
            }
            else if ((this.isLastPlacementTileFromCursor && this.isItemChangedLastTick) || this.isCursorMovedLastTick || isKeyboardMoveButtonHeld)
            {
                // Otherwise placement tile is the tile under the cursor
                this.isPlacementTileMovedLastTick = this.lastTileHighlightPos != this.Helper.Input.GetCursorPosition().Tile;
                if (this.isPlacementTileMovedLastTick)
                {
                    this.lastTileHighlightPos = this.Helper.Input.GetCursorPosition().Tile;
                    this.isLastPlacementTileFromCursor = true;
                    this.inputTools.OnPlacementTileChanged(this.lastTileHighlightPos);
                }
            }

            foreach (SButton button in this.buttonsPressing)
            {
                this.inputTools._Global.OnButtonHeld(button);
                foreach (string groupID in this.actions.GetActionsFromKey(button))
                    this.inputTools._Global.OnActionHeld(groupID);

                if (this.inputTools.IsConfirmButton(button) != IInputToolsAPI.InputDevice.None)
                    this.inputTools._Global.OnConfirmHeld(this.inputTools.IsConfirmButton(button));
                if (this.inputTools.IsCancelButton(button) != IInputToolsAPI.InputDevice.None)
                    this.inputTools._Global.OnCancelHeld(this.inputTools.IsCancelButton(button));
                if (this.inputTools.IsAltButton(button) != IInputToolsAPI.InputDevice.None)
                    this.inputTools._Global.OnAltHeld(this.inputTools.IsAltButton(button));
                if (this.inputTools.IsMenuButton(button) != IInputToolsAPI.InputDevice.None)
                    this.inputTools._Global.OnMenuHeld(this.inputTools.IsMenuButton(button));

                if (this.inputTools.IsMoveRightButton(button) != IInputToolsAPI.MoveSource.None)
                    this.inputTools._Global.OnMoveRightHeld(this.inputTools.IsMoveRightButton(button));
                if (this.inputTools.IsMoveDownButton(button) != IInputToolsAPI.MoveSource.None)
                    this.inputTools._Global.OnMoveDownHeld(this.inputTools.IsMoveDownButton(button));
                if (this.inputTools.IsMoveLeftButton(button) != IInputToolsAPI.MoveSource.None)
                    this.inputTools._Global.OnMoveLeftHeld(this.inputTools.IsMoveLeftButton(button));
                if (this.inputTools.IsMoveUpButton(button) != IInputToolsAPI.MoveSource.None)
                    this.inputTools._Global.OnMoveUpHeld(this.inputTools.IsMoveUpButton(button));
            }
            foreach (Tuple<SButton, SButton> buttonPair in this.buttonPairsPressing)
            {
                this.inputTools._Global.OnButtonPairHeld(buttonPair);
                foreach (string groupID in this.actions.GetActionsFromKeyPair(buttonPair))
                    this.inputTools._Global.OnActionHeld(groupID);
            }

            this.moveAxisLastTick = this.inputTools._Global.GetMoveAxis();
            if (this.moveAxisLastTick != Vector2.Zero)
                this.inputTools._Global.OnMoveAxisHeld(this.moveAxisLastTick);
        }
    }
}
