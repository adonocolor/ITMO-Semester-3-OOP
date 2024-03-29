﻿using NUnit.Framework;
using Shops.Entities;
using Shops.Services;
using Shops.Tools;


namespace Shops.Tests
{
    public class ShopTest
    {
        private ShopService _shopService;

        [SetUp]
        public void Setup()
        {
            _shopService = new ShopService();
        }

        [Test]
        public void ProductDelivery()
        {
            Shop shop = _shopService.RegisterShop("234", "523");
            Product banana =_shopService.RegisterProduct("Banana");
            Product apple = _shopService.RegisterProduct("Apple");
            _shopService.AddProductToDeliver(banana, 50, 40);
            _shopService.AddProductToDeliver(apple, 70, 100);
            _shopService.DeliverTo(shop);
            Assert.IsTrue(shop.ProductExists(banana) && shop.ProductExists(apple));
        }

        [Test]
        public void ChangeProductPrice()
        {
            Shop shop = _shopService.RegisterShop("234", "523");
            Product banana =_shopService.RegisterProduct("Banana");
            Product apple = _shopService.RegisterProduct("Apple");
            _shopService.AddProductToDeliver(banana, 50, 40);
            _shopService.AddProductToDeliver(apple, 70, 100);
            _shopService.DeliverTo(shop);
            _shopService.ChangePrice(shop, banana, 55);
            Assert.IsFalse(50 == shop.FindProduct(banana).Price);
            Assert.IsTrue(55 == shop.FindProduct(banana).Price);
        }

        [Test]
        public void ProfitSearch()
        {
            Shop shop = _shopService.RegisterShop("234", "523");
            Product banana = _shopService.RegisterProduct("Banana");
            Product apple = _shopService.RegisterProduct("Apple");
            _shopService.AddProductToDeliver(banana, 50, 2);
            _shopService.AddProductToDeliver(apple, 70, 2);
            _shopService.DeliverTo(shop);

            Shop shop_2 = _shopService.RegisterShop("432", "123");
            _shopService.AddProductToDeliver(banana, 70, 2);
            _shopService.AddProductToDeliver(apple, 70, 2);
            _shopService.DeliverTo(shop_2);

            _shopService.AddProductToSearch(apple, 1);
            _shopService.AddProductToSearch(banana, 1);
            Assert.AreEqual(120, _shopService.SearchProfitableDeal());
        } 
        
        [Test]
        public void BuyingProducts()
        {
            Shop shop = _shopService.RegisterShop("234", "523");
            Product banana = _shopService.RegisterProduct("Banana");
            Product apple = _shopService.RegisterProduct("Apple");
            _shopService.AddProductToDeliver(banana, 50, 40);
            _shopService.AddProductToDeliver(apple, 70, 100);
            _shopService.DeliverTo(shop);
            var bob = new Customer(240);
            bob.AddToWishlist(apple, 2);
            bob.AddToWishlist(banana, 3);
            Assert.Catch<ShopException>(() =>
            {
                _shopService.BuyProducts(bob, shop);
            });
        }
    }
}