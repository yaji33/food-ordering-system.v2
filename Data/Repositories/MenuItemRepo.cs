using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using food_ordering_system.v2.Data.Models;

namespace food_ordering_system.v2.Data.Repositories
{
    public class MenuItemRepo
    {
        public static List<MenuItem> GetAllMenuItems()
        {
            try
            {
                List<MenuItem> menuItems = new List<MenuItem>();
                string query = @"
                    SELECT mi.menu_item_id, mi.name, mi.description, mi.price, mi.category_id, 
                           c.category_name
                    FROM menu_items mi
                    LEFT JOIN categories c ON mi.category_id = c.category_id";

                using (MySqlDataReader reader = DBManager.ExecuteReader(query, CommandType.Text))
                {
                    while (reader.Read())
                    {
                        MenuItem menuItem = new MenuItem
                        {
                            MenuItemId = Convert.ToInt32(reader["menu_item_id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString(),
                            Price = Convert.ToDecimal(reader["price"])
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("category_id")))
                        {
                            menuItem.CategoryId = Convert.ToInt32(reader["category_id"]);
                            menuItem.Category = new Category
                            {
                                CategoryId = menuItem.CategoryId,
                                CategoryName = reader["category_name"].ToString()
                            };
                        }

                        menuItems.Add(menuItem);
                    }
                }

                return menuItems;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving menu items: {ex.Message}");
                return new List<MenuItem>();
            }
        }

        public static MenuItem GetMenuItemById(int menuItemId)
        {
            try
            {
                string query = @"
                    SELECT mi.menu_item_id, mi.name, mi.description, mi.price, mi.category_id, 
                           c.category_name
                    FROM menu_items mi
                    LEFT JOIN categories c ON mi.category_id = c.category_id
                    WHERE mi.menu_item_id = @MenuItemId";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@MenuItemId", MySqlDbType.Int32) { Value = menuItemId }
                };

                using (MySqlDataReader reader = DBManager.ExecuteReader(query, CommandType.Text, parameters))
                {
                    if (reader.Read())
                    {
                        MenuItem menuItem = new MenuItem
                        {
                            MenuItemId = Convert.ToInt32(reader["menu_item_id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString(),
                            Price = Convert.ToDecimal(reader["price"])
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("category_id")))
                        {
                            menuItem.CategoryId = Convert.ToInt32(reader["category_id"]);
                            menuItem.Category = new Category
                            {
                                CategoryId = menuItem.CategoryId,
                                CategoryName = reader["category_name"].ToString()
                            };
                        }

                        return menuItem;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving menu item by ID: {ex.Message}");
                return null;
            }
        }

        public static List<MenuItem> GetMenuItemsByCategoryId(int categoryId)
        {
            try
            {
                List<MenuItem> menuItems = new List<MenuItem>();
                string query = @"
                    SELECT mi.menu_item_id, mi.name, mi.description, mi.price, mi.category_id, 
                           c.category_name
                    FROM menu_items mi
                    JOIN categories c ON mi.category_id = c.category_id
                    WHERE mi.category_id = @CategoryId";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@CategoryId", MySqlDbType.Int32) { Value = categoryId }
                };

                using (MySqlDataReader reader = DBManager.ExecuteReader(query, CommandType.Text, parameters))
                {
                    while (reader.Read())
                    {
                        MenuItem menuItem = new MenuItem
                        {
                            MenuItemId = Convert.ToInt32(reader["menu_item_id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString(),
                            Price = Convert.ToDecimal(reader["price"]),
                            CategoryId = categoryId,
                            Category = new Category
                            {
                                CategoryId = categoryId,
                                CategoryName = reader["category_name"].ToString()
                            }
                        };

                        menuItems.Add(menuItem);
                    }
                }

                return menuItems;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving menu items by category ID: {ex.Message}");
                return new List<MenuItem>();
            }
        }

        public static List<MenuItem> GetMenuItemsByCategoryName(string categoryName)
        {
            try
            {
                List<MenuItem> menuItems = new List<MenuItem>();
                string query = @"
                    SELECT mi.menu_item_id, mi.name, mi.description, mi.price, mi.category_id, 
                           c.category_name
                    FROM menu_items mi
                    JOIN categories c ON mi.category_id = c.category_id
                    WHERE c.category_name = @CategoryName";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@CategoryName", MySqlDbType.VarChar) { Value = categoryName }
                };

                using (MySqlDataReader reader = DBManager.ExecuteReader(query, CommandType.Text, parameters))
                {
                    while (reader.Read())
                    {
                        MenuItem menuItem = new MenuItem
                        {
                            MenuItemId = Convert.ToInt32(reader["menu_item_id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString(),
                            Price = Convert.ToDecimal(reader["price"]),
                            CategoryId = Convert.ToInt32(reader["category_id"]),
                            Category = new Category
                            {
                                CategoryId = Convert.ToInt32(reader["category_id"]),
                                CategoryName = categoryName
                            }
                        };

                        menuItems.Add(menuItem);
                    }
                }

                return menuItems;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving menu items by category name: {ex.Message}");
                return new List<MenuItem>();
            }
        }

        public static bool AddMenuItem(MenuItem menuItem)
        {
            try
            {
                string query = @"INSERT INTO menu_items (name, description, price, category_id) 
                               VALUES (@Name, @Description, @Price, @CategoryId);
                               SELECT LAST_INSERT_ID();";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@Name", MySqlDbType.VarChar) { Value = menuItem.Name },
                    new MySqlParameter("@Description", MySqlDbType.VarChar) { Value = menuItem.Description },
                    new MySqlParameter("@Price", MySqlDbType.Decimal) { Value = menuItem.Price },
                    new MySqlParameter("@CategoryId", MySqlDbType.Int32) { Value = menuItem.CategoryId }
                };

                object result = DBManager.ExecuteScalar(query, CommandType.Text, parameters);
                int menuItemId = Convert.ToInt32(result);
                menuItem.MenuItemId = menuItemId;

                return menuItemId > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding menu item: {ex.Message}");
                return false;
            }
        }

