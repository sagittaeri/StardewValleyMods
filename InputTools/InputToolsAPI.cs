using System;
using System.Collections;
using System.Collections.Generic;
using InputTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using static InputTools.IInputToolsAPI;

namespace InputTools
{
    public class InputToolsAPI : IInputToolsAPI
    {
        internal InputStack _Global;
        private Actions actions;
        private ModEntry modEntry;

        internal InputToolsAPI(ModEntry modEntry)
        {
            this.modEntry = modEntry;
            this.actions = new Actions(modEntry);
            this._Global = new InputStack(modEntry, null);
        }

        internal IInputToolsAPI.InputDevice lastInputDeviceUsed = InputDevice.None;
        internal int lastTickUpdated;
        internal Vector2 lastMousePos;
        internal int lastScrollWheelPos;
        internal int lastHorizontalScrollWheelPos;
        internal bool mouseMovedLastTick;

        private Action<Tuple<SButton, SButton>> keyBindingCallback;
        private SButton keyBindingCandidate;
        private void KeyBindingSinglePressed(object? sender, SButton val)
        {
            if (this.IsCancelButton(val) != IInputToolsAPI.InputDevice.None)
            {
                this.StopListeningForKeybinding();
                this.keyBindingCallback?.Invoke(null);
                this.keyBindingCandidate = SButton.None;
                return;
            }
            this.keyBindingCandidate = val;
        }
        private void KeyBindingSingleReleased(object? sender, SButton val)
        {
            if (this.keyBindingCandidate == val)
            {
                this.StopListeningForKeybinding();
                this.keyBindingCallback?.Invoke(new Tuple<SButton, SButton>(val, SButton.None));
                this.keyBindingCandidate = SButton.None;
                return;
            }
        }
        private void KeyBindingPairPressed(object? sender, Tuple<SButton, SButton> val)
        {
            this.StopListeningForKeybinding();
            this.keyBindingCallback?.Invoke(val);
        }

        internal void OnPlacementTileChanged(Vector2 placement)
        {
            PlacementTileChanged?.Invoke(this, placement);
        }

        internal void OnPlacementItemChanged(Item item)
        {
            PlacementItemChanged?.Invoke(this, item);
        }

        public event EventHandler<IInputToolsAPI.InputDevice> InputDeviceChanged;
        public event EventHandler<Vector2> PlacementTileChanged;
        public event EventHandler<Item> PlacementItemChanged;

        public IInputToolsAPI.InputDevice CurrentInputDevice()
        {
            if (this.lastTickUpdated == Game1.ticks)
                return this.lastInputDeviceUsed;

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

            this.lastMousePos = Game1.input.GetMouseState().Position.ToVector2();
            this.lastScrollWheelPos = Game1.input.GetMouseState().ScrollWheelValue;
            this.lastHorizontalScrollWheelPos = Game1.input.GetMouseState().HorizontalScrollWheelValue;
            this.lastTickUpdated = Game1.ticks;
            if (this.lastInputDeviceUsed != newInputDevice)
            {
                this.lastInputDeviceUsed = newInputDevice;
                this.InputDeviceChanged?.Invoke(this, newInputDevice);
            }
            else
                this.lastInputDeviceUsed = newInputDevice;
            return this.lastInputDeviceUsed;
        }

        public IInputToolsAPI.InputDevice GetInputDevice(SButton button)
        {
            if (SButtonExtensions.TryGetController(button, out _))
                return IInputToolsAPI.InputDevice.Controller;
            if (SButtonExtensions.TryGetKeyboard(button, out _))
                return IInputToolsAPI.InputDevice.Keyboard;
            return IInputToolsAPI.InputDevice.Mouse;
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
                    pos.X = MathF.Floor(pos.X - 0.88f);
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

        public void ListenForKeybinding(Action<Tuple<SButton, SButton>> keyBindingCallback)
        {
            DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(() =>
            {
                this.StopListeningForKeybinding();
                this.Global.ButtonPressed += this.KeyBindingSinglePressed;
                this.Global.ButtonReleased += this.KeyBindingSingleReleased;
                this.Global.ButtonPairPressed += this.KeyBindingPairPressed;
                this.keyBindingCallback = keyBindingCallback;
            }), 1);
        }

