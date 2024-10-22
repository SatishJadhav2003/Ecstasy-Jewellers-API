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

        public async Task<bool> IncrementQty(int cartId)
        {
            bool incremented = false;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = "UPDATE Cart SET Quantity = Quantity + 1 WHERE Cart_ID = @Cart_ID;";

                    using (var command = new SqlCommand(query, conn))
                    {
                        // Add parameter to avoid SQL injection
                        command.Parameters.AddWithValue("@Cart_ID", cartId);

                        // Use ExecuteNonQuery to get the number of rows affected
                        var result = await command.ExecuteNonQueryAsync();

                        // If at least one row is affected, consider the increment successful
                        incremented = result > 0;

                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while updating the cart." + ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while updating the cart." + ex.Message);
            }

            return incremented;
        }

        public async Task<bool> DecrementQty(int cartId)
        {
            bool decremented = false;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = "UPDATE Cart SET Quantity = Quantity - 1 WHERE Cart_ID = @Cart_ID AND Quantity > 1;";

                    using (var command = new SqlCommand(query, conn))
                    {
                        // Add parameter to avoid SQL injection
                        command.Parameters.AddWithValue("@Cart_ID", cartId);

                        // Use ExecuteNonQuery to get the number of rows affected
                        var result = await command.ExecuteNonQueryAsync();

                        // If at least one row is affected, consider the decrement successful
                        decremented = result > 0;

                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while updating the cart." + ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while updating the cart." + ex.Message);
            }
            return decremented;
        }

        public async Task<IEnumerable<CartOutput>> GetUserCartItems(decimal User_ID)
        {
            var CartItems = new List<CartOutput>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("SELECT Cart_ID,Quantity, Products.Product_ID,Products.Product_Name,Products.Description,Products.Price  " +
                    ",(select Image_URL from Product_Images where Product_Images.Product_ID=Products.Product_ID and Product_Images.Is_Primary=1 ) as Product_Image " +
                    "FROM Cart inner join Products on Products.Product_ID =Cart.Product_ID " +
                    "WHERE User_ID = @User_ID", conn);
                    command.Parameters.AddWithValue("@User_ID", User_ID);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            CartItems.Add(new CartOutput
                            {
                                Cart_ID = (int)reader["Cart_ID"],
                                Product_ID = (int)reader["Product_ID"],
                                Quantity = (int)reader["Quantity"],
                                Price = (decimal)reader["Price"],
                                Product_Name = reader["Product_Name"].ToString() ?? "",
                                Description = reader["Description"].ToString() ?? "",
                                Product_Image = reader["Product_Image"].ToString() ?? "",
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving Cart Items." + ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving Cart Items." + ex);
            }
            return CartItems;
        }

        public async Task<bool> RemoveFromCart(int Cart_ID)
        {
            bool isDeleted = false;  // This will store the result of the deletion (true if successful)

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"DELETE FROM Cart WHERE Cart_ID = @Cart_ID";  // Query to delete the cart item by Cart_ID

                    using (var command = new SqlCommand(query, conn))
                    {
                        // Add parameter to avoid SQL injection
                        command.Parameters.AddWithValue("@Cart_ID", Cart_ID);

                        // Execute the delete query
                        var result = await command.ExecuteNonQueryAsync();

                        // If at least one row is affected, consider the delete successful
                        isDeleted = result > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while deleting the cart item." + ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while deleting the cart item." + ex.Message);
            }

            return isDeleted;  // Return whether the deletion was successful
        }


    }

    public class CartOutput
    {
        public int Cart_ID { get; set; }
        public int User_ID { get; set; }
        public int Product_ID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public string Product_Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Product_Image { get; set; } = "";
    }
}