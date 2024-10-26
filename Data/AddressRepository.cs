using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS
{
    public class AddressRepository
    {
        private readonly string _connectionString;

        public AddressRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Address>> GetUserAddresses(decimal User_ID)
        {
            var AddressItems = new List<Address>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("select * from Addresses where User_ID=@User_ID and Is_Deleted=0 order by Is_Default DESC", conn);
                    command.Parameters.AddWithValue("@User_ID", User_ID);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            AddressItems.Add(new Address
                            {
                                Address_ID = (int)reader["Address_ID"],
                                Mobile = (decimal)reader["Mobile"],
                                Name = reader["Name"].ToString() ?? "",
                                Address_Type = reader["Address_Type"].ToString() ?? "",
                                Address_Line1 = reader["Address_Line1"].ToString() ?? "",
                                Address_Line2 = reader["Address_Line2"].ToString() ?? "",
                                City = reader["City"].ToString() ?? "",
                                State = reader["State"].ToString() ?? "",
                                Country = reader["Country"].ToString() ?? "",
                                Postal_Code = (int)reader["Postal_Code"],
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving Address Items." + ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving Address Items." + ex);
            }
            return AddressItems;
        }
        public async Task<int> AddNewAddress(Address NewAddress)
        {
            int response = 0;  // This will store the newly inserted Address_ID
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"INSERT INTO Addresses (Mobile, User_ID,Name,Address_Type,Address_Line1,Address_Line2,City,State,Country,Postal_Code,Is_Default,Is_Deleted)
                          VALUES (@Mobile, @User_ID,@Name,@Address_Type,@Address_Line1,@Address_Line2,@City,@State,@Country,@Postal_Code,@Is_Default,@Is_Deleted);
                          SELECT SCOPE_IDENTITY();";  // To return the newly inserted Address_ID

                    using (var command = new SqlCommand(query, conn))
                    {
                        // Add parameters to avoid SQL injection
                        command.Parameters.AddWithValue("@Mobile", NewAddress.Mobile);
                        command.Parameters.AddWithValue("@User_ID", NewAddress.User_ID);
                        command.Parameters.AddWithValue("@Name", NewAddress.Name);
                        command.Parameters.AddWithValue("@Address_Type", NewAddress.Address_Type);
                        command.Parameters.AddWithValue("@Address_Line1", NewAddress.Address_Line1);
                        command.Parameters.AddWithValue("@Address_Line2", NewAddress.Address_Line2);
                        command.Parameters.AddWithValue("@City", NewAddress.City);
                        command.Parameters.AddWithValue("@State", NewAddress.State);
                        command.Parameters.AddWithValue("@Country", NewAddress.Country);
                        command.Parameters.AddWithValue("@Postal_Code", NewAddress.Postal_Code);
                        command.Parameters.AddWithValue("@Is_Default", 0);
                        command.Parameters.AddWithValue("@Is_Deleted", 0);

                        // Execute the query and retrieve the new Address_ID
                        var result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            response = Convert.ToInt32(result);  // Assign the new Address_ID
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while adding the Address." + ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while adding the Address." + ex.Message);
            }

            return response;  // Return the new Address_ID
        }

        public async Task<bool> UpdateAddress(int addressId, Address updatedAddress)
        {
            bool isUpdated = false;  // This will indicate whether the update was successful
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"UPDATE Addresses 
                          SET Mobile = @Mobile, 
                              User_ID = @User_ID,
                              Name = @Name,
                              Address_Type = @Address_Type,
                              Address_Line1 = @Address_Line1,
                              Address_Line2 = @Address_Line2,
                              City = @City,
                              State = @State,
                              Country = @Country,
                              Postal_Code = @Postal_Code,
                              Is_Default = @Is_Default,
                              Is_Deleted = @Is_Deleted
                          WHERE Address_ID = @Address_ID";  // Use Address_ID to identify which record to update

                    using (var command = new SqlCommand(query, conn))
                    {
                        // Add parameters to avoid SQL injection
                        command.Parameters.AddWithValue("@Mobile", updatedAddress.Mobile);
                        command.Parameters.AddWithValue("@User_ID", updatedAddress.User_ID);
                        command.Parameters.AddWithValue("@Name", updatedAddress.Name);
                        command.Parameters.AddWithValue("@Address_Type", updatedAddress.Address_Type);
                        command.Parameters.AddWithValue("@Address_Line1", updatedAddress.Address_Line1);
                        command.Parameters.AddWithValue("@Address_Line2", updatedAddress.Address_Line2);
                        command.Parameters.AddWithValue("@City", updatedAddress.City);
                        command.Parameters.AddWithValue("@State", updatedAddress.State);
                        command.Parameters.AddWithValue("@Country", updatedAddress.Country);
                        command.Parameters.AddWithValue("@Postal_Code", updatedAddress.Postal_Code);
                        command.Parameters.AddWithValue("@Is_Default", 0);
                        command.Parameters.AddWithValue("@Is_Deleted", 0);
                        command.Parameters.AddWithValue("@Address_ID", addressId);  // Add Address_ID parameter

                        // Execute the update command and check how many rows were affected
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        isUpdated = rowsAffected > 0;  // Update was successful if at least one row was affected
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while updating the Address: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while updating the Address: " + ex.Message);
            }

            return isUpdated;  // Return whether the update was successful
        }

        public async Task<bool> RemoveFromAddress(int Address_ID)
        {
            bool isDeleted = false;  // This will store the result of the deletion (true if successful)

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"Update Addresses set Is_Deleted=1 WHERE Address_ID = @Address_ID";  // Query to delete the Address item by Address_ID

                    using (var command = new SqlCommand(query, conn))
                    {
                        // Add parameter to avoid SQL injection
                        command.Parameters.AddWithValue("@Address_ID", Address_ID);

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
                throw new Exception("Database error occurred while deleting the Address item." + ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while deleting the Address item." + ex.Message);
            }

            return isDeleted;  // Return whether the deletion was successful
        }

    }

}