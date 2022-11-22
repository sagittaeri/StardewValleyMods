using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using InputTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using static InputTools.IInputToolsAPI;

namespace InputTools
{
    public class InputToolsAPI : IInputToolsAPI
    {
        private ModEntry modEntry;
        internal IModHelper Helper;
        internal IMonitor Monitor;

        internal InputLayer _Global;
        public Actions actions;
        public ControlStack stack;

        internal List<SButton> confirmButtons = new List<SButton>();
        internal List<SButton> cancelButtons = new List<SButton>();
        internal List<SButton> altButtons = new List<SButton>();
        internal List<SButton> menuButtons = new List<SButton>();
        internal List<SButton> moveRightButtons = new List<SButton>();
        internal List<SButton> moveDownButtons = new List<SButton>();
        internal List<SButton> moveLeftButtons = new List<SButton>();
        internal List<SButton> moveUpButtons = new List<SButton>();

        internal List<SButton> buttonsPressing = new List<SButton>();
        internal List<Tuple<SButton, SButton>> buttonPairsPressing = new List<Tuple<SButton, SButton>>();
        internal List<Tuple<SButton, SButton>> buttonPairsReleased = new List<Tuple<SButton, SButton>>();
        internal int tickButtonPairsReleased;
        internal IInputToolsAPI.InputDevice lastInputDevice;
        internal Vector2 lastCursorScreenPixels;
        internal Vector2 lastTileHighlightPos;
        internal Item lastItemHeld;
        internal bool isLastPlacementTileFromCursor;
        internal bool isFarmerMovedLastTick;
        internal bool isCursorMovedLastTick;
        internal bool isPlacementTileMovedLastTick;
        internal bool isItemChangedLastTick;
        internal Vector2 moveAxisLastTick;
        internal bool isDirty = Game1.options.optionsDirty;

        internal InputToolsAPI(ModEntry modEntry)
        {
            this.modEntry = modEntry;
            this.Helper = modEntry.Helper;
            this.Monitor = modEntry.Monitor;

            this.actions = new Actions(this);
            this.stack = new ControlStack(this);
            this._Global = new InputLayer(this, null) { blockBehaviour = BlockBehavior.PassBelow };
        }

        internal void ReloadOneKeybinding(ref List<SButton> outButtons, string id, InputButton[] sdvButtons, params SButton[] controllerButtons)
        {
            if (outButtons == null)
                outButtons = new List<SButton>();
            bool hasController = false;
            outButtons.Clear();
            foreach (InputButton b in sdvButtons)
            {
                outButtons.Add(b.ToSButton());
                if (this.GetInputDevice(outButtons[outButtons.Count - 1]) == InputDevice.Controller)
                    hasController = true;
            }
            if (!hasController && controllerButtons != null && controllerButtons.Length > 0)
                outButtons.AddRange(controllerButtons);

            this.Monitor.Log($"{id}: {string.Join('/', outButtons)}", LogLevel.Debug);
        }

