using Microsoft.AspNetCore.Mvc;
using GOCore;
using GO_CatalogService.Service;
using GO_CatalogService.Interface;
using Microsoft.AspNetCore.Authorization; // Tilføjet for [Authorize]

namespace GO_Bidding.Controllers; // Bemærk: Namespace er GO_Bidding.Controllers, ikke GO_CatalogService.Controllers

[ApiController]
[Route("catalog")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(ICatalogRepository catalogRepository, ILogger<CatalogController> logger)
    {
        _catalogRepository = catalogRepository;
        _logger = logger;
    }

    // Opret item - Kun admins
    [HttpPost("items")]
    [Authorize(Roles = "Admin")] // Kun brugere i "Admin" rollen kan tilgå dette endpoint
    public IActionResult CreateItem([FromBody] Item item)
    {
        if (item == null)
        {
            _logger.LogError("Item cannot be null");
            return BadRequest("Item cannot be null");
        }
        // Du kan overveje at tilføje mere validering her, f.eks. Item.Name, Item.Description
        // if (string.IsNullOrWhiteSpace(item.Name)) { return BadRequest("Item name is required"); }

        _catalogRepository.CreateItem(item);
        return Ok();
    }

    // Slet item - Kun admins
    [HttpDelete("items/{id}")]
    [Authorize(Roles = "Admin")] // Kun brugere i "Admin" rollen kan tilgå dette endpoint
    public IActionResult DeleteItem(Guid id)
    {
        _catalogRepository.DeleteItem(id);
        return Ok();
    }

    // Rediger item - Kun admins
    [HttpPut("items/{id}")]
    [Authorize(Roles = "Admin")] // Kun brugere i "Admin" rollen kan tilgå dette endpoint
    public IActionResult EditItem(Guid id, [FromBody] Item updatedItem)
    {
        if (updatedItem == null)
        {
            _logger.LogError("Updated item cannot be null");
            return BadRequest("Updated item cannot be null");
        }
        // Du kan overveje at tilføje mere validering her

        updatedItem.Id = id; // hvis nødvendigt
        _catalogRepository.EditItem(updatedItem);
        return Ok();
    }

   
    [HttpGet("items")]
    [AllowAnonymous]
    public IActionResult GetAllItems()
    {
        var items = _catalogRepository.GetAllItems();
        return Ok(items);
    }

  
    [HttpGet("items/{id}")]
    [AllowAnonymous]
    public IActionResult GetItemById(Guid id)
    {
        var item = _catalogRepository.GetItemById(id);
        if (item == null)
            return NotFound();
        return Ok(item);
    }

   
    [HttpGet("items/category/{category}")]
    [AllowAnonymous]
    public IActionResult GetItemsByCategory(string category)
    {
        var items = _catalogRepository.GetItemsByCategory(category);
        return Ok(items);
    }
}
