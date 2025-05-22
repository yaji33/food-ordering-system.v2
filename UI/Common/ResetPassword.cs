using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using food_ordering_system.v2.Business.Services;

namespace food_ordering_system.v2.UI.Common
{
    public partial class ResetPassword : Form
    {
        private AuthService authService;

        public ResetPassword()
        {
            InitializeComponent();
            authService = new AuthService();
        }

        private void ResetPassword_Load(object sender, EventArgs e)
        {
            // Set password characters to hide actual input
            txtNewPassword.PasswordChar = '•';
            txtConfirmPassword.PasswordChar = '•';
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetUserPassword();
        }

        private void ResetUserPassword()
        {
            try
            {
                string username = txtUsername.Text;
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;

                // Basic validation
                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show("Please enter your username.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    MessageBox.Show("Please enter and confirm your new password.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("Passwords do not match.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Call service to reset password
                var result = authService.ResetPassword(username, newPassword);

                if (result.Success)
                {
                    MessageBox.Show(result.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Return to login form
                    this.Close();
                }
                else
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtConfirmPassword_TextChanged(object sender, EventArgs e)
        {

        }
    }
}