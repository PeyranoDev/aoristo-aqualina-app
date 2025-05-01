using Common.Models.Responses;
using Data.Entities;
using Data.Repositories.Interfaces;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Services.Main.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Main.Implementations
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepository _invitationRepository;

        public InvitationService(IInvitationRepository invitationRepository)
        {
            _invitationRepository = invitationRepository;
        }
        public async Task<Invitation?> GetInvitationAsync(string token)
        {
            return await _invitationRepository.GetByTokenWithRoleAsync(token);
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            return await _invitationRepository.IsTokenValidAsync(token);
        }

        public async Task<string> CreateInvitationAsync(CreateInvitationDto dto, int userId)
        {
            var invitation = new Invitation
            {
                Email = dto.Email,
                RoleId = dto.RoleId,
                ExpiresAt = DateTime.UtcNow.AddHours(72),
                CreatedById = userId,
                ApartmentId = dto.ApartmentId
            };

            return await _invitationRepository.AddInvitationAsync(invitation);
        }

        public async Task<int> UpdateInvitationAsync(Invitation invitation)
        {
            return await _invitationRepository.UpdateInvitationAsync(invitation);
        }
    }
}
