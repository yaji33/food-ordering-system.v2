using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using food_ordering_system.v2.Data.Models;

namespace food_ordering_system.v2.Data.Repositories
{
    public class CustomerRepo
    {
        public static bool RegisterCustomer(Customer customer)
        {
            try
            {
               
                string query = @"INSERT INTO customers (first_name, last_name, email, phone_number, address, username, password, created_at) 
                                VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @Address, @Username, @Password, @RegistrationDate);
                                SELECT LAST_INSERT_ID();"; 

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@Username", MySqlDbType.VarChar) { Value = customer.Username },
                    new MySqlParameter("@Password", MySqlDbType.VarChar) { Value = customer.Password },
                    new MySqlParameter("@FirstName", MySqlDbType.VarChar) { Value = customer.FirstName },
                    new MySqlParameter("@LastName", MySqlDbType.VarChar) { Value = customer.LastName },
                    new MySqlParameter("@Email", MySqlDbType.VarChar) { Value = customer.Email },
                    new MySqlParameter("@PhoneNumber", MySqlDbType.VarChar) { Value = customer.PhoneNumber},
                    new MySqlParameter("@Address", MySqlDbType.VarChar) { Value = customer.Address },
                    new MySqlParameter("@RegistrationDate", MySqlDbType.DateTime) { Value = customer.RegistrationDate }
                };

                object result = DBManager.ExecuteScalar(query, CommandType.Text, parameters);
                int customerId = Convert.ToInt32(result);
                customer.CustomerId = customerId;

                return customerId > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering customer: {ex.Message}");
                return false;
            }
        }

        public static bool IsUsernameExists(string username)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM customers WHERE username = @Username";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@Username", MySqlDbType.VarChar) { Value = username }
                };

                object result = DBManager.ExecuteScalar(query, CommandType.Text, parameters);
                int count = Convert.ToInt32(result);

                return count > 0;
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error checking username existence: {ex.Message}");
                return false;
            }
        }

        // Authenticate customer for login
        public static Customer AuthenticateCustomer(string username, string password)
        {
            try
            {
                string query = @"SELECT customer_id, first_name, last_name, 
                                 email, phone_number, address, username, password, created_at 
                                 FROM customers 
                                 WHERE username = @Username AND password = @Password";

                
                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@Username", MySqlDbType.VarChar) { Value = username },
                    new MySqlParameter("@Password", MySqlDbType.VarChar) { Value = password }
                };

              
                using (MySqlDataReader reader = DBManager.ExecuteReader(query, CommandType.Text, parameters))
                {
                    if (reader.Read())
                    {
                        Customer customer = new Customer
                        {
                            CustomerId = Convert.ToInt32(reader["customer_id"]),
                            Username = reader["username"].ToString(),
                            Password = reader["password"].ToString(),
                            FirstName = reader["first_name"].ToString(),
                            LastName = reader["last_name"].ToString(),
                            Email = reader["email"].ToString(),
                            PhoneNumber = reader["phone_number"].ToString(),
                            Address = reader["address"].ToString(),
                            RegistrationDate = Convert.ToDateTime(reader["created_at"])
                        };

                        return customer;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error authenticating customer: {ex.Message}");
                return null;
            }
        }

       
        public static Customer GetCustomerByUsername(string username)
        {
            try
            {
                string query = @"SELECT customer_id, first_name, last_name, email, 
                               phone_number, address, username, password, created_at 
                               FROM customers 
                               WHERE username = @Username";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@Username", MySqlDbType.VarChar) { Value = username }
                };

                using (MySqlDataReader reader = DBManager.ExecuteReader(query, CommandType.Text, parameters))
                {
                    if (reader.Read())
                    {
                        Customer customer = new Customer
                        {
                            CustomerId = Convert.ToInt32(reader["customer_id"]),
                            Username = reader["username"].ToString(),
                            Password = reader["password"].ToString(),
                            FirstName = reader["first_name"].ToString(),
                            LastName = reader["last_name"].ToString(),
                            Email = reader["email"].ToString(),
                            PhoneNumber = reader["phone_number"]?.ToString(),
                            Address = reader["address"]?.ToString(),
                            RegistrationDate = Convert.ToDateTime(reader["created_at"])
                        };

                        return customer;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error getting customer by username: {ex.Message}");
                return null;
            }
        }

       
        public static bool UpdateCustomerPassword(int customerId, string hashedPassword)
        {
            try
            {
                string query = "UPDATE customers SET password = @Password WHERE customer_id = @CustomerId";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@Password", MySqlDbType.VarChar) { Value = hashedPassword },
                    new MySqlParameter("@CustomerId", MySqlDbType.Int32) { Value = customerId }
                };

                int rowsAffected = DBManager.ExecuteNonQuery(query, CommandType.Text, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error updating customer password: {ex.Message}");
                return false;
            }
        }
    }
}