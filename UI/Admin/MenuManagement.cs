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
using food_ordering_system.v2.Data.Repositories;
using MenuItem = food_ordering_system.v2.Data.Models.MenuItem;

namespace food_ordering_system.v2.UI.Admin
{
    public partial class MenuManagement : UserControl
    {
        private List<MenuItem> allMenuItems;
        private List<Category> allCategories;
        private Dictionary<int, Button> categoryButtons;
        private const string ALL_ITEMS = "All";

        public MenuManagement()
        {
            InitializeComponent();

            // Setup event handlers for loading controls
            this.Load += MenuManagement_Load;

            // Initialize button mapping and event handlers
            InitializeCategoryButtons();
        }

        private void InitializeCategoryButtons()
        {
            // Create a dictionary to map category IDs to buttons
            categoryButtons = new Dictionary<int, Button>();

            // Set up click events for category buttons
            btnAll.Click += CategoryButton_Click;
            btnRiceMeal.Click += CategoryButton_Click;
            btnBreakfast.Click += CategoryButton_Click;
            btnChicken.Click += CategoryButton_Click;
            btnPasta.Click += CategoryButton_Click;
            btnCombo.Click += CategoryButton_Click;
            btnSnack.Click += CategoryButton_Click;
            btnDessert.Click += CategoryButton_Click;
            btnBeverage.Click += CategoryButton_Click;

            // Tag the buttons with their category names for identification
            btnAll.Tag = ALL_ITEMS;
            btnRiceMeal.Tag = "Rice Meal";
            btnBreakfast.Tag = "Breakfast";
            btnChicken.Tag = "Chicken Specialties";
            btnPasta.Tag = "Pasta";
            btnCombo.Tag = "Combo Meal";
            btnSnack.Tag = "Snack";
            btnDessert.Tag = "Dessert";
            btnBeverage.Tag = "Beverage";
        }

        private void MenuManagement_Load(object sender, EventArgs e)
        {
            // Load all categories
            LoadCategories();

            // Load all menu items
            LoadAllMenuItems();

            // Set btnAll as selected initially
            HighlightSelectedCategoryButton(btnAll);
        }

