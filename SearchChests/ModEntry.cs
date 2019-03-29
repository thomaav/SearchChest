using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Menus;

namespace SearchChests
{
    internal class SearchTextBox : ChatTextBox
    {
        public SearchTextBox(Texture2D textBoxTexture, Texture2D caretTexture,
                             SpriteFont font, Color textColor)
            : base(textBoxTexture, caretTexture, font, textColor)
        {
        }
    }

    internal class SearchBox : IClickableMenu
    {
        private SearchTextBox searchBox;

        public SearchBox()
        {
            Texture2D chatBoxTexture = Game1.content.Load<Texture2D>("LooseSprites\\chatBox");
            searchBox = new SearchTextBox(chatBoxTexture, null, Game1.smallFont, Color.White);

            searchBox.Width = 896;
            searchBox.Height = 56;
            base.width = searchBox.Width;
            base.height = searchBox.Height;

            base.xPositionOnScreen = 0;
            base.yPositionOnScreen = 0;
            Utility.makeSafe(ref base.xPositionOnScreen, ref base.yPositionOnScreen,
                             searchBox.Width, searchBox.Height);
            searchBox.X = base.xPositionOnScreen;
            searchBox.Y = base.yPositionOnScreen;

            searchBox.Selected = false;
        }

        public override void draw(SpriteBatch b)
        {
            searchBox.Draw(b, false);
        }

        public override void clickAway()
        {
            base.clickAway();
            searchBox.Selected = false;
        }

        public override void receiveKeyPress(Keys key)
        {
            if (key == Keys.Enter)
                exitThisMenu(true);
        }

        public void Activate()
        {
            searchBox.Selected = true;
            searchBox.setText("");
        }
    }

    public class ModEntry : Mod
    {
        internal static IModHelper StaticHelper  { get; private set; }
        internal static IMonitor   StaticMonitor { get; private set; }

        private SearchBox searchBox;
        private ChestSearcher chestSearcher;

        public override void Entry(IModHelper helper)
        {
            StaticHelper = Helper;
            StaticMonitor = Monitor;

            SetupSearchBox();
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

        private void SetupSearchBox()
        {
            searchBox = new SearchBox();
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
            if (!Context.IsWorldReady)
                return;

            if (e.Button == Keys.Enter.ToSButton()) {
                SearchChests();
            }

            if (e.Button == Keys.S.ToSButton() && e.IsDown(SButton.LeftControl)) {
                searchBox.Activate();
                Game1.activeClickableMenu = searchBox;
            }
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

