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
    }

    public class RegisterUserInfo
    {
        public string Name { get; set; } = "";
        public string Phone_Number { get; set; } = "";
        public string Email { get; set; } = "";
    }
    public class LoginUser
    {
        public string Email {get;set;}="";
        public string Password {get;set;}="";

    }
}
