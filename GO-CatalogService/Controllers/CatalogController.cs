using Microsoft.AspNetCore.Mvc;
using GOCore;
using GO_CatalogService;
using GO_CatalogService.Service;
using GO_CatalogService.Repository;
using GO_CatalogService.Interface;

namespace GO_Bidding.Controllers;

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
    [HttpPost("items")]
    public IActionResult CreateItem([FromBody] Item item)
    {
        if (item == null)
        {
            _logger.LogError("Item cannot be null");
            return BadRequest("Item cannot be null");
        }
        _catalogRepository.CreateItem(item);
        return Ok();
    }
    [HttpDelete("items/{id}")]
public IActionResult DeleteItem(Guid id)
{
    _catalogRepository.DeleteItem(id);
    return Ok();
}

[HttpPut("items/{id}")]
public IActionResult EditItem(Guid id, [FromBody] Item updatedItem)
{
    updatedItem.Id = id; // hvis nødvendigt
    _catalogRepository.EditItem(updatedItem);
    return Ok();
}

    [HttpGet("items")]
    public IActionResult GetAllItems()
    {
        var items = _catalogRepository.GetAllItems();
        return Ok(items);
    }
    [HttpGet("items/{id}")]
    public IActionResult GetItemById(Guid id)
    {
        var item = _catalogRepository.GetItemById(id);
        if (item == null)
            return NotFound();
        return Ok(item);
    }
    [HttpGet("items/category/{category}")]
    public IActionResult GetItemsByCategory(string category)
    {
        var items = _catalogRepository.GetItemsByCategory(category);
        return Ok(items);
    }

}
