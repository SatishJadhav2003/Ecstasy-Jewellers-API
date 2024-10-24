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
                    var command = new SqlCommand("select * from Addresses where User_ID=@User_ID order by Is_Default DESC", conn);
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
    }

}