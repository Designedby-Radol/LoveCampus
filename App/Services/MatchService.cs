using System;
using System.Collections.Generic;
using System.Linq;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Interfaces;
using CampusLove.Infrastructure.Config;

namespace CampusLove.App.Services
{
    public class MatchService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IInteractionRepository _interactionRepository;
        private readonly UserService _userService;

        public MatchService()
        {
            _matchRepository = DatabaseConfig.RepositoryFactory.CreateRepository<IMatchRepository>();
            _interactionRepository = DatabaseConfig.RepositoryFactory.CreateRepository<IInteractionRepository>();
            _userService = new UserService();
        }

        public IEnumerable<Match> GetUserMatches(int userId)
        {
            var matches = _matchRepository.GetMatchesByUserId(userId).ToList();

            // Load user data for each match
            foreach (var match in matches)
            {
                if (match.User1 == null)
                    match.User1 = _userService.GetById(match.User1Id);
                
                if (match.User2 == null)
                    match.User2 = _userService.GetById(match.User2Id);
            }

            return matches;
        }

        public bool CheckForMatch(int sourceUserId, int targetUserId)
        {
            // Check if target user has already liked source user
            bool targetLikesSource = _interactionRepository.HasLiked(targetUserId, sourceUserId);

            if (targetLikesSource)
            {
                // Create a new match
                _matchRepository.CreateMatch(sourceUserId, targetUserId);
                return true;
            }

            return false;
        }

        public IEnumerable<User> GetUsersWithMostMatches(int limit = 5)
        {
            // Get all matches
            var allMatches = _matchRepository.GetAll().ToList();
            
            // Count matches per user
            var userMatchCounts = new Dictionary<int, int>();
            
            foreach (var match in allMatches)
            {
                if (userMatchCounts.ContainsKey(match.User1Id))
                    userMatchCounts[match.User1Id]++;
                else
                    userMatchCounts[match.User1Id] = 1;
                
                if (userMatchCounts.ContainsKey(match.User2Id))
                    userMatchCounts[match.User2Id]++;
                else
                    userMatchCounts[match.User2Id] = 1;
            }
            
            // Get top users
            var topUserIds = userMatchCounts
                .OrderByDescending(kv => kv.Value)
                .Take(limit)
                .Select(kv => kv.Key);
                
            // Get user details
            var result = new List<User>();
            foreach (var userId in topUserIds)
            {
                var user = _userService.GetById(userId);
                if (user != null)
                {
                    result.Add(user);
                }
            }
            
            return result;
        }
    }
}