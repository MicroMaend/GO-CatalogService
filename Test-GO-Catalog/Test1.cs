using GO_CatalogService.Service;
using GOCore;

namespace Test_GO_Catalog
{

    /* 
     Make test for all of these methods in the CatalogService class
    - CreateItem()
    - DeleteItem()
    - EditItem()
    - GetAllItems()
    - GetItemById()
    - GetItemByCategory()
    - GetItemByValue()
    - GetItemByName()
     */
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void CreateItem()
        {
            // Arrange
            var catalogService = new CatalogService();
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = "Test Item",
                Description = "This is a test item",
                Category = "Test Category"
            };

            // Act
            catalogService.CreateItem(item);
            var createdItem = catalogService.GetItemById(item.Id);

            // Assert
            Assert.AreEqual(item.Id, createdItem.Id);
            Assert.AreEqual(item.Name, createdItem.Name);
            Assert.AreEqual(item.Description, createdItem.Description);
            Assert.AreEqual(item.Category, createdItem.Category);

        }
    }
}
