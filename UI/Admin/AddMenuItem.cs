using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using food_ordering_system.v2.Data.Models;
using food_ordering_system.v2.Data.Repositories;
using MenuItem = food_ordering_system.v2.Data.Models.MenuItem;

namespace food_ordering_system.v2.UI.Admin
{
    public partial class AddMenuItem : Form
    {
        private MenuItem _editingItem = null;
        private List<Category> _categories;

        // Constructor for adding a new menu item
        public AddMenuItem()
        {
            InitializeComponent();
            this.Text = "Add Menu Item";
            btnSave.Text = "Add";

            // Load categories
            LoadCategories();
        }

        // Constructor for editing an existing menu item
        public AddMenuItem(MenuItem menuItem)
        {
            InitializeComponent();
            this.Text = "Edit Menu Item";
            btnSave.Text = "Update";
            _editingItem = menuItem;

            // Load categories
            LoadCategories();

            // Populate fields with menu item data
            PopulateFields();
        }



        private void LoadCategories()
        {
            try
            {
                // Clear combo box
                cboCategory.Items.Clear();

                // Get all categories
                _categories = CategoryRepo.GetAllCategories();

                // Add to combo box
                foreach (Category category in _categories)
                {
                    cboCategory.Items.Add(category.CategoryName);
                }

                // Select first item if available
                if (cboCategory.Items.Count > 0)
                {
                    cboCategory.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFields()
        {
            if (_editingItem != null)
            {
                txtName.Text = _editingItem.Name;
                txtDescription.Text = _editingItem.Description;
                txtPrice.Text = _editingItem.Price.ToString("0.00");

                // Select the category in the combo box
                if (_editingItem.Category != null)
                {
                    for (int i = 0; i < _categories.Count; i++)
                    {
                        if (_categories[i].CategoryId == _editingItem.CategoryId)
                        {
                            cboCategory.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Please enter a name for the menu item.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
                {
                    MessageBox.Show("Please enter a valid price for the menu item.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPrice.Focus();
                    return;
                }

                if (cboCategory.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a category for the menu item.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboCategory.Focus();
                    return;
                }

                // Get the selected category
                Category selectedCategory = _categories[cboCategory.SelectedIndex];

                // Create or update menu item
                MenuItem menuItem;

                if (_editingItem == null)
                {
                    // Create new menu item
                    menuItem = new MenuItem
                    {
                        Name = txtName.Text.Trim(),
                        Description = txtDescription.Text.Trim(),
                        Price = price,
                        CategoryId = selectedCategory.CategoryId
                    };

                    // Add to database
                    bool success = MenuItemRepo.AddMenuItem(menuItem);

                    if (success)
                    {
                        MessageBox.Show("Menu item added successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add menu item.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Update existing menu item
                    _editingItem.Name = txtName.Text.Trim();
                    _editingItem.Description = txtDescription.Text.Trim();
                    _editingItem.Price = price;
                    _editingItem.CategoryId = selectedCategory.CategoryId;

                    // Update in database
                    bool success = MenuItemRepo.UpdateMenuItem(_editingItem);

                    if (success)
                    {
                        MessageBox.Show("Menu item updated successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update menu item.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving menu item: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {

        }
    }
}