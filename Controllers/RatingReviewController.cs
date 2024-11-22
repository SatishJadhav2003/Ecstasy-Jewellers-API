using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingReviewController : ControllerBase
    {
        private readonly RatingReviewRepository _repository;

        public RatingReviewController(RatingReviewRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("CanUserRateProduct/{User_ID}/{Product_ID}")]
        public async Task<ActionResult<bool>> CanUserRateProduct(int User_ID, int Product_ID)
        {
            try
            {
                var canRate = await _repository.CanUserRateProduct(User_ID, Product_ID);
                return Ok(canRate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> SaveRatingAndReview([FromForm] Rating_Reviews reviewData, List<IFormFile> images)
        {
            try
            {
                if (reviewData == null)
                {
                    return BadRequest("Review data is required.");
                }

                // Save the review data using the repository
                var reviewId = await _repository.AddRatingReview(reviewData);

                if (reviewId <= 0)
                {
                    return StatusCode(500, "Failed to save review data.");
                }
                if (images == null || !images.Any())
                {
                    Console.WriteLine("No images received.");
                }
                else
                {
                    Console.WriteLine($"Received {images.Count} images.");
                }

                // Check if images are provided
                if (images != null && images.Any())
                {
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/review_images/");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                        Console.WriteLine("Created upload directory.");
                    }
                    else
                    {
                        Console.WriteLine("Upload directory exists.");
                    }

                    foreach (var image in images)
                    {
                        if (image.Length > 0)
                        {
                            // Save each image to disk
                            string fileName = $"{Guid.NewGuid()}_{image.FileName}";
                            string filePath = Path.Combine(uploadPath, fileName);

                            Console.WriteLine($"Saving file: {filePath}");
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(fileStream);
                            }
                            Console.WriteLine($"File saved: {filePath}");


                            // Create ReviewImage object
                            var reviewImage = new ReviewImage
                            {
                                Review_ID = reviewId,
                                Product_ID = reviewData.Product_ID,
                                Image = fileName
                            };
Console.WriteLine($"Adding image to DB: Review_ID={reviewImage.Review_ID}, Product_ID={reviewImage.Product_ID}, Image={reviewImage.Image}");

                            // Save the image path using the repository
                            await _repository.AddReviewImage(reviewImage);
                        }
                    }
                }

                return Ok(reviewId);
            }
            catch (Exception ex)
            {
                // Log the error and return a 500 status code
                return StatusCode(500, ex.Message);
            }
        }

    }
}
