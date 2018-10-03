using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cierge.Data;
using Cierge.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cierge.Services
{
    public interface ICustomClaimsUpdater
    {
        Task UpdateAll();
    }

    public class CustomClaimsUpdater : ICustomClaimsUpdater
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ClaimsOptions _claimsOptions;

        public CustomClaimsUpdater(ApplicationDbContext dbContext, IOptions<ClaimsOptions> claimsOptions)
        {
            _dbContext = dbContext;
            _claimsOptions = claimsOptions.Value;
        }

        public async Task UpdateAll()
        {
            var dbCustomClaims = await _dbContext.CustomClaims.ToListAsync();
            foreach (var claim in _claimsOptions.Claims)
            {
                var existing = dbCustomClaims.FirstOrDefault(x => x.Url == claim.Url);
                if (existing == null)
                    await AddNew(claim);
                else
                    UpdateExisting(claim, existing);
            }

            await _dbContext.SaveChangesAsync();
        }

        private static void UpdateExisting(ClaimsOptions.Claim claim, CustomClaim existing)
        {
            foreach (var changedProperty in claim.GetChangedProperties(existing))
                existing.SetValue(changedProperty.PropertySetter, changedProperty.UpdatedValue);
        }

        private async Task AddNew(ClaimsOptions.Claim claim)
        {
            await _dbContext.CustomClaims.AddAsync(new CustomClaim
            {
                Name = claim.Name,
                Caption = claim.Caption,
                Url = claim.Url
            });
        }
    }
}
