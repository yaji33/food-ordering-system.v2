using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using food_ordering_system.v2.Data;
using food_ordering_system.v2.Data.Models;
using food_ordering_system.v2.Data.Repositories;
using MySql.Data.MySqlClient;

namespace food_ordering_system.v2.UI.Admin
{
    public partial class OrderManagement : UserControl
    {
        private List<Order> _allOrders;
        private Order _selectedOrder;
        private Dictionary<int, bool> _orderPaymentStatus = new Dictionary<int, bool>();
        private Payment _selectedOrderPayment;

        public OrderManagement()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
      
            lblTitle = new Label
            {
                Text = "Order Management",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = true,
                Location = new Point(270, 15) 
            };
            Controls.Add(lblTitle);

            // Filter by status
            lblFilter = new Label
            {
                Text = "Filter by Status:",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(270, 55)  
            };
            Controls.Add(lblFilter);

            cmbStatusFilter = new ComboBox
            {
                Location = new Point(380, 52),  
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbStatusFilter.Items.AddRange(new string[] { "All", "Pending", "Processing", "Completed", "Cancelled" });
            cmbStatusFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndexChanged += CmbStatusFilter_SelectedIndexChanged;
            Controls.Add(cmbStatusFilter);

            // Payment filter
            lblPaymentFilter = new Label
            {
                Text = "Payment:",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(520, 55)  
            };
            Controls.Add(lblPaymentFilter);

            cmbPaymentFilter = new ComboBox
            {
                Location = new Point(580, 52),  
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbPaymentFilter.Items.AddRange(new string[] { "All", "Paid", "Unpaid", "Pending Payment" });
            cmbPaymentFilter.SelectedIndex = 0;
            cmbPaymentFilter.SelectedIndexChanged += CmbPaymentFilter_SelectedIndexChanged;
            Controls.Add(cmbPaymentFilter);

            // Refresh button
            btnRefresh = new Button
            {
                Text = "↻ Refresh",
                Size = new Size(90, 30),
                Location = new Point(700, 50),  
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(240, 240, 240)
            };
            btnRefresh.FlatAppearance.BorderSize = 1;
            btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnRefresh.Click += BtnRefresh_Click;
            Controls.Add(btnRefresh);

            
            lvOrders = new ListView
            {
                Location = new Point(270, 90), 
                Size = new Size(520, 450),  
                View = View.Details,
                FullRowSelect = true,
                HideSelection = false,
                Font = new Font("Segoe UI", 10),
                GridLines = true
            };

            
            lvOrders.Columns.Add("Order #", 65);
            lvOrders.Columns.Add("Customer ID", 80);
            lvOrders.Columns.Add("Date", 120);
            lvOrders.Columns.Add("Total", 80);
            lvOrders.Columns.Add("Status", 90);
            lvOrders.Columns.Add("Payment", 85); 

            lvOrders.SelectedIndexChanged += LvOrders_SelectedIndexChanged;
            Controls.Add(lvOrders);

           
            detailsPanel = new Panel
            {
                Location = new Point(810, 90),  
                Size = new Size(450, 450),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 248, 248)
            };
            Controls.Add(detailsPanel);

            // Order details header
            lblOrderHeader = new Label
            {
                Text = "Select an order to view details",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            detailsPanel.Controls.Add(lblOrderHeader);

            // Order info 
            lblOrderInfo = new Label
            {
                AutoSize = false,
                Size = new Size(430, 100), 
                Location = new Point(10, 40),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.Transparent
            };
            detailsPanel.Controls.Add(lblOrderInfo);

            // Payment details section
            lblPaymentInfo = new Label
            {
                AutoSize = false,
                Size = new Size(430, 40),
                Location = new Point(10, 140),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            detailsPanel.Controls.Add(lblPaymentInfo);

            
            btnPaymentAction = new Button
            {
                Text = "View Payment Details",
                Location = new Point(10, 180),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnPaymentAction.FlatAppearance.BorderSize = 0;
            btnPaymentAction.Click += BtnPaymentAction_Click;
            detailsPanel.Controls.Add(btnPaymentAction);

            // Status update section
            lblStatusUpdate = new Label
            {
                Text = "Update Status:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 220)
            };
            detailsPanel.Controls.Add(lblStatusUpdate);

            cmbNewStatus = new ComboBox
            {
                Location = new Point(110, 220),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbNewStatus.Items.AddRange(new string[] { "Pending", "Processing", "Completed", "Cancelled" });
            detailsPanel.Controls.Add(cmbNewStatus);

            btnUpdateStatus = new Button
            {
                Text = "Update Status",
                Location = new Point(240, 220),
                Size = new Size(110, 27),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnUpdateStatus.FlatAppearance.BorderSize = 0;
            btnUpdateStatus.Click += BtnUpdateStatus_Click;
            detailsPanel.Controls.Add(btnUpdateStatus);

            // Order items list
            lblOrderItems = new Label
            {
                Text = "Order Items:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 255)
            };
            detailsPanel.Controls.Add(lblOrderItems);

            lvOrderItems = new ListView
            {
                Location = new Point(10, 280),
                Size = new Size(430, 160),
                View = View.Details,
                FullRowSelect = true,
                Font = new Font("Segoe UI", 9),
                GridLines = true
            };

            // Add columns to order items ListView
            lvOrderItems.Columns.Add("Item", 200);
            lvOrderItems.Columns.Add("Qty", 50);
            lvOrderItems.Columns.Add("Price", 80);
            lvOrderItems.Columns.Add("Subtotal", 90);

            detailsPanel.Controls.Add(lvOrderItems);

        
            lblStatus = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(270, 550) 
            };
            Controls.Add(lblStatus);
        }

        private void RefreshOrdersList()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _allOrders = OrderRepo.GetAllOrders();

                // Reset the payment status dictionary
                _orderPaymentStatus.Clear();

                // Check payment status for each order
                foreach (var order in _allOrders)
                {
                    bool isPaid = PaymentRepo.IsOrderPaid(order.OrderId);
                    _orderPaymentStatus[order.OrderId] = isPaid;
                }

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load orders: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Could not load orders.";
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ApplyFilters()
        {
            lvOrders.Items.Clear();
            lvOrderItems.Items.Clear();
            lblOrderInfo.Text = "";
            lblPaymentInfo.Text = "";
            lblOrderHeader.Text = "Select an order to view details";
            cmbNewStatus.SelectedIndex = -1;
            _selectedOrderPayment = null;

            string selectedStatusFilter = cmbStatusFilter.SelectedItem?.ToString() ?? "All";
            string selectedPaymentFilter = cmbPaymentFilter.SelectedItem?.ToString() ?? "All";

            // Filter orders based on status
            var filteredOrders = selectedStatusFilter == "All"
                ? _allOrders
                : _allOrders.Where(o => o.OrderStatus.Equals(selectedStatusFilter, StringComparison.OrdinalIgnoreCase)).ToList();

            // Further filter by payment status
            if (selectedPaymentFilter != "All")
            {
                if (selectedPaymentFilter == "Paid")
                {
                    filteredOrders = filteredOrders.Where(o => _orderPaymentStatus.ContainsKey(o.OrderId) &&
                                                              _orderPaymentStatus[o.OrderId]).ToList();
                }
                else if (selectedPaymentFilter == "Not Paid" || selectedPaymentFilter == "Pending Payment")
                {
                    // Find orders with matching payment status
                    filteredOrders = filteredOrders.Where(o => {
                        var payment = PaymentRepo.GetPaymentByOrderId(o.OrderId);
                        return payment != null && payment.PaymentStatus == selectedPaymentFilter;
                    }).ToList();
                }
            }

            foreach (var order in filteredOrders)
            {
                ListViewItem item = new ListViewItem(order.OrderId.ToString());
                item.SubItems.Add(order.CustomerId.ToString());
                item.SubItems.Add(order.OrderDate.ToString("MMM dd, HH:mm"));
                item.SubItems.Add("₱" + order.TotalPrice.ToString("N2"));
                item.SubItems.Add(order.OrderStatus);

                // Add payment status
                bool isPaid = _orderPaymentStatus.ContainsKey(order.OrderId) && _orderPaymentStatus[order.OrderId];
                item.SubItems.Add(isPaid ? "Paid" : "Unpaid");

                item.Tag = order.OrderId;

                // Set item colors based on status
                switch (order.OrderStatus.ToLower())
                {
                    case "pending":
                        item.ForeColor = Color.Blue;
                        item.Font = new Font(item.Font, FontStyle.Bold);
                        item.BackColor = Color.FromArgb(240, 248, 255);
                        break;
                    case "processing":
                        item.ForeColor = Color.Orange;
                        item.BackColor = Color.FromArgb(255, 250, 240);
                        break;
                    case "completed":
                        item.ForeColor = Color.Green;
                        break;
                    case "cancelled":
                        item.ForeColor = Color.Red;
                        break;
                }

                // Add indicator for payment status
                if (isPaid)
                {
                    item.SubItems[5].ForeColor = Color.Green;
                    item.SubItems[5].Font = new Font(item.Font, FontStyle.Bold);
                }
                else
                {
                    item.SubItems[5].ForeColor = Color.Red;
                }

                lvOrders.Items.Add(item);
            }

            if (filteredOrders.Count == 0)
            {
                string statusMsg = selectedStatusFilter == "All" ? "" : $" {selectedStatusFilter.ToLower()}";
                string paymentMsg = selectedPaymentFilter == "All" ? "" : $" {selectedPaymentFilter.ToLower()}";
                lblStatus.Text = $"No{statusMsg}{paymentMsg} orders found.";
            }
            else
            {
                lblStatus.Text = $"Showing {filteredOrders.Count} order(s). Select an order to view details.";
            }
        }

        private int GetStatusPriority(string status)
        {
            switch (status.ToLower())
            {
                case "pending": return 1;
                case "processing": return 2;
                case "completed": return 3;
                case "cancelled": return 4;
                default: return 5;
            }
        }

        private void CmbStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_allOrders != null)
            {
                ApplyFilters();
            }
        }

        private void CmbPaymentFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_allOrders != null)
            {
                ApplyFilters();
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
                _selectedOrderPayment = PaymentRepo.GetPaymentByOrderId(orderId);

                if (_selectedOrder != null)
                {
                    lblOrderHeader.Text = $"Order #{_selectedOrder.OrderId}";

                    // Basic order info
                    lblOrderInfo.Text = $"Customer ID: {_selectedOrder.CustomerId}\n" +
                                        $"Date: {_selectedOrder.OrderDate:MMM dd, yyyy HH:mm}\n" +
                                        $"Current Status: {_selectedOrder.OrderStatus}\n" +
                                        $"Total: ₱{_selectedOrder.TotalPrice:N2}";

                    // Payment info
                    bool isPaid = _orderPaymentStatus.ContainsKey(_selectedOrder.OrderId) && _orderPaymentStatus[_selectedOrder.OrderId];

                    // Show payment info based on whether payment exists
                    if (_selectedOrderPayment != null)
                    {
                        lblPaymentInfo.Text = $"Payment Status: {_selectedOrderPayment.PaymentStatus}\n" +
                                           $"Payment Method: {_selectedOrderPayment.PaymentMethod}";

                        if (_selectedOrderPayment.PaymentStatus == "Paid")
                        {
                            lblPaymentInfo.ForeColor = Color.Green;
                            btnPaymentAction.Text = "View Payment Details";
                            btnPaymentAction.BackColor = Color.FromArgb(0, 120, 215); // Blue
                        }
                        else if (_selectedOrderPayment.PaymentStatus == "Not Paid" && _selectedOrderPayment.PaymentMethod == "Cash")
                        {
                            lblPaymentInfo.ForeColor = Color.Orange;
                            btnPaymentAction.Text = "Confirm Cash Payment";
                            btnPaymentAction.BackColor = Color.FromArgb(255, 128, 0); // Orange
                        }
                        else
                        {
                            lblPaymentInfo.ForeColor = Color.Red;
                            btnPaymentAction.Text = "View Payment Details";
                            btnPaymentAction.BackColor = Color.FromArgb(0, 120, 215); // Blue
                        }
                    }
                    else
                    {
                        lblPaymentInfo.Text = "No payment record found for this order.";
                        lblPaymentInfo.ForeColor = Color.Red;
                        btnPaymentAction.Text = "Create Payment Record";
                        btnPaymentAction.BackColor = Color.FromArgb(0, 170, 0); // Green
                    }

                    // Set current status in combo box
                    cmbNewStatus.SelectedItem = _selectedOrder.OrderStatus;

                    // Enable/disable status update based on payment status
                    if (_selectedOrder.OrderStatus == "Completed")
                    {
                        lblStatusUpdate.Text = "Order completed";
                        cmbNewStatus.Enabled = false;
                        btnUpdateStatus.Enabled = false;
                    }
                    else if (_selectedOrder.OrderStatus == "Cancelled")
                    {
                        lblStatusUpdate.Text = "Order cancelled";
                        cmbNewStatus.Enabled = false;
                        btnUpdateStatus.Enabled = false;
                    }
                    else if (_selectedOrder.OrderStatus == "Pending Payment" && _selectedOrderPayment?.PaymentStatus == "Not Paid")
                    {
                        lblStatusUpdate.Text = "Awaiting payment confirmation";
                        cmbNewStatus.Enabled = false;
                        btnUpdateStatus.Enabled = false;
                    }
                    else
                    {
                        lblStatusUpdate.Text = "Update Status:";
                        cmbNewStatus.Enabled = true;
                        btnUpdateStatus.Enabled = true;
                    }

                    // Load order items
                    lvOrderItems.Items.Clear();

                    foreach (var item in _selectedOrder.OrderItems)
                    {
                        ListViewItem lvi = new ListViewItem(item.MenuItem.Name);
                        lvi.SubItems.Add(item.Quantity.ToString());
                        lvi.SubItems.Add("₱" + item.MenuItem.Price.ToString("N2"));
                        lvi.SubItems.Add("₱" + (item.Quantity * item.MenuItem.Price).ToString("N2"));
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

        private void BtnPaymentAction_Click(object sender, EventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Please select an order first.", "No Order Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_selectedOrderPayment != null)
            {
                // For cash payments that are not yet paid
                if (_selectedOrderPayment.PaymentMethod == "Cash" && _selectedOrderPayment.PaymentStatus == "Not Paid")
                {
                    if (MessageBox.Show($"Confirm that cash payment of ₱{_selectedOrderPayment.AmountPaid:N2} for Order #{_selectedOrder.OrderId} has been received?",
                        "Confirm Cash Payment", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            // Update payment status to Paid
                            bool success = PaymentRepo.UpdatePaymentStatus(_selectedOrderPayment.PaymentId, "Paid");

                            if (success)
                            {
                                MessageBox.Show("Payment status updated to Paid.", "Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Update payment status in our cache
                                _orderPaymentStatus[_selectedOrder.OrderId] = true;

                                // Refresh the selected order details
                                LoadOrderDetails(_selectedOrder.OrderId);

                                // Update the payment status in the list view
                                if (lvOrders.SelectedItems.Count > 0)
                                {
                                    lvOrders.SelectedItems[0].SubItems[5].Text = "Paid";
                                    lvOrders.SelectedItems[0].SubItems[5].ForeColor = Color.Green;
                                    lvOrders.SelectedItems[0].SubItems[5].Font = new Font(lvOrders.Font, FontStyle.Bold);
                                }

                                // Update order status if it was "Pending Payment"
                                if (_selectedOrder.OrderStatus == "Pending Payment")
                                {
                                    OrderRepo.UpdateOrderStatus(_selectedOrder.OrderId, "Pending");
                                    _selectedOrder.OrderStatus = "Pending";

                                    // Update the order status in the list view
                                    if (lvOrders.SelectedItems.Count > 0)
                                    {
                                        lvOrders.SelectedItems[0].SubItems[4].Text = "Pending";
                                    }

                                    // Refresh order info
                                    lblOrderInfo.Text = $"Customer ID: {_selectedOrder.CustomerId}\n" +
                                                    $"Date: {_selectedOrder.OrderDate:MMM dd, yyyy HH:mm}\n" +
                                                    $"Current Status: {_selectedOrder.OrderStatus}\n" +
                                                    $"Total: ₱{_selectedOrder.TotalPrice:N2}";
                                }
                            }
                            else
                            {
                                MessageBox.Show("Failed to update payment status.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error updating payment status: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                   
                    if (MessageBox.Show($"Create a payment record for Order #{_selectedOrder.OrderId} with amount {_selectedOrder.TotalPrice:N2}?",
                    "Create Payment", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            // Create payment with Cash as default method
                            int paymentId = PaymentRepo.CreatePayment(
                                _selectedOrder.OrderId,
                                _selectedOrder.TotalPrice,
                                "Cash",
                                "Paid"
                            );

                            if (paymentId > 0)
                            {
                                MessageBox.Show($"Payment record created successfully with ID: {paymentId}",
                                    "Payment Created", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Update payment status in our cache
                                _orderPaymentStatus[_selectedOrder.OrderId] = true;

                                // Refresh the selected order details
                                LoadOrderDetails(_selectedOrder.OrderId);

                                // Update the payment status in the list view
                                if (lvOrders.SelectedItems.Count > 0)
                                {
                                    lvOrders.SelectedItems[0].SubItems[5].Text = "Paid";
                                    lvOrders.SelectedItems[0].SubItems[5].ForeColor = Color.Green;
                                    lvOrders.SelectedItems[0].SubItems[5].Font = new Font(lvOrders.Font, FontStyle.Bold);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Failed to create payment record.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error creating payment: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }

        }

        private void BtnUpdateStatus_Click(object sender, EventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Please select an order first.", "No Order Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbNewStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a status.", "No Status Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string newStatus = cmbNewStatus.SelectedItem.ToString();

            if (newStatus == _selectedOrder.OrderStatus)
            {
                MessageBox.Show("Status is already set to this value.", "No Change",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Check if trying to cancel an order that has payments
            if (newStatus.ToLower() == "cancelled")
            {
                try
                {
                    // Check if the order has any associated payments
                    string checkPaymentSql = "SELECT COUNT(*) FROM payments WHERE order_id = @OrderId";
                    MySqlParameter parameter = new MySqlParameter("@OrderId", _selectedOrder.OrderId);

                    int paymentCount = Convert.ToInt32(DBManager.ExecuteScalar(
                        checkPaymentSql,
                        CommandType.Text,
                        parameter));

                    if (paymentCount > 0)
                    {
                        MessageBox.Show(
                            "Cannot cancel this order as it has associated payment records.",
                            "Operation Denied",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error checking payment status: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                Cursor = Cursors.WaitCursor;

                // Update order status in database
                bool success = OrderRepo.UpdateOrderStatus(_selectedOrder.OrderId, newStatus);

                if (success)
                {
                    _selectedOrder.OrderStatus = newStatus;

                    // Update the list view item
                    var listItem = lvOrders.Items.Cast<ListViewItem>()
                        .FirstOrDefault(item => (int)item.Tag == _selectedOrder.OrderId);

                    if (listItem != null)
                    {
                        listItem.SubItems[4].Text = newStatus;

                        // Update color and style based on new status
                        switch (newStatus.ToLower())
                        {
                            case "pending":
                                listItem.ForeColor = Color.Blue;
                                listItem.Font = new Font(listItem.Font, FontStyle.Bold);
                                listItem.BackColor = Color.FromArgb(240, 248, 255);
                                break;
                            case "processing":
                                listItem.ForeColor = Color.Orange;
                                listItem.Font = new Font(listItem.Font, FontStyle.Regular);
                                listItem.BackColor = Color.FromArgb(255, 250, 240);
                                break;
                            case "completed":
                                listItem.ForeColor = Color.Green;
                                listItem.Font = new Font(listItem.Font, FontStyle.Regular);
                                listItem.BackColor = Color.White;
                                break;
                            case "cancelled":
                                listItem.ForeColor = Color.Red;
                                listItem.Font = new Font(listItem.Font, FontStyle.Regular);
                                listItem.BackColor = Color.White;
                                break;
                        }
                    }

                    // Update order info display
                    lblOrderInfo.Text = $"Customer ID: {_selectedOrder.CustomerId}\n" +
                                        $"Date: {_selectedOrder.OrderDate:MMM dd, yyyy HH:mm}\n" +
                                        $"Current Status: {_selectedOrder.OrderStatus}\n" +
                                        $"Total: ₱ {_selectedOrder.TotalPrice:N2}";

                    lblStatus.Text = $"Order #{_selectedOrder.OrderId} status updated to {newStatus}.";

                    MessageBox.Show($"Order status updated to {newStatus}.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update order status. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (MySqlException ex)
            {
              
                if (ex.Number == 1644) 
                {
                    MessageBox.Show(
                        "Cannot update this order's status because it has associated payment records.",
                        "Operation Denied",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"Database error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating order status: {ex.Message}", "Error",
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

        private void OrderManagement_Load(object sender, EventArgs e)
        {
            RefreshOrdersList();
        }

        
        private Label lblTitle;
        private Label lblFilter;
        private ComboBox cmbStatusFilter;
        private Button btnRefresh;
        private ListView lvOrders;
        private Panel detailsPanel;
        private Label lblOrderHeader;
        private Label lblOrderInfo;
        private Label lblStatusUpdate;
        private ComboBox cmbNewStatus;
        private Button btnUpdateStatus;
        private Label lblOrderItems;
        private ListView lvOrderItems;
        private Label lblStatus;

        // New UI elements for payment functionality
        private Label lblPaymentFilter;
        private ComboBox cmbPaymentFilter;
        private Label lblPaymentInfo;
        private Button btnPaymentAction;
    }
}