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
* Direct methods for multi-device Confirm, Cancel, Alt, Menu, MoveRight, MoveDown, MoveLeft and MoveUp input actions, including events if desired
* A way define and use custom input actions (e.g. Jump, NavigateDown, CustomConfirm, etc) and assign it to mouse, keyboard and gamepad effortlessy
* Check which input device was most recently used and get event updates whenever it changes
* Get a custom keybinding from the user, both single-button and button-pair
* Corrected location for item placement tile when gamepad is used
* Group input event handling and per-tick polling into an "Input Stack" which can be turned off and on
* In more complex systems, create multiple Input Stacks and dictate their behaviour e.g. whether or not to allow input from an Input Stack above to continue to the Input Stack below

For the full API, see [`IInputToolsAPI.cs`](https://github.com/sagittaeri/StardewValleyMods/blob/main/InputTools/IInputToolsAPI.cs).

For general information on how to use another mod's API in your mod,
see the [Mod Integration](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Integrations)
page on the Stardew Valley Wiki.
