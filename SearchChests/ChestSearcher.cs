using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Menus;

namespace SearchChests
{
    public class ChestSearcher {
        private Color containsItemColor = new Color(1, 1, 1, 255);
        private List<Tuple<Chest, Color>> oldChestTints =
            new List<Tuple<Chest, Color>>();

        private bool ItemNamesMatch(String itemDisplayName, String itemSearchedFor)
        {
            if (itemSearchedFor == "")
                return false;

            return itemDisplayName.Contains(itemSearchedFor);
        }

        internal void ResetChestTints()
        {
            foreach (var t in oldChestTints)
            {
                if (t.Item1.playerChoiceColor.Value != containsItemColor)
                    continue;

                t.Item1.playerChoiceColor.Value = t.Item2;
            }

            oldChestTints.Clear();
        }

        internal void CleanUp()
        {
            ResetChestTints();
        }

        internal List<Chest> GetChestsInLocation(GameLocation location)
        {
            List<Chest> chestsInLocation = new List<Chest>();

            foreach (var obj in location.Objects.Values)
            {
                if (obj is Chest)
                {
                    chestsInLocation.Add((Chest) obj);
                }
            }

            return chestsInLocation;
        }

        internal List<Chest> GetChestsInPlayerLocation()
        {
            return GetChestsInLocation(Game1.currentLocation);
        }

        internal void SearchChestsInLocation(String itemSearchedFor,
                                             GameLocation location)
        {
            ResetChestTints();

            List<Chest> chestsInLocation = GetChestsInLocation(location);
            foreach (Chest chest in chestsInLocation)
            {
                foreach (Item item in chest.items)
                {
                    String itemDisplayName = item.DisplayName.ToLower();
                    itemSearchedFor = itemSearchedFor.ToLower();

                    if (ItemNamesMatch(itemDisplayName, itemSearchedFor))
                    {
                        Color oldTint = chest.playerChoiceColor.Value;
                        oldChestTints.Add(Tuple.Create(chest, oldTint));
                        chest.playerChoiceColor.Value = containsItemColor;
                    }
                }
            }
        }

        internal void SearchChestsInPlayerLocation(String itemSearchedFor)
        {
            ResetChestTints();
            SearchChestsInLocation(itemSearchedFor, Game1.currentLocation);
        }

        internal void SearchChest(IClickableMenu chestMenu, String itemSearchedFor)
        {
            if (!(chestMenu is ItemGrabMenu))
                return;

            InventoryMenu chestInventory = ((ItemGrabMenu) chestMenu).ItemsToGrabMenu;
            foreach (var chestItem in chestInventory.inventory)
            {
                Item actualItem = chestInventory.getItemFromClickableComponent(chestItem);
                if (actualItem == null)
                    continue;

                String itemDisplayName = actualItem.DisplayName.ToLower();
                itemSearchedFor = itemSearchedFor.ToLower();
                if (ItemNamesMatch(itemDisplayName, itemSearchedFor))
                    chestItem.scale = 1.2f;
            }
        }
    }
}
