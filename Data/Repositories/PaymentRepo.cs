using System;
using System.Data;
using MySql.Data.MySqlClient;
using food_ordering_system.v2.Data.Models;

namespace food_ordering_system.v2.Data.Repositories
{
    public static class PaymentRepo
    {
      
        public static int CreatePayment(int orderId, decimal amountPaid, string paymentMethod, string paymentStatus = null)
        {
            try
            {
              
                if (paymentStatus == null)
                {
                    paymentStatus = paymentMethod.Equals("Cash", StringComparison.OrdinalIgnoreCase)
                        ? "Not Paid"
                        : "Paid";
                }

                string query = "INSERT INTO payments (order_id, amount_paid, payment_method, payment_date, payment_status) " +
                               "VALUES (@orderId, @amountPaid, @paymentMethod, @paymentDate, @paymentStatus); " +
                               "SELECT LAST_INSERT_ID();";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@orderId", orderId),
                    new MySqlParameter("@amountPaid", amountPaid),
                    new MySqlParameter("@paymentMethod", paymentMethod),
                    new MySqlParameter("@paymentDate", DateTime.Now),
                    new MySqlParameter("@paymentStatus", paymentStatus)
                };

                // Get the auto-generated payment_id
                object result = DBManager.ExecuteScalar(query, CommandType.Text, parameters);
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                return -1;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error creating payment record: {ex.Message}");
                throw;
            }
        }

        // Get payment details by order_id
        public static Payment GetPaymentByOrderId(int orderId)
        {
            try
            {
                string query = "SELECT payment_id, order_id, amount_paid, payment_method, payment_date, payment_status " +
                               "FROM payments WHERE order_id = @orderId";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@orderId", orderId)
                };

                DataTable dataTable = DBManager.GetDataTable(query, CommandType.Text, parameters);

                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    Payment payment = new Payment
                    {
                        PaymentId = Convert.ToInt32(row["payment_id"]),
                        OrderId = Convert.ToInt32(row["order_id"]),
                        AmountPaid = Convert.ToDecimal(row["amount_paid"]),
                        PaymentMethod = row["payment_method"].ToString(),
                        PaymentDate = Convert.ToDateTime(row["payment_date"]),
                        PaymentStatus = row["payment_status"].ToString()
                    };
                    return payment;
                }
                return null;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving payment details: {ex.Message}");
                throw;
            }
        }

        // Get all payments with order details (for admin)
        public static DataTable GetAllPaymentsWithOrderDetails()
        {
            try
            {
                string query = "SELECT p.payment_id, p.order_id, p.amount_paid, p.payment_method, p.payment_date, p.payment_status, " +
                               "o.customer_id, o.order_status, c.first_name, c.last_name " +
                               "FROM payments p " +
                               "JOIN orders o ON p.order_id = o.order_id " +
                               "JOIN customers c ON o.customer_id = c.customer_id " +
                               "ORDER BY p.payment_date DESC";

                return DBManager.GetDataTable(query, CommandType.Text, null);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving payment records: {ex.Message}");
                throw;
            }
        }

        // Update payment details
        public static bool UpdatePayment(int paymentId, decimal amountPaid, string paymentMethod, string paymentStatus = null)
        {
            try
            {
                string query;
                MySqlParameter[] parameters;

                if (paymentStatus != null)
                {
                    // Update all fields including payment status
                    query = "UPDATE payments SET amount_paid = @amountPaid, payment_method = @paymentMethod, " +
                            "payment_status = @paymentStatus WHERE payment_id = @paymentId";

                    parameters = new MySqlParameter[] {
                        new MySqlParameter("@amountPaid", amountPaid),
                        new MySqlParameter("@paymentMethod", paymentMethod),
                        new MySqlParameter("@paymentStatus", paymentStatus),
                        new MySqlParameter("@paymentId", paymentId)
                    };
                }
                else
                {
                    // Update without changing payment status
                    query = "UPDATE payments SET amount_paid = @amountPaid, payment_method = @paymentMethod " +
                            "WHERE payment_id = @paymentId";

                    parameters = new MySqlParameter[] {
                        new MySqlParameter("@amountPaid", amountPaid),
                        new MySqlParameter("@paymentMethod", paymentMethod),
                        new MySqlParameter("@paymentId", paymentId)
                    };
                }

                int rowsAffected = DBManager.ExecuteNonQuery(query, CommandType.Text, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating payment details: {ex.Message}");
                throw;
            }
        }

        // Update payment status only
        public static bool UpdatePaymentStatus(int paymentId, string paymentStatus)
        {
            try
            {
                string query = "UPDATE payments SET payment_status = @paymentStatus WHERE payment_id = @paymentId";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@paymentStatus", paymentStatus),
                    new MySqlParameter("@paymentId", paymentId)
                };

                int rowsAffected = DBManager.ExecuteNonQuery(query, CommandType.Text, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating payment status: {ex.Message}");
                throw;
            }
        }

        // Delete a payment record
        public static bool DeletePayment(int paymentId)
        {
            try
            {
                string query = "DELETE FROM payments WHERE payment_id = @paymentId";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@paymentId", paymentId)
                };

                int rowsAffected = DBManager.ExecuteNonQuery(query, CommandType.Text, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error deleting payment record: {ex.Message}");
                throw;
            }
        }

        // Check if an order has been paid
        public static bool IsOrderPaid(int orderId)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM payments WHERE order_id = @orderId AND payment_status = 'Paid'";

                MySqlParameter parameter = new MySqlParameter("@orderId", orderId);

                object result = DBManager.ExecuteScalar(query, CommandType.Text, parameter);

                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error checking if order is paid: {ex.Message}");
                throw;
            }
        }
    }
}