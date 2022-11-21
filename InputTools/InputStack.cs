using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public class InputStack : IInputToolsAPI.IInputStack
    {
        internal bool isActive = true;
        internal IInputToolsAPI.StackBlockBehavior blockBehaviour;

        private InputToolsAPI inputTools;
        internal InputStack(InputToolsAPI inputTools, object stackKey)
        {
            this.inputTools = inputTools;
            this.stackKey = stackKey;
        }

        internal InputStack GetStackBelow(bool stopAtBlock = true)
        {
            if (this == this.inputTools.Global)
            {
                if (this.inputTools.controlStack.stacks.Count > 0)
                    return this.inputTools.controlStack.GetStack(this.inputTools.controlStack.stacks[this.inputTools.controlStack.stacks.Count - 1]) as InputStack;
            }
            else if (!stopAtBlock || !this.isActive || this.blockBehaviour == StackBlockBehavior.PassBelow)
            {
                for (int i = this.inputTools.controlStack.stacks.Count - 1; i >= 0; i--)
                {
                    if (this.inputTools.controlStack.stacks[i] == this.stackKey && i > 0)
                        return this.inputTools.controlStack.GetStack(this.inputTools.controlStack.stacks[i - 1]) as InputStack;
                }
            }
            return null;
        }

        internal void OnInputDeviceChanged(IInputToolsAPI.InputDevice inputDevice)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                InputDeviceChanged?.Invoke(this.stackKey, inputDevice);
            this.GetStackBelow()?.OnInputDeviceChanged(inputDevice);
        }

        internal void OnButtonPressed(SButton button)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                ButtonPressed?.Invoke(this.stackKey, button);
            this.GetStackBelow()?.OnButtonPressed(button);
        }

        internal void OnButtonHeld(SButton button)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                ButtonHeld?.Invoke(this.stackKey, button);
            this.GetStackBelow()?.OnButtonHeld(button);
        }

        internal void OnButtonReleased(SButton button)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                ButtonReleased?.Invoke(this.stackKey, button);
            this.GetStackBelow()?.OnButtonReleased(button);
        }

        internal void OnButtonPairPressed(Tuple<SButton, SButton> buttons)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                ButtonPairPressed?.Invoke(this.stackKey, buttons);
            this.GetStackBelow()?.OnButtonPairPressed(buttons);
        }

        internal void OnButtonPairHeld(Tuple<SButton, SButton> buttons)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                ButtonPairHeld?.Invoke(this.stackKey, buttons);
            this.GetStackBelow()?.OnButtonPairHeld(buttons);
        }

        internal void OnButtonPairReleased(Tuple<SButton, SButton> buttons)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                ButtonPairReleased?.Invoke(this.stackKey, buttons);
            this.GetStackBelow()?.OnButtonPairReleased(buttons);
        }

        internal void OnConfirmPressed(IInputToolsAPI.InputDevice inputDevice)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                ConfirmPressed?.Invoke(this.stackKey, inputDevice);
            this.GetStackBelow()?.OnConfirmPressed(inputDevice);
        }

        internal void OnConfirmHeld(IInputToolsAPI.InputDevice inputDevice)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                ConfirmHeld?.Invoke(this.stackKey, inputDevice);
            this.GetStackBelow()?.OnConfirmHeld(inputDevice);
        }

        internal void OnConfirmReleased(IInputToolsAPI.InputDevice inputDevice)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                ConfirmReleased?.Invoke(this.stackKey, inputDevice);
            this.GetStackBelow()?.OnConfirmReleased(inputDevice);
        }

        internal void OnCancelPressed(IInputToolsAPI.InputDevice inputDevice)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                CancelPressed?.Invoke(this.stackKey, inputDevice);
            this.GetStackBelow()?.OnCancelPressed(inputDevice);
        }

        internal void OnCancelHeld(IInputToolsAPI.InputDevice inputDevice)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                CancelHeld?.Invoke(this.stackKey, inputDevice);
            this.GetStackBelow()?.OnCancelHeld(inputDevice);
        }

        internal void OnCancelReleased(IInputToolsAPI.InputDevice inputDevice)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                CancelReleased?.Invoke(this.stackKey, inputDevice);
            this.GetStackBelow()?.OnCancelReleased(inputDevice);
        }

        internal void OnAltPressed(IInputToolsAPI.InputDevice inputDevice)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                AltPressed?.Invoke(this.stackKey, inputDevice);
            this.GetStackBelow()?.OnAltPressed(inputDevice);
        }

        internal void OnAltHeld(IInputToolsAPI.InputDevice inputDevice)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                AltHeld?.Invoke(this.stackKey, inputDevice);
            this.GetStackBelow()?.OnAltHeld(inputDevice);
        }

        internal void OnAltReleased(IInputToolsAPI.InputDevice inputDevice)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                AltReleased?.Invoke(this.stackKey, inputDevice);
            this.GetStackBelow()?.OnAltReleased(inputDevice);
        }

        internal void OnMenuPressed(IInputToolsAPI.InputDevice inputDevice)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MenuPressed?.Invoke(this.stackKey, inputDevice);
            this.GetStackBelow()?.OnMenuPressed(inputDevice);
        }

        internal void OnMenuHeld(IInputToolsAPI.InputDevice inputDevice)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MenuHeld?.Invoke(this.stackKey, inputDevice);
            this.GetStackBelow()?.OnMenuHeld(inputDevice);
        }

        internal void OnMenuReleased(IInputToolsAPI.InputDevice inputDevice)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MenuReleased?.Invoke(this.stackKey, inputDevice);
            this.GetStackBelow()?.OnMenuReleased(inputDevice);
        }

        internal void OnMoveRightPressed(IInputToolsAPI.MoveSource moveSource)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveRightPressed?.Invoke(this.stackKey, moveSource);
            this.GetStackBelow()?.OnMoveRightPressed(moveSource);
        }

        internal void OnMoveRightHeld(IInputToolsAPI.MoveSource moveSource)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveRightHeld?.Invoke(this.stackKey, moveSource);
            this.GetStackBelow()?.OnMoveRightHeld(moveSource);
        }

        internal void OnMoveRightReleased(IInputToolsAPI.MoveSource moveSource)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveRightReleased?.Invoke(this.stackKey, moveSource);
            this.GetStackBelow()?.OnMoveRightReleased(moveSource);
        }

        internal void OnMoveDownPressed(IInputToolsAPI.MoveSource moveSource)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveDownPressed?.Invoke(this.stackKey, moveSource);
            this.GetStackBelow()?.OnMoveDownPressed(moveSource);
        }

        internal void OnMoveDownHeld(IInputToolsAPI.MoveSource moveSource)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveDownHeld?.Invoke(this.stackKey, moveSource);
            this.GetStackBelow()?.OnMoveDownHeld(moveSource);
        }

        internal void OnMoveDownReleased(IInputToolsAPI.MoveSource moveSource)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveDownReleased?.Invoke(this.stackKey, moveSource);
            this.GetStackBelow()?.OnMoveDownReleased(moveSource);
        }

        internal void OnMoveLeftPressed(IInputToolsAPI.MoveSource moveSource)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveLeftPressed?.Invoke(this.stackKey, moveSource);
            this.GetStackBelow()?.OnMoveLeftPressed(moveSource);
        }

        internal void OnMoveLeftHeld(IInputToolsAPI.MoveSource moveSource)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveLeftHeld?.Invoke(this.stackKey, moveSource);
            this.GetStackBelow()?.OnMoveLeftHeld(moveSource);
        }

        internal void OnMoveLeftReleased(IInputToolsAPI.MoveSource moveSource)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveLeftReleased?.Invoke(this.stackKey, moveSource);
            this.GetStackBelow()?.OnMoveLeftReleased(moveSource);
        }

        internal void OnMoveUpPressed(IInputToolsAPI.MoveSource moveSource)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveUpPressed?.Invoke(this.stackKey, moveSource);
            this.GetStackBelow()?.OnMoveUpPressed(moveSource);
        }

        internal void OnMoveUpHeld(IInputToolsAPI.MoveSource moveSource)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveUpHeld?.Invoke(this.stackKey, moveSource);
            this.GetStackBelow()?.OnMoveUpHeld(moveSource);
        }

        internal void OnMoveUpReleased(IInputToolsAPI.MoveSource moveSource)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveUpReleased?.Invoke(this.stackKey, moveSource);
            this.GetStackBelow()?.OnMoveUpReleased(moveSource);
        }

        internal void OnActionPressed(string actionID)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                ActionPressed?.Invoke(this.stackKey, actionID);
            this.GetStackBelow()?.OnActionPressed(actionID);
        }

        internal void OnActionHeld(string actionID)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                ActionHeld?.Invoke(this.stackKey, actionID);
            this.GetStackBelow()?.OnActionHeld(actionID);
        }

        internal void OnActionReleased(string actionID)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                ActionReleased?.Invoke(this.stackKey, actionID);
            this.GetStackBelow()?.OnActionReleased(actionID);
        }

        internal void OnMoveAxisPressed(Vector2 val)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveAxisPressed?.Invoke(this.stackKey, val);
            this.GetStackBelow()?.OnMoveAxisPressed(val);
        }

        internal void OnMoveAxisHeld(Vector2 val)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveAxisHeld?.Invoke(this.stackKey, val);
            this.GetStackBelow()?.OnMoveAxisHeld(val);
        }

        internal void OnMoveAxisReleased(Vector2 val)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MoveAxisReleased?.Invoke(this.stackKey, val);
            this.GetStackBelow()?.OnMoveAxisReleased(val);
        }

        internal void OnCursorMoved(InputDevice val)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                CursorMoved?.Invoke(this.stackKey, val);
            this.GetStackBelow()?.OnCursorMoved(val);
        }

        internal void OnMouseWheelMoved(Vector2 val)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                MouseWheelMoved?.Invoke(this.stackKey, val);
            this.GetStackBelow()?.OnMouseWheelMoved(val);
        }

        internal void OnPlacementTileChanged(Vector2 placement)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                PlacementTileChanged?.Invoke(this.stackKey, placement);
            this.GetStackBelow()?.OnPlacementTileChanged(placement);
        }

        internal void OnPlacementItemChanged(Item item)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                PlacementItemChanged?.Invoke(this.stackKey, item);
            this.GetStackBelow()?.OnPlacementItemChanged(item);
        }

        internal void OnStackUpdateTicked(UpdateTickedEventArgs e)
        {
            if (this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                StackUpdateTicked?.Invoke(this.stackKey, e);
            this.GetStackBelow()?.OnStackUpdateTicked(e);
        }

        public event EventHandler<IInputToolsAPI.InputDevice> InputDeviceChanged;

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

        public event EventHandler<Vector2> MouseWheelMoved;
        public event EventHandler<IInputToolsAPI.InputDevice> CursorMoved;
        public event EventHandler<Vector2> PlacementTileChanged;
        public event EventHandler<Item> PlacementItemChanged;

        public event EventHandler<UpdateTickedEventArgs> StackUpdateTicked;

        public object stackKey { get; }

        public IInputToolsAPI.IInputStack GetBelow(bool stopAtBlock = true)
        {
            return this.GetStackBelow(stopAtBlock);
        }

        public IInputToolsAPI.InputDevice CurrentInputDevice()
        {
            if (this.inputTools.lastTickUpdated == Game1.ticks)
                return this.inputTools.lastInputDeviceUsed;
            this.inputTools.lastTickUpdated = Game1.ticks;

            IInputToolsAPI.InputDevice newInputDevice = this.inputTools.lastInputDeviceUsed;
            if (Game1.isAnyGamePadButtonBeingPressed() || Game1.isAnyGamePadButtonBeingHeld() || Game1.isGamePadThumbstickInMotion())
                newInputDevice = InputDevice.Controller;
            else if (Game1.GetKeyboardState().GetPressedKeyCount() > 0)
                newInputDevice = InputDevice.Keyboard;
            else if (Game1.input.GetMouseState().LeftButton == ButtonState.Pressed
                || Game1.input.GetMouseState().MiddleButton == ButtonState.Pressed
                || Game1.input.GetMouseState().RightButton == ButtonState.Pressed
                || Game1.input.GetMouseState().XButton1 == ButtonState.Pressed
                || Game1.input.GetMouseState().XButton2 == ButtonState.Pressed
                || (Game1.input.GetMouseState().Position.ToVector2() != this.inputTools.lastMousePos && Game1.lastCursorMotionWasMouse)
                || Game1.input.GetMouseState().ScrollWheelValue != this.inputTools.lastScrollWheelPos
                || Game1.input.GetMouseState().HorizontalScrollWheelValue != this.inputTools.lastHorizontalScrollWheelPos
                )
                newInputDevice = InputDevice.Mouse;

            this.inputTools.mouseMovedLastTick = this.inputTools.lastMousePos != Game1.input.GetMouseState().Position.ToVector2();
            this.inputTools.mouseWheelMovedLastTick = (this.inputTools.lastScrollWheelPos != Game1.input.GetMouseState().ScrollWheelValue) || (this.inputTools.lastHorizontalScrollWheelPos != Game1.input.GetMouseState().HorizontalScrollWheelValue);

            this.inputTools.lastMousePos = Game1.input.GetMouseState().Position.ToVector2();
            this.inputTools.lastScrollWheelPos = Game1.input.GetMouseState().ScrollWheelValue;
            this.inputTools.lastHorizontalScrollWheelPos = Game1.input.GetMouseState().HorizontalScrollWheelValue;
            if (this.inputTools.lastInputDeviceUsed != newInputDevice)
            {
                this.inputTools.lastInputDeviceUsed = newInputDevice;
                this.OnInputDeviceChanged(newInputDevice);
            }
            else
                this.inputTools.lastInputDeviceUsed = newInputDevice;
            if (this.inputTools.mouseMovedLastTick)
                this.OnCursorMoved(newInputDevice);
            if (this.inputTools.mouseWheelMovedLastTick)
                this.OnMouseWheelMoved(this.GetMouseWheelPos());
            return this.inputTools.lastInputDeviceUsed;
        }

        public bool IsButtonPressed(SButton button)
        {
            if (!this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                return false;
            return this.inputTools.Helper.Input.GetState(button) == SButtonState.Pressed;
        }

        public bool IsButtonHeld(SButton button)
        {
            if (!this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                return false;
            return this.inputTools.Helper.Input.GetState(button) == SButtonState.Held;
        }

        public bool IsButtonReleased(SButton button)
        {
            if (!this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                return false;
            return this.inputTools.Helper.Input.GetState(button) == SButtonState.Released;
        }

        public bool IsButtonPairPressed(Tuple<SButton, SButton> buttonPair)
        {
            if (!this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                return false;
            return this.IsButtonHeld(buttonPair.Item1) && this.IsButtonPressed(buttonPair.Item2);
        }

        public bool IsButtonPairHeld(Tuple<SButton, SButton> buttonPair)
        {
            if (!this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                return false;
            return this.inputTools.buttonPairsPressing.Contains(buttonPair)
                && (this.IsButtonHeld(buttonPair.Item1) && this.IsButtonHeld(buttonPair.Item2));
        }

        public bool IsButtonPairReleased(Tuple<SButton, SButton> buttonPair)
        {
            if (!this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                return false;
            return Game1.ticks - this.inputTools.tickButtonPairsReleased <= 1 && this.inputTools.buttonPairsReleased.Contains(buttonPair)
                && (this.IsButtonReleased(buttonPair.Item1) || this.IsButtonReleased(buttonPair.Item2));
        }

        public bool IsMouseWheelMoved()
        {
            if (!this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                return false;
            return this.inputTools.mouseWheelMovedLastTick;
        }

        public IInputToolsAPI.InputDevice IsCursorMoved(bool mouse = true, bool controller = true)
        {
            this.CurrentInputDevice();
            if (!this.inputTools.controlStack.IsStackReachableByInput(this.stackKey))
                return IInputToolsAPI.InputDevice.None;
            if (controller && this.inputTools.mouseMovedLastTick &&
                (this.IsButtonHeld(SButton.RightThumbstickUp)
                || this.IsButtonHeld(SButton.RightThumbstickRight)
                || this.IsButtonHeld(SButton.RightThumbstickDown)
                || this.IsButtonHeld(SButton.RightThumbstickLeft)))
                return IInputToolsAPI.InputDevice.Controller;
            if (mouse && this.inputTools.mouseMovedLastTick)
                return IInputToolsAPI.InputDevice.Mouse;
            return IInputToolsAPI.InputDevice.None;
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
            return this.inputTools.isLastPlacementTileFromCursor;
        }

        public bool IsPlacementTileChanged()
        {
            return this.inputTools.isPlacementTileMovedLastTick;
        }

        public IInputToolsAPI.InputDevice IsConfirmPressed(bool keyboard = true, bool controller = true)
        {
            if (controller && this.IsButtonPressed(SButton.ControllerA))
                return IInputToolsAPI.InputDevice.Controller;
            if (keyboard && this.IsButtonPressed(SButton.Enter))
                return IInputToolsAPI.InputDevice.Keyboard;
            return InputDevice.None;
        }

        public IInputToolsAPI.InputDevice IsConfirmHeld(bool keyboard = true, bool controller = true)
        {
            if (controller && this.IsButtonHeld(SButton.ControllerA))
                return IInputToolsAPI.InputDevice.Controller;
            if (keyboard && this.IsButtonHeld(SButton.Enter))
                return IInputToolsAPI.InputDevice.Keyboard;
            return InputDevice.None;
        }

        public IInputToolsAPI.InputDevice IsConfirmReleased(bool keyboard = true, bool controller = true)
        {
            if (controller && this.IsButtonReleased(SButton.ControllerA))
                return IInputToolsAPI.InputDevice.Controller;
            if (keyboard && this.IsButtonReleased(SButton.Enter))
                return IInputToolsAPI.InputDevice.Keyboard;
            return InputDevice.None;
        }

        public IInputToolsAPI.InputDevice IsCancelPressed(bool keyboard = true, bool controller = true)
        {
            if (controller && this.IsButtonPressed(SButton.ControllerB))
                return IInputToolsAPI.InputDevice.Controller;
            if (keyboard && this.IsButtonPressed(SButton.Escape))
                return IInputToolsAPI.InputDevice.Keyboard;
            return InputDevice.None;
        }

        public IInputToolsAPI.InputDevice IsCancelHeld(bool keyboard = true, bool controller = true)
        {
            if (controller && this.IsButtonHeld(SButton.ControllerB))
                return IInputToolsAPI.InputDevice.Controller;
            if (keyboard && this.IsButtonHeld(SButton.Escape))
                return IInputToolsAPI.InputDevice.Keyboard;
            return InputDevice.None;
        }

        public IInputToolsAPI.InputDevice IsCancelReleased(bool keyboard = true, bool controller = true)
        {
            if (controller && this.IsButtonReleased(SButton.ControllerB))
                return IInputToolsAPI.InputDevice.Controller;
            if (keyboard && this.IsButtonReleased(SButton.Escape))
                return IInputToolsAPI.InputDevice.Keyboard;
            return InputDevice.None;
        }

        public IInputToolsAPI.InputDevice IsAltPressed(bool keyboard = true, bool mouse = true, bool controller = true)
        {
            if (controller && this.IsButtonPressed(SButton.ControllerX))
                return IInputToolsAPI.InputDevice.Controller;
            if (keyboard && this.IsButtonPressed(SButton.Space))
                return IInputToolsAPI.InputDevice.Keyboard;
            if (mouse && this.IsButtonPressed(SButton.MouseRight))
                return IInputToolsAPI.InputDevice.Mouse;
            return InputDevice.None;
        }

        public IInputToolsAPI.InputDevice IsAltHeld(bool keyboard = true, bool mouse = true, bool controller = true)
        {
            if (controller && this.IsButtonHeld(SButton.ControllerX))
                return IInputToolsAPI.InputDevice.Controller;
            if (keyboard && this.IsButtonHeld(SButton.Space))
                return IInputToolsAPI.InputDevice.Keyboard;
            if (mouse && this.IsButtonPressed(SButton.MouseRight))
                return IInputToolsAPI.InputDevice.Mouse;
            return InputDevice.None;
        }

        public IInputToolsAPI.InputDevice IsAltReleased(bool keyboard = true, bool mouse = true, bool controller = true)
        {
            if (controller && this.IsButtonReleased(SButton.ControllerX))
                return IInputToolsAPI.InputDevice.Controller;
            if (keyboard && this.IsButtonReleased(SButton.Space))
                return IInputToolsAPI.InputDevice.Keyboard;
            if (mouse && this.IsButtonPressed(SButton.MouseRight))
                return IInputToolsAPI.InputDevice.Mouse;
            return InputDevice.None;
        }


        public IInputToolsAPI.InputDevice IsMenuPressed(bool keyboard = true, bool controller = true)
        {
            if (controller && this.IsButtonPressed(SButton.ControllerY))
                return IInputToolsAPI.InputDevice.Controller;
            if (keyboard && this.IsButtonPressed(SButton.Escape))
                return IInputToolsAPI.InputDevice.Keyboard;
            return InputDevice.None;
        }

        public IInputToolsAPI.InputDevice IsMenuHeld(bool keyboard = true, bool controller = true)
        {
            if (controller && this.IsButtonHeld(SButton.ControllerY))
                return IInputToolsAPI.InputDevice.Controller;
            if (keyboard && this.IsButtonHeld(SButton.Escape))
                return IInputToolsAPI.InputDevice.Keyboard;
            return InputDevice.None;
        }

        public IInputToolsAPI.InputDevice IsMenuReleased(bool keyboard = true, bool controller = true)
        {
            if (controller && this.IsButtonReleased(SButton.ControllerY))
                return IInputToolsAPI.InputDevice.Controller;
            if (keyboard && this.IsButtonReleased(SButton.Escape))
                return IInputToolsAPI.InputDevice.Keyboard;
            return InputDevice.None;
        }

        public IInputToolsAPI.MoveSource IsMoveButtonPressed(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
        {
            if (this.IsMoveRightPressed(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != MoveSource.None)
                return this.IsMoveRightPressed(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick);
            if (this.IsMoveDownPressed(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != MoveSource.None)
                return this.IsMoveDownPressed(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick);
            if (this.IsMoveLeftPressed(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != MoveSource.None)
                return this.IsMoveLeftPressed(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick);
            if (this.IsMoveUpPressed(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != MoveSource.None)
                return this.IsMoveUpPressed(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick);
            return MoveSource.None;
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

        public IInputToolsAPI.MoveSource IsMoveButtonReleased(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
        {
            if (this.IsMoveRightReleased(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != MoveSource.None)
                return this.IsMoveRightReleased(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick);
            if (this.IsMoveDownReleased(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != MoveSource.None)
                return this.IsMoveDownReleased(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick);
            if (this.IsMoveLeftReleased(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != MoveSource.None)
                return this.IsMoveLeftReleased(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick);
            if (this.IsMoveUpReleased(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick) != MoveSource.None)
                return this.IsMoveUpReleased(keyboardWASD, keyboardArrows, controllerDPad, controllerThumbstick);
            return MoveSource.None;
        }

        public IInputToolsAPI.MoveSource IsMoveRightPressed(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
        {
            if (controllerDPad && this.IsButtonPressed(SButton.DPadRight))
                return IInputToolsAPI.MoveSource.ControllerDPad;
            if (controllerThumbstick && this.IsButtonPressed(SButton.LeftThumbstickRight))
                return IInputToolsAPI.MoveSource.ControllerLeftThumbstick;
            if (keyboardWASD && this.IsButtonPressed(SButton.D))
                return IInputToolsAPI.MoveSource.KeyboardWASD;
            if (keyboardArrows && this.IsButtonPressed(SButton.Right))
                return IInputToolsAPI.MoveSource.KeyboardArrow;
            return IInputToolsAPI.MoveSource.None;
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

        public IInputToolsAPI.MoveSource IsMoveRightReleased(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
        {
            if (controllerDPad && this.IsButtonReleased(SButton.DPadRight))
                return IInputToolsAPI.MoveSource.ControllerDPad;
            if (controllerThumbstick && this.IsButtonReleased(SButton.LeftThumbstickRight))
                return IInputToolsAPI.MoveSource.ControllerLeftThumbstick;
            if (keyboardWASD && this.IsButtonReleased(SButton.D))
                return IInputToolsAPI.MoveSource.KeyboardWASD;
            if (keyboardArrows && this.IsButtonReleased(SButton.Right))
                return IInputToolsAPI.MoveSource.KeyboardArrow;
            return IInputToolsAPI.MoveSource.None;
        }


        public IInputToolsAPI.MoveSource IsMoveDownPressed(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
        {
            if (controllerDPad && this.IsButtonPressed(SButton.DPadDown))
                return IInputToolsAPI.MoveSource.ControllerDPad;
            if (controllerThumbstick && this.IsButtonPressed(SButton.LeftThumbstickDown))
                return IInputToolsAPI.MoveSource.ControllerLeftThumbstick;
            if (keyboardWASD && this.IsButtonPressed(SButton.S))
                return IInputToolsAPI.MoveSource.KeyboardWASD;
            if (keyboardArrows && this.IsButtonPressed(SButton.Down))
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

        public IInputToolsAPI.MoveSource IsMoveDownReleased(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
        {
            if (controllerDPad && this.IsButtonReleased(SButton.DPadDown))
                return IInputToolsAPI.MoveSource.ControllerDPad;
            if (controllerThumbstick && this.IsButtonReleased(SButton.LeftThumbstickDown))
                return IInputToolsAPI.MoveSource.ControllerLeftThumbstick;
            if (keyboardWASD && this.IsButtonReleased(SButton.S))
                return IInputToolsAPI.MoveSource.KeyboardWASD;
            if (keyboardArrows && this.IsButtonReleased(SButton.Down))
                return IInputToolsAPI.MoveSource.KeyboardArrow;
            return IInputToolsAPI.MoveSource.None;
        }

        public IInputToolsAPI.MoveSource IsMoveLeftPressed(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
        {
            if (controllerDPad && this.IsButtonPressed(SButton.DPadLeft))
                return IInputToolsAPI.MoveSource.ControllerDPad;
            if (controllerThumbstick && this.IsButtonPressed(SButton.LeftThumbstickLeft))
                return IInputToolsAPI.MoveSource.ControllerLeftThumbstick;
            if (keyboardWASD && this.IsButtonPressed(SButton.A))
                return IInputToolsAPI.MoveSource.KeyboardWASD;
            if (keyboardArrows && this.IsButtonPressed(SButton.Left))
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

        public IInputToolsAPI.MoveSource IsMoveLeftReleased(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
        {
            if (controllerDPad && this.IsButtonReleased(SButton.DPadLeft))
                return IInputToolsAPI.MoveSource.ControllerDPad;
            if (controllerThumbstick && this.IsButtonReleased(SButton.LeftThumbstickLeft))
                return IInputToolsAPI.MoveSource.ControllerLeftThumbstick;
            if (keyboardWASD && this.IsButtonReleased(SButton.A))
                return IInputToolsAPI.MoveSource.KeyboardWASD;
            if (keyboardArrows && this.IsButtonReleased(SButton.Left))
                return IInputToolsAPI.MoveSource.KeyboardArrow;
            return IInputToolsAPI.MoveSource.None;
        }

        public IInputToolsAPI.MoveSource IsMoveUpPressed(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
        {
            if (controllerDPad && this.IsButtonPressed(SButton.DPadUp))
                return IInputToolsAPI.MoveSource.ControllerDPad;
            if (controllerThumbstick && this.IsButtonPressed(SButton.LeftThumbstickUp))
                return IInputToolsAPI.MoveSource.ControllerLeftThumbstick;
            if (keyboardWASD && this.IsButtonPressed(SButton.W))
                return IInputToolsAPI.MoveSource.KeyboardWASD;
            if (keyboardArrows && this.IsButtonPressed(SButton.Up))
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

        public IInputToolsAPI.MoveSource IsMoveUpReleased(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true)
        {
            if (controllerDPad && this.IsButtonReleased(SButton.DPadUp))
                return IInputToolsAPI.MoveSource.ControllerDPad;
            if (controllerThumbstick && this.IsButtonReleased(SButton.LeftThumbstickUp))
                return IInputToolsAPI.MoveSource.ControllerLeftThumbstick;
            if (keyboardWASD && this.IsButtonReleased(SButton.W))
                return IInputToolsAPI.MoveSource.KeyboardWASD;
            if (keyboardArrows && this.IsButtonReleased(SButton.Up))
                return IInputToolsAPI.MoveSource.KeyboardArrow;
            return IInputToolsAPI.MoveSource.None;
        }

        public Tuple<SButton, SButton> IsActionPressed(string actionID)
        {
            if (string.IsNullOrWhiteSpace(actionID))
                return null;
            foreach (Tuple<SButton, SButton> keyPair in this.inputTools.actions.GetKeyPairsFromActions(actionID))
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
            foreach (Tuple<SButton, SButton> keyPair in this.inputTools.actions.GetKeyPairsFromActions(actionID))
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
            foreach (Tuple<SButton, SButton> keyPair in this.inputTools.actions.GetKeyPairsFromActions(actionID))
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

        public Vector2 GetCursorScreenPos()
        {
            return this.inputTools.Helper.Input.GetCursorPosition().ScreenPixels;
        }

        public Vector2 GetCursorTilePos()
        {
            return this.inputTools.Helper.Input.GetCursorPosition().Tile;
        }

        public Vector2 GetMouseWheelPos()
        {
            return new Vector2(Game1.input.GetMouseState().HorizontalScrollWheelValue, Game1.input.GetMouseState().ScrollWheelValue);
        }

        public Vector2 GetPlacementTile()
        {
            return this.inputTools.lastTileHighlightPos;
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
            if (this.stackKey == null || this == this.inputTools.Global)
                return;
            this.inputTools.controlStack.MoveToTopOfStack(this.stackKey);
        }

        public bool IsStackReachableByInput()
        {
            if (this == this.inputTools.Global)
                return true;
            if (this.stackKey == null)
                return false;
            return this.inputTools.controlStack.IsStackReachableByInput(this.stackKey);
        }

        public void RemoveSelf()
        {
            if (this.stackKey == null || this == this.inputTools.Global)
                return;
            this.inputTools.controlStack.StackRemove(this.stackKey);
        }
    }
}
