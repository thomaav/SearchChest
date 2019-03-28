using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace SearchChests
{
    public class ObjectReflectionExposer
    {
        private IModHelper helper;

        public ObjectReflectionExposer(IModHelper helper)
        {
            this.helper = helper;
        }

        public FieldInfo[] GetPrivateFields<T>(T obj)
        {
            return obj.GetType().GetFields(BindingFlags.NonPublic |
                                           BindingFlags.Instance);
        }

        public FieldInfo[] GetPublicFields<T>(T obj)
        {
            return obj.GetType().GetFields(BindingFlags.Public |
                                           BindingFlags.Instance |
                                           BindingFlags.DeclaredOnly);
        }

        public FieldInfo[] GetAllFields<T>(T obj)
        {
            var privateFields = GetPrivateFields(obj);
            var publicFields = GetPublicFields(obj);

            var allFields = new FieldInfo[privateFields.Length + publicFields.Length];
            privateFields.CopyTo(allFields, 0);
            publicFields.CopyTo(allFields, privateFields.Length);

            return allFields;
        }
    }

    public class ChatBoxExposer
    {
        private IModHelper helper;

        public ChatBoxExposer(IModHelper helper)
        {
            this.helper = helper;
        }

        public List<String> GetAllChatMessages()
        {
            List<String> messages = new List<String>();
            var sdMessagesRepresentation = this.helper.Reflection.GetField
                <List<StardewValley.Menus.ChatMessage>>
                (Game1.chatBox, "messages").GetValue();

            foreach (var message in sdMessagesRepresentation)
            {
                messages.Add(message.message[0].message);
            }

            return messages;
        }
    }

    public class PlayerExposer
    {
        private IModHelper helper;

        public PlayerExposer(IModHelper helper)
        {
            this.helper = helper;
        }

        public String GetPlayerLocation()
        {
            var location = this.helper.Reflection.GetField
                <StardewValley.Network.NetLocationRef>
                (Game1.player, "currentLocationRef").GetValue();

            var locationName = this.helper.Reflection.GetField
                <Netcode.NetString>
                (location, "locationName").GetValue();

            return locationName;
        }
    }

    public class ModEntry : Mod
    {
        private ObjectReflectionExposer objectExposer;
        private ChatBoxExposer chatExposer;
        private PlayerExposer playerExposer;

        private String lastChatMessage;

        public override void Entry(IModHelper helper)
        {
            objectExposer = new ObjectReflectionExposer(helper);
            chatExposer = new ChatBoxExposer(helper);
            playerExposer = new PlayerExposer(helper);

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.OneSecondUpdateTicking += this.CheckChatBox;

            this.Monitor.Log($"{this.Monitor.GetType()}", LogLevel.Warn);
        }

        private void LogToMonitor(dynamic val)
        {
            this.Monitor.Log($"{val}", LogLevel.Warn);
        }

        private void OutputFields(FieldInfo[] fields)
        {
            foreach (FieldInfo field in fields)
            {
                LogToMonitor(field);
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
        }

        private void CheckChatBox(object sender, OneSecondUpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // Testing location and fetching chests.
            String playerLocation = playerExposer.GetPlayerLocation();
            LogToMonitor(playerLocation);

            // (TODO): Change this to just get a single message in the
            // exposer instead, we don't need all messages.
            List<String> chatMessages = chatExposer.GetAllChatMessages();
            if (chatMessages.Count == 0)
                return;

            String newestMessage = chatMessages[chatMessages.Count - 1];
            if (newestMessage != this.lastChatMessage)
            {
                this.lastChatMessage = newestMessage;
                LogToMonitor(this.lastChatMessage);
            }
        }
    }
}

