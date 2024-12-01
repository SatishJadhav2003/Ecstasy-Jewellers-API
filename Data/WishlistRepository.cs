using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS
{
    public class WishlistRepository
    {
        private readonly string _connectionString;

        public WishlistRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddtoWishlist(Wishlist toWishlist)
        {
            int response = 0;  // This will store the newly inserted Wishlist_ID
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"INSERT INTO Wishlist (Product_ID, User_ID)
                          VALUES (@Product_ID, @User_ID);
                          SELECT SCOPE_IDENTITY();";  // To return the newly inserted Wishlist_ID

                    using (var command = new SqlCommand(query, conn))
                    {
                        // Add parameters to avoid SQL injection
                        command.Parameters.AddWithValue("@Product_ID", toWishlist.Product_ID);
                        command.Parameters.AddWithValue("@User_ID", toWishlist.User_ID);

                        // Execute the query and retrieve the new Wishlist_ID
                        var result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            response = Convert.ToInt32(result);  // Assign the new Wishlist_ID
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while adding the Wishlist." + ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while adding the Wishlist." + ex.Message);
            }

            return response;  // Return the new Wishlist_ID
        }
        public async Task<IEnumerable<WishlistOutput>> GetUserWishlistItems(decimal User_ID)
        {
            var WishlistItems = new List<WishlistOutput>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand(@"SELECT Wishlist_ID, Products.Product_ID,Products.Product_Name,Products.Description,
(Price +(select Top(1) Metal_Prices.Price from Metal_Prices where Metal_Prices.Metal_ID=Products.Metal_ID order by Date_Added desc)) as Price
                    ,(select Image_URL from Product_Images where Product_Images.Product_ID=Products.Product_ID and Product_Images.Is_Primary=1 ) as Product_Image  
                    FROM Wishlist inner join Products on Products.Product_ID =Wishlist.Product_ID  
                    WHERE User_ID = @User_ID", conn);
                    command.Parameters.AddWithValue("@User_ID", User_ID);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            WishlistItems.Add(new WishlistOutput
                            {
                                Wishlist_ID = (int)reader["Wishlist_ID"],
                                Product_ID = (int)reader["Product_ID"],
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
                throw new Exception("Database error occurred while retrieving Wishlist Items." + ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving Wishlist Items." + ex);
            }
            return WishlistItems;
        }

        public async Task<bool> RemoveFromWishlist(int Wishlist_ID)
        {
            bool isDeleted = false;  // This will store the result of the deletion (true if successful)

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"DELETE FROM Wishlist WHERE Wishlist_ID = @Wishlist_ID";  // Query to delete the Wishlist item by Wishlist_ID

                    using (var command = new SqlCommand(query, conn))
                    {
                        // Add parameter to avoid SQL injection
                        command.Parameters.AddWithValue("@Wishlist_ID", Wishlist_ID);

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
                throw new Exception("Database error occurred while deleting the Wishlist item." + ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while deleting the Wishlist item." + ex.Message);
            }

            return isDeleted;  // Return whether the deletion was successful
        }


    }

    public class WishlistOutput
    {
        public int Wishlist_ID { get; set; }
        public int User_ID { get; set; }
        public int Product_ID { get; set; }
        public decimal Price { get; set; }

        public string Product_Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Product_Image { get; set; } = "";
    }
}