
# Release Notes

## Version 0.2.0

Warning: API-breaking changes from previous version. Be sure to copy the new interface file. I don't plan to break API frequently,
but since this is the 2nd release after getting a bunch of great feedback from the initial release, I figured it's better I do it
now than layer when there are more users.

- API change: Default actions (Confirm, MoveX, etc) to use Stardew Valley keybinding options where possible
- API change: Renamed InputStack to InputLayer (and thus most references to Stack is now Layer) which is more closely reflected to what it is, which should eliminate some confusion
- New function: "LayerPop" to make it clearer that the input layers are in a stack
- New function: "GetTextFromVirtualKeyboard" which works with keyboard, controller and mouse
- New function: "RightStickSnapToTile" to snap the cursor to tile when moved with right stick

## Version 0.1.0

Initial release
