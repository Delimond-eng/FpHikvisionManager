using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Data.SQLite;



namespace TimeAttandance
{
    class DatabaseHelper
    {
        private string _databaseFile;

        public DatabaseHelper(string databaseFile)
        {
            _databaseFile = databaseFile;
            CreateDatabase();
        }

        private void CreateDatabase()
        {
            if (!System.IO.File.Exists(_databaseFile))
            {
                SQLiteConnection.CreateFile(_databaseFile);

                using (var connection = new SQLiteConnection($"Data Source={_databaseFile};Version=3;"))
                {
                    connection.Open();
                    string createTableQuery = @"
                    CREATE TABLE Agents (
                        AgentId TEXT PRIMARY KEY,
                        FpTemplate1 BLOB,
                        FpTemplate2 BLOB,
                        FpTemplate3 BLOB
                    )";
                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void AddAgent(string agentId, byte[] template1, byte[] template2, byte[] template3)
        {
            using (var connection = new SQLiteConnection($"Data Source={_databaseFile};Version=3;"))
            {
                connection.Open();
                string insertQuery = "INSERT INTO Agents (AgentId, FpTemplate1, FpTemplate2, FpTemplate3) VALUES (@AgentId, @FpTemplate1, @FpTemplate2, @FpTemplate3)";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@AgentId", agentId);
                    command.Parameters.AddWithValue("@FpTemplate1", template1);
                    command.Parameters.AddWithValue("@FpTemplate2", template2);
                    command.Parameters.AddWithValue("@FpTemplate3", template3);
                    command.ExecuteNonQuery();
                }
            }
        }

        public SQLiteDataReader GetAllAgents()
        {
            var connection = new SQLiteConnection($"Data Source={_databaseFile};Version=3;");
            connection.Open();
            string selectQuery = "SELECT * FROM Agents";
            using (var command = new SQLiteCommand(selectQuery, connection))
            {
                return command.ExecuteReader();
            }
        }
    }
}
