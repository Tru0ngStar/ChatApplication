using System.IO;

namespace ChatServer
{
    public static class DatabaseContext
    {
        private static readonly string AccountsFile = Path.Combine(AppContext.BaseDirectory, "accounts.txt");
        private static readonly object FileLock = new();

        public static void Initialize()
        {
            // Ensure the accounts file exists
            try
            {
                lock (FileLock)
                {
                    if (!File.Exists(AccountsFile)) File.WriteAllText(AccountsFile, string.Empty);
                }
            }
            catch { /* if we fail to create the file, other methods will handle errors */ }
        }

        // File format: one account per line as "username:password"
        public static bool RegisterUser(string user, string pass)
        {
            try
            {
                lock (FileLock)
                {
                    var lines = File.ReadAllLines(AccountsFile);
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        var idx = line.IndexOf(':');
                        if (idx <= 0) continue;
                        var existingUser = line.Substring(0, idx);
                        if (string.Equals(existingUser, user, StringComparison.OrdinalIgnoreCase))
                            return false; // user exists
                    }

                    // Append new user
                    File.AppendAllLines(AccountsFile, new[] { $"{user}:{pass}" });
                    return true;
                }
            }
            catch { return false; }
        }

        public static bool ValidateUser(string user, string pass)
        {
            try
            {
                lock (FileLock)
                {
                    var lines = File.ReadAllLines(AccountsFile);
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        var parts = line.Split(':', 2);
                        if (parts.Length != 2) continue;
                        if (string.Equals(parts[0], user, StringComparison.OrdinalIgnoreCase) && parts[1] == pass)
                            return true;
                    }
                    return false;
                }
            }
            catch { return false; }
        }
    }
}
