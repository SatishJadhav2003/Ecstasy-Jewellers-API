using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS
{
    public class WatchlistRepository
    {
        private readonly string _connectionString;

        public WatchlistRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddtoWatchlist(Watchlist toWatchlist)
        {
            int response = 0;  // This will store the newly inserted Watchlist_ID
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"INSERT INTO Watchlist (Product_ID, User_ID)
                          VALUES (@Product_ID, @User_ID);
                          SELECT SCOPE_IDENTITY();";  // To return the newly inserted Watchlist_ID

                    using (var command = new SqlCommand(query, conn))
                    {
                        // Add parameters to avoid SQL injection
                        command.Parameters.AddWithValue("@Product_ID", toWatchlist.Product_ID);
                        command.Parameters.AddWithValue("@User_ID", toWatchlist.User_ID);

                        // Execute the query and retrieve the new Watchlist_ID
                        var result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            response = Convert.ToInt32(result);  // Assign the new Watchlist_ID
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while adding the Watchlist." + ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while adding the Watchlist." + ex.Message);
            }

            return response;  // Return the new Watchlist_ID
        }

        public async Task<IEnumerable<WatchlistOutput>> GetUserWatchlistItems(decimal User_ID)
        {
            var WatchlistItems = new List<WatchlistOutput>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("SELECT Watchlist_ID, Products.Product_ID,Products.Product_Name,Products.Description,Products.Price  " +
                    ",(select Image_URL from Product_Images where Product_Images.Product_ID=Products.Product_ID and Product_Images.Is_Primary=1 ) as Product_Image " +
                    "FROM Watchlist inner join Products on Products.Product_ID =Watchlist.Product_ID " +
                    "WHERE User_ID = @User_ID", conn);
                    command.Parameters.AddWithValue("@User_ID", User_ID);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            WatchlistItems.Add(new WatchlistOutput
                            {
                                Watchlist_ID = (int)reader["Watchlist_ID"],
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
                throw new Exception("Database error occurred while retrieving Watchlist Items." + ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving Watchlist Items." + ex);
            }
            return WatchlistItems;
        }

        public async Task<bool> RemoveFromWatchlist(int Watchlist_ID)
        {
            bool isDeleted = false;  // This will store the result of the deletion (true if successful)

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"DELETE FROM Watchlist WHERE Watchlist_ID = @Watchlist_ID";  // Query to delete the Watchlist item by Watchlist_ID

                    using (var command = new SqlCommand(query, conn))
                    {
                        // Add parameter to avoid SQL injection
                        command.Parameters.AddWithValue("@Watchlist_ID", Watchlist_ID);

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
                throw new Exception("Database error occurred while deleting the Watchlist item." + ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while deleting the Watchlist item." + ex.Message);
            }

            return isDeleted;  // Return whether the deletion was successful
        }

    }

    public class WatchlistOutput
    {
        public int Watchlist_ID { get; set; }
        public int User_ID { get; set; }
        public int Product_ID { get; set; }
        public decimal Price { get; set; }

        public string Product_Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Product_Image { get; set; } = "";
    }
}