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
            bool IsPlacementTileChanged();
            bool IsPlacementTileFromCursor();
            Vector2 GetPlacementTile();
            Vector2 GetPlacementTileWithController();
            Vector2 GetMoveVector();
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

    public bool IsPlacementTileFromCursor()
    {
        return this.modEntry.isLastPlacementTileFromCursor;
    }


    public bool IsPlacementTileChanged()
    {
        return this.modEntry.isPlacementTileMovedLastTick;
    }

    public Vector2 GetPlacementTile()
    {
        return this.modEntry.lastTileHighlightPos;
    }

    public Vector2 GetPlacementTileWithController()
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

    public bool IsButtonPressed(SButton button)
    {
        return this.modEntry.buttonsPressing.Contains(button);
    }

    public bool IsMoveRightPressed(bool keyboardWASD = true, bool keyboardArrows = true, bool controllerDPad = true, bool controllerThumbstick = true)
    {
        bool isControllerRightPressed = (controllerDPad && this.IsButtonPressed(SButton.DPadRight)) || (controllerThumbstick && this.IsButtonPressed(SButton.LeftThumbstickRight));
        bool isKeyboardRightPressed = (keyboardWASD && this.IsButtonPressed(SButton.D)) || (keyboardArrows && this.IsButtonPressed(SButton.Right));
        return isControllerRightPressed || isKeyboardRightPressed;
    }

    public bool IsMoveDownPressed(bool keyboardWASD = true, bool keyboardArrows = true, bool controllerDPad = true, bool controllerThumbstick = true)
    {
        bool isControllerDownPressed = (controllerDPad && this.IsButtonPressed(SButton.DPadDown)) || (controllerThumbstick && this.IsButtonPressed(SButton.LeftThumbstickDown));
        bool isKeyboardDownPressed = (keyboardWASD && this.IsButtonPressed(SButton.S)) || (keyboardArrows && this.IsButtonPressed(SButton.Down));
        return isControllerDownPressed || isKeyboardDownPressed;
    }

    public bool IsMoveLeftPressed(bool keyboardWASD = true, bool keyboardArrows = true, bool controllerDPad = true, bool controllerThumbstick = true)
    {
        bool isControllerLeftPressed = (controllerDPad && this.IsButtonPressed(SButton.DPadLeft)) || (controllerThumbstick && this.IsButtonPressed(SButton.LeftThumbstickLeft));
        bool isKeyboardLeftPressed = (keyboardWASD && this.IsButtonPressed(SButton.A)) || (keyboardArrows && this.IsButtonPressed(SButton.Left));
        return isControllerLeftPressed || isKeyboardLeftPressed;
    }

    public bool IsMoveUpPressed(bool keyboardWASD = true, bool keyboardArrows = true, bool controllerDPad = true, bool controllerThumbstick = true)
    {
        bool isControllerUpPressed = (controllerDPad && this.IsButtonPressed(SButton.DPadUp)) || (controllerThumbstick && this.IsButtonPressed(SButton.LeftThumbstickUp));
        bool isKeyboardUpPressed = (keyboardWASD && this.IsButtonPressed(SButton.U)) || (keyboardArrows && this.IsButtonPressed(SButton.Up));
        return isControllerUpPressed || isKeyboardUpPressed;
    }

    public Vector2 GetInputMoveAxis(bool keyboardWASD = true, bool keyboardArrows = true, bool controllerDPad = true, bool controllerThumbstick = true)
    {
        return new Vector2(this.IsMoveRightPressed(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) ?
                1 : (this.IsMoveLeftPressed(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) ? -1 : 0),
            this.IsMoveDownPressed(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) ?
                1 : (this.IsMoveUpPressed(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) ? -1 : 0));
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
        public bool isLastPlacementTileFromCursor;
        public bool isFarmerMovedLastTick;
        public bool isCursorMovedLastTick;
        public bool isPlacementTileMovedLastTick;

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

            this.Monitor.Log($"ButtonPressed{e.Button}", LogLevel.Debug);
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (this.buttonsPressing.Contains(e.Button))
                this.buttonsPressing.Remove(e.Button);

            this.Monitor.Log($"ButtonReleased{e.Button}", LogLevel.Debug);
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            this.isFarmerMovedLastTick = Game1.player.lastPosition != Game1.player.Position;
            this.isCursorMovedLastTick = this.lastCursorScreenPixels != this.Helper.Input.GetCursorPosition().ScreenPixels;
            this.lastCursorScreenPixels = this.Helper.Input.GetCursorPosition().ScreenPixels;
            this.isPlacementTileMovedLastTick = false;

            if (this.isFarmerMovedLastTick)
            {
                this.Monitor.Log($"InputMoveAxis{this.inputTools.GetInputMoveAxis()}", LogLevel.Debug);
            }

            if (this.isFarmerMovedLastTick && this.inputTools.IsControllerMoveButtonPressing())
            {
                // If controller last used, active tile is the grab tile i.e. tile in front of player
                Game1.timerUntilMouseFade = 0;
                this.isPlacementTileMovedLastTick = this.lastTileHighlightPos != this.inputTools.GetPlacementTileWithController();
                if (!this.isPlacementTileMovedLastTick) // hasn't moved far enough
                    return;
                this.lastTileHighlightPos = this.inputTools.GetPlacementTileWithController();
                this.isLastPlacementTileFromCursor = false;
            }
            else if (this.isCursorMovedLastTick || this.inputTools.IsKeyboardMoveButtonPressing())
            {
                // Otherwise active tile is the tile under the cursor
                this.isPlacementTileMovedLastTick = this.lastTileHighlightPos != this.Helper.Input.GetCursorPosition().Tile;
                if (!this.isPlacementTileMovedLastTick) // hasn't moved far enough
                    return;
                this.lastTileHighlightPos = this.Helper.Input.GetCursorPosition().Tile;
                this.isLastPlacementTileFromCursor = true;
            }
        }
    }
}
