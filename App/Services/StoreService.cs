using System;
using System.Collections.Generic;
using System.Linq;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Interfaces;
using CampusLove.Infrastructure.Config;

namespace CampusLove.App.Services
{
    public class StoreService
    {
        private readonly IStoreRepository _storeRepository;
        private readonly UserService _userService;

        public StoreService()
        {
            _storeRepository = DatabaseConfig.RepositoryFactory.CreateRepository<IStoreRepository>();
            _userService = new UserService();
        }

        public IEnumerable<StoreItem> GetAllItems()
        {
            return _storeRepository.GetAll();
        }

        public IEnumerable<StoreItem> GetItemsByType(string type)
        {
            return _storeRepository.GetItemsByType(type);
        }

        public bool PurchaseItem(int userId, int itemId)
        {
            var user = _userService.GetById(userId);
            var item = _storeRepository.GetById(itemId);

            if (user == null || item == null)
                return false;

            // Check if user has enough capcoins
            if (user.Capcoins < item.CapcoinPrice)
                return false;

            // Process purchase based on item type
            switch (item.Type.ToLower())
            {
                case "token":
                    // Add tokens to user's available credits
                    _userService.UpdateCredits(userId, user.AvailableCredits + item.Quantity);
                    break;

                case "like":
                    // Add more daily likes
                    _userService.UpdateCredits(userId, user.AvailableCredits + item.Quantity);
                    break;

                default:
                    return false;
            }

            // Deduct capcoins
            _userService.UpdateCapcoins(userId, user.Capcoins - item.CapcoinPrice);

            return true;
        }

        public StoreItem GetById(int id)
        {
            return _storeRepository.GetById(id);
        }

        public void Add(StoreItem item)
        {
            _storeRepository.Add(item);
        }

        public void Update(StoreItem item)
        {
            _storeRepository.Update(item);
        }

        public void Delete(int id)
        {
            _storeRepository.Delete(id);
        }
    }
}