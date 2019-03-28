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

        private static ObjectReflectionExposer objectExposer;
        private static GameObjectExposer gameExposer;

        private String lastChatMessage;
        private List<Tuple<Chest, Color>> oldChestTints =
            new List<Tuple<Chest, Color>>();

        public override void Entry(IModHelper helper)
        {
            StaticHelper = Helper;
            StaticMonitor = Monitor;

            objectExposer = new ObjectReflectionExposer();
            gameExposer = new GameObjectExposer();

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        internal static void Log(dynamic val)
        {
            StaticMonitor.Log($"{val}", LogLevel.Warn);
        }

        internal static void OutputFields<T>(T obj)
        {
            FieldInfo[] fields = objectExposer.GetAllFields(obj);

            foreach (FieldInfo field in fields)
            {
                Log(field);
            }
        }

        private void resetChestTints()
        {
            foreach (var t in oldChestTints)
            {
                t.Item1.Tint = t.Item2;
            }

            oldChestTints.Clear();
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // We really just want to check chests whenever the user
            // presses enter, which has a possibility of being to send
            // a chat message.
            if (e.Button != Keys.Enter.ToSButton())
                return;

            // (TODO): Change this to just get a single message in the
            // exposer instead, we don't need all messages.
            List<String> chatMessages = gameExposer.GetAllChatMessages();
            if (chatMessages.Count == 0)
                return;

            String newestMessage = chatMessages[chatMessages.Count - 1];
            if (newestMessage != this.lastChatMessage)
            {
                this.lastChatMessage = newestMessage;
            }

            if (this.lastChatMessage == "unset")
            {
                resetChestTints();
                return;
            }

            var playerLocation = gameExposer.GetPlayerLocation();
            List<Chest> chests = new List<Chest>();
            foreach (var obj in playerLocation.objects.Values)
            {
                if (obj is Chest)
                {
                    chests.Add((Chest) obj);
                }
            }

            foreach (Chest chest in chests)
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

