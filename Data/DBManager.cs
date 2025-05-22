using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace food_ordering_system.v2.Data
{
    public class DBManager
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ByteBite"].ConnectionString;

        // Get a new SQL connection
        public static MySqlConnection GetConnection()
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            return connection;
        }

        // Execute non-query commands (INSERT, UPDATE, DELETE)
        public static int ExecuteNonQuery(string commandText, CommandType commandType, params MySqlParameter[] parameters)
        {
            using (MySqlConnection connection = GetConnection())
            {
                using (MySqlCommand command = new MySqlCommand(commandText, connection))
                {
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        // Execute scalar commands (COUNT, SUM, etc.)
        public static object ExecuteScalar(string commandText, CommandType commandType, params MySqlParameter[] parameters)
        {
            using (MySqlConnection connection = GetConnection())
            {
                using (MySqlCommand command = new MySqlCommand(commandText, connection))
                {
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    return command.ExecuteScalar();
                }
            }
        }

        // Execute reader commands (SELECT)
        public static MySqlDataReader ExecuteReader(string commandText, CommandType commandType, params MySqlParameter[] parameters)
        {
            MySqlConnection connection = GetConnection();
            MySqlCommand command = new MySqlCommand(commandText, connection);
            command.CommandType = commandType;
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        // Get data table
        public static DataTable GetDataTable(string commandText, CommandType commandType, params MySqlParameter[] parameters)
        {
            using (MySqlConnection connection = GetConnection())
            {
                using (MySqlCommand command = new MySqlCommand(commandText, connection))
                {
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }
    }
}
