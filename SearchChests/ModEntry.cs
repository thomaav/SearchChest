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
using StardewValley.Menus;

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

            helper.Events.GameLoop.Saving += this.OnSaveResetSearch;
            helper.Events.Input.ButtonPressed += this.OnButtonPressedSearch;
            helper.Events.Display.MenuChanged += this.OnMenuChangedSearch;
        }

        internal static void Log(dynamic val)
        {
            StaticMonitor.Log($"{val}", LogLevel.Warn);
        }

        private void OnSaveResetSearch(object sender, SavingEventArgs e)
        {
            chestSearcher.CleanUp();
        }

        private void SearchChests()
        {
            if (!Context.IsWorldReady)
                return;

            chestSearcher.SearchChestsInPlayerLocation();
        }

        private void SearchChest(IClickableMenu chestMenu)
        {
            if (!Context.IsWorldReady)
                return;

            chestSearcher.SearchChest(chestMenu);
        }

        private void OnButtonPressedSearch(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button != Keys.Enter.ToSButton())
                return;

            SearchChests();
        }

        private void OnMenuChangedSearch(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu != null)
                SearchChest((IClickableMenu) e.NewMenu);

            if (e.OldMenu != null)
                SearchChests();
        }
    }
}

