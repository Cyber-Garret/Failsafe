using Destiny2;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neira.Bot;
using Neira.Bot.Services;
using Neira.Models;
using Neira.QuartzService;
using Neira.Services;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Serilog;
using System;
using System.IO;

namespace Neira
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			#region Quartz
			//Register Quartz dedicated service
			services.AddHostedService<QuartzHostedService>();
			// Add Quartz services
			services.AddSingleton<IJobFactory, SingletonJobFactory>();
			services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

			// Add our job
			services.AddSingleton<BungieJob>();
			services.AddSingleton(new JobSchedule(typeof(BungieJob), "0 0/15 * * * ?")); // run every 15 minute

			services.AddSingleton<GuardianStatJob>();
			services.AddSingleton(new JobSchedule(typeof(GuardianStatJob), "0 0 4 * * ?")); // run every 4:00 night

			services.AddSingleton<HellHoundJob>();
			services.AddSingleton(new JobSchedule(typeof(HellHoundJob), "0 0 0/1 * * ?")); // run every hour

			services.AddSingleton<XurArrivedJob>();
			services.AddSingleton(new JobSchedule(typeof(XurArrivedJob), "0 0 20 ? * FRI")); // run every Friday in 20:00

			services.AddSingleton<XurLeaveJob>();
			services.AddSingleton(new JobSchedule(typeof(XurLeaveJob), "0 0 20 ? * TUE")); // run every Tuesday in 20:00

			services.AddSingleton<MilestoneRemindJob>();
			services.AddSingleton(new JobSchedule(typeof(MilestoneRemindJob), "0/10 * * * * ?")); // run every 10 seconds

			services.AddSingleton<MilestoneClearingJob>();
			services.AddSingleton(new JobSchedule(typeof(MilestoneClearingJob), "0 0/30 * * * ?")); // run every 30 minute
			#endregion

			#region Web
			services.AddHttpContextAccessor();
			services.AddControllersWithViews();
			#endregion

			#region Bot
			services.AddHostedService<BotHostedService>();

			//Bot services for DI
			services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
			{
				ExclusiveBulkDelete = true,
				AlwaysDownloadUsers = true,
				LogLevel = LogSeverity.Warning,
				DefaultRetryMode = RetryMode.AlwaysRetry,
				MessageCacheSize = 300
			}))
				.AddSingleton<CommandService>()
				.AddSingleton<LoggingService>()
				.AddSingleton<InteractiveService>()
				.AddSingleton<EmoteService>()
				.AddSingleton<MilestoneService>()
				.AddSingleton<LevelingService>()
				.AddSingleton<CommandHandlerService>()
				.AddSingleton<GuildEventHandlerService>();
			#endregion

			#region Destiny2
			services.Configure<BungieSettings>(Configuration.GetSection("Bungie"));
			var bungie = Configuration.GetSection("Bungie").Get<BungieSettings>();

			services.AddScoped<IMaxPowerService, MaxPowerService>();
			services.AddScoped<IRecommendations, S9Recommendations>();
			services.AddScoped<IWeaponMods, WeaponMods>();


			var config = new Destiny2Config(Configuration["AppName"], Configuration["AppVersion"],
				Configuration["AppId"], Configuration["Url"], Configuration["Email"])
			{
				BaseUrl = bungie.BaseUrl,
				ApiKey = bungie.ApiKey,
				ManifestDatabasePath = Path.Combine(AppContext.BaseDirectory, "Destiny2Manifest")
			};
			services.AddDestiny2(config);

			services.AddBungieAuthentication(new AuthenticationConfiguration
			{
				LoginCookieName = bungie.LoginCookieName,
				ClientId = bungie.ClientId,
				ClientSecret = bungie.ClientSecret,
				AuthorizationEndpoint = bungie.AuthorizationEndpoint,
				TokenEndpoint = bungie.TokenEndpoint,
				CallbackPath = "/signin-bungie/"
			});
			#endregion
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			// https://stackoverflow.com/a/43878365/3857
			var options = new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			};
			options.KnownNetworks.Clear();
			options.KnownProxies.Clear();

			app.UseForwardedHeaders(options);

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseSerilogRequestLogging();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
