﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Bungie.OAuth2
{
	internal class BungieHandler : OAuthHandler<BungieOptions>
	{
		public BungieHandler(IOptionsMonitor<BungieOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
			: base(options, logger, encoder, clock) { }

		protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

			var response = await Backchannel.SendAsync(request, Context.RequestAborted);
			if (!response.IsSuccessStatusCode)
				throw new HttpRequestException($"Failed to retrieve Bungie user information ({response.StatusCode}).");

			var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

			var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, payload);
			context.RunClaimActions();

			await Events.CreatingTicket(context);
			return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
		}
	}
}
