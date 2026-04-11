using Microsoft.Data.Sqlite;

namespace ChatServer
{
    public static class DatabaseContext
    {
        private const string ConnectionString = "Data Source=chatapp.db";

        public static void Initialize()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Username TEXT PRIMARY KEY,
                    Password TEXT NOT NULL
                )";
            command.ExecuteNonQuery();
        }

        public static bool RegisterUser(string user, string pass)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Users (Username, Password) VALUES ($u, $p)";
                command.Parameters.AddWithValue("$u", user);
                command.Parameters.AddWithValue("$p", pass);
                return command.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }

        public static bool ValidateUser(string user, string pass)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = $u AND Password = $p";
            command.Parameters.AddWithValue("$u", user);
            command.Parameters.AddWithValue("$p", pass);
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
    }
}