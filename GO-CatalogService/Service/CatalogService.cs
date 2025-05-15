using GOCore;

namespace GO_CatalogService.Service
{
    public class CatalogService
    {
        public List<Item> Items { get; set; } = new List<Item>();

        public void CreateItem(Item item)
        {
            Items.Add(item);
        }

        public void DeleteItem(Item item)
        {
            Items.Remove(item);
        }

        public void EditItem(Item item)
        {
            var index = Items.FindIndex(i => i.Id == item.Id);
            if (index != -1)
            {
                Items[index] = item;
            }
        }

        public List<Item> GetAllItems()
        {
            foreach (var item in Items)
            {
                Console.WriteLine($"Id: {item.Id}, Name: {item.Name}, Description: {item.Description}");
            }
            return Items; // Ensure the method returns a value as expected.  
        }
        public Item GetItemById(Guid id)
        {
            return Items.FirstOrDefault(i => i.Id == id);
        }

        public List<Item> GetItemsByCategory(string category)
        {
            return Items.Where(i => i.Category == category).ToList();
        }

        public void GetItemByValue()
        {
            foreach (var item in Items)
            {
                Console.WriteLine($"Id: {item.Id}, Name: {item.Name}, Description: {item.Description}");
            }
        }
        public void GetItemByName(string name)
        {
            var item = Items.FirstOrDefault(i => i.Name == name);
            if (item != null)
            {
                Console.WriteLine($"Id: {item.Id}, Name: {item.Name}, Description: {item.Description}");
            }
            else
            {
                Console.WriteLine("Item not found");
            }
        }
    }
}
