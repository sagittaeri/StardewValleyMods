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

        internal Dictionary<object, InputLayer> layerDict = new Dictionary<object, InputLayer>();
        internal List<object> layers = new List<object>();

        public ControlStack(InputToolsAPI inputTools)
        {
            this.inputTools = inputTools;
        }

        public InputLayer LayerCreate(object layerKey, bool startActive = true, IInputToolsAPI.BlockBehavior blockBehaviour = IInputToolsAPI.BlockBehavior.Block)
        {
            if (layerKey == null)
            {
                this.inputTools.Monitor.Log($"Layer key required to create an input layer", LogLevel.Warn);
                return null;
            }
            if (this.layers.Contains(layerKey))
                this.inputTools.Monitor.Log($"Layer {layerKey} is being created more than once - remove it first if it's intentional", LogLevel.Warn);
            this.layerDict[layerKey] = new InputLayer(this.inputTools, layerKey) { isActive = startActive, blockBehaviour = blockBehaviour };
            this.layers.Add(layerKey);
            return this.layerDict[layerKey];
        }

        public void LayerRemove(object layerKey)
        {
            if (layerKey == null)
                return;
            if (this.layers.Contains(layerKey))
                this.layers.Remove(layerKey);
            else
                this.inputTools.Monitor.Log($"Tried to remove layer {layerKey} that hasn't been created", LogLevel.Warn);
            if (this.layerDict.ContainsKey(layerKey))
                this.layerDict.Remove(layerKey);
        }

        public IInputToolsAPI.IInputLayer GetLayer(object layerKey)
        {
            if (layerKey == null)
                return this.inputTools.Global as InputLayer;
            if (this.layerDict.ContainsKey(layerKey))
                return this.layerDict[layerKey];
            return null;
        }

        public void MoveToTopOfStack(object layerKey)
        {
            if (layerKey == null)
                return;
            if (!this.layers.Contains(layerKey))
                return;
            this.layers.Remove(layerKey);
            this.layers.Add(layerKey);
        }

        public bool IsLayerReachableByInput(object layerKey)
        {
            InputLayer layer = this.GetLayer(layerKey) as InputLayer;
            if (layer == null || !layer.isActive)
                return false;
            if (layerKey == null)
                return layer.isActive;
            if (layerKey != null && this.inputTools._Global.blockBehaviour == IInputToolsAPI.BlockBehavior.Block)
                return false;

            for (int i=this.layers.Count-1; i>=0; i--)
            {
                if (layerKey == this.layers[i])
                    return true;
                InputLayer layerI = this.GetLayer(this.layers[i]) as InputLayer;
                if (layerI.blockBehaviour == IInputToolsAPI.BlockBehavior.Block)
                    break;
            }
            return false;
        }
    }
}
