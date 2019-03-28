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
    public class ChestSearcher {
        private String lastChatMessage;
        private List<Tuple<Chest, Color>> oldChestTints =
            new List<Tuple<Chest, Color>>();

        internal void ResetChestTints()
        {
            foreach (var t in oldChestTints)
            {
                t.Item1.Tint = t.Item2;
            }

            oldChestTints.Clear();
        }

        internal void CleanUp()
        {
            ResetChestTints();
        }

        private void UpdateLastChatMessage()
        {
            String _lastChatMessage = ChatExposer.GetLastChatMessage();
            if (_lastChatMessage == null)
                return;

            if (_lastChatMessage != this.lastChatMessage)
                this.lastChatMessage = _lastChatMessage;
        }

        internal void SearchChestsInPlayerLocation()
        {
            ResetChestTints();
            UpdateLastChatMessage();

            if (this.lastChatMessage == "unset")
                return;

            var playerLocation = GameObjectExposer.GetPlayerLocation();
            List<Chest> chestsInLocation = new List<Chest>();

            foreach (var obj in playerLocation.objects.Values)
            {
                if (obj is Chest)
                {
                    chestsInLocation.Add((Chest) obj);
                }
            }

            foreach (Chest chest in chestsInLocation)
            {
                foreach (Item item in chest.items)
                {
                    String itemDisplayName = item.DisplayName.ToLower();
                    String itemSearchedFor = this.lastChatMessage.ToLower();

                    if (itemDisplayName == itemSearchedFor)
                    {
                        Color oldTint = chest.tint;
                        oldChestTints.Add(Tuple.Create(chest, oldTint));
                        chest.Tint = new Color(0, 0, 0, 255);
                    }
                }
            }
        }
    }
}
