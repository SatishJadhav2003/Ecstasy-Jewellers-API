using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS.Data
{
    public class ProductImagesRepository
    {
         private readonly string _connectionString;

        public ProductImagesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Product_Images>> GetAllProductImages(decimal Product_ID)
        {
            var products = new List<Product_Images>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("SELECT Product_ID, Image_ID, Image_URL, Is_Primary  FROM Product_Images  WHERE  Product_ID=" + Product_ID, conn);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Product_Images
                            {
                                Image_ID = (int)reader["Image_ID"],
                                Product_ID = (int)reader["Product_ID"],
                                Image_URL = reader["Image_URL"].ToString() ?? "",
                                Is_Primary = (bool)reader["Is_Primary"]
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving product Images." + ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving product Images." + ex);
            }

            return products;
        }
    }
}
