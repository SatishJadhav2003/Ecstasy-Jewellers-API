using System.Data.SqlClient;
using System.Security.Cryptography;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Method to create password hash and salt
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        // Method to verify password hash
        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }

        public async Task<User> Register(RegisterUserInfo user, string password)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // Generate password hash and salt
                    CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                    var command = new SqlCommand(
                        "INSERT INTO [Users] (First_Name, Email, Phone_Number, Is_Active, Password_Hash, Password_Salt) " +
                        "OUTPUT INSERTED.User_ID, INSERTED.First_Name, INSERTED.Email, INSERTED.Phone_Number " +
                        "VALUES (@Name, @Email, @Phone_Number, @IsActive, @PasswordHash, @PasswordSalt)", conn);

                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Phone_Number", user.Phone_Number);
                    command.Parameters.AddWithValue("@IsActive", true);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    command.Parameters.AddWithValue("@PasswordSalt", passwordSalt);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // Return the registered user information
                            return new User
                            {
                                User_ID = (int)reader["User_ID"],
                                First_Name = reader["First_Name"].ToString() ?? "",
                                Email = reader["Email"].ToString() ?? "",
                                Phone_Number = reader["Phone_Number"].ToString() ?? "",
                                Is_Active = true
                            };
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred while adding User.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding User.", ex);
            }

            return null;
        }

        public async Task<User> ValidateUserCredentials(string email, string password)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var command = new SqlCommand(
                    "SELECT User_ID, First_Name, Last_Name, Email, Phone_Number, Password_Hash, Password_Salt FROM Users WHERE Email = @Email", conn);
                command.Parameters.AddWithValue("@Email", email);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.HasRows)
                        return null;

                    await reader.ReadAsync();

                    int userId = reader.GetInt32(0);
                    string firstName = reader["First_Name"].ToString() ?? "";
                    string lastName = reader["Last_Name"].ToString() ?? "";
                    string phoneNumber = reader["Phone_Number"].ToString() ?? "";
                    byte[] storedPasswordHash = (byte[])reader["Password_Hash"];
                    byte[] storedPasswordSalt = (byte[])reader["Password_Salt"];

                    // Verify the password
                    if (!VerifyPasswordHash(password, storedPasswordHash, storedPasswordSalt))
                        return null;

                    // Return the user information if the password is correct
                    return new User
                    {
                        User_ID = userId,
                        First_Name = firstName,
                        Last_Name = lastName,
                        Email = email,
                        Phone_Number = phoneNumber
                    };
                }
            }
        }

        public async Task<IEnumerable<User>> GetUserInfo(decimal User_ID)
        {
            var items = new List<User>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("select User_ID,First_Name,Last_Name,Gender,Phone_Number,Phone_Verified,Email,Email_Verified from Users " +
                    "WHERE User_ID = @User_ID", conn);
                    command.Parameters.AddWithValue("@User_ID", User_ID);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add(new User
                            {
                                User_ID = (int)reader["User_ID"],
                                First_Name = reader["First_Name"].ToString() ?? "",
                                Last_Name = reader["Last_Name"].ToString() ?? "",
                                Gender = reader["Gender"].ToString() ?? "",
                                Email = reader["Email"].ToString() ?? "",
                                Email_Verified = (bool)reader["Email_Verified"],
                                Phone_Number = reader["Phone_Number"].ToString() ?? "",
                                Phone_Verified = (bool)reader["Phone_Verified"],
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving User Info." + ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving User Info." + ex);
            }
            return items;
        }

        public async Task<bool> UpdateUserInfo(User user)
        {
            bool isUpdated = false;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand(@"
                UPDATE Users 
                SET 
                    First_Name = @First_Name,
                    Last_Name = @Last_Name,
                    Gender = @Gender,
                    Phone_Number = @Phone_Number,
                    Phone_Verified = @Phone_Verified,
                    Email = @Email,
                    Email_Verified = @Email_Verified
                WHERE 
                    User_ID = @User_ID", conn);

                    // Add parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@User_ID", user.User_ID);
                    command.Parameters.AddWithValue("@First_Name", user.First_Name ?? string.Empty);
                    command.Parameters.AddWithValue("@Last_Name", user.Last_Name ?? string.Empty);
                    command.Parameters.AddWithValue("@Gender", user.Gender ?? string.Empty);
                    command.Parameters.AddWithValue("@Phone_Number", user.Phone_Number ?? string.Empty);
                    command.Parameters.AddWithValue("@Phone_Verified", user.Phone_Verified);
                    command.Parameters.AddWithValue("@Email", user.Email ?? string.Empty);
                    command.Parameters.AddWithValue("@Email_Verified", user.Email_Verified);

                    // Execute the query and check the result
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    isUpdated = rowsAffected > 0;
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while updating User Info." + ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while updating User Info." + ex);
            }

            return isUpdated;
        }

    }

    public class RegisterUserInfo
    {
        public string Name { get; set; } = "";
        public string Phone_Number { get; set; } = "";
        public string Email { get; set; } = "";
    }
    public class LoginUser
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";

    }
}
