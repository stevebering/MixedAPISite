using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixedAPISite.Models
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        IEnumerable<Product> GetByCategory(string category);
        Product Get(int id);
        Product Add(Product item);
        void Remove(int id);
        bool Update(Product item);
    }

    public class ProductRepository
        : IProductRepository
    {
        private List<Product> _products = new List<Product>();
        private int _nextId = 1;

        public ProductRepository()
        {
            Add(new Product
            {
                Id = 1,
                Name = "Tomato soup",
                Category = "Groceries",
                Price = 1.39M
            });
            Add(new Product
            {
                Id = 2,
                Name = "Yo-yo",
                Category = "Toys",
                Price = 3.75M
            });
            Add(new Product
            {
                Id = 3,
                Name = "Hammer",
                Category = "Hardware",
                Price = 16.99M
            });
        }

        public IEnumerable<Product> GetAll()
        {
            return _products;
        }

        public IEnumerable<Product> GetByCategory(string category)
        {
            return _products.Where(p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase));
        }

        public Product Get(int id)
        {
            return _products.SingleOrDefault(p => p.Id == id);
        }

        public Product Add(Product item)
        {
            item.Id = _nextId++;
            _products.Add(item);
            return item;
        }

        public void Remove(int id)
        {
            _products.RemoveAll(p => p.Id == id);
        }

        public bool Update(Product item)
        {
            int index = _products.FindIndex(p => p.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            _products.RemoveAt(index);
            _products.Add(item);
            return true;
        }
    }
}
