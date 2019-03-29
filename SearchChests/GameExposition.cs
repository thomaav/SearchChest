using System;
using System.Reflection;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace SearchChests
{
    public class ChatExposer
    {
        internal static String TrimChatMessage(String message)
        {
            return message
                .Substring(message.IndexOf(':') + 2)
                .Trim();
        }

        private static List<StardewValley.Menus.ChatMessage> _GetAllChatMessages()
        {
            return ModEntry.StaticHelper.Reflection.GetField
                <List<StardewValley.Menus.ChatMessage>>
                (Game1.chatBox, "messages").GetValue();
        }

        internal static String GetLastChatMessage()
        {
            var sdMessagesRepresentation = _GetAllChatMessages();

            if (sdMessagesRepresentation.Count == 0) {
                return null;
            }

            int lastMessageIdx = sdMessagesRepresentation.Count - 1;
            String lastMessage = sdMessagesRepresentation[lastMessageIdx].message[0].message;
            return TrimChatMessage(lastMessage);
        }

        internal static List<String> GetAllChatMessages()
        {
            List<String> messages = new List<String>();
            var sdMessagesRepresentation = _GetAllChatMessages();

            foreach (var message in sdMessagesRepresentation)
            {
                String messageText = message.message[0].message;
                messageText = TrimChatMessage(messageText);
                messages.Add(messageText);
            }

            return messages;
        }
    }
}
