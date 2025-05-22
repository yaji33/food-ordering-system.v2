using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using food_ordering_system.v2.Data.Models;
using food_ordering_system.v2.Data.Repositories;
using MenuItem = food_ordering_system.v2.Data.Models.MenuItem;

namespace food_ordering_system.v2.UI.Customer
{
    public partial class PaymentConfirmationDialog : Form
    {
        private List<MenuItem> cartItems;
        private decimal totalAmount;
        private string selectedPaymentMethod = "Cash";
        private int customerId; // Added to keep track of the customer
        private int createdOrderId = -1; // To store the created order ID

        public PaymentConfirmationDialog(List<MenuItem> items, decimal total, int customerId)
        {
            InitializeComponent();
            cartItems = items;
            totalAmount = total;
            this.customerId = customerId;

            // Connect the Load event handler
            this.Load += PaymentConfirmationDialog_Load;
        }

        private void PaymentConfirmationDialog_Load(object sender, EventArgs e)
        {
            SetupDialogUI();
            PopulateOrderSummary();
        }

        private void SetupDialogUI()
        {
            // Dialog Title
            Label lblTitle = new Label
            {
                Text = "Order Summary & Payment",
                Font = new Font("Aeonik TRIAL", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(lblTitle);

            // Separator line
            Panel separatorLine = new Panel
            {
                Width = this.Width - 40,
                Height = 2,
                Location = new Point(20, lblTitle.Bottom + 10),
                BackColor = Color.LightGray
            };
            this.Controls.Add(separatorLine);

            // Order Summary Panel
            Panel orderSummaryPanel = new Panel
            {
                Location = new Point(20, separatorLine.Bottom + 10),
                Size = new Size(this.Width - 40, 250),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                Name = "orderSummaryPanel"
            };
            this.Controls.Add(orderSummaryPanel);

            // Create label for order summary header
            Label lblOrderSummary = new Label
            {
                Text = "Items:",
                Font = new Font("Aeonik TRIAL", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            orderSummaryPanel.Controls.Add(lblOrderSummary);

            // Separator line 2
            Panel separatorLine2 = new Panel
            {
                Width = this.Width - 40,
                Height = 2,
                Location = new Point(20, orderSummaryPanel.Bottom + 20),
                BackColor = Color.LightGray
            };
            this.Controls.Add(separatorLine2);

            // Payment Method Section
            Label lblPaymentMethod = new Label
            {
                Text = "Payment Method:",
                Font = new Font("Aeonik TRIAL", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, separatorLine2.Bottom + 20)
            };
            this.Controls.Add(lblPaymentMethod);

            // Payment Method Options
            RadioButton rbCash = new RadioButton
            {
                Text = "Cash",
                Font = new Font("Aeonik TRIAL", 10),
                Location = new Point(30, lblPaymentMethod.Bottom + 10),
                Checked = true,
                Tag = "Cash"
            };
            rbCash.CheckedChanged += PaymentMethod_CheckedChanged;
            this.Controls.Add(rbCash);

            RadioButton rbCreditCard = new RadioButton
            {
                Text = "Credit Card",
                Font = new Font("Aeonik TRIAL", 10),
                Location = new Point(30, rbCash.Bottom + 10),
                Tag = "Credit Card"
            };
            rbCreditCard.CheckedChanged += PaymentMethod_CheckedChanged;
            this.Controls.Add(rbCreditCard);

            RadioButton rbDebitCard = new RadioButton
            {
                Text = "Debit Card",
                Font = new Font("Aeonik TRIAL", 10),
                Location = new Point(30, rbCreditCard.Bottom + 10),
                Tag = "Debit Card"
            };
            rbDebitCard.CheckedChanged += PaymentMethod_CheckedChanged;
            this.Controls.Add(rbDebitCard);

            RadioButton rbEWallet = new RadioButton
            {
                Text = "E-Wallet (GCash, Maya, etc.)",
                Font = new Font("Aeonik TRIAL", 10),
                Location = new Point(30, rbDebitCard.Bottom + 10),
                Tag = "E-Wallet"
            };
            rbEWallet.CheckedChanged += PaymentMethod_CheckedChanged;
            this.Controls.Add(rbEWallet);

          

            // Total Amount Label
            Label lblTotalCaption = new Label
            {
                Text = "Total Amount:",
                Font = new Font("Aeonik TRIAL", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, rbEWallet.Bottom + 30)
            };
            this.Controls.Add(lblTotalCaption);

            Label lblTotalAmount = new Label
            {
                Text = $"₱{totalAmount:0.00}",
                Font = new Font("Aeonik TRIAL", 14, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.DarkGreen,
                Location = new Point(this.Width - 150, lblTotalCaption.Top)
            };
            this.Controls.Add(lblTotalAmount);

            // Buttons
            Button btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(100, 40),
                Location = new Point(20, this.Height - 85),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);

            Button btnConfirm = new Button
            {
                Text = "Confirm Payment",
                Size = new Size(150, 40),
                Location = new Point(this.Width - 170, this.Height - 85),
                BackColor = Color.FromArgb(46, 182, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.Click += BtnConfirm_Click;
            this.Controls.Add(btnConfirm);
        }

        private void PopulateOrderSummary()
        {
            // Get the order summary panel
            Panel orderSummaryPanel = this.Controls.Find("orderSummaryPanel", true).FirstOrDefault() as Panel;
            if (orderSummaryPanel == null) return;

            // Start Y position (after the header)
            int yPos = 40;
            const int padding = 5;
            const int itemHeight = 60;

            // Display each item in the cart
            foreach (MenuItem item in cartItems)
            {
                int quantity = (int)item.Tag;
                decimal itemTotal = item.Price * quantity;

                // Item Container
                Panel itemPanel = new Panel
                {
                    Width = orderSummaryPanel.Width - 20,
                    Height = itemHeight,
                    Location = new Point(10, yPos),
                    BorderStyle = BorderStyle.None
                };

                // Item name and quantity
                Label nameLabel = new Label
                {
                    Text = $"{quantity} x {item.Name}",
                    Font = new Font("Aeonik TRIAL", 10, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(10, 10)
                };
                itemPanel.Controls.Add(nameLabel);

                // Item price
                Label priceLabel = new Label
                {
                    Text = $"₱{item.Price:0.00} each",
                    Font = new Font("Aeonik TRIAL", 9),
                    AutoSize = true,
                    ForeColor = Color.Gray,
                    Location = new Point(10, 30)
                };
                itemPanel.Controls.Add(priceLabel);

                // Item total
                Label totalLabel = new Label
                {
                    Text = $"₱{itemTotal:0.00}",
                    Font = new Font("Aeonik TRIAL", 10, FontStyle.Bold),
                    AutoSize = true,
                    ForeColor = Color.DarkGreen,
                    Location = new Point(itemPanel.Width - 100, 20)
                };
                itemPanel.Controls.Add(totalLabel);

                // Add a separator line
                Panel linePanel = new Panel
                {
                    Width = itemPanel.Width - 10,
                    Height = 1,
                    Location = new Point(5, itemHeight - 1),
                    BackColor = Color.LightGray
                };
                itemPanel.Controls.Add(linePanel);

                // Add the item panel to the order summary panel
                orderSummaryPanel.Controls.Add(itemPanel);

                // Increment Y position for next item
                yPos += itemHeight + padding;
            }
        }

        private void PaymentMethod_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                selectedPaymentMethod = rb.Tag.ToString();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                
                createdOrderId = OrderRepo.CreateOrder(customerId, totalAmount, "Pending");

                if (createdOrderId <= 0)
                {
                    MessageBox.Show("Failed to create order. Please try again.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create list of order items for insertion
                List<OrderItem> orderItems = new List<OrderItem>();
                foreach (MenuItem item in cartItems)
                {
                    int quantity = (int)item.Tag;
                    orderItems.Add(new OrderItem
                    {
                        OrderId = createdOrderId,
                        MenuItemId = item.MenuItemId,
                        Quantity = quantity,
                        MenuItem = item
                    });
                }

                // Add order items to the database
                OrderRepo.AddOrderItems(createdOrderId, orderItems);

               
                int paymentId = PaymentRepo.CreatePayment(createdOrderId, totalAmount, selectedPaymentMethod);

                if (paymentId <= 0)
                {
                    MessageBox.Show("Order created, but payment record could not be saved. Please contact support.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Update order status to reflect payment
                string initialOrderStatus = selectedPaymentMethod.Equals("Cash", StringComparison.OrdinalIgnoreCase)
                    ? "Pending Payment"
                    : "Pending";

                OrderRepo.UpdateOrderStatus(createdOrderId, initialOrderStatus);

                // Customize success message based on payment method
                string paymentMessage = selectedPaymentMethod.Equals("Cash", StringComparison.OrdinalIgnoreCase)
                    ? "Your order will be processed once payment is confirmed."
                    : "Your order has been confirmed and is being processed.";

                // Show success message
                MessageBox.Show($"Thank you for your order!\n\nOrder #: {createdOrderId}\nPayment Method: {selectedPaymentMethod}\nTotal Amount: ₱{totalAmount:0.00}\n\n{paymentMessage}",
                    "Order Confirmed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while processing your order: {ex.Message}\n\nFull error details: {ex.ToString()}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // If order was created but something failed afterwards, update status to reflect issue
                if (createdOrderId > 0)
                {
                    try
                    {
                        OrderRepo.UpdateOrderStatus(createdOrderId, "Payment Failed");
                    }
                    catch
                    {
                      
                    }
                }
            }
        }
    }
}