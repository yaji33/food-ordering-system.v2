using System;
using System.Data;
using MySql.Data.MySqlClient;
using food_ordering_system.v2.Data.Models;

namespace food_ordering_system.v2.Data.Repositories
{
    public static class AdminRepo
    {
        // Authenticate admin with username and password
        public static Admin AuthenticateAdmin(string username, string hashedPassword)
        {
            try
            {
                string query = "SELECT * FROM admin WHERE username = @username AND password = @password";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@username", username),
                    new MySqlParameter("@password", hashedPassword)
                };

                DataTable dataTable = DBManager.GetDataTable(query, CommandType.Text, parameters);

                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];

                    Admin admin = new Admin
                    {
                        AdminId = Convert.ToInt32(row["admin_id"]),
                        Username = row["username"].ToString(),
                        Password = row["password"].ToString(),
                        FullName = row["full_name"].ToString(),
                        Email = row["email"].ToString(),
                        Role = row["role"].ToString()
                    };

                    return admin;
                }

                return null; // No matching admin found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error authenticating admin: {ex.Message}");
                return null;
            }
        }
        
        public static Admin GetAdminByUsername(string username)
        {
            Admin admin = null;

            try
            {
                string query = @"SELECT admin_id, username, password, full_name, email 
                       FROM admin 
                       WHERE username = @Username";

                MySqlParameter[] parameters =
                {
            new MySqlParameter("@Username", MySqlDbType.VarChar) { Value = username }
        };

                DataTable dataTable = DBManager.GetDataTable(query, CommandType.Text, parameters);

                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];

                    admin = new Admin
                    {
                        AdminId = Convert.ToInt32(row["admin_id"]),
                        Username = row["username"].ToString(),
                        Password = row["password"].ToString(),
                        FullName = row["full_name"].ToString(),
                        Email = row["email"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error getting admin by username: {ex.Message}");
            }

            return admin;
        }

   
        public static bool UpdateAdminPassword(int adminId, string hashedPassword)
        {
            try
            {
                string query = "UPDATE admin SET password = @Password WHERE admin_id = @AdminId";

                MySqlParameter[] parameters =
                {
            new MySqlParameter("@Password", MySqlDbType.VarChar) { Value = hashedPassword },
            new MySqlParameter("@AdminId", MySqlDbType.Int32) { Value = adminId }
        };

                int rowsAffected = DBManager.ExecuteNonQuery(query, CommandType.Text, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error updating admin password: {ex.Message}");
                return false;
            }
        }
    }
}