using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using food_ordering_system.v2.Data.Models;
using food_ordering_system.v2.Business.Services;
namespace food_ordering_system.v2.UI.Customer
{
    public partial class MainForm : Form
    {
        private Panel panelContainer;
        private int currentCustomerId;
        public void UpdatePageTitle(string title)
        {
            this.Text = title;
        }
        public MainForm(int customerId)
        {
            InitializeComponent();
            UpdatePageTitle("Menu");
            currentCustomerId = customerId;
            panelContainer = new Panel();
            panelContainer.Dock = DockStyle.Fill;
            this.Controls.Add(panelContainer);
            this.Load += new EventHandler(MainForm_load);
        }
        private void LoadPage(UserControl page)
        {
            panelContainer.Controls.Clear();
            page.Dock = DockStyle.Fill;
            panelContainer.Controls.Add(page);
            page.BringToFront();
        }
        private void MainForm_load(object sender, EventArgs e)
        {
            // Load the default page when the form loads
            LoadPage(new MenuPage(currentCustomerId));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Create a custom popup/dialog for profile options
            Form profileDialog = new Form
            {
                Text = "Profile Options",
                Size = new Size(200, 150),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            // View Profile button
            Button viewProfileButton = new Button
            {
                Text = "View Profile",
                Size = new Size(160, 30),
                Location = new Point(15, 15),
                BackColor = Color.FromArgb(46, 139, 87), // SeaGreen
                ForeColor = Color.White
            };

            viewProfileButton.Click += (s, args) =>
            {
                // This is just a placeholder - the button will show but not function
                MessageBox.Show("Profile view functionality is not implemented yet.",
                               "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
                profileDialog.Close();
            };

            // Logout button
            Button logoutButton = new Button
            {
                Text = "Logout",
                Size = new Size(160, 30),
                Location = new Point(15, 55),
                BackColor = Color.FromArgb(220, 53, 69), // Bootstrap danger color
                ForeColor = Color.White
            };

            logoutButton.Click += (s, args) =>
            {
                DialogResult result = MessageBox.Show("Are you sure you want to logout?",
                                                    "Logout Confirmation",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    profileDialog.Close();

                    // Logout functionality
                    this.Hide();

                    // Create new login form
                    Form loginForm = new food_ordering_system.v2.UI.Common.LoginForm();
                    loginForm.Show();

                    // Dispose this form
                    this.Dispose();
                }
            };

            // Add buttons to form
            profileDialog.Controls.Add(viewProfileButton);
            profileDialog.Controls.Add(logoutButton);

            // Show the dialog
            profileDialog.ShowDialog(this);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Empty event handler
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            // This appears to be duplicate of MainForm_load
        }
        private void button2_Click(object sender, EventArgs e)
        {
            UpdatePageTitle("Menu");
            LoadPage(new MenuPage(currentCustomerId));
        }
        private void button3_Click(object sender, EventArgs e)
        {
            UpdatePageTitle("My Orders");
            LoadPage(new MyOrders(currentCustomerId));
        }
        private void button4_Click(object sender, EventArgs e)
        {
            UpdatePageTitle("History");
            //LoadPage(new OrderHistory(currentCustomerId));
        }
    }
}