        internal void ReloadKeybindings()
        {
            this.ReloadOneKeybinding(ref this.confirmButtons, "Confirm", Game1.options.actionButton, SButton.ControllerA);
            this.ReloadOneKeybinding(ref this.cancelButtons, "Cancel", Game1.options.menuButton, SButton.ControllerB);
            this.ReloadOneKeybinding(ref this.altButtons, "Alt", Game1.options.useToolButton, SButton.ControllerX);
            this.ReloadOneKeybinding(ref this.menuButtons, "Menu", Game1.options.menuButton, SButton.ControllerY);
            this.ReloadOneKeybinding(ref this.moveRightButtons, "MoveRight", Game1.options.moveRightButton, SButton.DPadRight, SButton.LeftThumbstickRight);
            this.ReloadOneKeybinding(ref this.moveDownButtons, "MoveDown", Game1.options.moveDownButton, SButton.DPadDown, SButton.LeftThumbstickDown);
            this.ReloadOneKeybinding(ref this.moveLeftButtons, "MoveLeft", Game1.options.moveLeftButton, SButton.DPadLeft, SButton.LeftThumbstickLeft);
            this.ReloadOneKeybinding(ref this.moveUpButtons, "MoveUp", Game1.options.moveUpButton, SButton.DPadUp, SButton.LeftThumbstickUp);
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>

        internal void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            //if (!Context.IsWorldReady)
            //    return;

            this.lastInputDevice = this.GetInputDevice(e.Button);

            if (this.buttonsPressing.Count > 0)
            {
                foreach (SButton heldButton in this.buttonsPressing)
                {
                    Tuple<SButton, SButton> buttonPair = new Tuple<SButton, SButton>(heldButton, e.Button);
                    if (!this.buttonPairsPressing.Contains(buttonPair))
                        this.buttonPairsPressing.Add(buttonPair);
                    foreach (string groupID in this.actions.GetActionsFromKeyPair(buttonPair))
                        this._Global.OnActionPressed(groupID);
                    this._Global.OnButtonPairPressed(buttonPair);
                    //this.Monitor.Log($"{Game1.ticks} ButtonPairPressed {buttonPair}", LogLevel.Debug);
                }
            }

            if (!this.buttonsPressing.Contains(e.Button))
                this.buttonsPressing.Add(e.Button);
            this._Global.OnButtonPressed(e.Button);
            foreach (string groupID in this.actions.GetActionsFromKey(e.Button))
                this._Global.OnActionPressed(groupID);

            if (this.IsConfirmButton(e.Button))
                this._Global.OnConfirmPressed(e.Button);
            if (this.IsCancelButton(e.Button))
                this._Global.OnCancelPressed(e.Button);
            if (this.IsAltButton(e.Button))
                this._Global.OnAltPressed(e.Button);
            if (this.IsMenuButton(e.Button))
                this._Global.OnMenuPressed(e.Button);

            if (this.IsMoveRightButton(e.Button))
                this._Global.OnMoveRightPressed(e.Button);
            if (this.IsMoveDownButton(e.Button))
                this._Global.OnMoveDownPressed(e.Button);
            if (this.IsMoveLeftButton(e.Button))
                this._Global.OnMoveLeftPressed(e.Button);
            if (this.IsMoveUpButton(e.Button))
                this._Global.OnMoveUpPressed(e.Button);

            Vector2 moveAxis = this._Global.GetMoveAxis();
            if (this.moveAxisLastTick == Vector2.Zero && moveAxis != Vector2.Zero)
                this._Global.OnMoveAxisPressed(moveAxis);
            this.moveAxisLastTick = moveAxis;

            //this.Monitor.Log($"{Game1.ticks} ButtonPressed {e.Button}", LogLevel.Debug);
        }

