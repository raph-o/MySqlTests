using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

namespace MySqlTests
{
    class SqlUtils
    {

        string host, database, username, password;
        int port;
        public MySqlConnection connection;
        public Dictionary<string, string> userInfos { get; set; }

        public SqlUtils (string host, int port, string database, string username, string password)
        {
            this.host = host;
            this.port = port;
            this.database = database;
            this.username = username;
            this.password = password;

            Init();
        }

        private void Connect ()
        {
            //Basic connection string : Server=?;User=?;Database=?;Port=?;Password=?;
            //We can change the order of the parameters
            string connectionStr = $"Server={host};User={username};Database={database};Port={port};Password={password};Allow User Variables=True";
            connection = new MySqlConnection(connectionStr);

            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                //If the connection is impossible, we print the error
                Console.WriteLine($"An exception occured during the connection: {e.Message}");
            }
        }

        public void Disconnect ()
        {
            connection.Close();
        }

        private void Init ()
        {
            Connect();

            try
            {
                //This sql command allows to select the columns username and mail
                string sql = "SELECT username, mail FROM users";
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                //We create a local table containing the same columns and same rows of the mysql database
                DataTable dataTable = new DataTable();
                MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
                
                //We fill the local table with the distant table
                sqlDataAdapter.Fill(dataTable);
                userInfos = new Dictionary<string, string>();

                //Row 0 is the username, row 1 is the mail because we select only the username and the mail's row
                foreach (DataRow row in dataTable.Rows)
                {
                    userInfos.Add(row[0].ToString(), row[1].ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"An exception occured during the initialisation of the list: {e.Message}");
            }
        }

        public void CreateUser (User user)
        {
            try
            {
                //Sql string and the command in order to prepare the query
                string sql = "INSERT INTO users (username, password, mail, date) VALUES (@username, @password, @mail, @date)";
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                //We put the correct values of the query to avoid sql injections
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@password", user.Password);
                cmd.Parameters.AddWithValue("@mail", user.Mail);
                cmd.Parameters.AddWithValue("@date", user.DateJoined);

                //We execute the query and we close the connection
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine($"An exception occured during the creation of the user: {e.Message}");
            }

            Console.WriteLine("User successfully created");
        }

        public void DeleteUser (String username)
        {
            try
            {
                string sql = "DELETE FROM users WHERE username = @username";
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                cmd.Parameters.AddWithValue("@username", username);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine($"An exception occured during the deletion of the user: {e.Message}");
            }

            Console.WriteLine("User successfully deleted");
        }

        public void UpdateUser (string oldUsername, string oldMail, User user)
        {
            try
            {
                string sql = "UPDATE users SET username = @newusername, password = @password, mail = @mail WHERE username = @username";
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                cmd.Parameters.AddWithValue("@newusername", user.Username);
                cmd.Parameters.AddWithValue("@password", user.Password);
                cmd.Parameters.AddWithValue("@mail", user.Mail);
                cmd.Parameters.AddWithValue("@username", oldUsername);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine($"An exception occured during the update of the user: {e.Message}");
            }

            userInfos.Remove(oldUsername);
            userInfos.Add(user.Username, user.Mail);

            Console.WriteLine("User successfully updated");
        }
    }
}
