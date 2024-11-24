using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS.Data
{
    public class ProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Product>> GetAllProductsByCategory(decimal Category_ID)
        {
            var products = new List<Product>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand(
                        "SELECT Product_ID, Product_Name, Description, Price, Weight, Dimensions, Stock_Quantity, Rating, Total_Ratings, Total_Reviews, " +
                        "(SELECT img.Image_URL FROM Product_Images img WHERE img.Product_ID = prod.Product_ID AND img.Is_Primary = 1) as Product_Image " +
                        "FROM Products prod WHERE Is_Active = 1 and Category_ID = @Category_ID", conn);

                    // Use parameterized query to avoid SQL injection
                    command.Parameters.AddWithValue("@Category_ID", Category_ID);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Product
                            {
                                Product_ID = reader["Product_ID"] == DBNull.Value ? 0 : (int)reader["Product_ID"],
                                Product_Name = reader["Product_Name"]?.ToString() ?? "",
                                Description = reader["Description"]?.ToString() ?? "",
                                Product_Image = reader["Product_Image"]?.ToString() ?? "",
                                Price = reader["Price"] == DBNull.Value ? 0.0m : (decimal)reader["Price"],
                                Weight = reader["Weight"] == DBNull.Value ? 0.0m : (decimal)reader["Weight"],
                                Stock_Quantity = reader["Stock_Quantity"] == DBNull.Value ? 0 : (int)reader["Stock_Quantity"],
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
                throw new Exception("Database error occurred while retrieving products.", ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving products.", ex);
            }

            return products;
        }

        public async Task<ProductData[]> GetProductById(int Product_ID)
        {
            var product = new List<ProductData>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand(
                        "SELECT PROD.Product_ID,PROD.Rating,PROD.Total_Ratings,PROD.Total_Reviews, PROD.Category_ID, PROD.Product_Name, PROD.Description, PROD.Price, PROD.Weight, PROD.Stock_Quantity, " +
                        "DIM.Dimension_ID, DIM.Title, DIM.Dim_Desc, " +
                        "(SELECT img.Image_URL FROM Product_Images img WHERE img.Product_ID = PROD.Product_ID AND img.Is_Primary = 1) as Product_Image " +
                        "FROM Products AS PROD " +
                        "LEFT JOIN Dimensions as DIM ON DIM.Product_ID = PROD.Product_ID " +
                        "WHERE PROD.Is_Active = 1 AND PROD.Product_ID = @Product_ID", conn);

                    command.Parameters.AddWithValue("@Product_ID", Product_ID);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            product.Add(new ProductData
                            {
                                Product_ID = (int)reader["Product_ID"],
                                Category_ID = (int)reader["Category_ID"],
                                Dimension_ID = reader["Dimension_ID"] != DBNull.Value ? (int)reader["Dimension_ID"] : 0,  // Handle nullable values
                                Product_Name = reader["Product_Name"].ToString() ?? "",
                                Product_Image = reader["Product_Image"].ToString() ?? "",
                                Description = reader["Description"].ToString() ?? "",
                                Price = (decimal)reader["Price"],
                                Weight = (decimal)reader["Weight"],
                                Title = reader["Title"].ToString() ?? "",
                                Dim_Desc = reader["Dim_Desc"].ToString() ?? "",
                                Stock_Quantity = (int)reader["Stock_Quantity"],
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

    }

    public class ProductData
    {
        public int Product_ID { get; set; }
        public string Product_Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Category_ID { get; set; }
        public decimal Price { get; set; }
        public int Stock_Quantity { get; set; }
        public int Total_Ratings { get; set; }
        public int Total_Reviews { get; set; }
        public decimal Rating { get; set; }
        public decimal Weight { get; set; }
        public string Product_Image { get; set; } = string.Empty;

        public bool Is_Active { get; set; }
        public DateTime Date_Added { get; set; }
        public DateTime? Updated_Date { get; set; }
        public int Dimension_ID { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Dim_Desc { get; set; } = string.Empty;
    }

}

