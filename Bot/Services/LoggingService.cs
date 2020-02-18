﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

namespace Bot.Services
{
	/// <summary>
	/// Provides logging for Discord.Net's API
	/// </summary>
	public class LoggingService
	{
		// declare the fields used later in this class
		private readonly ILogger logger;
		private readonly DiscordSocketClient discord;
		private readonly CommandService command;
		private readonly EmoteService emote;

		public LoggingService(IServiceProvider services)
		{
			// get the services we need via DI, and assign the fields declared above to them
			discord = services.GetRequiredService<DiscordSocketClient>();
			command = services.GetRequiredService<CommandService>();
			logger = services.GetRequiredService<ILogger<LoggingService>>();
			emote = services.GetRequiredService<EmoteService>();
		}

		public void Configure()
		{
			// hook into these events with the methods provided below
			discord.Ready += OnReadyAsync;
			discord.Log += OnLogAsync;
			discord.Disconnected += OnDisconnectedAsync;
			command.Log += OnLogAsync;
		}

		// this method executes on the bot being connected/ready
		public Task OnReadyAsync()
		{
			Task.Run(() =>
			{
				logger.LogWarning($"Капитан, я подключилась под аккаунтом -> [{discord.CurrentUser}] :)");
				logger.LogInformation($"В данный момент я на {discord.Guilds.Count} серверах. ");

				if (emote.Raid == null)
					emote.Configure();
			});

			return Task.CompletedTask;
		}

		// this method executes on the bot being disconnected from Discord API
		public Task OnDisconnectedAsync(Exception ex)
		{
			logger.LogInformation($"Нейра отключена. [{ex.Message}]");
			return Task.CompletedTask;
		}

		// this method switches out the severity level from Discord.Net's API, and logs appropriately
		public Task OnLogAsync(LogMessage msg)
		{
			string logText = $"{msg.Source}: {msg.Message}";
			switch (msg.Severity.ToString())
			{
				case "Critical":
					{
						logger.LogCritical(logText);
						break;
					}
				case "Warning":
					{
						logger.LogWarning(logText);
						break;
					}
				case "Info":
					{
						logger.LogInformation(logText);
						break;
					}
				case "Verbose":
					{
						logger.LogInformation(logText);
						break;
					}
				case "Debug":
					{
						logger.LogDebug(logText);
						break;
					}
				case "Error":
					{
						logger.LogError(logText);
						break;
					}
				default:
					{
						logger.LogWarning(logText);
						break;
					}
			}

			return Task.CompletedTask;

		}
	}
}