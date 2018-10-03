using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Cierge.Models;

namespace Cierge.Data
{
    public class ApplicationUser : IdentityUser, IAdditionalUserInfo
    {
        public IList<AuthEvent> Events { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        // from IAdditionalUserInfo

        public string FullName { get; set; }

        public string FavColor { get; set; }  // !! ADDING FIELDS: If you want field to exist

        public IList<UserCustomClaim> CustomClaims { get; set; }
    }
}
