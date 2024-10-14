// Controllers/CategoryController.cs
using System.Data.SqlClient;
using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly CategoryRepository _repository;

        public CategoryController(CategoryRepository Repository)
        {
            _repository = Repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EJ_Category>>> GetAllActiveCategory()
        {
            try
            {
                var category = await _repository.GetAllActiveCategory();
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }



        // private readonly string _connectionString;


        // public CategoryController(IConfiguration configuration)
        // {
        //     _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        // }
        // // Get All Category
        // [HttpGet]
        // public async IAsyncEnumerable<EJ_Category> GetAllCategories()
        // {
        //     List<EJ_Category> categories = new List<EJ_Category>();

        //     try
        //     {
        //         using (SqlConnection conn = new SqlConnection(_connectionString))
        //         {
        //             await conn.OpenAsync();
        //             SqlCommand cmd = new SqlCommand(
        //                 "SELECT Category_ID, Category_Name, Category_Image FROM EJ_Category WHERE Is_Active = 1", conn);

        //             using (var reader = await cmd.ExecuteReaderAsync())
        //             {
        //                 while (await reader.ReadAsync())
        //                 {
        //                     categories.Add(new EJ_Category
        //                     {
        //                         Category_ID = reader["Category_ID"] != DBNull.Value ? (decimal)reader["Category_ID"] : 0,
        //                         Category_Name = reader["Category_Name"] != DBNull.Value ? reader["Category_Name"].ToString() : string.Empty,
        //                         Category_Image = reader["Category_Image"] != DBNull.Value ? reader["Category_Image"].ToString() : string.Empty,
        //                     });
        //                 }
        //             }
        //         }
        //     }
        //     catch (SqlException sqlEx)
        //     {
        //         // Log SQL-specific error
        //         Console.WriteLine($"SQL Error: {sqlEx.Message}");
        //         yield break;  // Stop the iteration in case of error
        //     }
        //     catch (Exception ex)
        //     {
        //         // Log general error
        //         Console.WriteLine($"Error: {ex.Message}");
        //         yield break;  // Stop the iteration in case of error
        //     }

        //     // Yield the categories after the try-catch block
        //     foreach (var category in categories)
        //     {
        //         yield return category;
        //     }
        // }


        // Create Banner
        // [HttpPost]
        // public OperationResult AddCategory(EJ_Category category)
        // {
        //     var result = new OperationResult();

        //     try
        //     {
        //         using (SqlConnection conn = new SqlConnection(_connectionString))
        //         {
        //             conn.Open();
        //             SqlCommand cmd = new SqlCommand(
        //                 "INSERT INTO EJ_Category (Category_Name, Category_Image, Is_Active, Is_Edited, Is_Deleted, Inserted_Date) " +
        //                 "VALUES (@Category_Name, @Category_Image, @Is_Active, @Is_Edited, @Is_Deleted, @Inserted_Date)", conn);

        //             // Add parameters for the category data
        //             cmd.Parameters.AddWithValue("@Category_Name", category.Category_Name ?? (object)DBNull.Value);
        //             cmd.Parameters.AddWithValue("@Category_Image", category.Category_Image ?? (object)DBNull.Value);
        //             cmd.Parameters.AddWithValue("@Is_Active", true);
        //             cmd.Parameters.AddWithValue("@Is_Edited", false);
        //             cmd.Parameters.AddWithValue("@Is_Deleted", false);
        //             cmd.Parameters.AddWithValue("@Inserted_Date", DateTime.Now);

        //             // Execute the query
        //             cmd.ExecuteNonQuery();

        //             result.Success = true;
        //             result.Message = "Category added successfully.";
        //         }
        //     }
        //     catch (SqlException sqlEx)
        //     {
        //         // Log SQL-specific error
        //         Console.WriteLine($"SQL Error: {sqlEx.Message}");
        //         result.Success = false;
        //         result.Message = $"SQL Error: {sqlEx.Message}";
        //     }
        //     catch (Exception ex)
        //     {
        //         // Log general error
        //         Console.WriteLine($"Error: {ex.Message}");
        //         result.Success = false;
        //         result.Message = $"Error: {ex.Message}";
        //     }

        //     return result;
        // }
    }


}