        private void LoadCategories()
        {
            try
            {
                // Get all categories from the database
                allCategories = CategoryRepo.GetAllCategories();

                // Map category names to buttons
                MapCategoriesToButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MapCategoriesToButtons()
        {
            // Clear existing mappings
            categoryButtons.Clear();

            // Map predefined buttons to categories
            foreach (Category category in allCategories)
            {
                // Find button that matches category name (case insensitive)
                Button matchingButton = FindButtonByCategoryName(category.CategoryName);
                if (matchingButton != null)
                {
                    categoryButtons[category.CategoryId] = matchingButton;
                }
            }
        }

        private Button FindButtonByCategoryName(string categoryName)
        {
            // Normalize names for comparison
            string normalizedName = categoryName.Trim().ToLower();

            // Check each button's tag
            foreach (Control control in this.Controls)
            {
                if (control is Button button && button.Tag != null)
                {
                    string buttonTag = button.Tag.ToString().ToLower();

                    // Check for exact match or plural/singular variants
                    if (buttonTag == normalizedName ||
                        (buttonTag + "s") == normalizedName ||
                        buttonTag == (normalizedName + "s"))
                    {
                        return button;
                    }
                }
            }

            return null;
        }

        private void LoadAllMenuItems()
        {
            try
            {
                // Get all menu items from the database
                allMenuItems = MenuItemRepo.GetAllMenuItems();

                // Display all menu items
                DisplayMenuItems(allMenuItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading menu items: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayMenuItems(List<MenuItem> menuItems)
        {
            // Clear the panel
            panel1.Controls.Clear();

            // If no menu items, show a message
            if (menuItems == null || menuItems.Count == 0)
            {
                Label noItemsLabel = new Label
                {
                    Text = "No menu items found",
                    Font = new Font("Aeonik TRIAL", 12, FontStyle.Regular),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };
                panel1.Controls.Add(noItemsLabel);
                return;
            }

            // Create a panel for each menu item
            int yPos = 10;
            const int padding = 10;
            const int itemHeight = 110;  // Increased height for better spacing

            foreach (MenuItem item in menuItems)
            {
                // Create a panel for the menu item
                Panel itemPanel = new Panel
                {
                    Width = panel1.Width - 25,
                    Height = itemHeight,
                    Location = new Point(10, yPos),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                // Add name label
                Label nameLabel = new Label
                {
                    Text = item.Name,
                    Font = new Font("Aeonik TRIAL", 12, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(10, 10)
                };
                itemPanel.Controls.Add(nameLabel);

                // Add description label (with truncation if too long)
                string description = item.Description;
                if (description != null && description.Length > 60)
                {
                    description = description.Substring(0, 57) + "...";
                }

                Label descLabel = new Label
                {
                    Text = description ?? "No description available",
                    Font = new Font("Aeonik TRIAL", 9, FontStyle.Regular),
                    AutoSize = true,
                    MaximumSize = new Size(itemPanel.Width - 150, 0),
                    Location = new Point(10, 35)
                };
                itemPanel.Controls.Add(descLabel);

                // Add price label
                Label priceLabel = new Label
                {
                    Text = $"₱{item.Price:0.00}",
                    Font = new Font("Aeonik TRIAL", 11, FontStyle.Bold),
                    AutoSize = true,
                    ForeColor = Color.DarkGreen,
                    Location = new Point(10, itemHeight - 30)
                };
                itemPanel.Controls.Add(priceLabel);

                // Add category label
                Label categoryLabel = new Label
                {
                    Text = item.Category?.CategoryName ?? "No Category",
                    Font = new Font("Aeonik TRIAL", 9, FontStyle.Italic),
                    AutoSize = true,
                    ForeColor = Color.DarkGray,
                    Location = new Point(100, itemHeight - 28)
                };
                itemPanel.Controls.Add(categoryLabel);

                // Add edit button
                Button editButton = new Button
                {
                    Text = "Edit",
                    Size = new Size(70, 30),
                    Location = new Point(itemPanel.Width - 160, itemHeight - 40),
                    Tag = item,
                    BackColor = Color.LightBlue,
                    FlatStyle = FlatStyle.Flat
                };
                editButton.FlatAppearance.BorderSize = 0;
                editButton.Click += EditButton_Click;
                itemPanel.Controls.Add(editButton);

                // Add delete button
                Button deleteButton = new Button
                {
                    Text = "Delete",
                    Size = new Size(70, 30),
                    Location = new Point(itemPanel.Width - 80, itemHeight - 40),
                    Tag = item,
                    BackColor = Color.MistyRose,
                    FlatStyle = FlatStyle.Flat
                };
                deleteButton.FlatAppearance.BorderSize = 0;
                deleteButton.Click += DeleteButton_Click;
                itemPanel.Controls.Add(deleteButton);

                // Add to panel
                panel1.Controls.Add(itemPanel);

                // Increment Y position for next item
                yPos += itemHeight + padding;
            }

            // Set the panel's auto-scroll
            panel1.AutoScroll = true;
        }

        private void CategoryButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            // Highlight the selected button
            HighlightSelectedCategoryButton(clickedButton);

            // Get the category name from the button's tag
            string categoryName = clickedButton.Tag.ToString();

            if (categoryName == ALL_ITEMS)
            {
                // Show all items
                DisplayMenuItems(allMenuItems);
            }
            else
            {
                // Find the category with this name
                Category selectedCategory = allCategories.FirstOrDefault(c =>
                    string.Equals(c.CategoryName, categoryName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(c.CategoryName + "s", categoryName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(c.CategoryName, categoryName + "s", StringComparison.OrdinalIgnoreCase));

                if (selectedCategory != null)
                {
                    // Get and display menu items for this category
                    FilterMenuItemsByCategory(selectedCategory.CategoryId);
                }
                else
                {
                    MessageBox.Show($"Category '{categoryName}' not found in the database.",
                        "Category Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void HighlightSelectedCategoryButton(Button selectedButton)
        {
            // Reset all category buttons to default appearance
            foreach (Control control in this.Controls)
            {
                if (control is Button button && button.Tag != null)
                {
                    button.BackColor = SystemColors.Control;
                    button.ForeColor = SystemColors.ControlText;
                    button.Font = new Font(button.Font, FontStyle.Regular);
                }
            }

            // Highlight the selected button
            selectedButton.BackColor = Color.FromArgb(60, 100, 240);
            selectedButton.ForeColor = Color.White;
            selectedButton.Font = new Font(selectedButton.Font, FontStyle.Bold);
        }

        private void FilterMenuItemsByCategory(int categoryId)
        {
            try
            {
                List<MenuItem> filteredItems;

                if (categoryId > 0)
                {
                    // Get items for this category
                    filteredItems = MenuItemRepo.GetMenuItemsByCategoryId(categoryId);
                }
                else
                {
                    // Show all items
                    filteredItems = allMenuItems;
                }

                // Display filtered items
                DisplayMenuItems(filteredItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering menu items: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterMenuItemsBySearch(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // If search is empty, show all items
                DisplayMenuItems(allMenuItems);
                return;
            }

            // Filter items based on search text
            string search = searchText.Trim().ToLower();

            var filteredItems = allMenuItems.Where(item =>
                (item.Name != null && item.Name.ToLower().Contains(search)) ||
                (item.Description != null && item.Description.ToLower().Contains(search)) ||
                (item.Category != null && item.Category.CategoryName != null &&
                 item.Category.CategoryName.ToLower().Contains(search)))
                .ToList();

            // Display filtered items
            DisplayMenuItems(filteredItems);

            // Reset all category button highlights
            foreach (Control control in this.Controls)
            {
                if (control is Button button && button.Tag != null)
                {
                    button.BackColor = SystemColors.Control;
                    button.ForeColor = SystemColors.ControlText;
                    button.Font = new Font(button.Font, FontStyle.Regular);
                }
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            MenuItem menuItem = (MenuItem)button.Tag;

            // Open the edit menu item form with the selected menu item
            using (AddMenuItem editForm = new AddMenuItem(menuItem))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // Refresh the menu items if successful
                    LoadAllMenuItems();
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            MenuItem menuItem = (MenuItem)button.Tag;

            // Confirm deletion
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete the menu item '{menuItem.Name}'?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Delete the menu item
                    bool success = MenuItemRepo.DeleteMenuItem(menuItem.MenuItemId);

                    if (success)
                    {
                        MessageBox.Show("Menu item deleted successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refresh the menu items
                        LoadAllMenuItems();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete menu item.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting menu item: {ex.Message}", "Database Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Event handler for the Add button
        private void button1_Click(object sender, EventArgs e)
        {
            // Open the add menu item form
            using (AddMenuItem addForm = new AddMenuItem())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    // Refresh the menu items if successful
                    LoadAllMenuItems();
                }
            }
        }

        // Event handler for the search textbox
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Filter menu items based on search text
            FilterMenuItemsBySearch(txtSearch.Text);
        }

        private void btnPasta_Click(object sender, EventArgs e)
        {

        }

        private void btnBreakfast_Click(object sender, EventArgs e)
        {

        }

        private void btnDessert_Click(object sender, EventArgs e)
        {

        }

        private void btnAll_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}