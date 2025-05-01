using Data.Entities;
using Data.Repositories.Interfaces;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Implementations
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly AqualinaAPIContext _context;
        public InvitationRepository(AqualinaAPIContext context)
        {
            _context = context;
        }

        public async Task<Invitation?> GetByIdAsync(int id)
        {
            return await _context.Invitations.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<string> AddInvitationAsync(Invitation invitation)
        {
            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();
            return invitation.Token;
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            var invitation = await _context.Invitations
                .FirstOrDefaultAsync(i => i.Token == token && !i.IsUsed && i.ExpiresAt > DateTime.UtcNow);
            return invitation != null;
        }
        public async Task<Invitation?> GetByTokenWithRoleAsync(string token)
        {
            return await _context.Invitations.Include(i => i.Role).FirstOrDefaultAsync(i => i.Token == token);
        }

        public async Task<int> UpdateInvitationAsync(Invitation invitation)
        {
            _context.Invitations.Update(invitation);
            return invitation.Id;
        }
    }
}
