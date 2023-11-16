using ContactApp.API.Models.DTO;
using ContactApp.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ContactApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository contactRepository;

        public ContactsController(IContactRepository contactRepository)
        {
            this.contactRepository = contactRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContacts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var paginatedResult = await contactRepository.GetAllAsync(page, pageSize);
            return Ok(paginatedResult);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactDto>> GetContactById(int id)
        {
            var existingContact = await contactRepository.GetByIdAsync(id);
            return existingContact != null ? Ok(existingContact) : NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateContactRequestDto request)
        {
            var contact = await contactRepository.CreateAsync(request);
            return Ok(contact);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> EditCategory(
            [FromRoute] int id,
            UpdateContactRequestDto request
        )
        {
            var updatedContact = await contactRepository.UpdateAsync(id, request);
            return updatedContact != null ? Ok(updatedContact) : NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var result = await contactRepository.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
