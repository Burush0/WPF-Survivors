using MySql.Data.MySqlClient;
using System.Windows;

namespace ExamGame
{
    internal class DatabaseHelper
    {
        private static string server = "localhost";
        private static string database = "wpf";
        private static string username = "root";
        private static string password = "root";

        private static string connectionString = $"Server={server};Database={database};User ID={username};Password={password};";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public bool RegisterUser(string username, string password)
        {
            try
            {

                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string query = "INSERT INTO Users (username, password) VALUES (@username, @password)";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    MessageBox.Show("Username already exists. Please choose a different username.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                else
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }
        public bool AuthenticateUser(string username, string password)
        {
            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                    return userCount > 0;
                }
            }
        }

        public int GetGold(string username)
        {
            int gold = 0;

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string query = "SELECT gold FROM users WHERE username = @username";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            gold = reader.GetInt32("gold");
                        }
                    }
                }
            }

            return gold;
        }

        public void UpdateUserGold(string username, int newGoldValue)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string updateQuery = "UPDATE users SET gold = @newGoldValue WHERE username = @username";
                    using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@newGoldValue", newGoldValue);
                        updateCommand.Parameters.AddWithValue("@username", username);

                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"Gold value updated successfully for user: {username}");
                        }
                        else
                        {
                            Console.WriteLine($"User not found: {username}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating gold value: {ex.Message}");
            }
        }

        public List<LeaderboardEntry> RetrieveLeaderboard()
        {
            List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

            try
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT username, score FROM leaderboard ORDER BY score DESC";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string username = reader.GetString("username");
                                int score = reader.GetInt32("score");
                                entries.Add(new LeaderboardEntry { Username = username, Score = score });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving leaderboard entries: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return entries;
        }

        public void StoreNewRecord(string username, int score)
        {
            try
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    // Get the minimum score currently stored in the database
                    string minScoreQuery = "SELECT MIN(score) FROM leaderboard";
                    using (MySqlCommand minScoreCommand = new MySqlCommand(minScoreQuery, connection))
                    {
                        object result = minScoreCommand.ExecuteScalar();
                        int minScore = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : int.MinValue;

                        // Check if the provided score is greater than the minimum score
                        if (score > minScore || minScore == int.MinValue)
                        {
                            // Get the IDs of the rows to delete
                            string deleteIdsQuery = "SELECT id FROM leaderboard WHERE score = (SELECT MIN(score) FROM leaderboard) AND (SELECT COUNT(*) FROM leaderboard) >= 10";
                            List<int> deleteIds = new List<int>();
                            using (MySqlCommand deleteIdsCommand = new MySqlCommand(deleteIdsQuery, connection))
                            {
                                using (MySqlDataReader reader = deleteIdsCommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        deleteIds.Add(reader.GetInt32("id"));
                                    }
                                }
                            }

                            // Delete the rows using the obtained IDs
                            foreach (int id in deleteIds)
                            {
                                string deleteQuery = $"DELETE FROM leaderboard WHERE id = {id}";
                                using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                                {
                                    deleteCommand.ExecuteNonQuery();
                                }
                            }

                            // Insert the new record
                            string insertQuery = "INSERT INTO leaderboard (username, score) VALUES (@username, @score)";
                            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@username", username);
                                insertCommand.Parameters.AddWithValue("@score", score);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // The provided score is not greater than the minimum score, so no action is taken
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error storing new record: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
