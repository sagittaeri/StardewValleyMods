using System;
using System.Collections;
using System.Collections.Generic;
using InputTools;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using static InputToolsAPI;

public class InputToolsAPI
{
    /*
    // Copy and paste the interface below if needed
        public interface IInputToolsAPI
        {
            int GetInputDevice(SButton button); // 0 = Mouse, 1 = Keyboard, 2 = Controller
            bool IsKeyboardMoveButtonPressing();
            bool IsControllerMoveButtonPressing();
            bool IsControllerCursorButtonPressing();
            bool IsTargetedTileChanged();
            bool IsTargetedTileFromCursor();
            Vector2 GetTargetedTile();
            Vector2 GetTargetedTileWithController();
        }
    */

    private ModEntry modEntry;

    public InputToolsAPI(ModEntry modEntry)
    {
        this.modEntry = modEntry;
    }

    public enum InputDevice
    {
        Mouse,
        Keyboard,
        Controller
    }

    public int GetInputDevice(SButton button)
    {
        if (SButtonExtensions.TryGetController(button, out _))
            return (int)InputDevice.Controller;
        if (SButtonExtensions.TryGetKeyboard(button, out _))
            return (int)InputDevice.Keyboard;
        return (int)InputDevice.Mouse;
    }

    public bool IsKeyboardMoveButtonPressing()
    {
        return this.modEntry.buttonsPressing.Contains(SButton.W)
            || this.modEntry.buttonsPressing.Contains(SButton.D)
            || this.modEntry.buttonsPressing.Contains(SButton.S)
            || this.modEntry.buttonsPressing.Contains(SButton.A);
    }

    public bool IsControllerMoveButtonPressing()
    {
        return this.modEntry.buttonsPressing.Contains(SButton.LeftThumbstickUp)
            || this.modEntry.buttonsPressing.Contains(SButton.LeftThumbstickRight)
            || this.modEntry.buttonsPressing.Contains(SButton.LeftThumbstickDown)
            || this.modEntry.buttonsPressing.Contains(SButton.LeftThumbstickLeft)
            || this.modEntry.buttonsPressing.Contains(SButton.DPadLeft)
            || this.modEntry.buttonsPressing.Contains(SButton.DPadRight)
            || this.modEntry.buttonsPressing.Contains(SButton.DPadDown)
            || this.modEntry.buttonsPressing.Contains(SButton.DPadUp);
    }

    public bool IsControllerCursorButtonPressing()
    {
        return this.modEntry.buttonsPressing.Contains(SButton.RightThumbstickUp)
            || this.modEntry.buttonsPressing.Contains(SButton.RightThumbstickRight)
            || this.modEntry.buttonsPressing.Contains(SButton.RightThumbstickDown)
            || this.modEntry.buttonsPressing.Contains(SButton.RightThumbstickLeft);
    }

    public bool IsHeldItemBomb()
    {
        Item item = Game1.player.CurrentItem;
        if (item == null)
            return false;
        int itemID = item.ParentSheetIndex;
        return Utility.IsNormalObjectAtParentSheetIndex(item, itemID) && (itemID == 286 || itemID == 287 || itemID == 288);
    }

    public bool IsTargetedTileFromCursor()
    {
        return this.modEntry.isLastTargetedTileFromCursor;
    }


    public bool IsTargetedTileChanged()
    {
        return this.modEntry.isTargetedTileMovedLastTick;
    }

    public Vector2 GetTargetedTile()
    {
        //if (this.IsHeldItemBomb())
        //{
        //    if (this.modEntry.isLastTargetedTileFromCursor)
        //        return this.modEntry.lastTileHighlightPos;
        //    Vector2 pos = this.modEntry.lastTileHighlightPos;
        //    if (Game1.player.facingDirection == 1)
        //        pos.X--;
        //    else if (Game1.player.facingDirection == 3)
        //        pos.X++;

        //    if (Game1.player.facingDirection == 2)
        //        pos.Y--;
        //    else if (Game1.player.facingDirection == 0)
        //        pos.Y++;
        //    return pos;
        //}
        return this.modEntry.lastTileHighlightPos;
    }

