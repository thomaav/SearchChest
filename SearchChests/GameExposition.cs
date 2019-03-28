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
        internal static FieldInfo[] GetPrivateFields<T>(T obj)
        {
            return obj.GetType().GetFields(BindingFlags.NonPublic |
                                           BindingFlags.Instance);
        }

        internal static FieldInfo[] GetPublicFields<T>(T obj)
        {
            return obj.GetType().GetFields(BindingFlags.Public |
                                           BindingFlags.Instance |
                                           BindingFlags.DeclaredOnly);
        }

        internal static FieldInfo[] GetAllFields<T>(T obj)
        {
            var privateFields = GetPrivateFields(obj);
            var publicFields = GetPublicFields(obj);

            var allFields = new FieldInfo[privateFields.Length + publicFields.Length];
            privateFields.CopyTo(allFields, 0);
            publicFields.CopyTo(allFields, privateFields.Length);

            return allFields;
        }

        internal static MethodInfo[] GetAllMethods<T>(T obj)
        {
            return obj.GetType().GetMethods();
        }

        internal static void LogMembers<T>(T obj)
        {
            FieldInfo[] fields = ObjectReflectionExposer.GetAllFields(obj);
            MethodInfo[] methods = ObjectReflectionExposer.GetAllMethods(obj);

            foreach (FieldInfo field in fields)
            {
                ModEntry.Log(field);
            }

            foreach (MethodInfo method in methods)
            {
                ModEntry.Log(method);
            }
        }
    }

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
