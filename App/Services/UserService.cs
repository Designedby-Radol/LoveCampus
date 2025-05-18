using System;
using System.Collections.Generic;
using System.Linq;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Interfaces;
using CampusLove.Infrastructure.Config;

namespace CampusLove.App.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IInterestRepository _interestRepository;
        private readonly ICareerRepository _careerRepository;
        private readonly IGenderRepository _genderRepository;

        public UserService()
        {
            _userRepository = DatabaseConfig.RepositoryFactory.CreateRepository<IUserRepository>();
            _interestRepository = DatabaseConfig.RepositoryFactory.CreateRepository<IInterestRepository>();
            _careerRepository = DatabaseConfig.RepositoryFactory.CreateRepository<ICareerRepository>();
            _genderRepository = DatabaseConfig.RepositoryFactory.CreateRepository<IGenderRepository>();
        }

        public User Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = _userRepository.GetByEmail(email);
            
            if (user != null && user.Password == password)
            {
                _userRepository.UpdateLastAccess(user.Id);
                return user;
            }

            return null;
        }

        public User Register(User newUser, List<int> interestIds)
        {
            // Check if email already exists
            var existingUser = _userRepository.GetByEmail(newUser.Email);
            if (existingUser != null)
                throw new Exception("A user with this email already exists.");

            // Add user
            _userRepository.Add(newUser);

            // Add user interests
            if (interestIds.Count > 0)
            {
                _interestRepository.SaveUserInterests(newUser.Id, interestIds);
            }

            // Reload user to get the complete data
            return _userRepository.GetById(newUser.Id);
        }

        public IEnumerable<User> GetPotentialMatches(int userId, int limit = 10)
        {
            return _userRepository.GetPotentialMatches(userId, limit);
        }

        public IEnumerable<Gender> GetAllGenders()
        {
            return _genderRepository.GetAll();
        }

        public IEnumerable<Career> GetAllCareers()
        {
            return _careerRepository.GetAll();
        }

        public IEnumerable<Interest> GetAllInterests()
        {
            return _interestRepository.GetAll();
        }

        public IEnumerable<Interest> GetUserInterests(int userId)
        {
            return _interestRepository.GetInterestsByUserId(userId);
        }

        public void UpdateCredits(int userId, int newCreditAmount)
        {
            _userRepository.UpdateCredits(userId, newCreditAmount);
        }

        public void UpdateCapcoins(int userId, int newCapcoinAmount)
        {
            _userRepository.UpdateCapcoins(userId, newCapcoinAmount);
        }

        public User GetById(int id)
        {
            return _userRepository.GetById(id);
        }

        public IEnumerable<User> GetAll()
        {
            return _userRepository.GetAll();
        }

        public IEnumerable<User> GetMostLikedUsers(int limit = 5)
        {
            var users = _userRepository.GetAll().ToList();
            var interactionRepo = DatabaseConfig.RepositoryFactory.CreateRepository<IInteractionRepository>();

            // Get all interactions
            var interactions = interactionRepo.GetAll()
                .Where(i => i.Type == "like")
                .GroupBy(i => i.TargetUserId)
                .Select(g => new { UserId = g.Key, LikeCount = g.Count() })
                .OrderByDescending(x => x.LikeCount)
                .Take(limit);

            // Match with users
            var result = new List<User>();
            foreach (var interaction in interactions)
            {
                var user = users.FirstOrDefault(u => u.Id == interaction.UserId);
                if (user != null)
                {
                    result.Add(user);
                }
            }

            return result;
        }
    }
}