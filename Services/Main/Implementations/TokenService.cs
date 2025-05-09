using Common.Models.Requests;
using Data.Entities;
using Data.Repositories.Interfaces;
using Services.Main.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Main.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepository;

        public TokenService(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public async Task<bool> AddNotificationTokenAsync(NotificationTokenCreateDTO dto, int userId)
        {
            var existingToken = await _tokenRepository.GetNotificationTokenByTokenAsync(dto.Token);

            if (existingToken != null)
            {
                existingToken.Platform = dto.Platform;
                existingToken.DeviceModel = dto.DeviceModel;
                existingToken.LastSeen = DateTime.UtcNow;
                existingToken.UserId = userId;

                return await _tokenRepository.UpdateNotificationTokenAsync(existingToken);
            }

            var newToken = new NotificationToken
            {
                Token = dto.Token,
                Platform = dto.Platform,
                DeviceModel = dto.DeviceModel,
                UserId = userId,
                LastSeen = DateTime.UtcNow
            };

            var result = await _tokenRepository.AddNotificationTokenAsync(newToken);

            if (result)
            {
                return true;
            }
            else
            {
                throw new Exception("Failed to add notification token.");
            }

        }
    }
}
