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

        public async Task<int> AddProductImage(Product_Images productImage)
        {
            int newImageId = 0;  // This will store the newly inserted Image_ID
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"INSERT INTO Product_Images (Product_ID, Image_URL, Is_Primary)
                          VALUES (@Product_ID, @Image_URL, @Is_Primary);
                          SELECT SCOPE_IDENTITY();";  // To return the newly inserted Image_ID

                    using (var command = new SqlCommand(query, conn))
                    {
                        // Add parameters to avoid SQL injection
                        command.Parameters.AddWithValue("@Product_ID", productImage.Product_ID);
                        command.Parameters.AddWithValue("@Image_URL", productImage.Image_URL ?? "");
                        command.Parameters.AddWithValue("@Is_Primary", productImage.Is_Primary);

                        // Execute the query and retrieve the new Image_ID
                        var result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            newImageId = Convert.ToInt32(result);  // Assign the new Image_ID
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while adding the product image." + ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while adding the product image." + ex.Message);
            }

            return newImageId;  // Return the new Image_ID
        }

    }
}
