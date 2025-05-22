using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using food_ordering_system.v2.UI.Customer;
using food_ordering_system.v2.Business.Services;
using food_ordering_system.v2.Data.Models;

namespace food_ordering_system.v2.UI.Common
{
    public partial class LoginForm : Form
    {
        private AuthService authService;

        public LoginForm()
        {
            InitializeComponent();
            authService = new AuthService();

            // Set password character to hide actual input
            txtPassword.PasswordChar = '•'; 
            
           
        }

        // Empty event handlers
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void label2_Click_1(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            LoginUser();
        }

        private void LoginUser()
        {
            try
            {
                string username = txtUsername.Text;
                string password = txtPassword.Text;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter both username and password.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                
                var adminResult = authService.LoginAdmin(username, password);
                if (adminResult.Success)
                {
                    MessageBox.Show($"Welcome, Administrator!",
                                  "Admin Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    NavigateToAdminDashboard();
                    return;
                }

                
                var customerResult = authService.LoginCustomer(username, password);
                if (customerResult.Success)
                {
                    MessageBox.Show($"Welcome back, {customerResult.Customer.FirstName}!",
                                  "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    NavigateToMenu();
                    return;
                }
                else
                {
                    // Show error if login failed
                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.FormClosed += (s, args) => this.Show();
            registerForm.Show();
            this.Hide();
        }

        private void NavigateToMenu()
        {
            try
            {
               
                int loggedInCustomerId = AuthService.CurrentUserId;

                Customer.MainForm menuForm = new Customer.MainForm(loggedInCustomerId);
                menuForm.FormClosed += (s, args) =>
                {
                    // When MenuForm closes, logout and close the login form
                    authService.Logout();
                    this.Close();
                };

                menuForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening menu: {ex.Message}",
                              "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NavigateToAdminDashboard()
        {
            try
            {
                Admin.MainForm mainForm = new Admin.MainForm();
                mainForm.FormClosed += (s, args) =>
                {
                    authService.Logout();
                    this.Close();
                };

                mainForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening admin dashboard: {ex.Message}",
                              "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                // Navigate to Reset Password form
                ResetPassword resetPasswordForm = new ResetPassword();
                resetPasswordForm.FormClosed += (s, args) => this.Show();
                resetPasswordForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening reset password form: {ex.Message}",
                              "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}