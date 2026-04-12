using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ChatServer
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
        public string MessageType { get; set; } // "Message", "SystemNotification"
        public DateTime Timestamp { get; set; }
    }

    public static class ChatHistoryManager
    {
        private static readonly string HistoryFile = Path.Combine(AppContext.BaseDirectory, "chat_history.json");
        private static readonly object FileLock = new();
        private static List<ChatMessage> _messages = new();
        private static int _nextId = 1;

        public static void Initialize()
        {
            lock (FileLock)
            {
                try
                {
                    if (File.Exists(HistoryFile))
                    {
                        var json = File.ReadAllText(HistoryFile);
                        _messages = JsonSerializer.Deserialize<List<ChatMessage>>(json) ?? new List<ChatMessage>();
                        if (_messages.Count > 0)
                            _nextId = _messages.Max(m => m.Id) + 1;
                    }
                    else
                    {
                        _messages = new List<ChatMessage>();
                        _nextId = 1;
                    }
                }
                catch
                {
                    _messages = new List<ChatMessage>();
                    _nextId = 1;
                }
            }
        }

        public static void SaveMessage(int groupId, string sender, string content, string messageType = "Message")
        {
            if (string.IsNullOrWhiteSpace(sender) || string.IsNullOrWhiteSpace(content))
                return;

            lock (FileLock)
            {
                try
                {
                    var message = new ChatMessage
                    {
                        Id = _nextId++,
                        GroupId = groupId,
                        Sender = sender,
                        Content = content,
                        MessageType = messageType,
                        Timestamp = DateTime.Now
                    };

                    _messages.Add(message);
                    SaveToFile();
                }
                catch { /* Ignore save errors */ }
            }
        }

        public static List<ChatMessage> LoadGroupHistory(int groupId)
        {
            lock (FileLock)
            {
                try
                {
                    return _messages
                        .Where(m => m.GroupId == groupId)
                        .OrderBy(m => m.Timestamp)
                        .ToList();
                }
                catch
                {
                    return new List<ChatMessage>();
                }
            }
        }

        private static void SaveToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_messages, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(HistoryFile, json);
            }
            catch { /* Ignore file write errors */ }
        }

        public static void ClearGroupHistory(int groupId)
        {
            lock (FileLock)
            {
                try
                {
                    _messages.RemoveAll(m => m.GroupId == groupId);
                    SaveToFile();
                }
                catch { /* Ignore errors */ }
            }
        }
    }
}