        public void StopListeningForKeybinding()
        {
            this.Global.ButtonPressed -= this.KeyBindingSinglePressed;
            this.Global.ButtonReleased -= this.KeyBindingSingleReleased;
            this.Global.ButtonPairPressed -= this.KeyBindingPairPressed;
        }

        public void RegisterAction(string actionID, params SButton[] keyTriggers)
        {
            this.modEntry.actions.RegisterAction(actionID, keyTriggers);
        }

        public void RegisterAction(string actionID, params Tuple<SButton, SButton>[] keyTriggers)
        {
            this.modEntry.actions.RegisterAction(actionID, keyTriggers);
        }

        public void UnregisterAction(string actionID)
        {
            this.modEntry.actions.UnregisterAction(actionID);
        }

        public List<string> GetActionsFromKey(SButton key)
        {
            return this.modEntry.actions.GetActionsFromKey(key);
        }

        public List<string> GetActionsFromKeyPair(Tuple<SButton, SButton> keyPair)
        {
            return this.modEntry.actions.GetActionsFromKeyPair(keyPair);
        }

        public List<Tuple<SButton, SButton>> GetKeyPairsFromActions(string actionID)
        {
            return this.modEntry.actions.GetKeyPairsFromActions(actionID);
        }

        public IInputToolsAPI.IInputStack StackCreate(object stackKey, bool startActive = true, IInputToolsAPI.StackBlockBehavior defaultBlockBehaviour = IInputToolsAPI.StackBlockBehavior.Block)
        {
            return this.modEntry.controlStack.StackCreate(stackKey, startActive, defaultBlockBehaviour);
        }

        public void StackRemove(object stackKey)
        {
            this.modEntry.controlStack.StackRemove(stackKey);
        }

        public IInputToolsAPI.IInputStack GetStack(object stackKey)
        {
            return this.modEntry.controlStack.GetStack(stackKey);
        }


        public IInputStack Global { get { return this._Global; } }
        public class InputStack : IInputToolsAPI.IInputStack
        {
            internal bool isActive = true;
            internal IInputToolsAPI.StackBlockBehavior blockBehaviour;

            private ModEntry modEntry;
            internal InputStack(ModEntry modEntry, object stackKey)
            {
                this.modEntry = modEntry;
                this.stackKey = stackKey;
            }

            internal InputToolsAPI.InputStack GetStackBelow(bool stopAtBlock = true)
            {
                if (this == this.modEntry.inputTools.Global)
                {
                    if (this.modEntry.controlStack.stacks.Count > 0)
                        return this.modEntry.controlStack.GetStack(this.modEntry.controlStack.stacks[this.modEntry.controlStack.stacks.Count - 1]);
                }
                else if (!stopAtBlock || !this.isActive || this.blockBehaviour == StackBlockBehavior.PassBelow)
                {
                    for (int i = this.modEntry.controlStack.stacks.Count - 1; i >= 0; i--)
                    {
                        if (this.modEntry.controlStack.stacks[i] == this.stackKey && i > 0)
                            return this.modEntry.controlStack.GetStack(this.modEntry.controlStack.stacks[i - 1]);
                    }
                }
                return null;
            }

            internal void OnButtonPressed(SButton button)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    ButtonPressed?.Invoke(this.stackKey, button);
                this.GetStackBelow()?.OnButtonPressed(button);
            }

            internal void OnButtonHeld(SButton button)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    ButtonHeld?.Invoke(this.stackKey, button);
                this.GetStackBelow()?.OnButtonHeld(button);
            }

