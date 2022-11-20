using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using InputTools;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;


namespace InputTools
{
    public class ControlStack
    {
        private ModEntry modEntry;

        internal Dictionary<object, InputToolsAPI.InputStack> stacksDict = new Dictionary<object, InputToolsAPI.InputStack>();
        internal List<object> stacks = new List<object>();

        public ControlStack(ModEntry modEntry)
        {
            this.modEntry = modEntry;
        }

        public InputToolsAPI.InputStack StackCreate(object stackKey, bool startActive = true, IInputToolsAPI.StackBlockBehavior defaultBlockBehaviour = IInputToolsAPI.StackBlockBehavior.Block)
        {
            if (stackKey == null)
            {
                this.modEntry.Monitor.Log($"Stack key required to create an input stack", LogLevel.Warn);
                return null;
            }
            if (this.stacks.Contains(stackKey))
                this.modEntry.Monitor.Log($"Stack {stackKey} is being created more than once - remove it first if it's intentional", LogLevel.Warn);
            this.stacksDict[stackKey] = new InputToolsAPI.InputStack(this.modEntry, stackKey) { isActive = startActive, blockBehaviour = defaultBlockBehaviour };
            this.stacks.Add(stackKey);
            return this.stacksDict[stackKey];
        }

        public void StackRemove(object stackKey)
        {
            if (this.stacks.Contains(stackKey))
                this.stacks.Remove(stackKey);
            else
                this.modEntry.Monitor.Log($"Tried to remove stack {stackKey} that hasn't been created", LogLevel.Warn);
            if (this.stacksDict.ContainsKey(stackKey))
                this.stacksDict.Remove(stackKey);
        }

        public InputToolsAPI.InputStack GetStack(object stackKey)
        {
            if (this.stacksDict.ContainsKey(stackKey))
                return this.stacksDict[stackKey];
            return null;
        }

        public void MoveToTopOfStack(object stackKey)
        {
            if (!this.stacks.Contains(stackKey))
                return;
            this.stacks.Remove(stackKey);
            this.stacks.Add(stackKey);
        }

        public bool IsStackReachableByInput(object stackKey)
        {
            if (stackKey == null)
                return false;
            InputToolsAPI.InputStack stack = this.GetStack(stackKey);
            if (stack == null || !stack.isActive)
                return false;

            for (int i=this.stacks.Count-1; i>=0; i--)
            {
                if (stackKey == this.stacks[i])
                    return true;
                InputToolsAPI.InputStack stackI = this.GetStack(this.stacks[i]);
                if (stackI.isActive && stackI.blockBehaviour == IInputToolsAPI.StackBlockBehavior.Block)
                    continue;
            }
            return false;
        }
    }
}
