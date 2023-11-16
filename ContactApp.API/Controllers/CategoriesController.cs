using ContactApp.API.Models.DTO;
using ContactApp.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ContactApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var paginatedResult = await categoryRepository.GetAllAsync(page, pageSize);
            return Ok(paginatedResult);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var existingCategory = await categoryRepository.GetByIdAsync(id);
            return existingCategory != null ? Ok(existingCategory) : NotFound();
        }

        [HttpPost]
        //[Authorize(Roles = "Writer")]
        public async Task<ActionResult<CategoryDto>> CreateCategory(
            [FromBody] CreateCategoryRequestDto request
        )
        {
            var category = await categoryRepository.CreateAsync(request);
            return Ok(category);
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> EditCategory(
            [FromRoute] int id,
            UpdateCategoryRequestDto request
        )
        {
            var updatedCategory = await categoryRepository.UpdateAsync(id, request);
            return updatedCategory != null ? Ok(updatedCategory) : NotFound();
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var result = await categoryRepository.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
