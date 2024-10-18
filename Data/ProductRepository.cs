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
                    var command = new SqlCommand("SELECT Product_ID, Product_Name, Description, Price, Weight, Dimensions,Stock_Quantity,  (SELECT img.Image_URL FROM Product_Images img WHERE img.Product_ID = prod.Product_ID AND img.Is_Primary = 1) as Product_Image FROM Products prod WHERE Is_Active = 1 and Category_ID=" + Category_ID, conn);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Product
                            {
                                Product_ID = (int)reader["Product_ID"],
                                Product_Name = reader["Product_Name"].ToString() ?? "",
                                Description = reader["Description"].ToString() ?? "",
                                Product_Image = reader["Product_Image"].ToString() ?? "",
                                Price = (decimal)reader["Price"],
                                Weight = (decimal)reader["Weight"],
                                Stock_Quantity = (int)reader["Stock_Quantity"],
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving products."+ ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving products."+ ex);
            }

            return products;
        }
    }

}

