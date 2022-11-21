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
    /// <summary>
    /// The API which lets other mods interact with InputTools.
    ///
    /// Some basic concepts:
    ///     - A single button interaction is usually referred to as "Button" here
    ///     - A double button interaction (e.g. Ctrl+Space) is usually referred to as "Button Pair" here
    ///     - An Action refers to custom-defined actions that can be triggered by inputs such as "Jump" or "Navigate Left"
    ///
    /// More complex concepts:
    ///     - An Input Stack (sometimes just Stack) refers an object where all the input listeners (ButtonPressed, CursorMoved, etc)
    ///       resides. NOTE: it is called a "Stack" because there can be more than one stack, each of which can be turned off and on
    ///       at will for more complex input setups
    ///     - "Global" is a special Input Stack which always sits above all the other stacks, and is generally considered the
    ///       default (and only) Input Stack for most use-cases
    /// </summary>
    public interface IInputToolsAPI
    {
        /// <summary>Enum for input devices.</summary>
        public enum InputDevice
        {
            None,
            Mouse,
            Keyboard,
            Controller
        }

        /// <summary>Enum to identify where a "move" input came from.</summary>
        public enum MoveSource
        {
            None,
            KeyboardWASD,
            KeyboardArrow,
            ControllerLeftThumbstick,
            ControllerDPad,
        }

        /// <summary>Enum to specify whether or not an input stack blocks inputs from going to the stack below.</summary>
        public enum StackBlockBehavior
        {
            None,
            Block,
            PassBelow
        }

        /// <summary>List the other mods that have also loaded InputTools.</summary>
        /// <returns>Returns a list of unique mod ID.</returns>
        public List<string> GetListOfModIDs();

        /// <summary>Gets the input device of a specific button.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>Returns the input device the button is from if a valid button, and InputDevice.None is invalid.</returns>
        public IInputToolsAPI.InputDevice GetInputDevice(SButton button);

        /// <summary>Checks if a button is the Confirm action. Keyboard Enter and Controller A buttons are assigned to Confirm.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>Returns the input device the button is from if a valid button, and InputDevice.None is invalid.</returns>
        public IInputToolsAPI.InputDevice IsConfirmButton(SButton button);

        /// <summary>Checks if a button is the Cancel action. Keyboard Escape (same with Menu) and Controller B buttons are assigned to Cancel.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>Returns the input device the button is from if a valid button, and InputDevice.None is invalid.</returns>
        public IInputToolsAPI.InputDevice IsCancelButton(SButton button);

        /// <summary>Checks if a button is the Alt action. Keyboard Space and Controller X buttons are assigned to Alt.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>Returns the input device the button is from if a valid button, and InputDevice.None is invalid.</returns>
        public IInputToolsAPI.InputDevice IsAltButton(SButton button);

        /// <summary>Checks if a button is the Menu action. Keyboard Escape (same with Cancel) and Controller Y buttons are assigned to Menu.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>Returns the input device the button is from if a valid button, and InputDevice.None is invalid.</returns>
        public IInputToolsAPI.InputDevice IsMenuButton(SButton button);

        /// <summary>Checks if a button is the Move-Right action. Keyboard D and Controller DPad/Left-Thumbstick Right buttons are assigned to Move-Right.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>Returns the input device the button is from if a valid button, and InputDevice.None is invalid.</returns>
        public IInputToolsAPI.MoveSource IsMoveRightButton(SButton button);

        /// <summary>Checks if a button is the Move-Down action. Keyboard S and Controller DPad/Left-Thumbstick Down buttons are assigned to Move-Down.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>Returns the input device the button is from if a valid button, and InputDevice.None is invalid.</returns>
        public IInputToolsAPI.MoveSource IsMoveDownButton(SButton button);

        /// <summary>Checks if a button is the Move-Left action. Keyboard A and Controller DPad/Left-Thumbstick Left buttons are assigned to Move-Left.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>Returns the input device the button is from if a valid button, and InputDevice.None is invalid.</returns>
        public IInputToolsAPI.MoveSource IsMoveLeftButton(SButton button);

        /// <summary>Checks if a button is the Move-Up action. Keyboard W and Controller DPad/Left-Thumbstick Up buttons are assigned to Move-Up.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>Returns the input device the button is from if a valid button, and InputDevice.None is invalid.</returns>
        public IInputToolsAPI.MoveSource IsMoveUpButton(SButton button);

        /// <summary>Listens for the next keystroke (can be two keys). Useful to get keybinding from user.</summary>
        /// <param name="keyBindingCallback">Callback function with argument Tuple<SButton, SButton> when keybinding is done. Null is sent if user pressed Cancel instead.</param>
        public void ListenForKeybinding(Action<Tuple<SButton, SButton>> keyBindingCallback);

        /// <summary>Stops listening for keybinding before user chooses one.</summary>
        public void StopListeningForKeybinding();

        /// <summary>Registers a new input action (e.g. Jump, NavigateLeft, etc) or append an existing one</summary>
        /// <param name="actionID">The ID of the new input action.</param>
        /// <param name="keyTriggers">List of buttons that will trigger this action</param>
        public void RegisterAction(string actionID, params SButton[] keyTriggers);

        /// <summary>Registers a new input action (e.g. Jump, NavigateLeft, etc) or append an existing one</summary>
        /// <param name="actionID">The ID of the new input action.</param>
        /// <param name="keyTriggers">List of button pairs (e.g. Ctrl+Space) that will trigger this action</param>
        public void RegisterAction(string actionID, params Tuple<SButton, SButton>[] keyTriggers);

        /// <summary>Unregisters a previously registered input action.</summary>
        /// <param name="actionID">The ID of the input action to delete.</param>
        public void UnregisterAction(string actionID);

        /// <summary>Gets a list of actions that will be triggered by a button</summary>
        /// <param name="key">The button that will trigger the actions</param>
        /// <returns>The list of actions</returns>
        public List<string> GetActionsFromKey(SButton key);

        /// <summary>Gets a list of actions that will be triggered by a button pair</summary>
        /// <param name="key">The button pair that will trigger the actions</param>
        /// <returns>The list of actions</returns>
        public List<string> GetActionsFromKeyPair(Tuple<SButton, SButton> keyPair);

        /// <summary>Gets a list of buttons pairs that an action is triggered by</summary>
        /// <param name="actionID">The action ID</param>
        /// <returns>The list of button pairs</returns>
        public List<Tuple<SButton, SButton>> GetKeyPairsFromActions(string actionID);

        /// <summary>Creates a new custom Input Stack (i.e. object where all input listeners and methods are) at the top of the stack</summary>
        /// <param name="stackKey">Stack ID, can be any object</param>
        /// <param name="startActive">Whether or not input events are processed</param>
        /// <param name="defaultBlockBehaviour">Whether or not input events are sent down to the stack below</param>
        /// <returns></returns>
        public IInputToolsAPI.IInputStack StackCreate(object stackKey, bool startActive = true, IInputToolsAPI.StackBlockBehavior defaultBlockBehaviour = IInputToolsAPI.StackBlockBehavior.Block);

        /// <summary>Removes a custom Input Stack. This will disconnect any listeners as well.</summary>
        /// <param name="stackKey">Stack ID, can be any object</param>
        public void StackRemove(object stackKey);
        public IInputToolsAPI.IInputStack GetStack(object stackKey);

        /// <summary>The "default" Input Stack that sits above all the custom stacks</summary>
        public IInputStack Global { get; }

        /// <summary>
        /// An Input Stack (sometimes just Stack) refers an object where all the input listeners (ButtonPressed, CursorMoved, etc)
        /// resides. NOTE: it is called a "Stack" because there can be more than one stack, each of which can be turned off and on
        /// at will for more complex input setups
        ///
        /// Note: if an Input Stack is disabled, it doesn't fire ANY events
        /// </summary>
        public interface IInputStack
        {
            /// <summary>Event fired when the last input device changes e.g. from keyboard to mouse or controller</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> InputDeviceChanged;

            /// <summary>Event fired when a single button is pressed this tick</summary>
            public event EventHandler<SButton> ButtonPressed;
            /// <summary>Event fired when a single button is held since last tick</summary>
            public event EventHandler<SButton> ButtonHeld;
            /// <summary>Event fired when a single button is released this tick</summary>
            public event EventHandler<SButton> ButtonReleased;

            /// <summary>Event fired when a button pair is pressed this tick. Always fired before the single button event</summary>
            public event EventHandler<Tuple<SButton, SButton>> ButtonPairPressed;
            /// <summary>Event fired when a button pair is held since last tick. Always fired before the single button event</summary>
            public event EventHandler<Tuple<SButton, SButton>> ButtonPairHeld;
            /// <summary>Event fired when a button pair (either button) is released this tick. Always fired before the single button event</summary>
            public event EventHandler<Tuple<SButton, SButton>> ButtonPairReleased;

            /// <summary>Event fired when the Confirm action is pressed this tick</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> ConfirmPressed;
            /// <summary>Event fired when the Confirm action is held since last tick</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> ConfirmHeld;
            /// <summary>Event fired when the Confirm action is released this tick</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> ConfirmReleased;

            /// <summary>Event fired when the Cancel action is pressed this tick</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> CancelPressed;
            /// <summary>Event fired when the Cancel action is held since last tick</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> CancelHeld;
            /// <summary>Event fired when the Cancel action is released this tick</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> CancelReleased;

            /// <summary>Event fired when the Alt action is pressed this tick</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> AltPressed;
            /// <summary>Event fired when the Alt action is held since last tick</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> AltHeld;
            /// <summary>Event fired when the Alt action is released this tick</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> AltReleased;

            /// <summary>Event fired when the Menu action is pressed this tick</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> MenuPressed;
            /// <summary>Event fired when the Menu action is held since last tick</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> MenuHeld;
            /// <summary>Event fired when the Menu action is released this tick</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> MenuReleased;

            /// <summary>Event fired when the MoveRight action is pressed this tick</summary>
            public event EventHandler<IInputToolsAPI.MoveSource> MoveRightPressed;
            /// <summary>Event fired when the MoveRight action is held since last tick</summary>
            public event EventHandler<IInputToolsAPI.MoveSource> MoveRightHeld;
            /// <summary>Event fired when the MoveRight action is released this tick</summary>
            public event EventHandler<IInputToolsAPI.MoveSource> MoveRightReleased;

            /// <summary>Event fired when the MoveDown action is pressed this tick</summary>
            public event EventHandler<IInputToolsAPI.MoveSource> MoveDownPressed;
            /// <summary>Event fired when the MoveDown action is held since last tick</summary>
            public event EventHandler<IInputToolsAPI.MoveSource> MoveDownHeld;
            /// <summary>Event fired when the MoveDown action is released this tick</summary>
            public event EventHandler<IInputToolsAPI.MoveSource> MoveDownReleased;

            /// <summary>Event fired when the MoveLeft action is pressed this tick</summary>
            public event EventHandler<IInputToolsAPI.MoveSource> MoveLeftPressed;
            /// <summary>Event fired when the MoveLeft action is held since last tick</summary>
            public event EventHandler<IInputToolsAPI.MoveSource> MoveLeftHeld;
            /// <summary>Event fired when the MoveLeft action is released this tick</summary>
            public event EventHandler<IInputToolsAPI.MoveSource> MoveLeftReleased;

            /// <summary>Event fired when the MoveUp action is pressed this tick</summary>
            public event EventHandler<IInputToolsAPI.MoveSource> MoveUpPressed;
            /// <summary>Event fired when the MoveUp action is held since last tick</summary>
            public event EventHandler<IInputToolsAPI.MoveSource> MoveUpHeld;
            /// <summary>Event fired when the MoveUp action is released this tick</summary>
            public event EventHandler<IInputToolsAPI.MoveSource> MoveUpReleased;

            /// <summary>Event fired when buttons that can trigger the action is pressed this tick</summary>
            public event EventHandler<string> ActionPressed;
            /// <summary>Event fired when buttons that can trigger the action is held since last tick</summary>
            public event EventHandler<string> ActionHeld;
            /// <summary>Event fired when buttons that can trigger the action is released this tick</summary>
            public event EventHandler<string> ActionReleased;

            /// <summary>Event fired when buttons that moves the player is pressed this tick</summary>
            public event EventHandler<Vector2> MoveAxisPressed;
            /// <summary>Event fired when buttons that moves the player is held since last tick</summary>
            public event EventHandler<Vector2> MoveAxisHeld;
            /// <summary>Event fired when buttons that moves the player is released this tick</summary>
            public event EventHandler<Vector2> MoveAxisReleased;

            /// <summary>Event fired when the mouse wheel is moved this tick</summary>
            public event EventHandler<Vector2> MouseWheelMoved;
            /// <summary>Event fired when the mouse cursor is moved this tick</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> CursorMoved;
            /// <summary>Event fired when the placement tile has changed this tick</summary>
            public event EventHandler<Vector2> PlacementTileChanged;
            /// <summary>Event fired when the held item changed this tick</summary>
            public event EventHandler<Item> PlacementItemChanged;

            /// <summary>Event fired at every tick</summary>
            public event EventHandler<UpdateTickedEventArgs> StackUpdateTicked;

            /// <summary>Reference to the stack ID</summary>
            public object stackKey { get; }

            /// <summary>Gets the stack directly below this one.</summary>
            /// <param name="stopAtBlock">If false ignore the block settings and always get the stack below</param>
            /// <returns>The stack below if valid, null if not</returns>
            public IInputToolsAPI.IInputStack GetBelow(bool stopAtBlock = true);

            /// <summary>Gets the most-recently active input debice</summary>
            /// <returns>The input device enum</returns>
            public IInputToolsAPI.InputDevice CurrentInputDevice();

            /// <summary>Checks if a button is pressed this tick. Always false if stack is inactive.</summary>
            /// <param name="button">The button to check</param>
            /// <returns>true if it's pressed this tick</returns>
            public bool IsButtonPressed(SButton button);

            /// <summary>Checks if a button is held since last tick. Always false if stack is inactive.</summary>
            /// <param name="button">The button to check</param>
            /// <returns>true if it's being held since last tick</returns>
            public bool IsButtonHeld(SButton button);

            /// <summary>Checks if a button is released this tick. Always false if stack is inactive.</summary>
            /// <param name="button">The button to check</param>
            /// <returns>true if it's released this tick</returns>
            public bool IsButtonReleased(SButton button);

            /// <summary>Checks if a button pair is pressed this tick. Always false if stack is inactive</summary>
            /// <param name="buttonPair">The button pair</param>
            /// <returns>true if it's pressed this tick</returns>
            public bool IsButtonPairPressed(Tuple<SButton, SButton> buttonPair);

            /// <summary>Checks if a button pair is held since last tick. Always false if stack is inactive</summary>
            /// <param name="buttonPair">The button pair</param>
            /// <returns>true if it's being held since last tick</returns>
            public bool IsButtonPairHeld(Tuple<SButton, SButton> buttonPair);

            /// <summary>Checks if a button pair is released this tick. Always false if stack is inactive</summary>
            /// <param name="buttonPair">The button pair</param>
            /// <returns>true if it's released this tick</returns>
            public bool IsButtonPairReleased(Tuple<SButton, SButton> buttonPair);

            /// <summary>Checks if the mouse wheel is moved this tick. Always false if stack is inactive</summary>
            /// <returns>true if mouse wheel moved this tick</returns>
            public bool IsMouseWheelMoved();

            /// <summary>Checks if the cursor is moved this tick. Always false if stack is inactive</summary>
            /// <param name="mouse">Check mouse movements</param>
            /// <param name="controller">Check controller right stick</param>
            /// <returns>The device movement was from this tick. If both, Controller is returned. If not moved, None.</returns>
            public IInputToolsAPI.InputDevice IsCursorMoved(bool mouse = true, bool controller = true);

            /// <summary>Checks if the currently held item is a bomb. Used for placement tile calculattion.</summary>
            /// <returns>true is a bomb</returns>
            public bool IsHeldItemBomb();

            /// <summary>Checks if the last calculated placement was from the mouse cursor</summary>
            /// <returns>true is cursor, false if it's the farmer i.e. controller movement</returns>
            public bool IsPlacementTileFromCursor();

            /// <summary>Checks if placement tile had changed this tick. Always false if stack is inactive.</summary>
            /// <returns></returns>
            public bool IsPlacementTileChanged();

            /// <summary>Checks if the Confirm action was pressed this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboard"> Checks keyboard inputs</param>
            /// <param name="controller">Checks controller inputs</param>
            /// <returns>The device was pressed from. If both, Controller. If not pressed, None.</returns>
            public IInputToolsAPI.InputDevice IsConfirmPressed(bool keyboard = true, bool controller = true);

            /// <summary>Checks if the Confirm action was held since last tick. Always false if stack is inactive.</summary>
            /// <param name="keyboard"> Checks keyboard Enter</param>
            /// <param name="controller">Checks controller A</param>
            /// <returns>The device was held from. If both, Controller. If not held, None.</returns>
            public IInputToolsAPI.InputDevice IsConfirmHeld(bool keyboard = true, bool controller = true);

            /// <summary>Checks if the Confirm action was released this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboard"> Checks keyboard Enter</param>
            /// <param name="controller">Checks controller A</param>
            /// <returns>The device was released from. If both, Controller. If not released, None.</returns>
            public IInputToolsAPI.InputDevice IsConfirmReleased(bool keyboard = true, bool controller = true);

            /// <summary>Checks if the Cancel action was pressed this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboard"> Checks keyboard Enter</param>
            /// <param name="controller">Checks controller A</param>
            /// <returns>The device was pressed from. If both, Controller. If not pressed, None.</returns>
            public IInputToolsAPI.InputDevice IsCancelPressed(bool keyboard = true, bool controller = true);

            /// <summary>Checks if the Cancel action was held since last tick. Always false if stack is inactive.</summary>
            /// <param name="keyboard"> Checks keyboard Escape</param>
            /// <param name="controller">Checks controller B</param>
            /// <returns>The device was held from. If both, Controller. If not held, None.</returns>
            public IInputToolsAPI.InputDevice IsCancelHeld(bool keyboard = true, bool controller = true);

            /// <summary>Checks if the Cancel action was released this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboard"> Checks keyboard Escape</param>
            /// <param name="controller">Checks controller B</param>
            /// <returns>The device was released from. If both, Controller. If not released, None.</returns>
            public IInputToolsAPI.InputDevice IsCancelReleased(bool keyboard = true, bool controller = true);

            /// <summary>Checks if the Alt action was pressed this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboard"> Checks keyboard Space</param>
            /// <param name="mouse"> Checks mouse right click</param>
            /// <param name="controller">Checks controller X</param>
            /// <returns>The device was pressed from. If both, Controller. If not pressed, None.</returns>
            public IInputToolsAPI.InputDevice IsAltPressed(bool keyboard = true, bool mouse = true, bool controller = true);

            /// <summary>Checks if the Alt action was held since last tick. Always false if stack is inactive.</summary>
            /// <param name="keyboard"> Checks keyboard Space</param>
            /// <param name="mouse"> Checks mouse right click</param>
            /// <param name="controller">Checks controller X</param>
            /// <returns>The device was held from. If both, Controller. If not held, None.</returns>
            public IInputToolsAPI.InputDevice IsAltHeld(bool keyboard = true, bool mouse = true, bool controller = true);

            /// <summary>Checks if the Alt action was released this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboard"> Checks keyboard Space</param>
            /// <param name="mouse"> Checks mouse right click</param>
            /// <param name="controller">Checks controller X</param>
            /// <returns>The device was released from. If both, Controller. If not released, None.</returns>
            public IInputToolsAPI.InputDevice IsAltReleased(bool keyboard = true, bool mouse = true, bool controller = true);

            /// <summary>Checks if the Menu action was pressed this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboard"> Checks keyboard Escape</param>
            /// <param name="controller">Checks controller Y</param>
            /// <returns>The device was pressed from. If both, Controller. If not pressed, None.</returns>
            public IInputToolsAPI.InputDevice IsMenuPressed(bool keyboard = true, bool controller = true);

            /// <summary>Checks if the Menu action was released this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboard"> Checks keyboard Escape</param>
            /// <param name="controller">Checks controller Y</param>
            /// <returns>The device was released from. If both, Controller. If not held, None.</returns>
            public IInputToolsAPI.InputDevice IsMenuHeld(bool keyboard = true, bool controller = true);

            /// <summary>Checks if the Menu action was released this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboard"> Checks keyboard Escape</param>
            /// <param name="controller">Checks controller Y</param>
            /// <returns>The device was released from. If both, Controller. If not released, None.</returns>
            public IInputToolsAPI.InputDevice IsMenuReleased(bool keyboard = true, bool controller = true);

            /// <summary>Checks if any Move action was pressed this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was pressed from. None is not pressed.</returns>
            public IInputToolsAPI.MoveSource IsMoveButtonPressed(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if any Move action was held since last tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was held from. None is not held.</returns>
            public IInputToolsAPI.MoveSource IsMoveButtonHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if any Move action was released this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was released from. None is not released.</returns>
            public IInputToolsAPI.MoveSource IsMoveButtonReleased(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if Move-Right action was pressed this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was pressed from. None is not pressed.</returns>
            public IInputToolsAPI.MoveSource IsMoveRightPressed(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if Move-Right action was held since last tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was held from. None is not held.</returns>
            public IInputToolsAPI.MoveSource IsMoveRightHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if Move-Right action was released this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was released from. None is not released.</returns>
            public IInputToolsAPI.MoveSource IsMoveRightReleased(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if Move-Down action was pressed this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was pressed from. None is not pressed.</returns>
            public IInputToolsAPI.MoveSource IsMoveDownPressed(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if Move-Down action was held since last tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was held from. None is not held.</returns>
            public IInputToolsAPI.MoveSource IsMoveDownHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if Move-Down action was released this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was released from. None is not released.</returns>
            public IInputToolsAPI.MoveSource IsMoveDownReleased(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if Move-Left action was pressed this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was pressed from. None is not pressed.</returns>
            public IInputToolsAPI.MoveSource IsMoveLeftPressed(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if Move-Left action was held since last tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was held from. None is not held.</returns>
            public IInputToolsAPI.MoveSource IsMoveLeftHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if Move-Left action was released this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was released from. None is not released.</returns>
            public IInputToolsAPI.MoveSource IsMoveLeftReleased(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if Move-Up action was pressed this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was pressed from. None is not pressed.</returns>
            public IInputToolsAPI.MoveSource IsMoveUpPressed(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if Move-Up action was held since last tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was held from. None is not held.</returns>
            public IInputToolsAPI.MoveSource IsMoveUpHeld(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if Move-Up action was released this tick. Always false if stack is inactive.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>The input device it was released from. None is not released.</returns>
            public IInputToolsAPI.MoveSource IsMoveUpReleased(bool keyboardWASD = true, bool keyboardArrows = false, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Checks if a custom action was pressed this tick. Always false if stack is inactive.</summary>
            /// <param name="actionID">ID of the registered action</param>
            /// <returns>The button pair that triggered this action. Null if not pressed.</returns>
            public Tuple<SButton, SButton> IsActionPressed(string actionID);

            /// <summary>Checks if a custom action was held since last tick. Always false if stack is inactive.</summary>
            /// <param name="actionID">ID of the registered action</param>
            /// <returns>The button pair that triggered this action. Null if not held.</returns>
            public Tuple<SButton, SButton> IsActionHeld(string actionID);

            /// <summary>Checks if a custom action was released this tick. Always false if stack is inactive.</summary>
            /// <param name="actionID">ID of the registered action</param>
            /// <returns>The button pair that triggered this action. Null if not released.</returns>
            public Tuple<SButton, SButton> IsActionReleased(string actionID);

            /// <summary>Gets the Move action axis values. Note that it's not sensitive to thumbstick distance/velocity.</summary>
            /// <param name="keyboardWASD">Checks keyboard WASD</param>
            /// <param name="keyboardArrows">Checks keyboard arrow keys</param>
            /// <param name="controllerDPad">Checks controller DPad</param>
            /// <param name="controllerThumbstick">Checks controller left thumbstick</param>
            /// <returns>X and Y are either -1 (left, up), 0 (center), or +1 (right, down).</returns>
            public Vector2 GetMoveAxis(bool keyboardWASD = true, bool keyboardArrows = true, bool controllerDPad = true, bool controllerThumbstick = true);

            /// <summary>Gets the unscaled screen pixels position of the cursor</summary>
            /// <returns>Unscaled screen pixels position</returns>
            public Vector2 GetCursorScreenPos();

            /// <summary>Gets the tile position under the cursor</summary>
            /// <returns>X and Y in integers</returns>
            public Vector2 GetCursorTilePos();

            /// <summary>Gets the mouse wheel position</summary>
            /// <returns>X is the horizontal scroll and Y the vertical scroll in integer since game start</returns>
            public Vector2 GetMouseWheelPos();

            /// <summary>Gets the tile position of where a held item will be placed. Automatically corrected if controller is used.</summary>
            /// <returns>X and Y in integers</returns>
            public Vector2 GetPlacementTile();

            /// <summary>Gets the tile position of where a held item will be placed assuming a controller is used.</summary>
            /// <returns>X and Y in integers</returns>
            public Vector2 GetPlacementTileWithController();

            /// <summary>Sets this Input Stack active to receive inputs, or inactive to ignore inputs.</summary>
            /// <param name="active">True if active</param>
            public void SetStackActive(bool active);

            /// <summary>Sets this Input Stack's block behaviour to determine whether to pass inputs to the stack below.</summary>
            /// <param name="stackBlockBehaviour">Block stops inputs from continuing, PassBelow allows inputs to continue downwards.</param>
            public void SetStackDefaultBlockBehaviour(IInputToolsAPI.StackBlockBehavior stackBlockBehaviour);

            /// <summary>Moves this Input Stack to the top (but still below the Global stack) so that it can process inputs first.</summary>
            public void MoveToTopOfStack();

            /// <summary>Checks if this Input Stack is reachable or blocked by a stack above. Also automatically false if inactive.</summary>
            /// <returns>True if stack is reachable and active i.e. processing inputs.</returns>
            public bool IsStackReachableByInput();

            /// <summary>Deletes this Input Stack.</summary>
            public void RemoveSelf();
        }
    }
}
