using System.Collections.Generic;

namespace Cierge.Options
{
	public class OIDCOption
	{
		public string ClientId { get; set; }
		public string DisplayName { get; set; }
		public string PostLogoutRedirectUri { get; set; }
		public string RedirectUri { get; set; }
	}
}
