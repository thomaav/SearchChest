using System;
using System.Reflection;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace SearchChests
{
    public class ObjectReflectionExposer
    {
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

    public class GameObjectExposer
    {
        public List<String> GetAllChatMessages()
        {
            List<String> messages = new List<String>();
            var sdMessagesRepresentation = ModEntry.StaticHelper.Reflection.GetField
                <List<StardewValley.Menus.ChatMessage>>
                (Game1.chatBox, "messages").GetValue();

            foreach (var message in sdMessagesRepresentation)
            {
                String messageText = message.message[0].message;
                messageText = messageText
                    .Substring(messageText.IndexOf(':') + 2)
                    .Trim();
                messages.Add(messageText);
            }

            return messages;
        }

        public GameLocation GetPlayerLocation()
        {
            return Game1.currentLocation;
        }
    }
}
