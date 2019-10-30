﻿using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.EntityFrameworkCore;

using Neira.Bot.Database;
using Neira.Bot.Helpers;
using Neira.Bot.Preconditions;
using Neira.Bot.Services;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Neira.Bot.Modules
{
	[Cooldown(5)]
	[RequireBotPermission(ChannelPermission.SendMessages)]
	public class MainModule : BaseModule
	{
		private readonly CommandService commandService;
		private readonly DiscordSocketClient Client;
		private readonly MilestoneService Milestone;
		private readonly EmoteService CustomEmote;

		public MainModule(CommandService command, DiscordSocketClient socketClient, MilestoneService milestoneService, EmoteService emote)
		{
			commandService = command;
			Client = socketClient;
			Milestone = milestoneService;
			CustomEmote = emote;
		}

		[Command("справка")]
		[Summary("Основная справочная команда.")]
		public async Task MainHelp()
		{
			var app = await Client.GetApplicationInfoAsync();

			var mainCommands = string.Empty;
			var adminCommands = string.Empty;
			var economyCommands = string.Empty;

			var guild = await DatabaseHelper.GetGuildAccountAsync(Context.Guild.Id);

			foreach (CommandInfo command in commandService.Commands.ToList())
			{
				if (command.Module.Name == typeof(MainModule).Name)
					mainCommands += $"{guild.CommandPrefix ?? "!"}{command.Name}, ";
				else if (command.Module.Name == typeof(ModerationModule).Name)
					adminCommands += $"{guild.CommandPrefix ?? "!"}{command.Name}, ";
				else if (command.Module.Name == typeof(EconomyModule).Name)
					economyCommands += $"{guild.CommandPrefix ?? "!"}{command.Name}, ";

			}

			var embed = new EmbedBuilder()
				.WithColor(Color.Gold)
				.WithTitle($"Доброго времени суток. Меня зовут Нейроматрица, я ИИ \"Черного исхода\" адаптированный для Discord. Успешно функционирую с {app.CreatedAt.ToString("dd.MM.yyyy")}")
				.WithDescription(
				"Моя основная цель - своевременно сообщать когда прибывает или улетает посланник девяти Зур.\n" +
				"Также я могу предоставить информацию о экзотическом снаряжении,катализаторах.\n" +
				"Больше информации ты можешь найти в моей [группе ВК](https://vk.com/failsafe_bot)\n" +
				"и в [документации](https://docs.neira.su/)")
				.AddField("Основные команды", mainCommands.Substring(0, mainCommands.Length - 2))
				.AddField("Команды администраторов сервера", adminCommands.Substring(0, adminCommands.Length - 2))
				.AddField("Команды экономики и репутации", economyCommands.Substring(0, economyCommands.Length - 2))
				.WithFooter(NeiraWebsite);

			await ReplyAsync(embed: embed.Build());
		}

		[Command("инфо")]
		[Summary("Отображает полную информацию о команде.")]
		[Remarks("Пример: !инфо <команда>, например !инфо клан статус")]
		public async Task Info([Remainder] string searchCommand = null)
		{
			if (searchCommand == null)
			{
				await ReplyAndDeleteAsync(":x: Пожалуйста, введите полноcтью или частично команду о которой вы хотите получить справку.");
				return;
			}
			var command = commandService.Commands.Where(c => c.Name.IndexOf(searchCommand, StringComparison.CurrentCultureIgnoreCase) != -1).FirstOrDefault();
			if (command == null)
			{
				await ReplyAndDeleteAsync($":x: Команда **{searchCommand}** не была найдена.");
				return;
			}
			var embed = new EmbedBuilder();

			var guild = await DatabaseHelper.GetGuildAccountAsync(Context.Guild.Id);

			embed.WithAuthor(Client.CurrentUser);
			embed.WithTitle($"Информация о команде: {command.Name}");
			embed.WithColor(Color.Gold);
			embed.WithDescription(command.Summary ?? "Команда без описания.");
			// First alias always command atribute
			if (command.Aliases.Count > 1)
			{
				string alias = string.Empty;
				//Skip command attribute
				foreach (var item in command.Aliases.Skip(1))
				{
					alias += $"{guild.CommandPrefix ?? "!"}{item}, ";
				}
				embed.AddField("Синонимы команды:", alias.Substring(0, alias.Length - 2));
			}
			if (command.Remarks != null)
				embed.AddField("Заметка:", command.Remarks);

			await ReplyAsync(embed: embed.Build());
		}

		[Command("бип")]
		[Summary("Простая команда проверки моей работоспособности.")]
		public async Task Bip()
		{
			await ReplyAsync("Бип...");
		}

		[Command("зур")]
		[Summary("Отвечает в данный момент Зур в системе или нет, если да, то отображает ссылки на сторонние ресурсы с его расположением и ассортиментом.")]
		[Remarks("Пример: !зур, префикс команды может отличаться от стандартного.")]
		public async Task XurCommand()
		{
			#region Check DayOfWeek
			DateTime today = DateTime.Now;
			TimeSpan start = new TimeSpan(20, 0, 0);
			TimeSpan end = start; //setting the same value as your start and end time is same
			DateTime fridayStartCheck = new DateTime();
			DateTime tuesdayEndCheck = new DateTime();

			if (today.DayOfWeek == DayOfWeek.Friday)
			{
				fridayStartCheck = DateTime.Parse(today.ToShortDateString());
				fridayStartCheck += start;
				tuesdayEndCheck = DateTime.Parse(today.AddDays(+5).ToShortDateString());
				tuesdayEndCheck += end;
			}
			if (today.DayOfWeek == DayOfWeek.Saturday)
			{
				fridayStartCheck = DateTime.Parse(today.AddDays(-1).ToShortDateString());
				fridayStartCheck += start;
				tuesdayEndCheck = DateTime.Parse(today.AddDays(+4).ToShortDateString());
				tuesdayEndCheck += end;
			}
			if (today.DayOfWeek == DayOfWeek.Sunday)
			{
				fridayStartCheck = DateTime.Parse(today.AddDays(-2).ToShortDateString());
				fridayStartCheck += start;
				tuesdayEndCheck = DateTime.Parse(today.AddDays(+3).ToShortDateString());
				tuesdayEndCheck += end;
			}
			if (today.DayOfWeek == DayOfWeek.Monday)
			{
				fridayStartCheck = DateTime.Parse(today.AddDays(-3).ToShortDateString());
				fridayStartCheck += start;
				tuesdayEndCheck = DateTime.Parse(today.AddDays(+2).ToShortDateString());
				tuesdayEndCheck += end;
			}
			if (today.DayOfWeek == DayOfWeek.Tuesday)
			{
				fridayStartCheck = DateTime.Parse(today.AddDays(-5).ToShortDateString());
				fridayStartCheck += start;
				tuesdayEndCheck = DateTime.Parse(today.ToShortDateString());
				tuesdayEndCheck += end;
			}
			#endregion

			EmbedBuilder Embed = new EmbedBuilder()
				.WithThumbnailUrl("https://i.imgur.com/sFZZlwF.png");

			if (today >= fridayStartCheck && today <= tuesdayEndCheck)
			{

				Embed.WithColor(Color.Gold)
					.WithTitle($"Уважаемый пользователь {Context.User}")
					.WithDescription("По моим данным Зур в данный момент в пределах солнечной системы, но\n" +
					"так как мои алгоритмы глобального позиционирования пока еще в разработке, определить его точное местоположение я не могу.\n" +
					"[Но я уверена что тут ты сможешь отыскать его положение](https://whereisxur.com/)\n" +
					"[Или тут](https://ftw.in/game/destiny-2/find-xur)")
					.WithFooter("Напоминаю! Зур покинет пределы солнечной системы во вторник 20:00 по МСК. Это сообщение будет автоматически удалено через 2 минуты. ");

			}
			else
			{
				Embed.WithColor(Color.Red)
					.WithTitle($"Уважаемый пользователь {Context.User}")
					.WithDescription("По моим данным Зур не в пределах солнечной системы.")
					.WithFooter("Зур прибудет в пятницу 20:00 по МСК. Я сообщу. Это сообщение будет автоматически удалено через 2 минуты.");
			}
			await ReplyAndDeleteAsync(null, embed: Embed.Build(), timeout: TimeSpan.FromMinutes(2));
		}

		[Command("экзот")]
		[Summary("Отображает информацию о экзотическом снаряжении. Ищет как по полному названию, так и частичному.")]
		[Remarks("Пример: !экзот буря")]
		public async Task Exotic([Remainder]string Input = null)
		{
			if (Input == null)
			{
				await ReplyAndDeleteAsync(":x: Пожалуйста, введите полное или частичное название экзотического снаряжения.");
				return;
			}

			var gear = await DatabaseHelper.GetExoticAsync(Input);
			//If not found gear by user input
			if (gear == null)
			{
				await ReplyAndDeleteAsync(":x: Этой информации в моей базе данных нет. :frowning:");
				return;
			}

			var embed = new EmbedBuilder();

			embed.WithColor(Color.Gold);
			embed.WithTitle(gear.Type + " - " + gear.Name);//Exotic type and Name
			embed.WithThumbnailUrl(gear.IconUrl);//Icon
			embed.WithDescription(gear.Description);//Lore
			if (gear.isWeapon)//Only weapon can have catalyst
			{
				embed.AddField("Катализатор", gear.isHaveCatalyst == true ? "**Есть**" : "**Отсутствует**");
			}
			embed.AddField(gear.Perk, gear.PerkDescription, true);//Main Exotic perk
			if (gear.SecondPerk != null && gear.SecondPerkDescription != null)//Second perk if have.
				embed.AddField(gear.SecondPerk, gear.SecondPerkDescription, true);
			embed.AddField("Как получить:", gear.DropLocation, true);//How to obtain this exotic gear
			embed.WithImageUrl(gear.ImageUrl);//Exotic screenshot

			var app = await Context.Client.GetApplicationInfoAsync();//Get some bot info from discord api
			embed.WithFooter($"Если нашли какие либо неточности, сообщите моему создателю: {app.Owner.Username}#{app.Owner.Discriminator}.",
				"https://www.bungie.net/common/destiny2_content/icons/ee21b5bc72f9e48366c9addff163a187.png");


			await ReplyAsync($"Итак, {Context.User.Username}, вот что мне известно про это снаряжение.", embed: embed.Build());
		}

		[Command("каталик"), Alias("катализатор")]
		[Summary("Отображает информацию о катализаторе для оружия.")]
		[Remarks("Пример: !катализатор мида или !каталик туз")]
		public async Task GetCatalyst([Remainder]string Input = null)
		{
			var app = await Client.GetApplicationInfoAsync();
			var Embed = new EmbedBuilder();
			//check user input
			if (Input == null)
			{
				Embed.WithAuthor("Добро пожаловать в базу данных катализаторов экзотического оружия Нейроматрицы");
				Embed.WithThumbnailUrl("https://bungie.net/common/destiny2_content/icons/d8acfda580e28f7765dd6a813394c847.png");
				Embed.WithDescription("Для того чтобы найти информацию о нужном тебе катализаторе теперь достаточно написать `!катализатор мида` и я сразу отображу что я о нем знаю.");
				Embed.AddField(GlobalVariables.InvisibleString, "Полный список известных мне катализаторов ты можешь посмотреть [тут](https://docs.neira.su/prochee/baza-katalizatorov-i-ekzotikov).");
				Embed.WithColor(Color.Blue);
				Embed.WithFooter($"Это сообщение будет автоматически удалено через 2 минуты.",
					@"https://bungie.net/common/destiny2_content/icons/2caeb9d168a070bb0cf8142f5d755df7.jpg");

				await ReplyAndDeleteAsync(null, embed: Embed.Build(), timeout: TimeSpan.FromMinutes(2));
				return;
			}

			var catalyst = await DatabaseHelper.GetCatalystAsync(Input);

			//Если бд вернула null сообщаем пользователю что ничего не нашли.
			if (catalyst == null)
			{
				Embed.WithDescription(":x: Этой информации в моей базе данных нет. :frowning:");
				Embed.WithColor(Color.Red);
				Embed.AddField(GlobalVariables.InvisibleString, "Полный список известных мне катализаторов ты можешь посмотреть [тут](https://docs.neira.su/prochee/baza-katalizatorov-i-ekzotikov).");
				Embed.WithFooter("Это сообщение будет автоматически удалено через 1 минуту.");
				await ReplyAndDeleteAsync(null, embed: Embed.Build(), timeout: TimeSpan.FromMinutes(1));
				return;
			}

			Embed.WithTitle("Информация о катализаторе для оружия " + $"{catalyst.WeaponName}");
			Embed.WithColor(Color.Gold);
			if (!string.IsNullOrWhiteSpace(catalyst.Icon))
				Embed.WithThumbnailUrl(catalyst.Icon);
			if (!string.IsNullOrWhiteSpace(catalyst.Description))
				Embed.WithDescription(catalyst.Description);
			if (!string.IsNullOrWhiteSpace(catalyst.DropLocation))
				Embed.AddField("Как получить катализатор", catalyst.DropLocation);
			if (!string.IsNullOrWhiteSpace(catalyst.Masterwork))
				Embed.AddField("Задание катализатора", catalyst.Masterwork);
			if (!string.IsNullOrWhiteSpace(catalyst.Bonus))
				Embed.AddField("Бонус катализатор", catalyst.Bonus);
			Embed.WithFooter($"Если нашли какие либо неточности, сообщите моему создателю: {app.Owner.Username}#{app.Owner.Discriminator}.",
				@"https://bungie.net/common/destiny2_content/icons/2caeb9d168a070bb0cf8142f5d755df7.jpg");

			await ReplyAsync($"Итак, {Context.User.Username}, вот что мне известно про этот катализатор.", embed: Embed.Build());
		}

		[Command("моды")]
		[Summary("Подсказка о новых модификаторах в броню.")]
		public async Task ModsInfo()
		{
			var embed = new EmbedBuilder
			{
				Title = "Модификаторы брони 2.0",
				Color = Color.Gold,
				Footer = new EmbedFooterBuilder { Text = "neira.su", IconUrl = "http://neira.su/img/neira.png" }
			};
			//Elemens
			embed.AddField("Тип энергии к которому привязан тип оружия:", GlobalVariables.InvisibleString);
			embed.AddField($"{CustomEmote.Arc} Молния", "Импульсные винтовки, пулемёты, дробовики, мечи, луки.");
			embed.AddField($"{CustomEmote.Solar} Солнце", "Ракетные установки, автоматы, пистолеты-пулеметы, плазменные винтовки, линейно-плазменные винтовки.");
			embed.AddField($"{CustomEmote.Void} Пустота", "Револьверы, снайперские винтовки, гранатомёты, винтовки разведчиков, пистолеты");
			embed.AddField("Тип модификатора в доспехах", GlobalVariables.InvisibleString);
			//Armor Type
			embed.AddField("Шлем", "Прицельность, локаторы боеприпасов.");
			embed.AddField("Рукавицы", "Скорость перезарядки, откат гранат и ближнего боя.");
			embed.AddField("Нагрудник", "Резервный боезапас, стойкий прицел.");
			embed.AddField("Броня для ног", "Сборщик боеприпасов, легкость оружия.");
			embed.AddField("Классовый предмет", "Добивающий прием, восстановление способностей.");

			await ReplyAsync(embed: embed.Build());
		}

		[Command("помощь"), Alias("донат")]
		[Summary("Информация о том как помочь развитию бота")]
		public async Task Donate()
		{
			var app = await Client.GetApplicationInfoAsync();

			var embed = new EmbedBuilder()
			{
				Title = "Поддержите Нейроматрицу!",
				Color = Color.Green,
				Description = "Бип! Я стараюсь вкладывать много любви и усилий в разработку Нейроматрицы и для хорошей и стабильной работы мне нужна Ваша помощь! Ваша материальная поддержка придаст мне больше мотивации развивать бота и придавать ему больше возможностей и функций. И, как минимум, покрыть расходы на ежемесячную аренду хостинга.",
				ImageUrl = "https://www.bungie.net/img/UserThemes/d2_13/mobiletheme.jpg",
				Footer = NeiraWebsite
			}
				.AddField("Patreon", "При помощи системы Патреон вы можете оформить месячную подписку или единоразово купить мне кофе на любую сумму.\n[Я на Patreon](https://www.patreon.com/Cyber_Garret)")
				.AddField("YandexMoney", "Так же вы можете помочь мне через [Яндекс Деньги](https://money.yandex.ru/to/410019748161790)")
				.AddField(GlobalVariables.InvisibleString, $"В любом случае спасибо что проявляете интерес к Нейроматрице. С наилучшими пожеланиями {app.Owner.Username}#{app.Owner.Discriminator}.");

			await ReplyAsync(embed: embed.Build());
		}

		[Command("сбор")]
		[Summary("Команда для анонса активностей.")]
		[Remarks("Пример: !сбор <Название> <Дата и время начала активности> <Заметка лидера(Не обязательно)>, например !сбор пн 17.07-20:00 Тестовая заметка.\nВведите любой параметр команды неверно, и я отображу по нему справку.")]
		[RequireBotPermission(ChannelPermission.AddReactions | ChannelPermission.EmbedLinks | ChannelPermission.ManageMessages | ChannelPermission.MentionEveryone)]
		[Cooldown(10)]
		public async Task GoMilestone(string milestoneName, string raidTime, [Remainder]string userMemo = null)
		{
			try
			{
				var guild = await DatabaseHelper.GetGuildAccountAsync(Context.Guild.Id);

				var milestone = await DatabaseHelper.GetMilestone(milestoneName);

				if (milestone == null)
				{
					var AvailableRaids = "Доступные для регистрации активности:\n\n";
					var info = DatabaseHelper.GetAllMilestones();

					foreach (var item in info)
					{
						AvailableRaids += $"**{item.Name}** или просто **{item.Alias}**\n";
					}

					var message = new EmbedBuilder()
						.WithTitle("Страж, я не разобрала в какую активность ты хочешь пойти")
						.WithColor(Color.Red)
						.WithDescription(AvailableRaids += "\nПример: !сбор пж 17.07-20:00")
						.WithFooter("Хочу напомнить, что я ищу как по полному названию рейда так и частичному. Это сообщение будет автоматически удалено через 2 минуты.");
					await ReplyAndDeleteAsync(null, embed: message.Build(), timeout: TimeSpan.FromMinutes(2));
					return;
				}

				string[] formats = { "dd.MM-HH:mm", "dd,MM-HH,mm", "dd.MM.HH.mm", "dd,MM,HH,mm" };

				DateTime.TryParseExact(raidTime, formats, CultureInfo.InstalledUICulture, DateTimeStyles.None, out DateTime dateTime);

				if (dateTime == new DateTime())
				{
					var message = new EmbedBuilder()
						.WithTitle("Страж, ты указал неизвестный мне формат времени")
						.WithColor(Color.Gold)
						.AddField("Я понимаю время начала рейда в таком формате",
						"Формат времени: **<день>.<месяц>-<час>:<минута>**\n" +
						"**День:** от 01 до 31\n" +
						"**Месяц:** от 01 до 12\n" +
						"**Час:** от 00 до 23\n" +
						"**Минута:** от 00 до 59\n" +
						"В итоге у тебя должно получиться: **05.07-20:05** Пример: !сбор пж 21.05-20:00")
						.AddField("Уведомление", "Время начала активности учитывается только по московскому времени. Также за 15 минут до начала активности, я уведомлю участников личным сообщением.")
						.WithFooter("Это сообщение будет автоматически удалено через 2 минуты.");
					await ReplyAndDeleteAsync(null, embed: message.Build(), timeout: TimeSpan.FromMinutes(2));
					return;
				}
				if (dateTime < DateTime.Now)
				{
					var message = new EmbedBuilder()
						.WithColor(Color.Red)
						.WithDescription($"Собрался в прошлое? Тебя ждет увлекательное шоу \"остаться в живых\" в исполнении моей команды Золотого Века. Не забудь попкорн\nБип...Удачи и передай привет моему капитану.");
					await ReplyAndDeleteAsync(null, embed: message.Build());
					return;
				}

				var msg = await ReplyAsync(message: guild.GlobalMention, embed: EmbedsHelper.MilestoneNew(Context.User, milestone, dateTime, CustomEmote.Raid, userMemo));
				await Milestone.RegisterNewMilestoneAsync(msg.Id, Context, milestone.Id, dateTime, userMemo);

				//Slots
				await msg.AddReactionAsync(CustomEmote.Raid);
			}
			catch (Exception ex)
			{
				//reply to user if any error
				await ReplyAndDeleteAsync("Страж, произошла критическая ошибка, я не могу в данный момент выполнить команду.\nУже пишу моему создателю, он сейчас все поправит.");
				//Get App info 
				var app = await Client.GetApplicationInfoAsync();
				//Get Owner for DM
				var owner = await app.Owner.GetOrCreateDMChannelAsync();
				//Send DM message with exception
				await owner.SendMessageAsync($"Капитан, проблема с командой сбор! **{ex.Message}** больше подробностей в консоли.");
				//Log full exception in console
				await Logger.LogFullException(new LogMessage(LogSeverity.Critical, "Milestone command", ex.Message, ex));
			}
		}

		[Command("рейд")]
		[Summary("Команда для анонса активностей. Альтернативная версия.")]
		[Remarks("Пример: !рейд <Название> <Дата и время начала активности> <Заметка лидера(Не обязательно)>, например !рейд пн 17.07-20:00 Тестовая заметка.\nВведите любой параметр команды неверно, и я отображу по нему справку.")]
		[RequireBotPermission(ChannelPermission.AddReactions | ChannelPermission.EmbedLinks | ChannelPermission.ManageMessages | ChannelPermission.MentionEveryone)]
		[Cooldown(10)]
		public async Task GoRaid(string milestoneName, string raidTime, [Remainder]string userMemo = null)
		{
			try
			{
				var guild = await DatabaseHelper.GetGuildAccountAsync(Context.Guild.Id);

				var milestone = await DatabaseHelper.GetMilestone(milestoneName);

				if (milestone == null)
				{
					var AvailableRaids = "Доступные для регистрации активности:\n\n";
					var info = DatabaseHelper.GetAllMilestones();

					foreach (var item in info)
					{
						AvailableRaids += $"**{item.Name}** или просто **{item.Alias}**\n";
					}

					var message = new EmbedBuilder()
						.WithTitle("Страж, я не разобрала в какую активность ты хочешь пойти")
						.WithColor(Color.Red)
						.WithDescription(AvailableRaids += "\nПример: !рейд пж 17.07-20:00")
						.WithFooter("Хочу напомнить, что я ищу как по полному названию рейда так и частичному. Это сообщение будет автоматически удалено через 2 минуты.");
					await ReplyAndDeleteAsync(null, embed: message.Build(), timeout: TimeSpan.FromMinutes(2));
					return;
				}

				string[] formats = { "dd.MM-HH:mm", "dd,MM-HH,mm", "dd.MM.HH.mm", "dd,MM,HH,mm" };

				DateTime.TryParseExact(raidTime, formats, CultureInfo.InstalledUICulture, DateTimeStyles.None, out DateTime dateTime);

				if (dateTime == new DateTime())
				{
					var message = new EmbedBuilder()
						.WithTitle("Страж, ты указал неизвестный мне формат времени")
						.WithColor(Color.Gold)
						.AddField("Я понимаю время начала рейда в таком формате",
						"Формат времени: **<день>.<месяц>-<час>:<минута>**\n" +
						"**День:** от 01 до 31\n" +
						"**Месяц:** от 01 до 12\n" +
						"**Час:** от 00 до 23\n" +
						"**Минута:** от 00 до 59\n" +
						"В итоге у тебя должно получиться: **05.07-20:05** Пример: !сбор пж 21.05-20:00")
						.AddField("Уведомление", "Время начала активности учитывается только по московскому времени. Также за 15 минут до начала активности, я уведомлю участников личным сообщением.")
						.WithFooter("Это сообщение будет автоматически удалено через 2 минуты.");
					await ReplyAndDeleteAsync(null, embed: message.Build(), timeout: TimeSpan.FromMinutes(2));
					return;
				}
				if (dateTime < DateTime.Now)
				{
					var message = new EmbedBuilder()
						.WithColor(Color.Red)
						.WithDescription($"Собрался в прошлое? Тебя ждет увлекательное шоу \"остаться в живых\" в исполнении моей команды Золотого Века. Не забудь попкорн\nБип...Удачи и передай привет моему капитану.");
					await ReplyAndDeleteAsync(null, embed: message.Build());
					return;
				}

				var msg = await ReplyAsync(message: guild.GlobalMention, embed: EmbedsHelper.MilestoneNewV2(Context.User, milestone, dateTime, userMemo));
				await Milestone.RegisterNewMilestoneV2Async(msg.Id, Context, milestone.Id, dateTime, userMemo);

				//Slots
				//Slots
				await msg.AddReactionAsync(CustomEmote.ReactSecond);
				await msg.AddReactionAsync(CustomEmote.ReactThird);
				await msg.AddReactionAsync(CustomEmote.ReactFourth);
				await msg.AddReactionAsync(CustomEmote.ReactFifth);
				await msg.AddReactionAsync(CustomEmote.ReactSixth);
			}
			catch (Exception ex)
			{
				//reply to user if any error
				await ReplyAndDeleteAsync("Страж, произошла критическая ошибка, я не могу в данный момент выполнить команду.\nУже пишу моему создателю, он сейчас все поправит.");
				//Get App info 
				var app = await Client.GetApplicationInfoAsync();
				//Get Owner for DM
				var owner = await app.Owner.GetOrCreateDMChannelAsync();
				//Send DM message with exception
				await owner.SendMessageAsync($"Капитан, проблема с командой сбор! **{ex.Message}** больше подробностей в консоли.");
				//Log full exception in console
				await Logger.LogFullException(new LogMessage(LogSeverity.Critical, "Milestone command", ex.Message, ex));
			}
		}

		[Command("клан")]
		[Summary("Отображает отсортированный онлайн Destiny 2 клана, зарегистрированного в базе данных моим создателем.\n**Клан должен быть зарегистрирован моим создателем.**")]
		public async Task GetDestinyClanInfo()
		{
			try
			{
				//Find Destiny 2 Clan associated to Discord Guild
				var clans = DatabaseHelper.GetDestinyClan(Context.Guild.Id);

				//If not found any associated clan
				if (clans.Count == 0)
				{
					var app = await Context.Client.GetApplicationInfoAsync();
					var text = $"Для регистрации клана в моей системе напиши моему создателю - {app.Owner.Username}#{app.Owner.Discriminator}\n<https://discordapp.com/invite/WcuNPM9>";
					await ReplyAndDeleteAsync(text, timeout: TimeSpan.FromMinutes(2));
				}
				else
				{
					//If Discord guild associated to multiple Clans we create Paginated message
					if (clans.Count > 1)
					{
						//create Paginated message
						var message = new PaginatedMessage
						{
							Title = "Онлайн статус кланов на сервере",
							Color = Color.Gold,
							//Change some message options.
							Options = new PaginatedAppearanceOptions
							{
								DisplayInformationIcon = false,
								JumpDisplayOptions = JumpDisplayOptions.Never,
								FooterFormat = "Страница {0}/{1}",
								Timeout = TimeSpan.FromMinutes(5)
							}
						};
						// List of clans
						var pages = new List<string>();

						foreach (var clan in clans)
						{
							//Check if Bungie worker have any data and stored to Db.
							if (clan.Members.Count > 1)
								//Sort Guardians by last play time.
								pages.Add(MiscHelpers.ClanStatus(clan));
						}
						//Add list of clans in paginated message
						message.Pages = pages;

						//Reply to user
						await PagedReplyAsync(message);
					}
					else
					{
						//Reply to user if Discord guild associated to one clan.
						await ReplyAsync(embed: EmbedsHelper.ClanStatus(clans.First()).Build());
					}
				}
			}
			catch (Exception ex)
			{
				await Logger.Log(new LogMessage(LogSeverity.Critical, "GetDestinyClanInfo", ex.Message));
			}
		}
	}
}
