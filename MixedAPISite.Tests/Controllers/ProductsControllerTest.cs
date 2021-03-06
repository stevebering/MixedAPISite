﻿using System;
using System.Web.Http;
using MixedAPISite.Controllers;
using MixedAPISite.Models;
using Moq;
using Xunit;

namespace MixedAPISite.Tests.Controllers
{
    public class ProductsControllerTest
    {
        [Fact]
        public void GetAllProducts() {
            var mock = new Mock<IProductRepository>();

            var controller = new ProductsController(mock.Object);
            var result = controller.GetAllProducts();

            mock.Verify(r => r.GetAll());
        }

        [Fact]
        public void GetProductById_should_call_get_on_product_repository() {
            // arrange 
            const int productId = 1;
            var mock = new Mock<IProductRepository>();

            mock.Setup(r => r.Get(productId)).Returns(new Product());

            var controller = new ProductsController(mock.Object);

            // act
            var result = controller.GetProductById(1);

            // assert
            mock.Verify(r => r.Get(productId));
        }

        [Fact]
        public void GetProductById_should_return_product_from_repository() {
            // arrange 
            const int productId = 1;
            var mock = new Mock<IProductRepository>();

            var expected = new Product();

            mock.Setup(r => r.Get(productId)).Returns(expected);

            var controller = new ProductsController(mock.Object);

            // act
            var result = controller.GetProductById(1);

            // assert
            Assert.Equal(expected, result);
        }


        [Fact]
        public void GetProductById_should_throw_if_no_product_found() {
            const int productId = 1;
            var mock = new Mock<IProductRepository>();

            mock.Setup(r => r.Get(productId))
                .Returns((Product)null);

            var controller = new ProductsController(mock.Object);

            HttpResponseException e = Assert.Throws<HttpResponseException>(() => controller.GetProductById(1));
            Assert.Equal(e.Response.StatusCode, System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public void GetProductsByCategory() {
            const string categoryName = "any";

            var mock = new Mock<IProductRepository>();

            var controller = new ProductsController(mock.Object);
            var result = controller.GetProductsByCategory(categoryName);

            mock.Verify(r => r.GetByCategory(categoryName));
        }
    }
}
