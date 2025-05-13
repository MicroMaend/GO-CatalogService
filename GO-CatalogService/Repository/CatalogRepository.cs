using GOCore;
namespace GO_CatalogService.Repository
{
        public interface ICatalogRepository
        {
            void CreateItem(Item item);
            void DeleteItem(Item item);
            void EditItem(Item item);
            List<Item> GetAllItems();
            Item GetItemById(Guid id);
            List<Item> GetItemsByCategory(string category);
            List<Item> GetItemsByName(string name);
        }
}
