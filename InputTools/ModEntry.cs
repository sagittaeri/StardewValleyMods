using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using InputTools;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace InputTools
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public Dictionary<string, InputToolsAPI> inputTools = new Dictionary<string, InputToolsAPI>();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
        }

        public override object GetApi(IModInfo mod)
        {
            this.inputTools[mod.Manifest.UniqueID] = new InputToolsAPI(this);
            this.Helper.Events.Input.ButtonPressed += this.inputTools[mod.Manifest.UniqueID].OnButtonPressed;
            this.Helper.Events.Input.ButtonReleased += this.inputTools[mod.Manifest.UniqueID].OnButtonReleased;
            this.Helper.Events.GameLoop.UpdateTicked += this.inputTools[mod.Manifest.UniqueID].OnUpdateTicked;
            return this.inputTools[mod.Manifest.UniqueID];
        }

        public List<string> GetListOfModIDs()
        {
            return new List<string>(this.inputTools.Keys);
        }

        public InputToolsAPI GetInstanceFromAnotherMod(string uniqueModID)
        {
            if (this.inputTools.ContainsKey(uniqueModID))
               return this.inputTools[uniqueModID];
            return null;
        }
    }
}
