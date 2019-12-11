﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Neira.Database;
using Neira.Models;
using Neira.ViewModels;

using System.Diagnostics;
using System.Linq;
using System;
using Discord.WebSocket;

namespace Neira.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly DiscordSocketClient _discord;

		public HomeController(IServiceProvider service, ILogger<HomeController> logger)
		{
			_logger = logger;
			_discord = service.GetRequiredService<DiscordSocketClient>();
		}

		public IActionResult Index()
		{
			using var Db = new NeiraLinkContext();
			var model = new IndexViewModel
			{
				BotInfo = Db.BotInfos.FirstOrDefault()
			};

			return View(model);
		}
		[Route("top-servers")]
		public IActionResult TopServers()
		{
			var guilds = _discord.Guilds.OrderByDescending(g => g.MemberCount);
			return View(guilds.Take(50));
		}

		[Route("AddBot")]
		public IActionResult AddBot() => RedirectPermanent($"https://discordapp.com/oauth2/authorize?client_id=521693707238506498&scope=bot&permissions=269479104");

		[Route("YandexMoney")]
		public IActionResult YandexMoney() => RedirectPermanent($"https://money.yandex.ru/to/410019748161790");

		[Route("Patreon")]
		public IActionResult Patreon() => RedirectPermanent($"https://www.patreon.com/Cyber_Garret");

		[Route("BlackExodus")]
		public IActionResult BlackExodus() => RedirectPermanent($"https://discord.gg/WcuNPM9");

		[Route("VKGroup")]
		public IActionResult VkGroup() => RedirectPermanent($"https://vk.com/failsafe_bot");

		[Route("MyGithub")]
		public IActionResult Github() => RedirectPermanent($"https://github.com/Cyber-Garret");

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}