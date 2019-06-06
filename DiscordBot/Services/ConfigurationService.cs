﻿using System;
using Microsoft.Extensions.DependencyInjection;

using DiscordBot.Models;

namespace DiscordBot.Services
{
	public class ConfigurationService
	{
		#region Private fields
		private readonly IServiceProvider _services;
		#endregion

		public ConfigurationService(IServiceProvider services)
		{
			_services = services;
		}

		public void Configure()
		{
			Global.Token = _services.GetRequiredService<Configuration>().Token;
			Global.Version = _services.GetRequiredService<Configuration>().Version;
		}
	}
}
