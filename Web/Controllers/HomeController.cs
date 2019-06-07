﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using Web.Models;

using Core;

namespace Web.Controllers
{
	public class HomeController : Controller
	{
		readonly FailsafeContext db;
		public HomeController(FailsafeContext context)
		{
			db = context;
		}
		#region Actions
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		public IActionResult About()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		[Route("AddMe")]
		public IActionResult Add_Me()
		{
			return RedirectPermanent(@"https://discordapp.com/oauth2/authorize?client_id=521693707238506498&scope=bot&permissions=8");
		}
		[Route("JoinMe")]
		public IActionResult Join_Me()
		{
			return RedirectPermanent(@"https://discord.gg/WcuNPM9");
		}
		#endregion
	}
}
