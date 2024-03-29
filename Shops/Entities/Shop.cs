﻿using System.Collections.Generic;
using System.Linq;
using Shops.Services;
using Shops.Tools;

namespace Shops.Entities
{
    public class Shop
    {
        private static uint _id = 0;
        private readonly List<ProductExtra> _stock = new List<ProductExtra>();
        public Shop(string address, string name)
        {
            _id++;
            Address = address;
            Name = name;
            Id = _id;
        }

        internal uint Id { get; }
        internal string Address { get; }
        private string Name { get; }

        public void AddInStock(ProductExtra product)
        {
            ProductExtra foundProduct = FindProduct(product);

            if (foundProduct != null)
            {
                foundProduct.Price = product.Price;
                foundProduct.Quantity += product.Quantity;
                return;
            }

            _stock.Add(product);
        }

        public bool ProductExists(uint productId)
        {
            return _stock.Any(product => product.Id == productId);
        }

        public bool ProductExists(Product product)
        {
            return _stock.Any(item => item.Id == product.Id);
        }

        public ProductExtra FindProduct(Product product)
        {
            return _stock.FirstOrDefault(item => item.Id == product.Id);
        }

        public void ChangeProductPrice(uint productId, decimal newPrice)
        {
            ProductExtra foundProduct = FindProduct(productId);
            if (foundProduct == null)
                throw new ShopException("No shop with the ID like that!");
            foundProduct.Price = newPrice;
        }

        public ProductExtra GetItemFromStock(int i)
        {
            if (!Enumerable.Range(0, _stock.Capacity).Contains(i))
                throw new ShopException("Index out of range!");
            if (_stock[i] == null)
                throw new ShopException("No item like this");
            return _stock[i];
        }

        public void ChangeItemQuantity(int i, int quantity)
        {
            if (!Enumerable.Range(0, _stock.Capacity).Contains(i))
                throw new ShopException("Index out of range!");

            if (_stock[i] == null)
            {
                throw new ShopException("No item like this");
            }

            if (_stock[i].Quantity - quantity < 0)
            {
                throw new ShopException("Not enough products in this particular shop");
            }

            _stock[i].Quantity -= quantity;
        }

        public decimal FindProfitableDeal(ShopService shopService, decimal totalAmount)
        {
            if (!shopService.Package.All(t => ProductExists(t.Id) && FindProduct(t.Id).Quantity >= t.Quantity))
                throw new ShopException("Not enough products or products don't exist!");

            decimal currentTotalAmount = shopService.Package.Where(product =>
                    product.Quantity <= FindProduct(product.Id).Quantity).
                Sum(product => FindProduct(product.Id).Price * product.Quantity);

            if (currentTotalAmount < totalAmount)
                totalAmount = currentTotalAmount;
            return totalAmount;
        }

        public void BuyProducts(Customer customer)
        {
            for (int i = 0; i < customer.GetWishlistQuantity(); i++)
            {
                for (int j = 0; j < StockQuantity(); j++)
                {
                    if (customer.GetItemIdFromWishList(i) != GetItemFromStock(j).Id ||
                        customer.GetItemQuantityFromWishList(i) > GetItemFromStock(j).Quantity) continue;
                    if (customer.Balance - (GetItemFromStock(j).Price * customer.GetItemQuantityFromWishList(i)) < 0)
                    {
                        throw new ShopException("Not enough money!");
                    }

                    customer.Payment(GetItemFromStock(j).Price * customer.GetItemQuantityFromWishList(i));
                    ChangeItemQuantity(j, customer.GetItemQuantityFromWishList(i));
                }
            }
        }

        public int StockQuantity()
        {
            return _stock.Count;
        }

        internal ProductExtra FindProduct(uint productId)
        {
            return _stock.FirstOrDefault(item => item.Id == productId);
        }
    }
}