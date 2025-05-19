using Microsoft.AspNetCore.Mvc;
using GOCore;
using GO_CatalogService;
using GO_CatalogService.Service;
using GO_CatalogService.Repository;
using GO_CatalogService.Interface;

namespace GO_Bidding.Controllers;

[ApiController]
[Route("[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly ILogger<CatalogController> _logger;
    public CatalogController(ICatalogRepository catalogRepository, ILogger<CatalogController> logger)
    {
        _catalogRepository = catalogRepository;
        _logger = logger;
    }
    [HttpPost("CreateItem")]
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
    [HttpDelete("DeleteItem")]
    public IActionResult DeleteItem([FromBody] Item item)
    {
        _catalogRepository.DeleteItem(item);
        return Ok();
    }
    [HttpPut("EditItem")]
    public IActionResult EditItem([FromBody] Item item)
    {
        _catalogRepository.EditItem(item);
        return Ok();
    }
    [HttpGet("GetAllItems")]
    public IActionResult GetAllItems()
    {
        var items = _catalogRepository.GetAllItems();
        return Ok(items);
    }
    [HttpGet("GetItemById/{id}")]
    public IActionResult GetItemById(Guid id)
    {
        var item = _catalogRepository.GetItemById(id);
        if (item == null)
            return NotFound();
        return Ok(item);
    }
    [HttpGet("GetItemsByCategory/{category}")]
    public IActionResult GetItemsByCategory(string category)
    {
        var items = _catalogRepository.GetItemsByCategory(category);
        return Ok(items);
    }

}
