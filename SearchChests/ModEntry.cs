using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;

namespace SearchChests
{
    public class ModEntry : Mod
    {
        internal static IModHelper StaticHelper  { get; private set; }
        internal static IMonitor   StaticMonitor { get; private set; }

        private ChestSearcher chestSearcher;

        public override void Entry(IModHelper helper)
        {
            StaticHelper = Helper;
            StaticMonitor = Monitor;

            chestSearcher = new ChestSearcher();

            helper.Events.Input.ButtonPressed += this.OnButtonPressedSearch;
            helper.Events.GameLoop.Saving += this.OnSaveResetSearch;
        }

        internal static void Log(dynamic val)
        {
            StaticMonitor.Log($"{val}", LogLevel.Warn);
        }

        private void OnSaveResetSearch(object sender, SavingEventArgs e)
        {
            chestSearcher.CleanUp();
        }

        private void OnButtonPressedSearch(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button != Keys.Enter.ToSButton())
                return;

            chestSearcher.SearchChestsInPlayerLocation();
        }
    }
}

