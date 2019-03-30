using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace SearchChests
{
    internal class SearchTextBox : ChatTextBox
    {
        internal String searchText = "";

        public SearchTextBox(Texture2D textBoxTexture, Texture2D caretTexture,
                             SpriteFont font, Color textColor)
            : base(textBoxTexture, caretTexture, font, textColor)
        {
        }

        private void UpdateSearchText()
        {
            if (finalText.Count == 0)
                return;

            searchText = finalText.Last().message;
        }

        private void UpdateChestSearch()
        {
            ModEntry.chestSearcher.SearchChestsInPlayerLocation(searchText);
        }

        internal void ResetSearch()
        {
            setText("");
            UpdateSearchText();
            UpdateChestSearch();
        }

        public override void RecieveTextInput(char c)
        {
            base.RecieveTextInput(c);
            UpdateSearchText();
            UpdateChestSearch();
        }

        public override void RecieveCommandInput(char c)
        {
            base.RecieveCommandInput(c);
            UpdateSearchText();
            UpdateChestSearch();
        }
    }

    internal class SearchBox : IClickableMenu
    {
        private SearchTextBox searchBox;

        public SearchBox()
        {
            Texture2D chatBoxTexture = Game1.content.Load<Texture2D>("LooseSprites\\chatBox");
            searchBox = new SearchTextBox(chatBoxTexture, null, Game1.smallFont, Color.White);

            searchBox.Width = 300;
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

        internal String SearchText
        {
            get
            {
                return searchBox.searchText;
            }
        }

        internal void ResetSearch()
        {
            searchBox.ResetSearch();
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
            if (key == Keys.Enter) {
                exitThisMenu(true);
            }

            if (key == Keys.A && Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                searchBox.ResetSearch();
        }

        public void Activate()
        {
            searchBox.Selected = true;
            searchBox.setText(searchBox.searchText);
        }
    }
}
