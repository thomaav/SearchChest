using System;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SearchChests
{
    public class ModEntry : Mod
    {
        internal static IModHelper StaticHelper  { get; private set; }
        internal static IMonitor   StaticMonitor { get; private set; }

        private ModConfig Config;

        private SearchBox searchBox;
        internal static ChestSearcher chestSearcher;

        public override void Entry(IModHelper helper)
        {
            StaticHelper = Helper;
            StaticMonitor = Monitor;

            Config = this.Helper.ReadConfig<ModConfig>();

            searchBox = new SearchBox();
            chestSearcher = new ChestSearcher();

            helper.Events.GameLoop.Saving += this.OnSaveResetSearch;
            helper.Events.Input.ButtonPressed += this.OnButtonPressedSearch;
            helper.Events.Display.MenuChanged += this.OnMenuChangedSearch;
            helper.Events.Player.Warped += this.OnWarpSearch;
        }

        internal static void Log(dynamic val)
        {
            StaticMonitor.Log($"{val}", LogLevel.Warn);
        }

        private void OnSaveResetSearch(object sender, SavingEventArgs e)
        {
            chestSearcher.CleanUp();
            searchBox.ResetSearch();
        }

        private void OnButtonPressedSearch(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == Config.SearchKey.ToSButton()
                && (!Config.UseControlModifier || e.IsDown(SButton.LeftControl))) {
                if (Game1.currentLocation.currentEvent != null)
                    return;

                if (!e.IsDown(SButton.LeftShift))
                    searchBox.ResetSearch();

                searchBox.Activate();
                Game1.activeClickableMenu = searchBox;
            }
        }

        private void OnMenuChangedSearch(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.NewMenu != null)
                chestSearcher.SearchChest((IClickableMenu) e.NewMenu, searchBox.SearchText);

            if (e.OldMenu != null)
                chestSearcher.SearchChestsInPlayerLocation(searchBox.SearchText);
        }

        private void OnWarpSearch(object sender, WarpedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            chestSearcher.SearchChestsInPlayerLocation(searchBox.SearchText);
        }
    }
}

