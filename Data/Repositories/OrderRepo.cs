using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using food_ordering_system.v2.Data.Models;

namespace food_ordering_system.v2.Data.Repositories
{
    public static class OrderRepo
    {
        public static int CreateOrder(int customerId, decimal totalPrice, string orderStatus = "Pending")
        {
            try
            {
                string query = "INSERT INTO orders (customer_id, order_date, total_price, order_status) " +
                               "VALUES (@customerId, @orderDate, @totalPrice, @orderStatus); " +
                               "SELECT LAST_INSERT_ID();";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@customerId", customerId),
                    new MySqlParameter("@orderDate", DateTime.Now),
                    new MySqlParameter("@totalPrice", totalPrice),
                    new MySqlParameter("@orderStatus", orderStatus)
                };

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
                Console.WriteLine($"Error creating order: {ex.Message}");
                throw;
            }
        }

        // Add order items to an existing order
        public static void AddOrderItems(int orderId, List<OrderItem> orderItems)
        {
            try
            {
                foreach (OrderItem item in orderItems)
                {
                    string query = "INSERT INTO order_items (order_id, menu_item_id, quantity) " +
                                   "VALUES (@orderId, @menuItemId, @quantity)";

                    MySqlParameter[] parameters = {
                        new MySqlParameter("@orderId", orderId),
                        new MySqlParameter("@menuItemId", item.MenuItemId),
                        new MySqlParameter("@quantity", item.Quantity)
                    };

                    DBManager.ExecuteNonQuery(query, CommandType.Text, parameters);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error adding order items: {ex.Message}");
                throw;
            }
        }

        // Get orders for a specific customer
        public static List<Order> GetOrdersByCustomerId(int customerId)
        {
            List<Order> customerOrders = new List<Order>();

            try
            {
                string query = "SELECT order_id, customer_id, order_date, total_price, order_status " +
                               "FROM orders WHERE customer_id = @customerId " +
                               "ORDER BY order_date DESC";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@customerId", customerId)
                };

                DataTable dataTable = DBManager.GetDataTable(query, CommandType.Text, parameters);

                foreach (DataRow row in dataTable.Rows)
                {
                    Order order = new Order
                    {
                        OrderId = Convert.ToInt32(row["order_id"]),
                        CustomerId = Convert.ToInt32(row["customer_id"]),
                        OrderDate = Convert.ToDateTime(row["order_date"]),
                        TotalPrice = Convert.ToDecimal(row["total_price"]),
                        OrderStatus = row["order_status"].ToString()
                    };

                    customerOrders.Add(order);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error getting customer orders: {ex.Message}");
                throw;
            }

            return customerOrders;
        }

        // Get all orders (for admin)
        public static List<Order> GetAllOrders()
        {
            List<Order> allOrders = new List<Order>();

            try
            {
                string query = "SELECT o.order_id, o.customer_id, o.order_date, o.total_price, o.order_status, " +
                               "c.first_name, c.last_name, c.email " +
                               "FROM orders o " +
                               "JOIN customers c ON o.customer_id = c.customer_id " +
                               "ORDER BY o.order_date DESC";

                DataTable dataTable = DBManager.GetDataTable(query, CommandType.Text, null);

                foreach (DataRow row in dataTable.Rows)
                {
                    Order order = new Order
                    {
                        OrderId = Convert.ToInt32(row["order_id"]),
                        CustomerId = Convert.ToInt32(row["customer_id"]),
                        OrderDate = Convert.ToDateTime(row["order_date"]),
                        TotalPrice = Convert.ToDecimal(row["total_price"]),
                        OrderStatus = row["order_status"].ToString(),

                        // Include customer information
                        Customer = new Customer
                        {
                            CustomerId = Convert.ToInt32(row["customer_id"]),
                            FirstName = row["first_name"].ToString(),
                            LastName = row["last_name"].ToString(),
                            Email = row["email"].ToString()
                        }
                    };

                    allOrders.Add(order);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error getting all orders: {ex.Message}");
                throw;
            }

            return allOrders;
        }

        // Get order details including all items
        public static Order GetOrderById(int orderId)
        {
            Order order = null;

            try
            {
                // Get the order
                string orderQuery = "SELECT o.order_id, o.customer_id, o.order_date, o.total_price, o.order_status, " +
                                    "c.first_name, c.last_name, c.email " +
                                    "FROM orders o " +
                                    "JOIN customers c ON o.customer_id = c.customer_id " +
                                    "WHERE o.order_id = @orderId";

                MySqlParameter[] orderParameters = {
                    new MySqlParameter("@orderId", orderId)
                };

                DataTable orderDataTable = DBManager.GetDataTable(orderQuery, CommandType.Text, orderParameters);

                if (orderDataTable.Rows.Count > 0)
                {
                    DataRow orderRow = orderDataTable.Rows[0];
                    order = new Order
                    {
                        OrderId = Convert.ToInt32(orderRow["order_id"]),
                        CustomerId = Convert.ToInt32(orderRow["customer_id"]),
                        OrderDate = Convert.ToDateTime(orderRow["order_date"]),
                        TotalPrice = Convert.ToDecimal(orderRow["total_price"]),
                        OrderStatus = orderRow["order_status"].ToString(),
                        OrderItems = new List<OrderItem>(),

                        // Include customer information
                        Customer = new Customer
                        {
                            CustomerId = Convert.ToInt32(orderRow["customer_id"]),
                            FirstName = orderRow["first_name"].ToString(),
                            LastName = orderRow["last_name"].ToString(),
                            Email = orderRow["email"].ToString()
                        }
                    };

                    // Get the order items
                    string itemsQuery = "SELECT oi.order_item_id, oi.order_id, oi.menu_item_id, oi.quantity, " +
                                        "mi.name, mi.price " +
                                        "FROM order_items oi " +
                                        "JOIN menu_items mi ON oi.menu_item_id = mi.menu_item_id " +
                                        "WHERE oi.order_id = @orderId";

                    MySqlParameter[] itemsParameters = {
                        new MySqlParameter("@orderId", orderId)
                    };

                    DataTable itemsDataTable = DBManager.GetDataTable(itemsQuery, CommandType.Text, itemsParameters);

                    foreach (DataRow itemRow in itemsDataTable.Rows)
                    {
                        OrderItem item = new OrderItem
                        {
                            OrderItemId = Convert.ToInt32(itemRow["order_item_id"]),
                            OrderId = Convert.ToInt32(itemRow["order_id"]),
                            MenuItemId = Convert.ToInt32(itemRow["menu_item_id"]),
                            Quantity = Convert.ToInt32(itemRow["quantity"]),

                            // Include menu item information
                            MenuItem = new MenuItem
                            {
                                MenuItemId = Convert.ToInt32(itemRow["menu_item_id"]),
                                Name = itemRow["name"].ToString(),
                                Price = Convert.ToDecimal(itemRow["price"])
                            }
                        };

                        order.OrderItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error getting order details: {ex.Message}");
                throw;
            }

            return order;
        }

        // Update order status
        public static bool UpdateOrderStatus(int orderId, string newStatus)
        {
            try
            {
                string query = "UPDATE orders SET order_status = @orderStatus " +
                               "WHERE order_id = @orderId";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@orderStatus", newStatus),
                    new MySqlParameter("@orderId", orderId)
                };

                int rowsAffected = DBManager.ExecuteNonQuery(query, CommandType.Text, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating order status: {ex.Message}");
                throw;
            }
        }
    }
}