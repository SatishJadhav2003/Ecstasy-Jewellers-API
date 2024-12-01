using System.Data.SqlClient;
using ECSTASYJEWELS.Data;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS
{
    public class CategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Category>> GetAllActiveCategory()
        {
            var category = new List<Category>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("SELECT Category_ID, Category_Name, Category_Image FROM Category WHERE Is_Active = 1", conn);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            category.Add(new Category
                            {
                                Category_ID = (int)reader["Category_ID"],
                                Category_Name = reader["Category_Name"].ToString()??"",
                                Category_Image = reader["Category_Image"].ToString() ??""
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving Category.", ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving Category.", ex);
            }
            return category;
        }
        
        public async Task<IEnumerable<Category>> GetCategoryByID(int Category_ID)
        {
            var category = new List<Category>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("SELECT Category_ID, Category_Name, Category_Image FROM Category WHERE Category_ID = "+Category_ID, conn);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            category.Add(new Category
                            {
                                Category_ID = (int)reader["Category_ID"],
                                Category_Name = reader["Category_Name"].ToString()??"",
                                Category_Image = reader["Category_Image"].ToString() ??""
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving Category.", ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving Category.", ex);
            }
            return category;
        }
       
    
        public async Task<ProductData[]> GetFeatureCategoryProducts()
        {
            var product = new List<ProductData>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand(
                        @"SELECT TOP 6 
                    Product_ID, Product_Name, Description, 
                    (Price +(select Top(1) Metal_Prices.Price from Metal_Prices where Metal_Prices.Metal_ID=prod.Metal_ID order by Date_Added desc)) as Price,
                    Weight, Stock_Quantity, Rating, Total_Ratings, Total_Reviews,
                    (SELECT img.Image_URL 
                     FROM Product_Images img 
                     WHERE img.Product_ID = prod.Product_ID AND img.Is_Primary = 1) AS Product_Image,Category_Name,prod.Category_ID
                FROM Products prod
                JOIN Category on Category.Category_ID = prod.Category_ID
                WHERE prod.Is_Active = 1 and Category.Is_Feature =1 order by prod.Total_Ratings desc", conn);


                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            product.Add(new ProductData
                            {
                                Product_ID = (int)reader["Product_ID"],
                                Category_ID = (int)reader["Category_ID"],
                                Product_Name = reader["Product_Name"].ToString() ?? "",
                                Category_Name = reader["Category_Name"].ToString() ?? "",
                                Product_Image = reader["Product_Image"].ToString() ?? "",
                                Description = reader["Description"].ToString() ?? "",
                                Price = (decimal)reader["Price"],
                                Rating = reader["Rating"] == DBNull.Value ? 0.0m : (decimal)reader["Rating"],
                                Total_Ratings = reader["Total_Ratings"] == DBNull.Value ? 0 : (int)reader["Total_Ratings"],
                                Total_Reviews = reader["Total_Reviews"] == DBNull.Value ? 0 : (int)reader["Total_Reviews"]
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving the product." + ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving the product." + ex);
            }

            return product.ToArray();  // Convert the list to an array before returning
        }

        
        public async Task<bool> AddCategory(Category category)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("INSERT INTO Category (Category_Name, Category_Image, Is_Active) VALUES (@Name, @Image, @IsActive)", conn);
                    command.Parameters.AddWithValue("@Name", category.Category_Name);
                    command.Parameters.AddWithValue("@Image", category.Category_Image);
                    command.Parameters.AddWithValue("@IsActive", true); // Assuming new categories are active by default

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0; // Returns true if a row was inserted
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while adding Category.", ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while adding Category.", ex);
            }
        }
    }
}