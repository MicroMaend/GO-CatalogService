using GOCore;
namespace GO_CatalogService.Interface
{
    public interface ICatalogRepository
    {
        void CreateItem(Item item);
        void DeleteItem(Guid id);         
        void EditItem(Item item);            
        List<Item> GetAllItems();
        Item GetItemById(Guid id);
        List<Item> GetItemsByCategory(string category);
        List<Item> GetItemsByName(string name);
    }
}
