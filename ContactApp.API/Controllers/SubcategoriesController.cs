using ContactApp.API.Models.DTO;
using ContactApp.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ContactApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubcategoriesController : ControllerBase
    {
        private readonly ISubcategoryRepository subcategoryRepository;

        public SubcategoriesController(ISubcategoryRepository subcategoryRepository)
        {
            this.subcategoryRepository = subcategoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubcategories(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var paginatedResult = await subcategoryRepository.GetAllAsync(page, pageSize);
            return Ok(paginatedResult);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubcategoryDto>> GetSubcategoryById(int id)
        {
            var existingSubcategory = await subcategoryRepository.GetByIdAsync(id);
            return existingSubcategory != null ? Ok(existingSubcategory) : NotFound();
        }

        [HttpPost]
        //[Authorize(Roles = "Writer")]
        public async Task<ActionResult<SubcategoryDto>> CreateSubcategory(
            [FromBody] CreateSubcategoryRequestDto request
        )
        {
            var category = await subcategoryRepository.CreateAsync(request);
            return Ok(category);
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> EditCategory(
            [FromRoute] int id,
            UpdateSubcategoryRequestDto request
        )
        {
            var updatedSubcategory = await subcategoryRepository.UpdateAsync(id, request);
            return updatedSubcategory != null ? Ok(updatedSubcategory) : NotFound();
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteSubcategory([FromRoute] int id)
        {
            var result = await subcategoryRepository.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
