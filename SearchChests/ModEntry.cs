using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace SearchChests
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            this.Monitor.Log("A warning message in the entrypoint.", LogLevel.Warn);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            this.Monitor.Log("A warning message.", LogLevel.Warn);
        }
    }
}