        public static bool UpdateMenuItem(MenuItem menuItem)
        {
            try
            {
                string query = @"UPDATE menu_items 
                               SET name = @Name, 
                                   description = @Description, 
                                   price = @Price, 
                                   category_id = @CategoryId 
                               WHERE menu_item_id = @MenuItemId";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@MenuItemId", MySqlDbType.Int32) { Value = menuItem.MenuItemId },
                    new MySqlParameter("@Name", MySqlDbType.VarChar) { Value = menuItem.Name },
                    new MySqlParameter("@Description", MySqlDbType.VarChar) { Value = menuItem.Description },
                    new MySqlParameter("@Price", MySqlDbType.Decimal) { Value = menuItem.Price },
                    new MySqlParameter("@CategoryId", MySqlDbType.Int32) { Value = menuItem.CategoryId }
                };

                int rowsAffected = DBManager.ExecuteNonQuery(query, CommandType.Text, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating menu item: {ex.Message}");
                return false;
            }
        }

        public static bool DeleteMenuItem(int menuItemId)
        {
            try
            {
                string query = "DELETE FROM menu_items WHERE menu_item_id = @MenuItemId";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@MenuItemId", MySqlDbType.Int32) { Value = menuItemId }
                };

                int rowsAffected = DBManager.ExecuteNonQuery(query, CommandType.Text, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting menu item: {ex.Message}");
                return false;
            }
        }

        public static bool UpdateMenuItemCategory(int menuItemId, int categoryId)
        {
            try
            {
                string query = "UPDATE menu_items SET category_id = @CategoryId WHERE menu_item_id = @MenuItemId";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@MenuItemId", MySqlDbType.Int32) { Value = menuItemId },
                    new MySqlParameter("@CategoryId", MySqlDbType.Int32) { Value = categoryId }
                };

                int rowsAffected = DBManager.ExecuteNonQuery(query, CommandType.Text, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating menu item category: {ex.Message}");
                return false;
            }
        }
    }
}