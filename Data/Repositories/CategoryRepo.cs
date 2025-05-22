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
    public class CategoryRepo
    {
        public static List<Category> GetAllCategories()
        {
            try
            {
                List<Category> categories = new List<Category>();
                string query = "SELECT category_id, category_name FROM categories";

                using (MySqlDataReader reader = DBManager.ExecuteReader(query, CommandType.Text))
                {
                    while (reader.Read())
                    {
                        Category category = new Category
                        {
                            CategoryId = Convert.ToInt32(reader["category_id"]),
                            CategoryName = reader["category_name"].ToString()
                        };
                        categories.Add(category);
                    }
                }

                return categories;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving categories: {ex.Message}");
                return new List<Category>();
            }
        }

        public static Category GetCategoryById(int categoryId)
        {
            try
            {
                string query = "SELECT category_id, category_name FROM categories WHERE category_id = @CategoryId";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@CategoryId", MySqlDbType.Int32) { Value = categoryId }
                };

                using (MySqlDataReader reader = DBManager.ExecuteReader(query, CommandType.Text, parameters))
                {
                    if (reader.Read())
                    {
                        Category category = new Category
                        {
                            CategoryId = Convert.ToInt32(reader["category_id"]),
                            CategoryName = reader["category_name"].ToString()
                        };

                        return category;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving category by ID: {ex.Message}");
                return null;
            }
        }

        public static Category GetCategoryByName(string categoryName)
        {
            try
            {
                string query = "SELECT category_id, category_name FROM categories WHERE category_name = @CategoryName";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@CategoryName", MySqlDbType.VarChar) { Value = categoryName }
                };

                using (MySqlDataReader reader = DBManager.ExecuteReader(query, CommandType.Text, parameters))
                {
                    if (reader.Read())
                    {
                        Category category = new Category
                        {
                            CategoryId = Convert.ToInt32(reader["category_id"]),
                            CategoryName = reader["category_name"].ToString()
                        };

                        return category;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving category by name: {ex.Message}");
                return null;
            }
        }

        public static bool AddCategory(Category category)
        {
            try
            {
                string query = @"INSERT INTO categories (category_name) 
                               VALUES (@CategoryName);
                               SELECT LAST_INSERT_ID();";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@CategoryName", MySqlDbType.VarChar) { Value = category.CategoryName }
                };

                object result = DBManager.ExecuteScalar(query, CommandType.Text, parameters);
                int categoryId = Convert.ToInt32(result);
                category.CategoryId = categoryId;

                return categoryId > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding category: {ex.Message}");
                return false;
            }
        }

        public static bool UpdateCategory(Category category)
        {
            try
            {
                string query = "UPDATE categories SET category_name = @CategoryName WHERE category_id = @CategoryId";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@CategoryId", MySqlDbType.Int32) { Value = category.CategoryId },
                    new MySqlParameter("@CategoryName", MySqlDbType.VarChar) { Value = category.CategoryName }
                };

                int rowsAffected = DBManager.ExecuteNonQuery(query, CommandType.Text, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating category: {ex.Message}");
                return false;
            }
        }

        public static bool DeleteCategory(int categoryId)
        {
            try
            {
                string query = "DELETE FROM categories WHERE category_id = @CategoryId";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@CategoryId", MySqlDbType.Int32) { Value = categoryId }
                };

                int rowsAffected = DBManager.ExecuteNonQuery(query, CommandType.Text, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting category: {ex.Message}");
                return false;
            }
        }
    }
}