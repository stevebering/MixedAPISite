using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MixedAPISite.Models;

namespace MixedAPISite.Controllers
{
    public class ProductsController : ApiController
    {
        private IProductRepository _repository;

        public ProductsController(IProductRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _repository.GetAll();
        }

        public Product GetProductById(int id)
        {
            var product = _repository.Get(id);
            if (product == null)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound);
                throw new HttpResponseException(resp);
            }

            return product;
        }

        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            return _repository.GetByCategory(category);
        }
    }
}
