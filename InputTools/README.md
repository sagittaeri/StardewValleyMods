# Stardew Valley Input Tools

A Stardew Valley SMAPI mod to provide an array of tools to make it easier to
handle inputs across mouse, keyboard and controller.

## For mod users

This mod is mostly meant for other modders as a modding tool, but it does do one
thing for users:

* Immediately force hide the mouse cursor when controller is used

### Compatibility

* Works with Stardew Valley 1.5 on Linux/Mac/Windows.
* Works with keyboard, mouse, and most gamepads
* Works in single player and multiplayer.
* No known incompatibilities.

### Installation

Follow the usual installation proceedure for SMAPI mods:
1. Install [SMAPI](https://smapi.io)
2. Download the latest realease of this mod and unzip it into the `Mods` directory
3. Run the game using SMAPI

## For mod developers

This mod provides an API for the following:
* Allow both event handlers (`ButtonPairPressed += MyDelegate`) and per tick polling (`IsButtonPairPressed(CtrlSpace)`) to check if a button pair (e.g. ctrl+space) is Pressed, Held or Released
* Direct methods and event handlers for multi-device Confirm, Cancel, Alt, Menu, MoveRight, MoveDown, MoveLeft and MoveUp input actions
* A way define and use custom input actions (e.g. Jump, NavigateDown, CustomConfirm, etc) and assign it to mouse, keyboard and gamepad effortlessy
* Check which input device was most recently used and get event updates whenever it changes
* Get a custom keybinding from the user, both single-button and button-pair
* Corrected location for item placement tile when gamepad is used
* Group input event handling and per-tick polling into an "Input Stack" which can be turned off and on
* In more complex systems, create multiple Input Stacks and dictate their behaviour e.g. whether or not to allow input from an Input Stack above to continue to the Input Stack below

For the full API, see [`IInputToolsAPI.cs`](https://github.com/sagittaeri/StardewValleyMods/blob/main/InputTools/IInputToolsAPI.cs).

### Concepts
#### Button Pairs
Button pairs are when two buttons are pushed at the same time and are stored as `Tuple<SButton, SButton>`. For example:
```cs
// Define ctrl+space
Tuple<SButton, SButton> ctrlSpace = new Tuple<SButton, SButton>(SButton.LeftControl, SButton.Space);

this.InputToolsAPI.Global.ButtonPairPressed +=  new EventHandler<Tuple<SButton, SButton>>((s, e) =>
{
    // This line is entered whenever any button pair is pressed
    if (e == ctrlSpace)
    {
        // ctrl+space is pressed - do something cool
    }
});
```
Note that this defines `Ctrl+Space` and not `Space+Ctrl` - the order in the tuple has to match the keystroke.

#### Actions
Actions are events that are triggered by an input or a set of inputs. For example, the Jump action could be triggered by "Space" on the keyboard and "X" on the gamepad and so on. A good way to implement cross-device support to your mod is to use Actions. For example:
```cs
// Add custom action "Jump"
this.InputToolsAPI.RegisterAction("Jump", SButton.Space, SButton.ControllerX);
this.InputToolsAPI.RegisterAction("Jump", new Tuple<SButton, SButton>(SButton.MouseLeft, SButton.MouseRight));

this.InputToolsAPI.Global.ActionPressed +=  new EventHandler<string>((s, e) =>
{
    // This line is entered whenever any custom action is pressed
    if (e == "Jump")
    {
        // Do jump!
    }
});
```
This says `Jump` is triggered in three ways; (1) `Space` on keyboard; (2) `X` on gamepad; and (3) `Left+Right` mouse clicks. Then it uses an event handler to listen for all custom action input events.

#### Input Stacks
This is for advanced usage of Input Tools. If you're only using it for small stuff, you can just use the default stack `InputToolsAPI.Global` to do all your input work.

An Input Stack is an object which contains all the input events and per-tick polling methods. You can set a stack to `inactive` to pause input handling which can be useful especially if you require multiple different sets of input handling, many of them conflicting. For example, the Escape button can have different uses depending on the state of the game, so you can create an Input Stack for each state for input handling.

Additionally, the reason why they are called "Stacks" is because they are like UI popups: when they are created, they are added on top of the stack. Each stack can determine whether or not to allow input events to pass down to the stack below, which is useful for UI popups where you don't want the Escape button to close all the popups at once, just the one on top.

An example:

```cs
// Define an Input Stack for the initial state
IInputToolsAPI.IInputStack normalStack = this.InputToolsAPI.StackCreate("Normal");
normalStack.ButtonPressed += new EventHandler<SButton>((s, e) =>
{
    // In this context, s = "Normal", and e is an SButton enum
    if (e == SButton.Escape)
    {
        // Perform action in normal state
    }
});
normalStack.StackUpdateTicked += new EventHandler<UpdateTickedEventArgs>((s, e) =>
{
    // This is like a normal UpdateTicked event, except it's specific for this stack
    if (normalStack.IsButtonPairPressed(new Tuple<SButton, SButton>(SButton.LeftShift, SButton.Enter)))
    {
        // When LShift+Enter is pressed, switch to the Popup stack
        IInputToolsAPI.GetStack("Popup").SetStackDefaultBlockBehaviour(IInputToolsAPI.StackBlockBehavior.Block);
        IInputToolsAPI.GetStack("Popup").SetStackActive(true);

        // While Popup stack is active and blocking, Normal stack will no longer receive button events
        // Additionally, normalStack.IsButtonPairPressed() etc will always return false since the Popup stack
        // above is preventing input events from reaching Normal
    }
};

// Define the Input Stack for the popup. Note that stacks are always created on top of the existing stacks.
// The default stack, InputToolsAPI.Global, is the exception, which is always above all the other stacks,
// which means if InputToolsAPI.Global is set to Block, no other stack will receive input events
IInputToolsAPI.IInputStack popupStack = this.InputToolsAPI.StackCreate("Popup");
popupStack.ButtonPressed += new EventHandler<SButton>((s, e) =>
{
    // In this context, s = "Popup", and e is an SButton enum
    if (e == SButton.Escape)
    {
        // Deactivate this stack and allow inputs to pass below to Normal stack again
        IInputToolsAPI.GetStack(s).SetStackDefaultBlockBehaviour(IInputToolsAPI.StackBlockBehavior.PassBelow);
        IInputToolsAPI.GetStack(s).SetStackActive(false);
    }
});

// By default, created stacks are active and will block inputs, so turn them off and allow inputs to pass down
popupStack.SetStackDefaultBlockBehaviour(IInputToolsAPI.StackBlockBehavior.PassBelow);
popupStack.SetStackActive(false);

```

### Mod Integration

To start integrating this mod into your own, you'll first need to define the interface of the whole API or just functions you wish to use. An example for a simple interface with only one function:
```cs
public interface IInputToolsAPI
{
    public IInputStack Global { get; }
    public interface IInputStack
    {
        public Vector2 GetPlacementTile();
    }
}
```
Note: `IInputStack Global` object is the default Input Stack, and for most use-cases you'll need it.

If you want to implement the entire API instead, just copy this file onto your project: [`IInputToolsAPI.cs`](https://github.com/sagittaeri/StardewValleyMods/blob/main/InputTools/IInputToolsAPI.cs)

* *Pros of implementing just the functions you need:* less chances of the integration breaking due to API changes.
* *Pros of implementing the whole API:* Intellisense can help you discover functions you need easier.

The following method inside your `ModEntry.cs` is recommended for a soft integration:
```cs
private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
{
    try
    {
        this.InputToolsAPI = this.Helper.ModRegistry.GetApi<InputTools.IInputToolsAPI>("Sagittaeri.InputTools");
    }
    catch (Exception exception)
    {
        this.Monitor.Log($"Failed to load Sagittaeri.InputTools. Reason: {exception.Message}", LogLevel.Error);
    }
    if (this.InputToolsAPI != null)
    {
        this.Monitor.Log("Loaded Sagittaeri.InputTools successfully - controller will be supported", LogLevel.Debug);
    }
}
```
This will allow any integration issues to fail gracefully, and you only need to check that `InputToolsAPI != null` before using the tool.

For general information on how to use another mod's API in your mod,
see the [Mod Integration](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Integrations)
page on the Stardew Valley Wiki.
