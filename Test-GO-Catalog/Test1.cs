using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using GO_Bidding.Controllers;
using GO_CatalogService.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using GOCore;

[TestClass]
public class CatalogControllerTests
{
    private CatalogController _controller;
    private Mock<ICatalogRepository> _mockRepo;
    private Mock<ILogger<CatalogController>> _mockLogger;

    [TestInitialize]
    public void Setup()
    {
        _mockRepo = new Mock<ICatalogRepository>();
        _mockLogger = new Mock<ILogger<CatalogController>>();
        _controller = new CatalogController(_mockRepo.Object, _mockLogger.Object);
    }

    [TestMethod]
    public void CreateItem_ShouldReturnBadRequest_WhenItemIsNull()
    {
        var result = _controller.CreateItem(null);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void CreateItem_ShouldCallRepository_WhenItemIsValid()
    {
        var item = new Item { Id = Guid.NewGuid(), Name = "Test" };

        var result = _controller.CreateItem(item);

        _mockRepo.Verify(repo => repo.CreateItem(item), Times.Once);
        Assert.IsInstanceOfType(result, typeof(OkResult));
    }

    [TestMethod]
    public void GetItemById_ShouldReturnNotFound_WhenItemDoesNotExist()
    {
        _mockRepo.Setup(r => r.GetItemById(It.IsAny<Guid>())).Returns((Item)null);

        var result = _controller.GetItemById(Guid.NewGuid());

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void DeleteItem_ShouldCallRepository()
    {
        var id = Guid.NewGuid();

        var result = _controller.DeleteItem(id);

        _mockRepo.Verify(r => r.DeleteItem(id), Times.Once);
        Assert.IsInstanceOfType(result, typeof(OkResult));
    }

    [TestMethod]
    public void GetItemsByCategory_ShouldReturnExpectedItems()
    {
        var category = "Electronics";
        var expectedItems = new List<Item> { new Item { Name = "TV", Category = category } };
        _mockRepo.Setup(r => r.GetItemsByCategory(category)).Returns(expectedItems);

        var result = _controller.GetItemsByCategory(category) as OkObjectResult;

        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(expectedItems, (List<Item>)result.Value);
    }

}