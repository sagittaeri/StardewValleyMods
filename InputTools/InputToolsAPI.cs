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

        internal InputStack _Global;
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

        internal InputToolsAPI(ModEntry modEntry)
        {
            this.modEntry = modEntry;
            this.Helper = modEntry.Helper;
            this.Monitor = modEntry.Monitor;

            this.actions = new Actions(this);
            this.controlStack = new ControlStack(this);
            this._Global = new InputStack(this, null) { blockBehaviour = StackBlockBehavior.PassBelow };
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

            if (this.IsConfirmButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this._Global.OnConfirmPressed(this.IsConfirmButton(e.Button));
            if (this.IsCancelButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this._Global.OnCancelPressed(this.IsCancelButton(e.Button));
            if (this.IsAltButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this._Global.OnAltPressed(this.IsAltButton(e.Button));
            if (this.IsMenuButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this._Global.OnMenuPressed(this.IsMenuButton(e.Button));

            if (this.IsMoveRightButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this._Global.OnMoveRightPressed(this.IsMoveRightButton(e.Button));
            if (this.IsMoveDownButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this._Global.OnMoveDownPressed(this.IsMoveDownButton(e.Button));
            if (this.IsMoveLeftButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this._Global.OnMoveLeftPressed(this.IsMoveLeftButton(e.Button));
            if (this.IsMoveUpButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this._Global.OnMoveUpPressed(this.IsMoveUpButton(e.Button));

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

            if (this.IsConfirmButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this._Global.OnConfirmReleased(this.IsConfirmButton(e.Button));
            if (this.IsCancelButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this._Global.OnCancelReleased(this.IsCancelButton(e.Button));
            if (this.IsAltButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this._Global.OnAltReleased(this.IsAltButton(e.Button));
            if (this.IsMenuButton(e.Button) != IInputToolsAPI.InputDevice.None)
                this._Global.OnMenuReleased(this.IsMenuButton(e.Button));

            if (this.IsMoveRightButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this._Global.OnMoveRightReleased(this.IsMoveRightButton(e.Button));
            if (this.IsMoveDownButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this._Global.OnMoveDownReleased(this.IsMoveDownButton(e.Button));
            if (this.IsMoveLeftButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this._Global.OnMoveLeftReleased(this.IsMoveLeftButton(e.Button));
            if (this.IsMoveUpButton(e.Button) != IInputToolsAPI.MoveSource.None)
                this._Global.OnMoveUpReleased(this.IsMoveUpButton(e.Button));

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

            this.Global.CurrentInputDevice();

            this.isFarmerMovedLastTick = Game1.player.lastPosition != Game1.player.Position;
            this.isCursorMovedLastTick = this.lastCursorScreenPixels != this.Helper.Input.GetCursorPosition().ScreenPixels;
            this.lastCursorScreenPixels = this.Helper.Input.GetCursorPosition().ScreenPixels;
            this.isPlacementTileMovedLastTick = false;
            this.isItemChangedLastTick = this.lastItemHeld != Game1.player.CurrentItem;
            this.lastItemHeld = Game1.player.CurrentItem;
            if (this.isItemChangedLastTick)
                this._Global.OnPlacementItemChanged(Game1.player.CurrentItem);

            bool isKeyboardMoveButtonHeld = this._Global.IsMoveButtonHeld(keyboardWASD: true, keyboardArrows: false, controllerThumbstick: false, controllerDPad: false) != IInputToolsAPI.MoveSource.None;
            bool isControllerMoveButtonHeld = this._Global.IsMoveButtonHeld(keyboardWASD: false, keyboardArrows: false, controllerThumbstick: true, controllerDPad: true) != IInputToolsAPI.MoveSource.None;
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

                if (this.IsConfirmButton(button) != IInputToolsAPI.InputDevice.None)
                    this._Global.OnConfirmHeld(this.IsConfirmButton(button));
                if (this.IsCancelButton(button) != IInputToolsAPI.InputDevice.None)
                    this._Global.OnCancelHeld(this.IsCancelButton(button));
                if (this.IsAltButton(button) != IInputToolsAPI.InputDevice.None)
                    this._Global.OnAltHeld(this.IsAltButton(button));
                if (this.IsMenuButton(button) != IInputToolsAPI.InputDevice.None)
                    this._Global.OnMenuHeld(this.IsMenuButton(button));

                if (this.IsMoveRightButton(button) != IInputToolsAPI.MoveSource.None)
                    this._Global.OnMoveRightHeld(this.IsMoveRightButton(button));
                if (this.IsMoveDownButton(button) != IInputToolsAPI.MoveSource.None)
                    this._Global.OnMoveDownHeld(this.IsMoveDownButton(button));
                if (this.IsMoveLeftButton(button) != IInputToolsAPI.MoveSource.None)
                    this._Global.OnMoveLeftHeld(this.IsMoveLeftButton(button));
                if (this.IsMoveUpButton(button) != IInputToolsAPI.MoveSource.None)
                    this._Global.OnMoveUpHeld(this.IsMoveUpButton(button));
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

            this._Global.OnStackUpdateTicked(e);
        }

        internal IInputToolsAPI.InputDevice lastInputDeviceUsed = InputDevice.None;
        internal int lastTickUpdated;
        internal Vector2 lastMousePos;
        internal int lastScrollWheelPos;
        internal int lastHorizontalScrollWheelPos;
        internal bool mouseMovedLastTick;
        internal bool mouseWheelMovedLastTick;

        private Action<Tuple<SButton, SButton>> keyBindingCallback;
        private Tuple<SButton, SButton> keyBindingCandidate;
        private bool? savedGlobalActive;
        private IInputToolsAPI.StackBlockBehavior? savedGlobalBlock;
        private void KeyBindingSinglePressed(object? sender, SButton val)
        {
            if (this.IsCancelButton(val) != IInputToolsAPI.InputDevice.None)
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

        public IInputToolsAPI.InputDevice GetInputDevice(SButton button)
        {
            if (SButtonExtensions.TryGetController(button, out _))
                return IInputToolsAPI.InputDevice.Controller;
            if (SButtonExtensions.TryGetKeyboard(button, out _))
                return IInputToolsAPI.InputDevice.Keyboard;
            return IInputToolsAPI.InputDevice.Mouse;
        }

        public IInputToolsAPI.InputDevice IsConfirmButton(SButton button)
        {
            return button == SButton.ControllerA ? IInputToolsAPI.InputDevice.Controller :
                button == SButton.Enter ? IInputToolsAPI.InputDevice.Keyboard :
                IInputToolsAPI.InputDevice.None;
        }

        public IInputToolsAPI.InputDevice IsCancelButton(SButton button)
        {
            return button == SButton.ControllerB ? IInputToolsAPI.InputDevice.Controller :
                button == SButton.Escape ? IInputToolsAPI.InputDevice.Keyboard :
                IInputToolsAPI.InputDevice.None;
        }

        public IInputToolsAPI.InputDevice IsAltButton(SButton button)
        {
            return button == SButton.ControllerX ? IInputToolsAPI.InputDevice.Controller :
                button == SButton.Space ? IInputToolsAPI.InputDevice.Keyboard :
                button == SButton.MouseRight ? IInputToolsAPI.InputDevice.Mouse :
                IInputToolsAPI.InputDevice.None;
        }

        public IInputToolsAPI.InputDevice IsMenuButton(SButton button)
        {
            return button == SButton.ControllerY ? IInputToolsAPI.InputDevice.Controller :
                button == SButton.Escape ? IInputToolsAPI.InputDevice.Keyboard :
                IInputToolsAPI.InputDevice.None;
        }

        public IInputToolsAPI.MoveSource IsMoveRightButton(SButton button)
        {
            return button == SButton.D ? IInputToolsAPI.MoveSource.KeyboardWASD :
                button == SButton.Right ? IInputToolsAPI.MoveSource.KeyboardArrow :
                button == SButton.DPadRight ? IInputToolsAPI.MoveSource.ControllerDPad :
                button == SButton.LeftThumbstickRight ? IInputToolsAPI.MoveSource.ControllerLeftThumbstick :
                IInputToolsAPI.MoveSource.None;
        }

        public IInputToolsAPI.MoveSource IsMoveDownButton(SButton button)
        {
            return button == SButton.S ? IInputToolsAPI.MoveSource.KeyboardWASD :
                button == SButton.Down ? IInputToolsAPI.MoveSource.KeyboardArrow :
                button == SButton.DPadDown ? IInputToolsAPI.MoveSource.ControllerDPad :
                button == SButton.LeftThumbstickDown ? IInputToolsAPI.MoveSource.ControllerLeftThumbstick :
                IInputToolsAPI.MoveSource.None;
        }

        public IInputToolsAPI.MoveSource IsMoveLeftButton(SButton button)
        {
            return button == SButton.A ? IInputToolsAPI.MoveSource.KeyboardWASD :
                button == SButton.Left ? IInputToolsAPI.MoveSource.KeyboardArrow :
                button == SButton.DPadLeft ? IInputToolsAPI.MoveSource.ControllerDPad :
                button == SButton.LeftThumbstickLeft ? IInputToolsAPI.MoveSource.ControllerLeftThumbstick :
                IInputToolsAPI.MoveSource.None;
        }

        public IInputToolsAPI.MoveSource IsMoveUpButton(SButton button)
        {
            return button == SButton.W ? IInputToolsAPI.MoveSource.KeyboardWASD :
                button == SButton.Up ? IInputToolsAPI.MoveSource.KeyboardArrow :
                button == SButton.DPadUp ? IInputToolsAPI.MoveSource.ControllerDPad :
                button == SButton.LeftThumbstickUp ? IInputToolsAPI.MoveSource.ControllerLeftThumbstick :
                IInputToolsAPI.MoveSource.None;
        }

        public void GetTextFromVirtualKeyboard(Action<string> finishedCallback, Action<string> updateCallback = null, int? textboxWidth = 300, string initialText = "")
        {
            DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(() =>
            {
                this.StopListeningForKeybinding();
                IInputStack tempStack = this.StackCreate(this);
                this.savedGlobalActive = this._Global.isActive;
                this.savedGlobalBlock = this._Global.blockBehaviour;
                this.Global.SetStackActive(false);
                this.Global.SetStackDefaultBlockBehaviour(StackBlockBehavior.PassBelow);
                TextBox textbox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textbox"), null, Game1.smallFont, Game1.textColor);
                textbox.TitleText = "Enter input";
                textbox.Text = initialText;
                if (textboxWidth.HasValue)
                    textbox.Width = textboxWidth.Value;
                textbox.SelectMe();
                Game1.showTextEntry(textbox);

                string textboxTextLastTick = initialText;
                tempStack.StackUpdateTicked += new EventHandler<UpdateTickedEventArgs>((s, e) =>
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
            IInputStack tempStack = this.GetStack(this);
            if (tempStack == null)
                return;
            this.StackRemove(this);
            if (this.savedGlobalActive != null)
                this.Global.SetStackActive(this.savedGlobalActive.Value);
            if (this.savedGlobalBlock != null)
                this.Global.SetStackDefaultBlockBehaviour(this.savedGlobalBlock.Value);
            this.savedGlobalActive = null;
            this.savedGlobalBlock = null;
        }

        public void ListenForKeybinding(Action<Tuple<SButton, SButton>> keyBindingCallback)
        {
            DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(() =>
            {
                this.StopListeningForKeybinding();
                IInputStack tempStack = this.StackCreate(this);
                this.savedGlobalActive = this._Global.isActive;
                this.savedGlobalBlock = this._Global.blockBehaviour;
                this.Global.SetStackActive(false);
                this.Global.SetStackDefaultBlockBehaviour(StackBlockBehavior.PassBelow);
                tempStack.ButtonPressed += this.KeyBindingSinglePressed;
                tempStack.ButtonReleased += this.KeyBindingSingleReleased;
                tempStack.ButtonPairPressed += this.KeyBindingPairPressed;
                tempStack.ButtonPairReleased += this.KeyBindingPairReleased;
                this.keyBindingCandidate = null;
                this.keyBindingCallback = keyBindingCallback;
            }), 1);
        }

        public void StopListeningForKeybinding()
        {
            IInputStack tempStack = this.GetStack(this);
            if (tempStack == null)
                return;
            tempStack.ButtonPressed -= this.KeyBindingSinglePressed;
            tempStack.ButtonReleased -= this.KeyBindingSingleReleased;
            tempStack.ButtonPairPressed -= this.KeyBindingPairPressed;
            tempStack.ButtonPairReleased -= this.KeyBindingPairReleased;
            this.StackRemove(this);
            if (this.savedGlobalActive != null)
                this.Global.SetStackActive(this.savedGlobalActive.Value);
            if (this.savedGlobalBlock != null)
                this.Global.SetStackDefaultBlockBehaviour(this.savedGlobalBlock.Value);
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

        public IInputToolsAPI.IInputStack StackCreate(object stackKey, bool startActive = true, IInputToolsAPI.StackBlockBehavior defaultBlockBehaviour = IInputToolsAPI.StackBlockBehavior.Block)
        {
            return this.controlStack.StackCreate(stackKey, startActive, defaultBlockBehaviour);
        }

        public void StackRemove(object stackKey)
        {
            this.controlStack.StackRemove(stackKey);
        }

        public IInputToolsAPI.IInputStack GetStack(object stackKey)
        {
            return this.controlStack.GetStack(stackKey);
        }

        public IInputStack Global { get { return this._Global; } }
    }
}
