
using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace Banner.DataAccess
{
    public class EJ_BannerDataAccess
    {
        private readonly string _connectionString;


        public EJ_BannerDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }


        // Create Banner
        public void AddBanner(EJ_Banner banner)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO EJ_Banner (Banner_Name, Banner_Image) VALUES (@Banner_Name, @Banner_Image)", conn);
                cmd.Parameters.AddWithValue("@Banner_Name", banner.Banner_Name);
                cmd.Parameters.AddWithValue("@Banner_Image", banner.Banner_Image);
                cmd.ExecuteNonQuery();
            }
        }

        // Read All Banners
       public List<EJ_Banner> GetAllBanners()
{
    var banners = new List<EJ_Banner>();

    try
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM EJ_Banner", conn);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    banners.Add(new EJ_Banner
                    {
                        Banner_ID = reader["Banner_ID"] != DBNull.Value ? (int)reader["Banner_ID"] : 0,
                        Banner_Name = reader["Banner_Name"] != DBNull.Value ? reader["Banner_Name"].ToString() : string.Empty,
                        Banner_Image = reader["Banner_Image"] != DBNull.Value ? reader["Banner_Image"].ToString() : string.Empty,
                        Is_Active = reader["Is_Active"] != DBNull.Value && (bool)reader["Is_Active"],
                        Is_Deleted = reader["Is_Deleted"] != DBNull.Value && (bool)reader["Is_Deleted"],
                        Is_Edited = reader["Is_Edited"] != DBNull.Value && (bool)reader["Is_Edited"],
                    });
                }
            }
        }
    }
    catch (Exception ex)
    {
        // Log or handle the exception appropriately
        Console.WriteLine($"Error: {ex.Message}");
    }

    return banners;
}


        // Update Product
        // public void UpdateProduct(Product product)
        // {
        //     using (SqlConnection conn = new SqlConnection(_connectionString))
        //     {
        //         conn.Open();
        //         SqlCommand cmd = new SqlCommand("UPDATE Products SET Name = @Name, Price = @Price WHERE Id = @Id", conn);
        //         cmd.Parameters.AddWithValue("@Id", product.Id);
        //         cmd.Parameters.AddWithValue("@Name", product.Name);
        //         cmd.Parameters.AddWithValue("@Price", product.Price);
        //         cmd.ExecuteNonQuery();
        //     }
        // }

        // Delete Product
        // public void DeleteProduct(int id)
        // {
        //     using (SqlConnection conn = new SqlConnection(_connectionString))
        //     {
        //         conn.Open();
        //         SqlCommand cmd = new SqlCommand("DELETE FROM Products WHERE Id = @Id", conn);
        //         cmd.Parameters.AddWithValue("@Id", id);
        //         cmd.ExecuteNonQuery();
        //     }
        // }
    }
}
