using Microsoft.AspNetCore.Mvc;
using GOCore;
using GO_CatalogService;
using GO_CatalogService.Service;
using GO_CatalogService.Repository;
using GO_CatalogService.Interface;
namespace GO_Bidding.Controllers;

[ApiController]
[Route("[catalog]")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogRepository _catalogRepository;
    public CatalogController(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }
    [HttpPost("Item")]
    public IActionResult CreateItem([FromBody] Item item)
    {
        _catalogRepository.CreateItem(item);
        return Ok();
    }
    [HttpDelete("Item")]
    public IActionResult DeleteItem([FromBody] Item item)
    {
        _catalogRepository.DeleteItem(item);
        return Ok();
    }
    [HttpPut("Item")]
    public IActionResult EditItem([FromBody] Item item)
    {
        _catalogRepository.EditItem(item);
        return Ok();
    }
    [HttpGet("Items")]
    public IActionResult GetAllItems()
    {
        var items = _catalogRepository.GetAllItems();
        return Ok(items);
    }
    [HttpGet("Item/{id}")]
    public IActionResult GetItemById(Guid id)
    {
        var item = _catalogRepository.GetItemById(id);
        if (item == null)
            return NotFound();
        return Ok(item);
    }
    [HttpGet("Items/{category}")]
    public IActionResult GetItemsByCategory(string category)
    {
        var items = _catalogRepository.GetItemsByCategory(category);
        return Ok(items);
    }
}
