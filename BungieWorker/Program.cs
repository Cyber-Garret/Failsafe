﻿using System;
using System.Threading.Tasks;

namespace BungieWorker
{
	class Program
	{
		public static TimeSpan BungieTimer = TimeSpan.FromMinutes(1);
		static void Main()
		{
			Console.Title = $"Neira Bungie Worker";
			Logger.Log.Information($"Start the Bungie Worker. Shedule job every {BungieTimer.Minutes} min.");
			try
			{
				new Program().StartAsync().GetAwaiter().GetResult();
			}
			catch (Exception ex)
			{
				Logger.Log.Error(ex, $"[Bungie Worker Main] {ex.Message}");
			}
		}

		private async Task StartAsync()
		{
			_ = new Workers();

			await Task.Delay(-1);
		}
	}
}