            internal void OnButtonReleased(SButton button)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    ButtonReleased?.Invoke(this.stackKey, button);
                this.GetStackBelow()?.OnButtonReleased(button);
            }

            internal void OnButtonPairPressed(Tuple<SButton, SButton> buttons)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    ButtonPairPressed?.Invoke(this.stackKey, buttons);
                this.GetStackBelow()?.OnButtonPairPressed(buttons);
            }

            internal void OnButtonPairHeld(Tuple<SButton, SButton> buttons)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    ButtonPairHeld?.Invoke(this.stackKey, buttons);
                this.GetStackBelow()?.OnButtonPairHeld(buttons);
            }

            internal void OnButtonPairReleased(Tuple<SButton, SButton> buttons)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    ButtonPairReleased?.Invoke(this.stackKey, buttons);
                this.GetStackBelow()?.OnButtonPairReleased(buttons);
            }

            internal void OnConfirmPressed(IInputToolsAPI.InputDevice inputDevice)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    ConfirmPressed?.Invoke(this.stackKey, inputDevice);
                this.GetStackBelow()?.OnConfirmPressed(inputDevice);
            }

            internal void OnConfirmHeld(IInputToolsAPI.InputDevice inputDevice)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    ConfirmHeld?.Invoke(this.stackKey, inputDevice);
                this.GetStackBelow()?.OnConfirmHeld(inputDevice);
            }

            internal void OnConfirmReleased(IInputToolsAPI.InputDevice inputDevice)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    ConfirmReleased?.Invoke(this.stackKey, inputDevice);
                this.GetStackBelow()?.OnConfirmReleased(inputDevice);
            }

            internal void OnCancelPressed(IInputToolsAPI.InputDevice inputDevice)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    CancelPressed?.Invoke(this.stackKey, inputDevice);
                this.GetStackBelow()?.OnCancelPressed(inputDevice);
            }

            internal void OnCancelHeld(IInputToolsAPI.InputDevice inputDevice)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    CancelHeld?.Invoke(this.stackKey, inputDevice);
                this.GetStackBelow()?.OnCancelHeld(inputDevice);
            }

            internal void OnCancelReleased(IInputToolsAPI.InputDevice inputDevice)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    CancelReleased?.Invoke(this.stackKey, inputDevice);
                this.GetStackBelow()?.OnCancelReleased(inputDevice);
            }

            internal void OnAltPressed(IInputToolsAPI.InputDevice inputDevice)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    AltPressed?.Invoke(this.stackKey, inputDevice);
                this.GetStackBelow()?.OnAltPressed(inputDevice);
            }

            internal void OnAltHeld(IInputToolsAPI.InputDevice inputDevice)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    AltHeld?.Invoke(this.stackKey, inputDevice);
                this.GetStackBelow()?.OnAltHeld(inputDevice);
            }

            internal void OnAltReleased(IInputToolsAPI.InputDevice inputDevice)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    AltReleased?.Invoke(this.stackKey, inputDevice);
                this.GetStackBelow()?.OnAltReleased(inputDevice);
            }

            internal void OnMenuPressed(IInputToolsAPI.InputDevice inputDevice)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MenuPressed?.Invoke(this.stackKey, inputDevice);
                this.GetStackBelow()?.OnMenuPressed(inputDevice);
            }

            internal void OnMenuHeld(IInputToolsAPI.InputDevice inputDevice)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MenuHeld?.Invoke(this.stackKey, inputDevice);
                this.GetStackBelow()?.OnMenuHeld(inputDevice);
            }

            internal void OnMenuReleased(IInputToolsAPI.InputDevice inputDevice)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MenuReleased?.Invoke(this.stackKey, inputDevice);
                this.GetStackBelow()?.OnMenuReleased(inputDevice);
            }

            internal void OnMoveRightPressed(IInputToolsAPI.MoveSource moveSource)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveRightPressed?.Invoke(this.stackKey, moveSource);
                this.GetStackBelow()?.OnMoveRightPressed(moveSource);
            }

            internal void OnMoveRightHeld(IInputToolsAPI.MoveSource moveSource)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveRightHeld?.Invoke(this.stackKey, moveSource);
                this.GetStackBelow()?.OnMoveRightHeld(moveSource);
            }

            internal void OnMoveRightReleased(IInputToolsAPI.MoveSource moveSource)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveRightReleased?.Invoke(this.stackKey, moveSource);
                this.GetStackBelow()?.OnMoveRightReleased(moveSource);
            }

            internal void OnMoveDownPressed(IInputToolsAPI.MoveSource moveSource)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveDownPressed?.Invoke(this.stackKey, moveSource);
                this.GetStackBelow()?.OnMoveDownPressed(moveSource);
            }

            internal void OnMoveDownHeld(IInputToolsAPI.MoveSource moveSource)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveDownHeld?.Invoke(this.stackKey, moveSource);
                this.GetStackBelow()?.OnMoveDownHeld(moveSource);
            }

            internal void OnMoveDownReleased(IInputToolsAPI.MoveSource moveSource)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveDownReleased?.Invoke(this.stackKey, moveSource);
                this.GetStackBelow()?.OnMoveDownReleased(moveSource);
            }

            internal void OnMoveLeftPressed(IInputToolsAPI.MoveSource moveSource)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveLeftPressed?.Invoke(this.stackKey, moveSource);
                this.GetStackBelow()?.OnMoveLeftPressed(moveSource);
            }

            internal void OnMoveLeftHeld(IInputToolsAPI.MoveSource moveSource)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveLeftHeld?.Invoke(this.stackKey, moveSource);
                this.GetStackBelow()?.OnMoveLeftHeld(moveSource);
            }

            internal void OnMoveLeftReleased(IInputToolsAPI.MoveSource moveSource)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveLeftReleased?.Invoke(this.stackKey, moveSource);
                this.GetStackBelow()?.OnMoveLeftReleased(moveSource);
            }

            internal void OnMoveUpPressed(IInputToolsAPI.MoveSource moveSource)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveUpPressed?.Invoke(this.stackKey, moveSource);
                this.GetStackBelow()?.OnMoveUpPressed(moveSource);
            }

            internal void OnMoveUpHeld(IInputToolsAPI.MoveSource moveSource)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveUpHeld?.Invoke(this.stackKey, moveSource);
                this.GetStackBelow()?.OnMoveUpHeld(moveSource);
            }

            internal void OnMoveUpReleased(IInputToolsAPI.MoveSource moveSource)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveUpReleased?.Invoke(this.stackKey, moveSource);
                this.GetStackBelow()?.OnMoveUpReleased(moveSource);
            }

            internal void OnActionPressed(string actionID)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    ActionPressed?.Invoke(this.stackKey, actionID);
                this.GetStackBelow()?.OnActionPressed(actionID);
            }

            internal void OnActionHeld(string actionID)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    ActionHeld?.Invoke(this.stackKey, actionID);
                this.GetStackBelow()?.OnActionHeld(actionID);
            }

            internal void OnActionReleased(string actionID)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    ActionReleased?.Invoke(this.stackKey, actionID);
                this.GetStackBelow()?.OnActionReleased(actionID);
            }

            internal void OnMoveAxisPressed(Vector2 val)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveAxisPressed?.Invoke(this.stackKey, val);
                this.GetStackBelow()?.OnMoveAxisPressed(val);
            }

            internal void OnMoveAxisHeld(Vector2 val)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveAxisHeld?.Invoke(this.stackKey, val);
                this.GetStackBelow()?.OnMoveAxisHeld(val);
            }

            internal void OnMoveAxisReleased(Vector2 val)
            {
                if (this == this.modEntry.inputTools.Global || this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    MoveAxisReleased?.Invoke(this.stackKey, val);
                this.GetStackBelow()?.OnMoveAxisReleased(val);
            }

            public event EventHandler<SButton> ButtonPressed;
            public event EventHandler<SButton> ButtonHeld;
            public event EventHandler<SButton> ButtonReleased;

            public event EventHandler<Tuple<SButton, SButton>> ButtonPairPressed;
            public event EventHandler<Tuple<SButton, SButton>> ButtonPairHeld;
            public event EventHandler<Tuple<SButton, SButton>> ButtonPairReleased;

            public event EventHandler<IInputToolsAPI.InputDevice> ConfirmPressed;
            public event EventHandler<IInputToolsAPI.InputDevice> ConfirmHeld;
            public event EventHandler<IInputToolsAPI.InputDevice> ConfirmReleased;

            public event EventHandler<IInputToolsAPI.InputDevice> CancelPressed;
            public event EventHandler<IInputToolsAPI.InputDevice> CancelHeld;
            public event EventHandler<IInputToolsAPI.InputDevice> CancelReleased;

            public event EventHandler<IInputToolsAPI.InputDevice> AltPressed;
            public event EventHandler<IInputToolsAPI.InputDevice> AltHeld;
            public event EventHandler<IInputToolsAPI.InputDevice> AltReleased;

            public event EventHandler<IInputToolsAPI.InputDevice> MenuPressed;
            public event EventHandler<IInputToolsAPI.InputDevice> MenuHeld;
            public event EventHandler<IInputToolsAPI.InputDevice> MenuReleased;

            public event EventHandler<IInputToolsAPI.MoveSource> MoveRightPressed;
            public event EventHandler<IInputToolsAPI.MoveSource> MoveRightHeld;
            public event EventHandler<IInputToolsAPI.MoveSource> MoveRightReleased;

            public event EventHandler<IInputToolsAPI.MoveSource> MoveDownPressed;
            public event EventHandler<IInputToolsAPI.MoveSource> MoveDownHeld;
            public event EventHandler<IInputToolsAPI.MoveSource> MoveDownReleased;

            public event EventHandler<IInputToolsAPI.MoveSource> MoveLeftPressed;
            public event EventHandler<IInputToolsAPI.MoveSource> MoveLeftHeld;
            public event EventHandler<IInputToolsAPI.MoveSource> MoveLeftReleased;

            public event EventHandler<IInputToolsAPI.MoveSource> MoveUpPressed;
            public event EventHandler<IInputToolsAPI.MoveSource> MoveUpHeld;
            public event EventHandler<IInputToolsAPI.MoveSource> MoveUpReleased;

            public event EventHandler<string> ActionPressed;
            public event EventHandler<string> ActionHeld;
            public event EventHandler<string> ActionReleased;

            public event EventHandler<Vector2> MoveAxisPressed;
            public event EventHandler<Vector2> MoveAxisHeld;
            public event EventHandler<Vector2> MoveAxisReleased;

            public object stackKey { get; }

            public bool IsButtonPressed(SButton button)
            {
                if (this.stackKey != null && !this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    return false;
                return this.modEntry.Helper.Input.GetState(button) == SButtonState.Pressed;
            }

            public bool IsButtonHeld(SButton button)
            {
                if (this.stackKey != null && !this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    return false;
                return this.modEntry.Helper.Input.GetState(button) == SButtonState.Held;
            }

            public bool IsButtonReleased(SButton button)
            {
                if (this.stackKey != null && !this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    return false;
                return this.modEntry.Helper.Input.GetState(button) == SButtonState.Released;
            }

            public bool IsButtonPairPressed(Tuple<SButton, SButton> buttonPair)
            {
                if (this.stackKey != null && !this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    return false;
                return this.IsButtonHeld(buttonPair.Item1) && this.IsButtonPressed(buttonPair.Item2);
            }

            public bool IsButtonPairHeld(Tuple<SButton, SButton> buttonPair)
            {
                if (this.stackKey != null && !this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    return false;
                return this.modEntry.buttonPairsPressing.Contains(buttonPair)
                    && (this.IsButtonHeld(buttonPair.Item1) && this.IsButtonHeld(buttonPair.Item2));
            }

            public bool IsButtonPairReleased(Tuple<SButton, SButton> buttonPair)
            {
                if (this.stackKey != null && !this.modEntry.controlStack.IsStackReachableByInput(this.stackKey))
                    return false;
                return Game1.ticks - this.modEntry.tickButtonPairsReleased <= 1 && this.modEntry.buttonPairsReleased.Contains(buttonPair)
                    && (this.IsButtonReleased(buttonPair.Item1) || this.IsButtonReleased(buttonPair.Item2));
            }

            public IInputToolsAPI.InputDevice IsCursorMoved(bool mouse = true, bool controller = true)
            {
                this.modEntry.inputTools.CurrentInputDevice();
                if (controller && this.modEntry.inputTools.mouseMovedLastTick &&
                    (this.IsButtonHeld(SButton.RightThumbstickUp)
                    || this.IsButtonHeld(SButton.RightThumbstickRight)
                    || this.IsButtonHeld(SButton.RightThumbstickDown)
                    || this.IsButtonHeld(SButton.RightThumbstickLeft)))
                    return IInputToolsAPI.InputDevice.Controller;
                if (mouse && this.modEntry.inputTools.mouseMovedLastTick)
                    return IInputToolsAPI.InputDevice.Mouse;
                return IInputToolsAPI.InputDevice.None;
            }

            public IInputToolsAPI.MoveSource IsMoveButtonHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
            {
                if (this.IsMoveRightHeld(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != MoveSource.None)
                    return this.IsMoveRightHeld(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick);
                if (this.IsMoveDownHeld(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != MoveSource.None)
                    return this.IsMoveDownHeld(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick);
                if (this.IsMoveLeftHeld(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != MoveSource.None)
                    return this.IsMoveLeftHeld(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick);
                if (this.IsMoveUpHeld(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != MoveSource.None)
                    return this.IsMoveUpHeld(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick);
                return MoveSource.None;
            }

            public IInputToolsAPI.MoveSource IsMoveRightHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
            {
                if (controllerDPad && this.IsButtonHeld(SButton.DPadRight))
                    return IInputToolsAPI.MoveSource.ControllerDPad;
                if (controllerThumbstick && this.IsButtonHeld(SButton.LeftThumbstickRight))
                    return IInputToolsAPI.MoveSource.ControllerLeftThumbstick;
                if (keyboardWASD && this.IsButtonHeld(SButton.D))
                    return IInputToolsAPI.MoveSource.KeyboardWASD;
                if (keyboardArrows && this.IsButtonHeld(SButton.Right))
                    return IInputToolsAPI.MoveSource.KeyboardArrow;
                return IInputToolsAPI.MoveSource.None;
            }

            public IInputToolsAPI.MoveSource IsMoveDownHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
            {
                if (controllerDPad && this.IsButtonHeld(SButton.DPadDown))
                    return IInputToolsAPI.MoveSource.ControllerDPad;
                if (controllerThumbstick && this.IsButtonHeld(SButton.LeftThumbstickDown))
                    return IInputToolsAPI.MoveSource.ControllerLeftThumbstick;
                if (keyboardWASD && this.IsButtonHeld(SButton.S))
                    return IInputToolsAPI.MoveSource.KeyboardWASD;
                if (keyboardArrows && this.IsButtonHeld(SButton.Down))
                    return IInputToolsAPI.MoveSource.KeyboardArrow;
                return IInputToolsAPI.MoveSource.None;
            }

            public IInputToolsAPI.MoveSource IsMoveLeftHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
            {
                if (controllerDPad && this.IsButtonHeld(SButton.DPadLeft))
                    return IInputToolsAPI.MoveSource.ControllerDPad;
                if (controllerThumbstick && this.IsButtonHeld(SButton.LeftThumbstickLeft))
                    return IInputToolsAPI.MoveSource.ControllerLeftThumbstick;
                if (keyboardWASD && this.IsButtonHeld(SButton.A))
                    return IInputToolsAPI.MoveSource.KeyboardWASD;
                if (keyboardArrows && this.IsButtonHeld(SButton.Left))
                    return IInputToolsAPI.MoveSource.KeyboardArrow;
                return IInputToolsAPI.MoveSource.None;
            }

            public IInputToolsAPI.MoveSource IsMoveUpHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
            {
                if (controllerDPad && this.IsButtonHeld(SButton.DPadUp))
                    return IInputToolsAPI.MoveSource.ControllerDPad;
                if (controllerThumbstick && this.IsButtonHeld(SButton.LeftThumbstickUp))
                    return IInputToolsAPI.MoveSource.ControllerLeftThumbstick;
                if (keyboardWASD && this.IsButtonHeld(SButton.W))
                    return IInputToolsAPI.MoveSource.KeyboardWASD;
                if (keyboardArrows && this.IsButtonHeld(SButton.Up))
                    return IInputToolsAPI.MoveSource.KeyboardArrow;
                return IInputToolsAPI.MoveSource.None;
            }

            public Tuple<SButton, SButton> IsActionPressed(string actionID)
            {
                if (string.IsNullOrWhiteSpace(actionID))
                    return null;
                foreach (Tuple<SButton, SButton> keyPair in this.modEntry.actions.GetKeyPairsFromActions(actionID))
                {
                    if (this.IsButtonPairPressed(keyPair))
                        return keyPair;
                }
                return null;
            }

            public Tuple<SButton, SButton> IsActionHeld(string actionID)
            {
                if (string.IsNullOrWhiteSpace(actionID))
                    return null;
                foreach (Tuple<SButton, SButton> keyPair in this.modEntry.actions.GetKeyPairsFromActions(actionID))
                {
                    if (this.IsButtonPairHeld(keyPair))
                        return keyPair;
                }
                return null;
            }

            public Tuple<SButton, SButton> IsActionReleased(string actionID)
            {
                if (string.IsNullOrWhiteSpace(actionID))
                    return null;
                foreach (Tuple<SButton, SButton> keyPair in this.modEntry.actions.GetKeyPairsFromActions(actionID))
                {
                    if (this.IsButtonPairReleased(keyPair))
                        return keyPair;
                }
                return null;
            }

            public Vector2 GetMoveAxis(bool keyboardWASD = true, bool keyboardArrows = true, bool controllerDPad = true, bool controllerThumbstick = true)
            {
                return new Vector2(this.IsMoveRightHeld(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != IInputToolsAPI.MoveSource.None ?
                        1 : (this.IsMoveLeftHeld(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != IInputToolsAPI.MoveSource.None ? -1 : 0),
                    this.IsMoveDownHeld(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != IInputToolsAPI.MoveSource.None ?
                        1 : (this.IsMoveUpHeld(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != IInputToolsAPI.MoveSource.None ? -1 : 0));
            }

            public void SetStackActive(bool active)
            {
                this.isActive = active;
            }

            public void SetStackDefaultBlockBehaviour(IInputToolsAPI.StackBlockBehavior stackBlockBehaviour)
            {
                this.blockBehaviour = stackBlockBehaviour;
            }

            public void MoveToTopOfStack()
            {
                if (this.stackKey == null || this == this.modEntry.inputTools.Global)
                    return;
                this.modEntry.controlStack.MoveToTopOfStack(this.stackKey);
            }

            public bool IsStackReachableByInput()
            {
                if (this == this.modEntry.inputTools.Global)
                    return true;
                if (this.stackKey == null)
                    return false;
                return this.modEntry.controlStack.IsStackReachableByInput(this.stackKey);
            }

            public void RemoveSelf()
            {
                if (this.stackKey == null || this == this.modEntry.inputTools.Global)
                    return;
                this.modEntry.controlStack.StackRemove(this.stackKey);
            }
        }
    }
}
