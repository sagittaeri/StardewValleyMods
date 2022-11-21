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
        private InputToolsAPI inputTools;

        internal Dictionary<object, InputStack> stacksDict = new Dictionary<object, InputStack>();
        internal List<object> stacks = new List<object>();

        public ControlStack(InputToolsAPI inputTools)
        {
            this.inputTools = inputTools;
        }

        public InputStack StackCreate(object stackKey, bool startActive = true, IInputToolsAPI.StackBlockBehavior defaultBlockBehaviour = IInputToolsAPI.StackBlockBehavior.Block)
        {
            if (stackKey == null)
            {
                this.inputTools.Monitor.Log($"Stack key required to create an input stack", LogLevel.Warn);
                return null;
            }
            if (this.stacks.Contains(stackKey))
                this.inputTools.Monitor.Log($"Stack {stackKey} is being created more than once - remove it first if it's intentional", LogLevel.Warn);
            this.stacksDict[stackKey] = new InputStack(this.inputTools, stackKey) { isActive = startActive, blockBehaviour = defaultBlockBehaviour };
            this.stacks.Add(stackKey);
            return this.stacksDict[stackKey];
        }

        public void StackRemove(object stackKey)
        {
            if (stackKey == null)
                return;
            if (this.stacks.Contains(stackKey))
                this.stacks.Remove(stackKey);
            else
                this.inputTools.Monitor.Log($"Tried to remove stack {stackKey} that hasn't been created", LogLevel.Warn);
            if (this.stacksDict.ContainsKey(stackKey))
                this.stacksDict.Remove(stackKey);
        }

        public IInputToolsAPI.IInputStack GetStack(object stackKey)
        {
            if (stackKey == null)
                return this.inputTools.Global as InputStack;
            if (this.stacksDict.ContainsKey(stackKey))
                return this.stacksDict[stackKey];
            return null;
        }

        public void MoveToTopOfStack(object stackKey)
        {
            if (stackKey == null)
                return;
            if (!this.stacks.Contains(stackKey))
                return;
            this.stacks.Remove(stackKey);
            this.stacks.Add(stackKey);
        }

        public bool IsStackReachableByInput(object stackKey)
        {
            InputStack stack = this.GetStack(stackKey) as InputStack;
            if (stack == null || !stack.isActive)
                return false;
            if (stackKey == null)
                return stack.isActive;
            if (stackKey != null && this.inputTools._Global.blockBehaviour == IInputToolsAPI.StackBlockBehavior.Block)
                return false;

            for (int i=this.stacks.Count-1; i>=0; i--)
            {
                if (stackKey == this.stacks[i])
                    return true;
                InputStack stackI = this.GetStack(this.stacks[i]) as InputStack;
                if (stackI.blockBehaviour == IInputToolsAPI.StackBlockBehavior.Block)
                    break;
            }
            return false;
        }
    }
}
