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
using Orders = food_ordering_system.v2.Data.Models.Order;

namespace food_ordering_system.v2.UI.Customer
{
    public partial class MenuPage : UserControl
    {
        private List<MenuItem> allMenuItems;
        
        private List<Category> allCategories;
        private Dictionary<int, Button> categoryButtons;
        private List<MenuItem> cartItems;
        private const string ALL_ITEMS = "All";
        private decimal totalAmount = 0;
        private TextBox txtSearch;
        private int currentCustomerId;

        public MenuPage(int customerId)
        {
            InitializeComponent();

            currentCustomerId = customerId;


            // Initialize cart items list
            cartItems = new List<MenuItem>();

            // Setup event handlers for loading controls
            this.Load += MenuPage_Load;

            // Initialize search box
            SetupSearchBox();

            // Setup existing category buttons
            SetupCategoryButtons();
            
        }

        private void SetupSearchBox()
        {
            // Create search text box and add it to panel1
            txtSearch = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(20, 20),
                Font = new Font("Aeonik TRIAL", 10),
                //PlaceholderText = "Search menu items...",
                Name = "txtSearch"
            };

            txtSearch.TextChanged += TxtSearch_TextChanged;
            panel1.Controls.Add(txtSearch);
        }

        private void SetupCategoryButtons()
        {
            categoryButtons = new Dictionary<int, Button>();

            // Style existing category buttons
            StyleCategoryButton(btnAll, ALL_ITEMS, true); // Default selected
            StyleCategoryButton(btnRiceMeal, "Rice Meal", false);
            StyleCategoryButton(btnBreakfast, "Breakfast", false);
            StyleCategoryButton(btnChicken, "Chicken Specialties", false);
            StyleCategoryButton(btnPasta, "Pasta", false);
            StyleCategoryButton(btnCombo, "Combo Meals", false);
            StyleCategoryButton(btnSnack, "Snacks", false);
            StyleCategoryButton(btnDessert, "Desserts", false);
            StyleCategoryButton(btnBeverage, "Beverages", false);

            // Add click handlers
            btnAll.Click += CategoryButton_Click;
            btnRiceMeal.Click += CategoryButton_Click;
            btnBreakfast.Click += CategoryButton_Click;
            btnChicken.Click += CategoryButton_Click;
            btnPasta.Click += CategoryButton_Click;
            btnCombo.Click += CategoryButton_Click;
            btnSnack.Click += CategoryButton_Click;
            btnDessert.Click += CategoryButton_Click;
            btnBeverage.Click += CategoryButton_Click;
        }

        private void StyleCategoryButton(Button button, string categoryName, bool isSelected)
        {
            button.Tag = categoryName;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("Aeonik TRIAL", 10, isSelected ? FontStyle.Bold : FontStyle.Regular);

            if (isSelected)
            {
                button.BackColor = Color.FromArgb(255, 195, 100);
                button.ForeColor = Color.White;
            }
            else
            {
                button.BackColor = SystemColors.Control;
                button.ForeColor = SystemColors.ControlText;
            }

            button.FlatAppearance.BorderSize = 1;
        }

        private void MenuPage_Load(object sender, EventArgs e)
        {
            // Load all categories
            LoadCategories();

            // Load all menu items
            LoadAllMenuItems();

            // Initialize cart panel
            InitializeCartPanel();
        }

        private void LoadCategories()
        {
            try
            {
                // Get all categories from the database
                allCategories = CategoryRepo.GetAllCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            // Clear panel3 (the menu items display area)
            panel3.Controls.Clear();

            // If no menu items, show a message
            if (menuItems == null || menuItems.Count == 0)
            {
                Label noItemsLabel = new Label
                {
                    Text = "No menu items found",
                    Font = new Font("Aeonik TRIAL", 12, FontStyle.Regular),
                    AutoSize = true,
                    Location = new Point(20, 20),
                    ForeColor = Color.Gray
                };
                panel3.Controls.Add(noItemsLabel);
                return;
            }

            // Create a scrollable panel for menu items
            Panel scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10)
            };
            panel3.Controls.Add(scrollPanel);

            // Create a panel for each menu item
            int yPos = 10;
            const int padding = 10;
            const int itemHeight = 130;