        internal void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
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
                        this._Global.OnButtonPairReleased(buttonPair);
                        //this.Monitor.Log($"{Game1.ticks} ButtonPairRemoved {buttonPair}", LogLevel.Debug);
                        foreach (string groupID in this.actions.GetActionsFromKeyPair(buttonPair))
                            this._Global.OnActionReleased(groupID);
                    }
                }
            }

            if (this.buttonsPressing.Contains(e.Button))
                this.buttonsPressing.Remove(e.Button);
            this._Global.OnButtonReleased(e.Button);
            foreach (string groupID in this.actions.GetActionsFromKey(e.Button))
                this._Global.OnActionReleased(groupID);

            if (this.IsConfirmButton(e.Button))
                this._Global.OnConfirmReleased(e.Button);
            if (this.IsCancelButton(e.Button))
                this._Global.OnCancelReleased(e.Button);
            if (this.IsAltButton(e.Button))
                this._Global.OnAltReleased(e.Button);
            if (this.IsMenuButton(e.Button))
                this._Global.OnMenuReleased(e.Button);

            if (this.IsMoveRightButton(e.Button))
                this._Global.OnMoveRightReleased(e.Button);
            if (this.IsMoveDownButton(e.Button))
                this._Global.OnMoveDownReleased(e.Button);
            if (this.IsMoveLeftButton(e.Button))
                this._Global.OnMoveLeftReleased(e.Button);
            if (this.IsMoveUpButton(e.Button))
                this._Global.OnMoveUpReleased(e.Button);

            Vector2 moveAxis = this._Global.GetMoveAxis();
            if (this.moveAxisLastTick != Vector2.Zero && moveAxis == Vector2.Zero)
                this._Global.OnMoveAxisReleased(moveAxis);
            this.moveAxisLastTick = Vector2.Zero;

            //this.Monitor.Log($"{Game1.ticks} ButtonReleased {e.Button}", LogLevel.Debug);
        }

        internal void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            //if (!Context.IsWorldReady)
            //    return;

            this.IsKeybindingConfigChanged();
            this.GetCurrentInputDevice();

            this.isFarmerMovedLastTick = Game1.player.lastPosition != Game1.player.Position;
            this.isCursorMovedLastTick = this.lastCursorScreenPixels != this.Helper.Input.GetCursorPosition().ScreenPixels;
            this.lastCursorScreenPixels = this.Helper.Input.GetCursorPosition().ScreenPixels;
            this.isPlacementTileMovedLastTick = false;
            this.isItemChangedLastTick = this.lastItemHeld != Game1.player.CurrentItem;
            this.lastItemHeld = Game1.player.CurrentItem;
            if (this.isItemChangedLastTick)
                this._Global.OnPlacementItemChanged(Game1.player.CurrentItem);

            SButton moveButtonHeld = this._Global.IsMoveButtonHeld();
            bool isKeyboardMoveButtonHeld = this.GetInputDevice(moveButtonHeld) == InputDevice.Keyboard;
            bool isControllerMoveButtonHeld = this.GetInputDevice(moveButtonHeld) == InputDevice.Controller;
            if ((!Game1.wasMouseVisibleThisFrame && this.isItemChangedLastTick) || (this.isFarmerMovedLastTick && !isKeyboardMoveButtonHeld) || isControllerMoveButtonHeld)
            {
                // If controller last used, placement tile is the grab tile i.e. tile in front of player
                Game1.timerUntilMouseFade = 0;
                this.isPlacementTileMovedLastTick = this.lastTileHighlightPos != this._Global.GetPlacementTileWithController();
                if (this.isPlacementTileMovedLastTick)
                {
                    this.lastTileHighlightPos = this._Global.GetPlacementTileWithController();
                    this.isLastPlacementTileFromCursor = false;
                    this._Global.OnPlacementTileChanged(this.lastTileHighlightPos);
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
                    this._Global.OnPlacementTileChanged(this.lastTileHighlightPos);
                }
            }

            foreach (SButton button in this.buttonsPressing)
            {
                this._Global.OnButtonHeld(button);
                foreach (string groupID in this.actions.GetActionsFromKey(button))
                    this._Global.OnActionHeld(groupID);

                if (this.IsConfirmButton(button))
                    this._Global.OnConfirmHeld(button);
                if (this.IsCancelButton(button))
                    this._Global.OnCancelHeld(button);
                if (this.IsAltButton(button))
                    this._Global.OnAltHeld(button);
                if (this.IsMenuButton(button))
                    this._Global.OnMenuHeld(button);

                if (this.IsMoveRightButton(button))
                    this._Global.OnMoveRightHeld(button);
                if (this.IsMoveDownButton(button))
                    this._Global.OnMoveDownHeld(button);
                if (this.IsMoveLeftButton(button))
                    this._Global.OnMoveLeftHeld(button);
                if (this.IsMoveUpButton(button))
                    this._Global.OnMoveUpHeld(button);
            }
            foreach (Tuple<SButton, SButton> buttonPair in this.buttonPairsPressing)
            {
                this._Global.OnButtonPairHeld(buttonPair);
                foreach (string groupID in this.actions.GetActionsFromKeyPair(buttonPair))
                    this._Global.OnActionHeld(groupID);
            }

            this.moveAxisLastTick = this._Global.GetMoveAxis();
            if (this.moveAxisLastTick != Vector2.Zero)
                this._Global.OnMoveAxisHeld(this.moveAxisLastTick);

            this._Global.OnLayerUpdateTicked(e);
        }
        internal void OnInputDeviceChanged(IInputToolsAPI.InputDevice inputDevice)
        {
            InputDeviceChanged?.Invoke(this, inputDevice);
        }

        internal void OnKeybindingConfigChanged()
        {
            KeybindingConfigChanged?.Invoke(this, null);
        }

        internal IInputToolsAPI.InputDevice lastInputDeviceUsed = InputDevice.None;
        internal bool lastConfigChanged = false;
        internal int lastTickInputDeviceUpdated;
        internal int lastTickConfigUpdated;
        internal Vector2 lastMousePos;
        internal int lastScrollWheelPos;
        internal int lastHorizontalScrollWheelPos;
        internal bool mouseMovedLastTick;
        internal bool mouseWheelMovedLastTick;

        private Action<Tuple<SButton, SButton>> keyBindingCallback;
        private Tuple<SButton, SButton> keyBindingCandidate;
        private bool? savedGlobalActive;
        private IInputToolsAPI.BlockBehavior? savedGlobalBlock;
        private void KeyBindingSinglePressed(object? sender, SButton val)
        {
            if (this.IsCancelButton(val))
            {
                this.StopListeningForKeybinding();
                this.keyBindingCallback?.Invoke(null);
                this.keyBindingCandidate = null;
                return;
            }
            if (this.keyBindingCandidate == null)
                this.keyBindingCandidate = new Tuple<SButton, SButton>(val, SButton.None);
        }
        private void KeyBindingSingleReleased(object? sender, SButton val)
        {
            if (this.keyBindingCandidate != null && this.keyBindingCandidate.Item1 == val && this.keyBindingCandidate.Item2 == SButton.None)
            {
                this.StopListeningForKeybinding();
                this.keyBindingCallback?.Invoke(new Tuple<SButton, SButton>(val, SButton.None));
                this.keyBindingCandidate = null;
            }
        }
        private void KeyBindingPairPressed(object? sender, Tuple<SButton, SButton> val)
        {
            this.keyBindingCandidate = val;
        }
        private void KeyBindingPairReleased(object? sender, Tuple<SButton, SButton> val)
        {
            if (this.keyBindingCandidate == val)
            {
                this.StopListeningForKeybinding();
                this.keyBindingCallback?.Invoke(val);
            }
        }

        public List<string> GetListOfModIDs()
        {
            return this.modEntry.GetListOfModIDs();
        }

        public event EventHandler<IInputToolsAPI.InputDevice> InputDeviceChanged;
        public event EventHandler KeybindingConfigChanged;

        public IInputToolsAPI.InputDevice GetCurrentInputDevice()
        {
            if (this.lastTickInputDeviceUpdated == Game1.ticks)
                return this.lastInputDeviceUsed;
            this.lastTickInputDeviceUpdated = Game1.ticks;

            IInputToolsAPI.InputDevice newInputDevice = this.lastInputDeviceUsed;
            if (Game1.isAnyGamePadButtonBeingPressed() || Game1.isAnyGamePadButtonBeingHeld() || Game1.isGamePadThumbstickInMotion())
                newInputDevice = InputDevice.Controller;
            else if (Game1.GetKeyboardState().GetPressedKeyCount() > 0)
                newInputDevice = InputDevice.Keyboard;
            else if (Game1.input.GetMouseState().LeftButton == ButtonState.Pressed
                || Game1.input.GetMouseState().MiddleButton == ButtonState.Pressed
                || Game1.input.GetMouseState().RightButton == ButtonState.Pressed
                || Game1.input.GetMouseState().XButton1 == ButtonState.Pressed
                || Game1.input.GetMouseState().XButton2 == ButtonState.Pressed
                || (Game1.input.GetMouseState().Position.ToVector2() != this.lastMousePos && Game1.lastCursorMotionWasMouse)
                || Game1.input.GetMouseState().ScrollWheelValue != this.lastScrollWheelPos
                || Game1.input.GetMouseState().HorizontalScrollWheelValue != this.lastHorizontalScrollWheelPos
                )
                newInputDevice = InputDevice.Mouse;

            this.mouseMovedLastTick = this.lastMousePos != Game1.input.GetMouseState().Position.ToVector2();
            this.mouseWheelMovedLastTick = (this.lastScrollWheelPos != Game1.input.GetMouseState().ScrollWheelValue) || (this.lastHorizontalScrollWheelPos != Game1.input.GetMouseState().HorizontalScrollWheelValue);

            this.lastMousePos = Game1.input.GetMouseState().Position.ToVector2();
            this.lastScrollWheelPos = Game1.input.GetMouseState().ScrollWheelValue;
            this.lastHorizontalScrollWheelPos = Game1.input.GetMouseState().HorizontalScrollWheelValue;
            if (this.lastInputDeviceUsed != newInputDevice)
            {
                this.lastInputDeviceUsed = newInputDevice;
                this.OnInputDeviceChanged(newInputDevice);
            }
            else
                this.lastInputDeviceUsed = newInputDevice;
            if (this.mouseMovedLastTick)
                this._Global.OnCursorMoved(newInputDevice);
            if (this.mouseWheelMovedLastTick)
                this._Global.OnMouseWheelMoved(this._Global.GetMouseWheelPos());
            return this.lastInputDeviceUsed;
        }

        public bool IsKeybindingConfigChanged()
        {
            if (this.lastTickConfigUpdated == Game1.ticks)
                return this.lastConfigChanged;
            this.lastTickConfigUpdated = Game1.ticks;

            this.lastConfigChanged = false;
            if (Game1.options.optionsDirty != this.isDirty && Game1.options.optionsDirty)
            {
                this.lastConfigChanged = true;
                Game1.options.SaveDefaultOptions();
                this.ReloadKeybindings();
                KeybindingConfigChanged?.Invoke(this, null);
            }
            this.isDirty = Game1.options.optionsDirty;
            return this.lastConfigChanged;
        }

        public IInputToolsAPI.InputDevice GetInputDevice(SButton button)
        {
            if (SButtonExtensions.TryGetController(button, out _))
                return IInputToolsAPI.InputDevice.Controller;
            if (SButtonExtensions.TryGetKeyboard(button, out _))
                return IInputToolsAPI.InputDevice.Keyboard;
            return IInputToolsAPI.InputDevice.Mouse;
        }

        public bool IsConfirmButton(SButton button)
        {
            return this.confirmButtons.Contains(button);
        }

        public bool IsCancelButton(SButton button)
        {
            return this.cancelButtons.Contains(button);
        }

        public bool IsAltButton(SButton button)
        {
            return this.altButtons.Contains(button);
        }

        public bool IsMenuButton(SButton button)
        {
            return this.menuButtons.Contains(button);
        }

        public bool IsMoveRightButton(SButton button)
        {
            return this.moveRightButtons.Contains(button);
        }

        public bool IsMoveDownButton(SButton button)
        {
            return this.moveDownButtons.Contains(button);
        }

        public bool IsMoveLeftButton(SButton button)
        {
            return this.moveLeftButtons.Contains(button);
        }

        public bool IsMoveUpButton(SButton button)
        {
            return this.moveUpButtons.Contains(button);
        }

        public void GetTextFromVirtualKeyboard(Action<string> finishedCallback, Action<string> updateCallback = null, int? textboxWidth = 300, string initialText = "")
        {
            DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(() =>
            {
                this.StopListeningForKeybinding();
                IInputLayer tempLayer = this.LayerCreate(this);
                this.savedGlobalActive = this._Global.isActive;
                this.savedGlobalBlock = this._Global.blockBehaviour;
                this.Global.SetLayerActive(false);
                this.Global.SetLayerBlockBehaviour(BlockBehavior.PassBelow);
                TextBox textbox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textbox"), null, Game1.smallFont, Game1.textColor);
                textbox.TitleText = "Enter input";
                textbox.Text = initialText;
                if (textboxWidth.HasValue)
                    textbox.Width = textboxWidth.Value;
                textbox.SelectMe();
                Game1.showTextEntry(textbox);

                string textboxTextLastTick = initialText;
                tempLayer.LayerUpdateTicked += new EventHandler<UpdateTickedEventArgs>((s, e) =>
                {
                    if (Game1.textEntry != null)
                    {
                        if (textboxTextLastTick != textbox.Text)
                            updateCallback?.Invoke(textbox.Text);
                        textboxTextLastTick = textbox.Text;
                        textbox.SelectMe();
                    }
                    else
                    {
                        this.CloseVirtualKeyboard();
                        finishedCallback.Invoke(null);
                    }
                });
                textbox.OnEnterPressed += new TextBoxEvent(target =>
                {
                    this.CloseVirtualKeyboard();
                    finishedCallback.Invoke(textbox.Text);
                });
            }), 1);
        }

        public void CloseVirtualKeyboard()
        {
            IInputLayer tempLayer = this.GetLayer(this);
            if (tempLayer == null)
                return;
            this.LayerRemove(this);
            if (this.savedGlobalActive != null)
                this.Global.SetLayerActive(this.savedGlobalActive.Value);
            if (this.savedGlobalBlock != null)
                this.Global.SetLayerBlockBehaviour(this.savedGlobalBlock.Value);
            this.savedGlobalActive = null;
            this.savedGlobalBlock = null;
        }

        public void ListenForKeybinding(Action<Tuple<SButton, SButton>> keyBindingCallback)
        {
            DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(() =>
            {
                this.StopListeningForKeybinding();
                IInputLayer tempLayer = this.LayerCreate(this);
                this.savedGlobalActive = this._Global.isActive;
                this.savedGlobalBlock = this._Global.blockBehaviour;
                this.Global.SetLayerActive(false);
                this.Global.SetLayerBlockBehaviour(BlockBehavior.PassBelow);
                tempLayer.ButtonPressed += this.KeyBindingSinglePressed;
                tempLayer.ButtonReleased += this.KeyBindingSingleReleased;
                tempLayer.ButtonPairPressed += this.KeyBindingPairPressed;
                tempLayer.ButtonPairReleased += this.KeyBindingPairReleased;
                this.keyBindingCandidate = null;
                this.keyBindingCallback = keyBindingCallback;
            }), 1);
        }

        public void StopListeningForKeybinding()
        {
            IInputLayer tempLayer = this.GetLayer(this);
            if (tempLayer == null)
                return;
            tempLayer.ButtonPressed -= this.KeyBindingSinglePressed;
            tempLayer.ButtonReleased -= this.KeyBindingSingleReleased;
            tempLayer.ButtonPairPressed -= this.KeyBindingPairPressed;
            tempLayer.ButtonPairReleased -= this.KeyBindingPairReleased;
            this.LayerRemove(this);
            if (this.savedGlobalActive != null)
                this.Global.SetLayerActive(this.savedGlobalActive.Value);
            if (this.savedGlobalBlock != null)
                this.Global.SetLayerBlockBehaviour(this.savedGlobalBlock.Value);
            this.savedGlobalActive = null;
            this.savedGlobalBlock = null;
        }

        public void RegisterAction(string actionID, params SButton[] keyTriggers)
        {
            this.actions.RegisterAction(actionID, keyTriggers);
        }

        public void RegisterAction(string actionID, params Tuple<SButton, SButton>[] keyTriggers)
        {
            this.actions.RegisterAction(actionID, keyTriggers);
        }

        public void UnregisterAction(string actionID)
        {
            this.actions.UnregisterAction(actionID);
        }

        public List<string> GetActionsFromKey(SButton key)
        {
            return this.actions.GetActionsFromKey(key);
        }

        public List<string> GetActionsFromKeyPair(Tuple<SButton, SButton> keyPair)
        {
            return this.actions.GetActionsFromKeyPair(keyPair);
        }

        public List<Tuple<SButton, SButton>> GetKeyPairsFromActions(string actionID)
        {
            return this.actions.GetKeyPairsFromActions(actionID);
        }

        public IInputToolsAPI.IInputLayer LayerCreate(object layerKey, bool startActive = true, IInputToolsAPI.BlockBehavior defaultBlockBehaviour = IInputToolsAPI.BlockBehavior.Block)
        {
            return this.stack.LayerCreate(layerKey, startActive, defaultBlockBehaviour);
        }

        public void LayerRemove(object layerKey)
        {
            this.stack.LayerRemove(layerKey);
        }

        public IInputToolsAPI.IInputLayer GetLayer(object layerKey)
        {
            return this.stack.GetLayer(layerKey);
        }

        public IInputLayer Global { get { return this._Global; } }
    }
}
