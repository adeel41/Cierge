using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cierge.Data
{
    public class CustomClaim
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public void SetValue(Delegate propertySetter, object value) => propertySetter.DynamicInvoke(this, value);
    }

    public class UserCustomClaim
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int CustomClaimId { get; set; }
        public CustomClaim CustomClaim { get; set; }
    }
}
