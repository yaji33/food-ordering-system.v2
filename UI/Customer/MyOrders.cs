using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using food_ordering_system.v2.Data.Models;
using food_ordering_system.v2.Data.Repositories;

namespace food_ordering_system.v2.UI.Customer
{
    public partial class MyOrders : UserControl
    {
        private int _currentCustomerId;
        private List<Order> _customerOrders;
        private Order _selectedOrder;

        public MyOrders(int customerId)
        {
            InitializeComponent();
            _currentCustomerId = customerId;
            InitializeCustomComponents();
        }

        public void SetCustomerId(int customerId)
        {
            _currentCustomerId = customerId;
            RefreshOrdersList();
        }

        private void InitializeCustomComponents()
        {
            // Main title
            lblTitle = new Label
            {
                Text = "My Orders",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            Controls.Add(lblTitle);

            // Orders list view
            lvOrders = new ListView
            {
                Location = new Point(20, 60),
                Size = new Size(400, 500),
                View = View.Details,
                FullRowSelect = true,
                HideSelection = false,
                Font = new Font("Segoe UI", 10)
            };

            // Add columns to ListView
            lvOrders.Columns.Add("Order #", 70);
            lvOrders.Columns.Add("Date", 120);
            lvOrders.Columns.Add("Total", 80);
            lvOrders.Columns.Add("Status", 110);

            lvOrders.SelectedIndexChanged += LvOrders_SelectedIndexChanged;
            Controls.Add(lvOrders);

            // Order details panel
            detailsPanel = new Panel
            {
                Location = new Point(440, 60),
                Size = new Size(400, 500),
                BorderStyle = BorderStyle.None
            };
            Controls.Add(detailsPanel);

            // Order details header
            lblOrderHeader = new Label
            {
                Text = "Order Details",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = true,
                Location = new Point(0, 0)
            };
            detailsPanel.Controls.Add(lblOrderHeader);

            // Order info 
            lblOrderInfo = new Label
            {
                AutoSize = false,
                Size = new Size(400, 80),
                Location = new Point(0, 40),
                Font = new Font("Segoe UI", 10)
            };
            detailsPanel.Controls.Add(lblOrderInfo);

            // Order items list
            lvOrderItems = new ListView
            {
                Location = new Point(0, 130),
                Size = new Size(400, 370),
                View = View.Details,
                FullRowSelect = true,
                Font = new Font("Segoe UI", 10)
            };

            // Add columns to order items ListView
            lvOrderItems.Columns.Add("Item", 200);
            lvOrderItems.Columns.Add("Qty", 50);
            lvOrderItems.Columns.Add("Price", 70);
            lvOrderItems.Columns.Add("Subtotal", 80);

            detailsPanel.Controls.Add(lvOrderItems);

            // Refresh button - smaller and more subtle
            btnRefresh = new Button
            {
                Text = "↻ Refresh",
                Size = new Size(90, 30),
                Location = new Point(330, 15),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnRefresh.FlatAppearance.BorderSize = 1;
            btnRefresh.Click += BtnRefresh_Click;
            Controls.Add(btnRefresh);

            // Status message
            lblStatus = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(20, 570)
            };
            Controls.Add(lblStatus);
        }

        private void RefreshOrdersList()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _customerOrders = OrderRepo.GetOrdersByCustomerId(_currentCustomerId);

                lvOrders.Items.Clear();
                lvOrderItems.Items.Clear();
                lblOrderInfo.Text = "";
                lblOrderHeader.Text = "Order Details";

                foreach (var order in _customerOrders)
                {
                    ListViewItem item = new ListViewItem(order.OrderId.ToString());
                    item.SubItems.Add(order.OrderDate.ToString("MMM dd, HH:mm"));
                    item.SubItems.Add("₱" + order.TotalPrice.ToString("F0"));
                    item.SubItems.Add(order.OrderStatus);
                    item.Tag = order.OrderId;

                    // Set item colors based on status
                    switch (order.OrderStatus.ToLower())
                    {
                        case "pending":
                            item.ForeColor = Color.Blue;
                            break;
                        case "processing":
                            item.ForeColor = Color.Orange;
                            break;
                        case "completed":
                        case "delivered":
                            item.ForeColor = Color.Green;
                            break;
                        case "cancelled":
                            item.ForeColor = Color.Red;
                            break;
                    }

                    lvOrders.Items.Add(item);
                }

                if (_customerOrders.Count == 0)
                {
                    lblStatus.Text = "You have no orders yet.";
                }
                else
                {
                    lblStatus.Text = $"Found {_customerOrders.Count} order(s). Select an order to view details.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load orders. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Could not load orders.";
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void LvOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvOrders.SelectedItems.Count == 0)
                return;

            int orderId = Convert.ToInt32(lvOrders.SelectedItems[0].Tag);
            LoadOrderDetails(orderId);
        }

        private void LoadOrderDetails(int orderId)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _selectedOrder = OrderRepo.GetOrderById(orderId);

                if (_selectedOrder != null)
                {
                    lblOrderHeader.Text = $"Order #{_selectedOrder.OrderId}";

                    lblOrderInfo.Text = $"Date: {_selectedOrder.OrderDate:MMM dd, yyyy HH:mm}\n" +
                                        $"Status: {_selectedOrder.OrderStatus}\n" +
                                        $"Total: ₱{_selectedOrder.TotalPrice:F2}";

                    lvOrderItems.Items.Clear();

                    foreach (var item in _selectedOrder.OrderItems)
                    {
                        ListViewItem lvi = new ListViewItem(item.MenuItem.Name);
                        lvi.SubItems.Add(item.Quantity.ToString());
                        lvi.SubItems.Add("₱" + item.MenuItem.Price.ToString("F0"));
                        lvi.SubItems.Add("₱" + (item.Quantity * item.MenuItem.Price).ToString("F0"));
                        lvOrderItems.Items.Add(lvi);
                    }

                    lblStatus.Text = $"Viewing Order #{_selectedOrder.OrderId}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load order details.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshOrdersList();
        }

        private void MyOrders_Load(object sender, EventArgs e)
        {
            RefreshOrdersList();
        }

        // Designer variables
        private Label lblTitle;
        private Button btnRefresh;
        private ListView lvOrders;
        private Panel detailsPanel;
        private Label lblOrderHeader;
        private Label lblOrderInfo;
        private ListView lvOrderItems;
        private Label lblStatus;
    }
}