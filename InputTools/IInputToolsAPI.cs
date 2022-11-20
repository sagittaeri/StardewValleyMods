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

namespace InputTools
{
    public interface IInputToolsAPI
    {
        public enum InputDevice
        {
            None,
            Mouse,
            Keyboard,
            Controller
        }

        public enum MoveSource
        {
            None,
            KeyboardWASD,
            KeyboardArrow,
            ControllerLeftThumbstick,
            ControllerDPad,
        }

        public enum StackBlockBehavior
        {
            None,
            Block,
            PassBelow
        }

        public event EventHandler<IInputToolsAPI.InputDevice> InputDeviceChanged;
        public event EventHandler<Vector2> PlacementTileChanged;
        public event EventHandler<Item> PlacementItemChanged;

        public IInputToolsAPI.InputDevice CurrentInputDevice();
        public IInputToolsAPI.InputDevice GetInputDevice(SButton button);
        public bool IsHeldItemBomb();
        public bool IsPlacementTileFromCursor();
        public bool IsPlacementTileChanged();
        public Vector2 GetPlacementTile();
        public Vector2 GetPlacementTileWithController();
        public IInputToolsAPI.InputDevice IsConfirmButton(SButton button);
        public IInputToolsAPI.InputDevice IsCancelButton(SButton button);
        public IInputToolsAPI.InputDevice IsAltButton(SButton button);
        public IInputToolsAPI.InputDevice IsMenuButton(SButton button);
        public IInputToolsAPI.MoveSource IsMoveRightButton(SButton button);
        public IInputToolsAPI.MoveSource IsMoveDownButton(SButton button);
        public IInputToolsAPI.MoveSource IsMoveLeftButton(SButton button);
        public IInputToolsAPI.MoveSource IsMoveUpButton(SButton button);
        public void ListenForKeybinding(Action<Tuple<SButton, SButton>> keyBindingCallback);
        public void StopListeningForKeybinding();
        public void RegisterAction(string actionID, params SButton[] keyTriggers);
        public void RegisterAction(string actionID, params Tuple<SButton, SButton>[] keyTriggers);
        public void UnregisterAction(string actionID);
        public List<string> GetActionsFromKey(SButton key);
        public List<string> GetActionsFromKeyPair(Tuple<SButton, SButton> keyPair);
        public List<Tuple<SButton, SButton>> GetKeyPairsFromActions(string actionID);

        public IInputToolsAPI.IInputStack StackCreate(object stackKey, bool startActive = true, IInputToolsAPI.StackBlockBehavior defaultBlockBehaviour = IInputToolsAPI.StackBlockBehavior.Block);
        public void StackRemove(object stackKey);
        public IInputToolsAPI.IInputStack GetStack(object stackKey);

        public IInputStack Global { get; }
        public interface IInputStack
        {
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

            public bool IsButtonPressed(SButton button);
            public bool IsButtonHeld(SButton button);
            public bool IsButtonReleased(SButton button);
            public bool IsButtonPairPressed(Tuple<SButton, SButton> buttonPair);
            public bool IsButtonPairHeld(Tuple<SButton, SButton> buttonPair);
            public bool IsButtonPairReleased(Tuple<SButton, SButton> buttonPair);
            public IInputToolsAPI.InputDevice IsCursorMoved(bool mouse = true, bool controller = true);
            public IInputToolsAPI.MoveSource IsMoveButtonHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);
            public IInputToolsAPI.MoveSource IsMoveRightHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);
            public IInputToolsAPI.MoveSource IsMoveDownHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);
            public IInputToolsAPI.MoveSource IsMoveLeftHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);
            public IInputToolsAPI.MoveSource IsMoveUpHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);
            public Tuple<SButton, SButton> IsActionPressed(string actionID);
            public Tuple<SButton, SButton> IsActionHeld(string actionID);
            public Tuple<SButton, SButton> IsActionReleased(string actionID);
            public Vector2 GetMoveAxis(bool keyboardWASD = true, bool keyboardArrows = true, bool controllerDPad = true, bool controllerThumbstick = true);
            public void SetStackActive(bool active);
            public void SetStackDefaultBlockBehaviour(IInputToolsAPI.StackBlockBehavior stackBlockBehaviour);
            public void MoveToTopOfStack();
            public bool IsStackReachableByInput();
            public void RemoveSelf();
        }
    }
}