            foreach (MenuItem item in menuItems)
            {
                // Create a panel for the menu item
                Panel itemPanel = new Panel
                {
                    Width = scrollPanel.Width - 40,
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

                // Add quantity controls
                NumericUpDown quantityPicker = new NumericUpDown
                {
                    Minimum = 1,
                    Maximum = 99,
                    Value = 1,
                    Size = new Size(60, 30),
                    Location = new Point(itemPanel.Width - 220, itemHeight - 40),
                    Tag = item
                };
                itemPanel.Controls.Add(quantityPicker);

                // Add "Add to Cart" button
                Button addToCartButton = new Button
                {
                    Text = "Add to Cart",
                    Size = new Size(120, 30),
                    Location = new Point(itemPanel.Width - 150, itemHeight - 40),
                    Tag = new object[] { item, quantityPicker },
                    BackColor = Color.FromArgb(46, 182, 125),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                addToCartButton.FlatAppearance.BorderSize = 0;
                addToCartButton.Click += AddToCartButton_Click;
                itemPanel.Controls.Add(addToCartButton);

                // Add to panel
                scrollPanel.Controls.Add(itemPanel);

                // Increment Y position for next item
                yPos += itemHeight + padding;
            }
        }

        private void InitializeCartPanel()
        {
            // Clear existing controls in panel2 except label1
            var controlsToRemove = panel2.Controls.Cast<Control>().Where(c => c != label1).ToList();
            foreach (Control control in controlsToRemove)
            {
                panel2.Controls.Remove(control);
            }

            // Add clear cart button
            Button clearCartButton = new Button
            {
                Text = "Clear Cart",
                Size = new Size(100, 30),
                Location = new Point(panel2.Width - 120, 15),
                BackColor = Color.MistyRose,
                FlatStyle = FlatStyle.Flat
            };
            clearCartButton.FlatAppearance.BorderSize = 0;
            clearCartButton.Click += ClearCartButton_Click;
            panel2.Controls.Add(clearCartButton);

            // Add cart items panel
            Panel cartItemsPanel = new Panel
            {
                Location = new Point(10, 60),
                Size = new Size(panel2.Width - 20, panel2.Height - 170),
                AutoScroll = true,
                BorderStyle = BorderStyle.None,
                Name = "cartItemsPanel"
            };
            panel2.Controls.Add(cartItemsPanel);

            // Add total section
            Panel totalPanel = new Panel
            {
                Location = new Point(10, panel2.Height - 100),
                Size = new Size(panel2.Width - 20, 40),
                BorderStyle = BorderStyle.None
            };

            Label totalLabel = new Label
            {
                Text = "Total:",
                Font = new Font("Aeonik TRIAL", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            totalPanel.Controls.Add(totalLabel);

            Label totalAmountLabel = new Label
            {
                Text = $"₱{totalAmount:0.00}",
                Font = new Font("Aeonik TRIAL", 12, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.DarkGreen,
                Location = new Point(totalPanel.Width - 120, 10),
                Name = "totalAmountLabel"
            };
            totalPanel.Controls.Add(totalAmountLabel);
            panel2.Controls.Add(totalPanel);

            // Add checkout button
            Button checkoutButton = new Button
            {
                Text = "Proceed to Checkout",
                Size = new Size(panel2.Width - 40, 40),
                Location = new Point(20, panel2.Height - 50),
                BackColor = Color.FromArgb(255, 165, 52),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Aeonik TRIAL", 10, FontStyle.Bold)
            };
            checkoutButton.FlatAppearance.BorderSize = 0;
            checkoutButton.Click += CheckoutButton_Click;
            panel2.Controls.Add(checkoutButton);

            // Initialize with empty cart message
            UpdateCartUI();
        }

        private void CategoryButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            // Reset all category buttons
            ResetAllCategoryButtons();

            // Highlight the clicked button
            clickedButton.BackColor = Color.FromArgb(255, 195, 100);
            clickedButton.ForeColor = Color.White;
            clickedButton.Font = new Font(clickedButton.Font, FontStyle.Bold);

            // Get the category name from the button's tag
            string categoryName = clickedButton.Tag.ToString();

            if (categoryName == ALL_ITEMS)
            {
                // Show all menu items
                DisplayMenuItems(allMenuItems);
            }
            else
            {
                // Find the category with this name
                Category selectedCategory = allCategories?.FirstOrDefault(c =>
                    string.Equals(c.CategoryName, categoryName, StringComparison.OrdinalIgnoreCase));

                if (selectedCategory != null)
                {
                    // Get items for this category
                    List<MenuItem> filteredItems = MenuItemRepo.GetMenuItemsByCategoryId(selectedCategory.CategoryId);
                    DisplayMenuItems(filteredItems);
                }
                else
                {
                    // Filter from loaded items if category not found in database
                    var filteredItems = allMenuItems?.Where(item =>
                        item.Category?.CategoryName != null &&
                        string.Equals(item.Category.CategoryName, categoryName, StringComparison.OrdinalIgnoreCase))
                        .ToList() ?? new List<MenuItem>();
                    DisplayMenuItems(filteredItems);
                }
            }

            // Clear search box when category is selected
            if (txtSearch != null)
            {
                txtSearch.Clear();
            }
        }

        private void ResetAllCategoryButtons()
        {
            Button[] buttons = { btnAll, btnRiceMeal, btnBreakfast, btnChicken, btnPasta, btnCombo, btnSnack, btnDessert, btnBeverage };

            foreach (Button button in buttons)
            {
                button.BackColor = SystemColors.Control;
                button.ForeColor = SystemColors.ControlText;
                button.Font = new Font(button.Font, FontStyle.Regular);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            TextBox searchBox = (TextBox)sender;
            string searchText = searchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                // If search is empty, show all items
                DisplayMenuItems(allMenuItems);
                return;
            }

            // Filter items based on search text
            var filteredItems = allMenuItems?.Where(item =>
                (item.Name != null && item.Name.ToLower().Contains(searchText)) ||
                (item.Description != null && item.Description.ToLower().Contains(searchText)) ||
                (item.Category != null && item.Category.CategoryName != null &&
                 item.Category.CategoryName.ToLower().Contains(searchText)))
                .ToList() ?? new List<MenuItem>();

            // Display filtered items
            DisplayMenuItems(filteredItems);

            // Reset category button highlights when searching
            ResetAllCategoryButtons();
        }

        private void AddToCartButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            object[] tag = (object[])button.Tag;

            MenuItem menuItem = (MenuItem)tag[0];
            NumericUpDown quantityPicker = (NumericUpDown)tag[1];
            int quantity = (int)quantityPicker.Value;

            // Check if item is already in cart
            MenuItem existingItem = cartItems.FirstOrDefault(item => item.MenuItemId == menuItem.MenuItemId);

            if (existingItem != null)
            {
                // Update the quantity in the cart
                int currentQuantity = (int)existingItem.Tag;
                existingItem.Tag = currentQuantity + quantity;
            }
            else
            {
                // Create a copy of the menu item with quantity
                MenuItem cartItem = new MenuItem
                {
                    MenuItemId = menuItem.MenuItemId,
                    Name = menuItem.Name,
                    Description = menuItem.Description,
                    Price = menuItem.Price,
                    CategoryId = menuItem.CategoryId,
                    Category = menuItem.Category,
                    Tag = quantity  // Store quantity in Tag property
                };

                cartItems.Add(cartItem);
            }

            // Update the cart UI
            UpdateCartUI();

            // Display confirmation message
            MessageBox.Show($"{quantity} x {menuItem.Name} added to cart.", "Added to Cart",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Reset quantity to 1
            quantityPicker.Value = 1;
        }

        private void RemoveFromCartButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            MenuItem menuItem = (MenuItem)button.Tag;

            // Remove the item from cart
            cartItems.RemoveAll(item => item.MenuItemId == menuItem.MenuItemId);

            // Update the cart UI
            UpdateCartUI();
        }

        private void ClearCartButton_Click(object sender, EventArgs e)
        {
            // Clear the cart items
            cartItems.Clear();

            // Update the cart UI
            UpdateCartUI();
        }

        private void UpdateCartUI()
        {
            // Get the cart items panel
            Panel cartItemsPanel = panel2.Controls["cartItemsPanel"] as Panel;
            if (cartItemsPanel == null) return;

            // Clear the panel
            cartItemsPanel.Controls.Clear();

            // Reset total
            totalAmount = 0;

            // If cart is empty, show message
            if (cartItems.Count == 0)
            {
                Label emptyCartLabel = new Label
                {
                    Text = "Your cart is empty",
                    Font = new Font("Aeonik TRIAL", 10, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(10, 20)
                };
                cartItemsPanel.Controls.Add(emptyCartLabel);
            }
            else
            {
                // Create a panel for each cart item
                int yPos = 10;
                const int padding = 5;
                const int itemHeight = 80;

                foreach (MenuItem item in cartItems)
                {
                    int quantity = (int)item.Tag;
                    decimal itemTotal = item.Price * quantity;
                    totalAmount += itemTotal;

                    // Create a panel for the cart item
                    Panel itemPanel = new Panel
                    {
                        Width = cartItemsPanel.Width - 10,
                        Height = itemHeight,
                        Location = new Point(0, yPos),
                        BorderStyle = BorderStyle.None,
                        BackColor = Color.White
                    };

                    // Add horizontal line at the top
                    Panel linePanel = new Panel
                    {
                        Width = itemPanel.Width,
                        Height = 1,
                        Location = new Point(0, 0),
                        BackColor = Color.LightGray
                    };
                    itemPanel.Controls.Add(linePanel);

                    // Add quantity and name label
                    Label nameLabel = new Label
                    {
                        Text = $"{quantity} x {item.Name}",
                        Font = new Font("Aeonik TRIAL", 10, FontStyle.Bold),
                        AutoSize = true,
                        Location = new Point(10, 10)
                    };
                    itemPanel.Controls.Add(nameLabel);

                    // Add price label
                    Label priceLabel = new Label
                    {
                        Text = $"₱{item.Price:0.00} each",
                        Font = new Font("Aeonik TRIAL", 9, FontStyle.Regular),
                        AutoSize = true,
                        ForeColor = Color.DarkGray,
                        Location = new Point(10, 30)
                    };
                    itemPanel.Controls.Add(priceLabel);

                    // Add item total label
                    Label totalLabel = new Label
                    {
                        Text = $"₱{itemTotal:0.00}",
                        Font = new Font("Aeonik TRIAL", 10, FontStyle.Bold),
                        AutoSize = true,
                        ForeColor = Color.DarkGreen,
                        Location = new Point(itemPanel.Width - 100, 20)
                    };
                    itemPanel.Controls.Add(totalLabel);

                    // Add remove button
                    Button removeButton = new Button
                    {
                        Text = "X",
                        Size = new Size(25, 25),
                        Location = new Point(itemPanel.Width - 30, 17),
                        Tag = item,
                        BackColor = Color.MistyRose,
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Aeonik TRIAL", 8, FontStyle.Bold)
                    };
                    removeButton.FlatAppearance.BorderSize = 0;
                    removeButton.Click += RemoveFromCartButton_Click;
                    itemPanel.Controls.Add(removeButton);

                    // Add quantity controls
                    Label qtyLabel = new Label
                    {
                        Text = "Qty:",
                        Font = new Font("Aeonik TRIAL", 8, FontStyle.Regular),
                        AutoSize = true,
                        Location = new Point(10, 55)
                    };
                    itemPanel.Controls.Add(qtyLabel);

                    NumericUpDown qtyPicker = new NumericUpDown
                    {
                        Minimum = 1,
                        Maximum = 99,
                        Value = quantity,
                        Size = new Size(50, 25),
                        Location = new Point(40, 52),
                        Tag = item
                    };
                    qtyPicker.ValueChanged += CartQuantity_ValueChanged;
                    itemPanel.Controls.Add(qtyPicker);

                    // Add to panel
                    cartItemsPanel.Controls.Add(itemPanel);

                    // Increment Y position for next item
                    yPos += itemHeight + padding;
                }
            }

            // Update total amount display
            Label totalAmountLabel = panel2.Controls.Find("totalAmountLabel", true).FirstOrDefault() as Label;
            if (totalAmountLabel != null)
            {
                totalAmountLabel.Text = $"₱{totalAmount:0.00}";
            }
        }

        private void CartQuantity_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown qtyPicker = (NumericUpDown)sender;
            MenuItem item = (MenuItem)qtyPicker.Tag;

            // Update quantity
            item.Tag = (int)qtyPicker.Value;

            // Update cart UI
            UpdateCartUI();
        }

        private void CheckoutButton_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Your cart is empty. Please add items before checkout.",
                    "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Create a deep copy of cart items to pass to the dialog
            List<MenuItem> cartItemsCopy = cartItems.Select(item => new MenuItem
            {
                MenuItemId = item.MenuItemId,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                CategoryId = item.CategoryId,
                Category = item.Category,
                Tag = item.Tag  // Copy the quantity
            }).ToList();

            // Show the payment confirmation dialog
            using (PaymentConfirmationDialog paymentDialog = new PaymentConfirmationDialog(cartItemsCopy, totalAmount, currentCustomerId))
            {
                DialogResult result = paymentDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Payment was confirmed, clear the cart
                    cartItems.Clear();
                    UpdateCartUI();
                }
                // If canceled, do nothing and keep the cart as is
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            // Already implemented in the designer
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            // Panel for menu items display
        }

        // These event handlers are now handled by CategoryButton_Click
        private void btnAll_Click(object sender, EventArgs e)
        {
            // Handled by CategoryButton_Click
        }

        private void btnRiceMeal_Click(object sender, EventArgs e)
        {
            // Handled by CategoryButton_Click
        }

        private void btnBreakfast_Click(object sender, EventArgs e)
        {
            // Handled by CategoryButton_Click
        }

        private void btnChicken_Click(object sender, EventArgs e)
        {
            // Handled by CategoryButton_Click
        }

        private void btnPasta_Click(object sender, EventArgs e)
        {
            // Handled by CategoryButton_Click
        }

        private void btnCombo_Click(object sender, EventArgs e)
        {
            // Handled by CategoryButton_Click
        }

        private void btnSnack_Click(object sender, EventArgs e)
        {
            // Handled by CategoryButton_Click
        }

        private void btnDessert_Click(object sender, EventArgs e)
        {
            // Handled by CategoryButton_Click
        }

        private void btnBeverage_Click(object sender, EventArgs e)
        {
            // Handled by CategoryButton_Click
        }
    }
}