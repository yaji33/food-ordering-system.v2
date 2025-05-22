using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using food_ordering_system.v2.UI.Admin;

namespace food_ordering_system.v2.UI.Admin
{
    public partial class MainForm : Form
    {
        private Panel panelContainer;
        public void UpdatePageTitle(string title)
        {

            this.Text = title;

        }
        public MainForm()
        {
            InitializeComponent();
            UpdatePageTitle("Dashboard");
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
            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdatePageTitle("Menu");
            LoadPage(new MenuManagement());
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            UpdatePageTitle("Orders");
            LoadPage(new OrderManagement());
        }

        private void button4_Click(object sender, EventArgs e)
        {

            UpdatePageTitle("Transactions");
            LoadPage(new Transactions());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Display confirmation message
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout Confirmation",
                                                 MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {

                // Close the current form
                this.Hide();

                // Open the login form
                
                Form loginForm = new food_ordering_system.v2.UI.Common.LoginForm();
                loginForm.Show();

                // Dispose the current form once the login form is displayed
                this.Dispose();
            }
        }
    }
}
