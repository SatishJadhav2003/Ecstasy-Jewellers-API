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

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(fileStream);
                            }


                            // Create ReviewImage object
                            var reviewImage = new Review_Images
                            {
                                Review_ID = reviewId,
                                Product_ID = reviewData.Product_ID,
                                Image = fileName
                            };
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

        [HttpGet("GetReviewImagesByProduct/{productId}")]
        public async Task<ActionResult<IEnumerable<Review_Images>>> GetReviewImagesByProduct(int productId)
        {
            try
            {
                var images = await _repository.GetReviewImagesByProduct(productId);
                if (images == null || !images.Any())
                {
                    return NotFound("No images found for the given Product ID.");
                }
                return Ok(images);
            }
            catch (Exception ex)
            {
                // Log the exception and return 500
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GetImageById/{imageId}")]
        public async Task<ActionResult<Review_Images>> GetImageById(int imageId)
        {
            try
            {
                var image = await _repository.GetReviewImageById(imageId);
                if (image == null)
                {
                    return NotFound("No image found for the given Image ID.");
                }
                return Ok(image);
            }
            catch (Exception ex)
            {
                // Log the exception and return 500
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetReviewsByProduct(int productId)
        {
            var reviews = await _repository.GetReviewsAndRatingsByProduct(productId);
            if (reviews == null || !reviews.Any())
            {
                return NotFound(new { Message = "No reviews found for the given product." });
            }
            return Ok(reviews);
        }

        [HttpGet("images/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            try
            {
                // Set the path to where the images are stored
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/review_images/", fileName);

                if (!System.IO.File.Exists(imagePath))
                {
                    return NotFound("Image not found.");
                }

                // Return the image as a file
                var fileBytes = System.IO.File.ReadAllBytes(imagePath);
                return File(fileBytes, "image/jpeg"); // Adjust MIME type if needed
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
