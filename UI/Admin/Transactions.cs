using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using food_ordering_system.v2.Data.Repositories;

namespace food_ordering_system.v2.UI.Admin
{
    public partial class Transactions : UserControl
    {
        private DataTable transactionsData;
        private int selectedPaymentId = -1;

        public Transactions()
        {
            InitializeComponent();
        }

        private void Transactions_Load(object sender, EventArgs e)
        {
            SetupDataGridView();
            LoadTransactions();
            SetupContextMenu();
        }

        private void SetupDataGridView()
        {
            // Configure DataGridView appearance
            dgvTransactions.AutoGenerateColumns = false;
            dgvTransactions.AllowUserToAddRows = false;
            dgvTransactions.AllowUserToDeleteRows = false;
            dgvTransactions.ReadOnly = true;
            dgvTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTransactions.MultiSelect = false;
            dgvTransactions.BackgroundColor = Color.White;
            dgvTransactions.BorderStyle = BorderStyle.Fixed3D;
            dgvTransactions.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvTransactions.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 122, 183);
            dgvTransactions.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTransactions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvTransactions.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 122, 183);
            dgvTransactions.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvTransactions.RowHeadersVisible = false;
            dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Clear existing columns
            dgvTransactions.Columns.Clear();

            // Add columns
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "payment_id",
                HeaderText = "Payment ID",
                DataPropertyName = "payment_id",
                Width = 80,
                FillWeight = 10
            });

            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "order_id",
                HeaderText = "Order ID",
                DataPropertyName = "order_id",
                Width = 80,
                FillWeight = 10
            });

            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerName",
                HeaderText = "Customer",
                DataPropertyName = "CustomerName",
                FillWeight = 20
            });

            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "payment_date",
                HeaderText = "Payment Date",
                DataPropertyName = "payment_date",
                DefaultCellStyle = { Format = "MMM dd, yyyy hh:mm tt" },
                FillWeight = 20
            });

            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "amount_paid",
                HeaderText = "Amount",
                DataPropertyName = "amount_paid",
                DefaultCellStyle = { Format = "₱#,##0.00", Alignment = DataGridViewContentAlignment.MiddleRight },
                FillWeight = 15
            });

            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "payment_method",
                HeaderText = "Payment Method",
                DataPropertyName = "payment_method",
                FillWeight = 15
            });

            // Add new payment status column
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "payment_status",
                HeaderText = "Status",
                DataPropertyName = "payment_status",
                FillWeight = 10
            });
        }

        private void SetupContextMenu()
        {
            // Create context menu for right-click
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            // Add menu item for toggling payment status
            ToolStripMenuItem toggleStatusItem = new ToolStripMenuItem("Toggle Payment Status");
            toggleStatusItem.Click += TogglePaymentStatus_Click;
            contextMenu.Items.Add(toggleStatusItem);

            // Add menu item for viewing detailed payment info
            ToolStripMenuItem viewDetailsItem = new ToolStripMenuItem("View Payment Details");
            viewDetailsItem.Click += ViewPaymentDetails_Click;
            contextMenu.Items.Add(viewDetailsItem);

            // Assign context menu to data grid view
            dgvTransactions.ContextMenuStrip = contextMenu;

            // Handle cell right-click to show context menu
            dgvTransactions.CellMouseDown += DgvTransactions_CellMouseDown;
        }

        private void DgvTransactions_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Right mouse button clicked
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dgvTransactions.ClearSelection();
                dgvTransactions.Rows[e.RowIndex].Selected = true;

                // Store the selected payment ID
                if (dgvTransactions.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dgvTransactions.SelectedRows[0];
                    selectedPaymentId = Convert.ToInt32(row.Cells["payment_id"].Value);
                }
                else
                {
                    selectedPaymentId = -1;
                }
            }
        }

        private void TogglePaymentStatus_Click(object sender, EventArgs e)
        {
            if (selectedPaymentId <= 0)
            {
                MessageBox.Show("Please select a payment record first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get the currently selected row
                DataGridViewRow row = dgvTransactions.SelectedRows[0];
                string currentStatus = row.Cells["payment_status"].Value.ToString();

                // Determine the new status (toggle between "Paid" and "Not Paid")
                string newStatus = (currentStatus == "Paid") ? "Not Paid" : "Paid";

                // Update the payment status in the database
                bool success = PaymentRepo.UpdatePaymentStatus(selectedPaymentId, newStatus);

                if (success)
                {
                    // Update the status in the UI
                    row.Cells["payment_status"].Value = newStatus;

                    // Apply visual style based on status
                    ApplyPaymentStatusStyle(row);

                    MessageBox.Show($"Payment status updated to '{newStatus}'.", "Status Updated",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update payment status.", "Update Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating payment status: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViewPaymentDetails_Click(object sender, EventArgs e)
        {
            if (selectedPaymentId <= 0)
            {
                MessageBox.Show("Please select a payment record first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

           
            try
            {
                DataGridViewRow row = dgvTransactions.SelectedRows[0];

                string details = $"Payment ID: {row.Cells["payment_id"].Value}\n" +
                                 $"Order ID: {row.Cells["order_id"].Value}\n" +
                                 $"Customer: {row.Cells["CustomerName"].Value}\n" +
                                 $"Date: {Convert.ToDateTime(row.Cells["payment_date"].Value):g}\n" +
                                 $"Amount: ₱{Convert.ToDecimal(row.Cells["amount_paid"].Value):N2}\n" +
                                 $"Method: {row.Cells["payment_method"].Value}\n" +
                                 $"Status: {row.Cells["payment_status"].Value}";

                MessageBox.Show(details, "Payment Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving payment details: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTransactions()
        {
            try
            {
                // Use the existing PaymentRepo to get all payments with order details
                transactionsData = PaymentRepo.GetAllPaymentsWithOrderDetails();

                // Add a computed column for customer name
                if (!transactionsData.Columns.Contains("CustomerName"))
                {
                    transactionsData.Columns.Add("CustomerName", typeof(string));
                    foreach (DataRow row in transactionsData.Rows)
                    {
                        string firstName = row["first_name"]?.ToString() ?? "";
                        string lastName = row["last_name"]?.ToString() ?? "";
                        row["CustomerName"] = $"{firstName} {lastName}".Trim();
                    }
                }

                dgvTransactions.DataSource = transactionsData;

                // Apply visual styles based on payment status
                ApplyPaymentStatusStyles();

                // Update summary labels
                UpdateSummaryInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transactions: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyPaymentStatusStyles()
        {
            foreach (DataGridViewRow row in dgvTransactions.Rows)
            {
                ApplyPaymentStatusStyle(row);
            }
        }

        private void ApplyPaymentStatusStyle(DataGridViewRow row)
        {
            if (row.Cells["payment_status"].Value != null)
            {
                string status = row.Cells["payment_status"].Value.ToString();

                if (status == "Paid")
                {
                    row.Cells["payment_status"].Style.ForeColor = Color.Green;
                    row.Cells["payment_status"].Style.Font = new Font(dgvTransactions.Font, FontStyle.Bold);
                }
                else if (status == "Not Paid")
                {
                    row.Cells["payment_status"].Style.ForeColor = Color.Red;
                    row.Cells["payment_status"].Style.Font = new Font(dgvTransactions.Font, FontStyle.Bold);
                }
            }
        }

        private void UpdateSummaryInfo()
        {
            if (transactionsData != null && transactionsData.Rows.Count > 0)
            {
                int totalTransactions = transactionsData.Rows.Count;
                decimal totalRevenue = 0;
                int paidTransactions = 0;
                int unpaidTransactions = 0;

                foreach (DataRow row in transactionsData.Rows)
                {
                    if (row["amount_paid"] != DBNull.Value)
                    {
                        // Only count paid transactions in revenue
                        string status = row["payment_status"]?.ToString() ?? "Paid";
                        if (status == "Paid")
                        {
                            totalRevenue += Convert.ToDecimal(row["amount_paid"]);
                            paidTransactions++;
                        }
                        else
                        {
                            unpaidTransactions++;
                        }
                    }
                }

                lblTotalTransactions.Text = $"Total Transactions: {totalTransactions}";
                lblTotalRevenue.Text = $"Total Revenue: ₱{totalRevenue:N2}";

                // Add paid/unpaid counts to the UI
                if (lblPaidCount == null)
                {
                    lblPaidCount = new Label
                    {
                        AutoSize = true,
                        Location = new Point(lblTotalTransactions.Left, lblTotalTransactions.Bottom + 10),
                        Text = $"Paid: {paidTransactions}",
                        ForeColor = Color.Green,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold)
                    };
                    Controls.Add(lblPaidCount);
                }
                else
                {
                    lblPaidCount.Text = $"Paid: {paidTransactions}";
                }

                if (lblUnpaidCount == null)
                {
                    lblUnpaidCount = new Label
                    {
                        AutoSize = true,
                        Location = new Point(lblPaidCount.Right + 30, lblPaidCount.Top),
                        Text = $"Unpaid: {unpaidTransactions}",
                        ForeColor = Color.Red,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold)
                    };
                    Controls.Add(lblUnpaidCount);
                }
                else
                {
                    lblUnpaidCount.Text = $"Unpaid: {unpaidTransactions}";
                }

                // Calculate today's transactions
                decimal todayRevenue = 0;
                DateTime today = DateTime.Today;

                foreach (DataRow row in transactionsData.Rows)
                {
                    if (row["payment_date"] != DBNull.Value)
                    {
                        DateTime paymentDate = Convert.ToDateTime(row["payment_date"]);
                        string status = row["payment_status"]?.ToString() ?? "Paid";

                        if (paymentDate.Date == today && row["amount_paid"] != DBNull.Value && status == "Paid")
                        {
                            todayRevenue += Convert.ToDecimal(row["amount_paid"]);
                        }
                    }
                }

                lblTodayRevenue.Text = $"Today's Revenue: ₱{todayRevenue:N2}";
            }
            else
            {
                lblTotalTransactions.Text = "Total Transactions: 0";
                lblTotalRevenue.Text = "Total Revenue: ₱0.00";
                lblTodayRevenue.Text = "Today's Revenue: ₱0.00";

                if (lblPaidCount != null) lblPaidCount.Text = "Paid: 0";
                if (lblUnpaidCount != null) lblUnpaidCount.Text = "Unpaid: 0";
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTransactions();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt",
                    DefaultExt = "csv",
                    FileName = $"Transactions_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCSV(saveFileDialog.FileName);
                    MessageBox.Show("Transactions exported successfully!", "Export Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting transactions: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCSV(string fileName)
        {
            StringBuilder csv = new StringBuilder();

            // Add headers
            csv.AppendLine("Payment ID,Order ID,Customer Name,Payment Date,Amount,Payment Method,Payment Status");

            // Add data rows
            foreach (DataGridViewRow row in dgvTransactions.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    string[] values = new string[]
                    {
                        row.Cells["payment_id"].Value?.ToString() ?? "",
                        row.Cells["order_id"].Value?.ToString() ?? "",
                        row.Cells["CustomerName"].Value?.ToString() ?? "",
                        row.Cells["payment_date"].Value?.ToString() ?? "",
                        row.Cells["amount_paid"].Value?.ToString() ?? "",
                        row.Cells["payment_method"].Value?.ToString() ?? "",
                        row.Cells["payment_status"].Value?.ToString() ?? ""
                    };

                    // Escape commas and quotes in values
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i].Contains(",") || values[i].Contains("\""))
                        {
                            values[i] = "\"" + values[i].Replace("\"", "\"\"") + "\"";
                        }
                    }

                    csv.AppendLine(string.Join(",", values));
                }
            }

            System.IO.File.WriteAllText(fileName, csv.ToString());
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterTransactions();
        }

        private void cmbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterTransactions();
        }

        private void FilterTransactions()
        {
            if (transactionsData == null) return;

            string searchText = txtSearch.Text.Trim();
            string filterBy = cmbFilterBy.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(searchText))
            {
                transactionsData.DefaultView.RowFilter = "";
            }
            else
            {
                string filter = "";
                switch (filterBy)
                {
                    case "Customer Name":
                        filter = $"CustomerName LIKE '%{searchText}%'";
                        break;
                    case "Order ID":
                        filter = $"order_id LIKE '%{searchText}%'";
                        break;
                    case "Payment Method":
                        filter = $"payment_method LIKE '%{searchText}%'";
                        break;
                    case "Payment Status":
                        filter = $"payment_status LIKE '%{searchText}%'";
                        break;
                    default:
                        filter = $"CustomerName LIKE '%{searchText}%' OR Convert(order_id, 'System.String') LIKE '%{searchText}%' OR payment_method LIKE '%{searchText}%' OR payment_status LIKE '%{searchText}%'";
                        break;
                }
                transactionsData.DefaultView.RowFilter = filter;
            }

            // Reapply styles after filtering
            ApplyPaymentStatusStyles();
        }

        private void dgvTransactions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        // New UI elements
        private Label lblPaidCount;
        private Label lblUnpaidCount;

        private void lblTotalRevenue_Click(object sender, EventArgs e)
        {

        }
    }
}