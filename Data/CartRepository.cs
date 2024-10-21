using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS
{
    public class CartRepository
    {
        private readonly string _connectionString;

        public CartRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddtoCart(Cart toCart)
        {
            int response = 0;  // This will store the newly inserted Cart_ID
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"INSERT INTO Cart (Product_ID, User_ID, Quantity)
                          VALUES (@Product_ID, @User_ID, @Quantity);
                          SELECT SCOPE_IDENTITY();";  // To return the newly inserted Cart_ID

                    using (var command = new SqlCommand(query, conn))
                    {
                        // Add parameters to avoid SQL injection
                        command.Parameters.AddWithValue("@Product_ID", toCart.Product_ID);
                        command.Parameters.AddWithValue("@User_ID", toCart.User_ID);
                        command.Parameters.AddWithValue("@Quantity", 1);

                        // Execute the query and retrieve the new Cart_ID
                        var result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            response = Convert.ToInt32(result);  // Assign the new Cart_ID
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while adding the cart." + ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while adding the cart." + ex.Message);
            }

            return response;  // Return the new Cart_ID
        }

    }
}