    public Vector2 GetTargetedTileWithController()
    {
        Vector2 pos = Game1.player.Position / Game1.tileSize;

        if (this.IsHeldItemBomb())
        {
            if (Game1.player.facingDirection == 1)
                pos.X = MathF.Round(pos.X + 0.47f);
            else if (Game1.player.facingDirection == 3)
                pos.X = MathF.Round(pos.X - 0.45f);
            else
                pos.X = MathF.Round(pos.X - 0f);

            if (Game1.player.facingDirection == 2)
                pos.Y = MathF.Round(pos.Y + 0.05f);
            else if (Game1.player.facingDirection == 0)
                pos.Y = MathF.Round(pos.Y - 0.58f);
            else
                pos.Y = MathF.Round(pos.Y - 0.2f);
        }
        else
        {
            if (Game1.player.facingDirection == 1)
                pos.X = MathF.Ceiling(pos.X + 0.85f);
            else if (Game1.player.facingDirection == 3)
                pos.X = MathF.Floor(pos.X - 0.9f);
            else
                pos.X = MathF.Round(pos.X - 0f);

            if (Game1.player.facingDirection == 2)
                pos.Y = MathF.Ceiling(pos.Y + 0.5f);
            else if (Game1.player.facingDirection == 0)
                pos.Y = MathF.Floor(pos.Y - 1f);
            else
                pos.Y = MathF.Round(pos.Y - 0.25f);
        }
        return pos;
    }
}

namespace InputTools
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public InputToolsAPI inputTools;
        public List<SButton> buttonsPressing = new List<SButton>();
        public InputDevice lastInputDevice;
        public Vector2 lastCursorScreenPixels;
        public Vector2 lastTileHighlightPos;
        public bool isLastTargetedTileFromCursor;
        public bool isFarmerMovedLastTick;
        public bool isCursorMovedLastTick;
        public bool isTargetedTileMovedLastTick;

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
            if (!Context.IsWorldReady)
                return;
            if (!this.buttonsPressing.Contains(e.Button))
                this.buttonsPressing.Add(e.Button);
            this.lastInputDevice = (InputDevice)this.inputTools.GetInputDevice(e.Button);
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (this.buttonsPressing.Contains(e.Button))
                this.buttonsPressing.Remove(e.Button);
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            this.isFarmerMovedLastTick = Game1.player.lastPosition != Game1.player.Position;
            this.isCursorMovedLastTick = this.lastCursorScreenPixels != this.Helper.Input.GetCursorPosition().ScreenPixels;
            this.lastCursorScreenPixels = this.Helper.Input.GetCursorPosition().ScreenPixels;
            this.isTargetedTileMovedLastTick = false;

            if (this.isFarmerMovedLastTick && this.inputTools.IsControllerMoveButtonPressing())
            {
                // If controller last used, active tile is the grab tile i.e. tile in front of player
                Game1.timerUntilMouseFade = 0;
                this.isTargetedTileMovedLastTick = this.lastTileHighlightPos != this.inputTools.GetTargetedTileWithController();
                if (!this.isTargetedTileMovedLastTick) // hasn't moved far enough
                    return;
                //this.lastTileHighlightPos = this.Helper.Input.GetCursorPosition().GrabTile;
                this.lastTileHighlightPos = this.inputTools.GetTargetedTileWithController();
                this.isLastTargetedTileFromCursor = false;
            }
            else if (this.isCursorMovedLastTick || this.inputTools.IsKeyboardMoveButtonPressing())
            {
                // Otherwise active tile is the tile under the cursor
                this.isTargetedTileMovedLastTick = this.lastTileHighlightPos != this.Helper.Input.GetCursorPosition().Tile;
                if (!this.isTargetedTileMovedLastTick) // hasn't moved far enough
                    return;
                this.lastTileHighlightPos = this.Helper.Input.GetCursorPosition().Tile;
                this.isLastTargetedTileFromCursor = true;
            }
        }
    }
}
