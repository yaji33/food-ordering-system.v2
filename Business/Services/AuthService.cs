using System;
using food_ordering_system.v2.Data.Models;
using food_ordering_system.v2.Data.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace food_ordering_system.v2.Business.Services
{
    public class AuthService
    {
    
        public static int CurrentUserId { get; private set; }
        public static string CurrentUsername { get; private set; }
        public static string CurrentUserRole { get; private set; } // "Customer" or "Admin"
        public static bool IsLoggedIn => !string.IsNullOrEmpty(CurrentUsername);

        // Register a new customer
        public (bool Success, string Message) RegisterCustomer(string username, string password, string confirmPassword,
            string firstName, string lastName, string email, string phoneNumber, string address)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email))
            {
                return (false, "Please fill in all required fields.");
            }

            if (password != confirmPassword)
            {
                return (false, "Passwords do not match.");
            }

            // Check if username already exists
            if (CustomerRepo.IsUsernameExists(username))
            {
                return (false, "Username already exists. Please choose another one.");
            }

            try
            {
                // Hash the password before saving
                string hashedPassword = HashPassword(password);

                // Create customer object
                Customer customer = new Customer
                {
                    Username = username,
                    Password = hashedPassword,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    RegistrationDate = DateTime.Now
                };

                // Register the customer
                bool result = CustomerRepo.RegisterCustomer(customer);

                if (result)
                {
                    return (true, "Registration successful! You can now log in.");
                }
                else
                {
                    return (false, "Failed to register. Please try again.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred during registration: {ex.Message}");
            }
        }

        // Customer login
        public (bool Success, string Message, Customer Customer) LoginCustomer(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return (false, "Please enter both username and password.", null);
                }

                // Hash the password to match stored hash
                string hashedPassword = HashPassword(password);

                // Attempt to authenticate
                Customer customer = CustomerRepo.AuthenticateCustomer(username, hashedPassword);

                if (customer != null)
                {
                    // Set current user session
                    CurrentUserId = customer.CustomerId;
                    CurrentUsername = customer.Username;
                    CurrentUserRole = "Customer";

                    return (true, "Login successful!", customer);
                }
                else
                {
                    return (false, "Invalid username or password.", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred during login: {ex.Message}", null);
            }
        }
        public (bool Success, string Message, Admin Admin) LoginAdmin(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return (false, "Please enter both username and password.", null);
                }

                // Hash the password to match stored hash
                string hashedPassword = HashPassword(password);

                // Attempt to authenticate admin
                Admin admin = AdminRepo.AuthenticateAdmin(username, hashedPassword);

                if (admin != null)
                {
                    // Set current user session
                    CurrentUserId = admin.AdminId;
                    CurrentUsername = admin.Username;
                    CurrentUserRole = "Admin";

                    return (true, "Admin login successful!", admin);
                }
                else
                {
                    return (false, "Invalid admin credentials.", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred during admin login: {ex.Message}", null);
            }
        }
       

        // Reset password functionality
        public (bool Success, string Message) ResetPassword(string username, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(newPassword))
                {
                    return (false, "Please enter both username and new password.");
                }

                // Check if it's a customer account
                var customer = CustomerRepo.GetCustomerByUsername(username);
                if (customer != null)
                {
                    // Hash the new password
                    string hashedPassword = HashPassword(newPassword);

                    // Update the customer's password
                    bool result = CustomerRepo.UpdateCustomerPassword(customer.CustomerId, hashedPassword);

                    if (result)
                    {
                        return (true, "Password reset successful! You can now log in with your new password.");
                    }
                    else
                    {
                        return (false, "Failed to reset password. Please try again.");
                    }
                }

                // Check if it's an admin account
                var admin = AdminRepo.GetAdminByUsername(username);
                if (admin != null)
                {
                    // Hash the new password
                    string hashedPassword = HashPassword(newPassword);

                    // Update the admin's password
                    bool result = AdminRepo.UpdateAdminPassword(admin.AdminId, hashedPassword);

                    if (result)
                    {
                        return (true, "Admin password reset successful! You can now log in with your new password.");
                    }
                    else
                    {
                        return (false, "Failed to reset admin password. Please try again.");
                    }
                }

              
                return (false, "Username not found. Please check your username and try again.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred during password reset: {ex.Message}");
            }
        }


        // Logout - clear session
        public void Logout()
        {
            CurrentUserId = 0;
            CurrentUsername = null;
            CurrentUserRole = null;
        }

        // Check if user is logged in
        public bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(CurrentUsername);
        }

        // Check if logged in user is admin
        

        
        private string HashPassword(string password)
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