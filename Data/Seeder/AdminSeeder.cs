using System;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using MySql.Data.MySqlClient;

namespace food_ordering_system.v2.Data.Seeders
{
    public class AdminSeeder
    {
        // Default admin credentials 
        private const string DefaultUsername = "admin";
        private const string DefaultPassword = "Admin123!";
        private const string DefaultFullName = "System Administrator";
        private const string DefaultEmail = "admin@foodordering.com";
        private const string DefaultRole = "Admin";

        public static void SeedAdmin()
        {
            try
            {
                // Check if admin table is empty
                string checkQuery = "SELECT COUNT(*) FROM admin";
                object result = DBManager.ExecuteScalar(checkQuery, CommandType.Text);

                int adminCount = Convert.ToInt32(result);

                // Only seed if no admin exists
                if (adminCount == 0)
                {
                  
                    string hashedPassword = HashPassword(DefaultPassword);

                    // Insert the admin record
                    string insertQuery = @"INSERT INTO admin 
                                          (username, password, full_name, email, role) 
                                          VALUES (@username, @password, @fullName, @email, @role)";

                    MySqlParameter[] parameters =
                    {
                        new MySqlParameter("@username", DefaultUsername),
                        new MySqlParameter("@password", hashedPassword),
                        new MySqlParameter("@fullName", DefaultFullName),
                        new MySqlParameter("@email", DefaultEmail),
                        new MySqlParameter("@role", DefaultRole)
                    };

                    int rowsAffected = DBManager.ExecuteNonQuery(insertQuery, CommandType.Text, parameters);

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Admin account has been seeded successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to seed admin account.");
                    }
                }
                else
                {
                    Console.WriteLine("Admin account already exists. Skipping seed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding admin account: {ex.Message}");
            }
        }

       
        private static string HashPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}