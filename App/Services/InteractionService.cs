using System;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Interfaces;
using CampusLove.Infrastructure.Config;

namespace CampusLove.App.Services
{
    public class InteractionService
    {
        private readonly IInteractionRepository _interactionRepository;
        private readonly UserService _userService;
        private readonly MatchService _matchService;
        private const int MAX_DAILY_LIKES = 10;

        public InteractionService()
        {
            _interactionRepository = DatabaseConfig.RepositoryFactory.CreateRepository<IInteractionRepository>();
            _userService = new UserService();
            _matchService = new MatchService();
        }

        public bool LikeUser(int sourceUserId, int targetUserId)
        {
            // Get source user
            var sourceUser = _userService.GetById(sourceUserId);
            if (sourceUser == null)
                throw new ArgumentException("Source user not found");

            // Check if user has credits available
            if (sourceUser.AvailableCredits <= 0)
                return false;

            // Check if user has already liked the target
            if (_interactionRepository.HasLiked(sourceUserId, targetUserId))
                return false;

            // Create interaction
            var interaction = new Interaction(sourceUserId, targetUserId, "like");
            _interactionRepository.Add(interaction);

            // Reduce credits
            _userService.UpdateCredits(sourceUserId, sourceUser.AvailableCredits - 1);

            // Check for match
            bool isMatch = _matchService.CheckForMatch(sourceUserId, targetUserId);

            return isMatch;
        }

        public void DislikeUser(int sourceUserId, int targetUserId)
        {
            // Check if user has already interacted with the target
            if (_interactionRepository.HasLiked(sourceUserId, targetUserId))
                return;

            // Create interaction
            var interaction = new Interaction(sourceUserId, targetUserId, "dislike");
            _interactionRepository.Add(interaction);
        }

        public int GetRemainingLikes(int userId)
        {
            var user = _userService.GetById(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            return user.AvailableCredits;
        }

        public int GetDailyLikeCount(int userId)
        {
            return _interactionRepository.CountDailyLikes(userId);
        }

        public bool CanLike(int userId)
        {
            var user = _userService.GetById(userId);
            return user != null && user.AvailableCredits > 0;
        }
    }
}