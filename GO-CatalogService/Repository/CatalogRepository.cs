﻿using GOCore;
using GO_CatalogService.Interface;
using MongoDB.Driver;

namespace GO_CatalogService.Repository
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly IMongoCollection<Item> _items;

        public CatalogRepository(string connectionString)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("GO-CatalogServiceDB");
            _items = database.GetCollection<Item>("Items");

            try
            {
                var count = _items.EstimatedDocumentCount();
                Console.WriteLine($"Antal dokumenter i Items-kollektionen ved opstart: {count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved tælling af dokumenter ved opstart: {ex.Message}");
            }
        }
        public void CreateItem(Item item)
        {
            _items.InsertOne(item);
        }
        public void DeleteItem(Guid id)
        {
            _items.DeleteOne(i => i.Id == id);
        }
        public void EditItem(Item item)
        {
            var filter = Builders<Item>.Filter.Eq(i => i.Id, item.Id);
            _items.ReplaceOne(filter, item);
        }
        public List<Item> GetAllItems()
        {
            return _items.Find(item => true).ToList();
        }
        public Item GetItemById(Guid id)
        {
            return _items.Find(i => i.Id == id).FirstOrDefault();
        }
        public List<Item> GetItemsByCategory(string category)
        {
            return _items.Find(i => i.Category == category).ToList();
        }
        public List<Item> GetItemsByName(string name)
        {
            return _items.Find(i => i.Name == name).ToList();
        }
        public void GetItemByValue()
        {
            foreach (var item in _items.AsQueryable())
            {
                Console.WriteLine($"Id: {item.Id}, Name: {item.Name}, Description: {item.Description}");
            }
        }
        public void GetItemByName(string name)
        {
            var item = _items.Find(i => i.Name == name).FirstOrDefault();
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