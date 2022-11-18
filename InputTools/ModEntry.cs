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
            bool IsActiveTileChanged();
            bool IsActiveTileFromCursor();
            Vector2 GetActiveTile();
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
            || this.modEntry.buttonsPressing.Contains(SButton.LeftThumbstickLeft);
    }

    public bool IsControllerCursorButtonPressing()
    {
        return this.modEntry.buttonsPressing.Contains(SButton.RightThumbstickUp)
            || this.modEntry.buttonsPressing.Contains(SButton.RightThumbstickRight)
            || this.modEntry.buttonsPressing.Contains(SButton.RightThumbstickDown)
            || this.modEntry.buttonsPressing.Contains(SButton.RightThumbstickLeft);
    }

    public bool IsActiveTileFromCursor()
    {
        return this.modEntry.lastActiveTileFromCursor;
    }


    public bool IsActiveTileChanged()
    {
        return this.modEntry.activeTileMovedLastTick;
    }

    public Vector2 GetActiveTile()
    {
        return this.modEntry.lastTileHighlightPos;
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
        public bool lastActiveTileFromCursor;
        public bool farmerMovedLastTick;
        public bool cursorMovedLastTick;
        public bool activeTileMovedLastTick;

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
            this.farmerMovedLastTick = Game1.player.lastPosition != Game1.player.Position;
            this.cursorMovedLastTick = this.lastCursorScreenPixels != this.Helper.Input.GetCursorPosition().ScreenPixels;
            this.lastCursorScreenPixels = this.Helper.Input.GetCursorPosition().ScreenPixels;
            this.activeTileMovedLastTick = false;

            if (this.farmerMovedLastTick && this.inputTools.IsControllerMoveButtonPressing())
            {
                // If controller last used, active tile is the grab tile i.e. tile in front of player
                this.activeTileMovedLastTick = this.lastTileHighlightPos != this.Helper.Input.GetCursorPosition().GrabTile;
                if (!this.activeTileMovedLastTick) // hasn't moved far enough
                    return;
                this.lastTileHighlightPos = this.Helper.Input.GetCursorPosition().GrabTile;
                this.lastActiveTileFromCursor = false;
                Game1.timerUntilMouseFade = 0;
            }
            else if (this.cursorMovedLastTick || this.inputTools.IsKeyboardMoveButtonPressing())
            {
                // Otherwise active tile is the tile under the cursor
                this.activeTileMovedLastTick = this.lastTileHighlightPos != this.Helper.Input.GetCursorPosition().Tile;
                if (!this.activeTileMovedLastTick) // hasn't moved far enough
                    return;
                this.lastTileHighlightPos = this.Helper.Input.GetCursorPosition().Tile;
                this.lastActiveTileFromCursor = true;
            }
        }
    }
}
