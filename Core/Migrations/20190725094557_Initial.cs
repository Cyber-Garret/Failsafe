﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Catalysts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WeaponName = table.Column<string>(maxLength: 100, nullable: false),
                    Icon = table.Column<string>(maxLength: 1000, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: false),
                    DropLocation = table.Column<string>(maxLength: 1000, nullable: false),
                    Masterwork = table.Column<string>(maxLength: 1000, nullable: false),
                    Bonus = table.Column<string>(maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalysts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clans",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(nullable: false),
                    Motto = table.Column<string>(nullable: true),
                    About = table.Column<string>(nullable: true),
                    MemberCount = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gears",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Type = table.Column<string>(maxLength: 100, nullable: true),
                    IconUrl = table.Column<string>(maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(maxLength: 1000, nullable: true),
                    Description = table.Column<string>(maxLength: 1024, nullable: true),
                    Perk = table.Column<string>(maxLength: 100, nullable: true),
                    PerkDescription = table.Column<string>(maxLength: 1024, nullable: true),
                    SecondPerk = table.Column<string>(maxLength: 100, nullable: true),
                    SecondPerkDescription = table.Column<string>(maxLength: 1000, nullable: true),
                    DropLocation = table.Column<string>(maxLength: 300, nullable: true),
                    isWeapon = table.Column<bool>(nullable: false),
                    isHaveCatalyst = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    NotificationChannel = table.Column<ulong>(nullable: false),
                    EnableNotification = table.Column<bool>(nullable: false),
                    LoggingChannel = table.Column<ulong>(nullable: false),
                    EnableLogging = table.Column<bool>(nullable: false),
                    WelcomeMessage = table.Column<string>(nullable: true),
                    LeaveMessage = table.Column<string>(nullable: true),
                    AutoroleID = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Milestones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Alias = table.Column<string>(maxLength: 50, nullable: true),
                    PreviewDesc = table.Column<string>(maxLength: 1024, nullable: true),
                    Type = table.Column<string>(maxLength: 50, nullable: true),
                    Icon = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milestones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clan_Members",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    DestinyMembershipType = table.Column<long>(nullable: false),
                    DestinyMembershipId = table.Column<string>(nullable: true),
                    BungieMembershipType = table.Column<long>(nullable: true),
                    BungieMembershipId = table.Column<string>(nullable: true),
                    IconPath = table.Column<string>(nullable: true),
                    ClanJoinDate = table.Column<DateTimeOffset>(nullable: true),
                    DateLastPlayed = table.Column<DateTimeOffset>(nullable: true),
                    ClanId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clan_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clan_Members_Clans_ClanId",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActiveMilestones",
                columns: table => new
                {
                    MessageId = table.Column<ulong>(nullable: false),
                    GuildName = table.Column<string>(nullable: false),
                    MilestoneId = table.Column<int>(nullable: false),
                    Memo = table.Column<string>(maxLength: 1000, nullable: true),
                    DateExpire = table.Column<DateTime>(nullable: false),
                    User1 = table.Column<ulong>(nullable: false),
                    User2 = table.Column<ulong>(nullable: false),
                    User3 = table.Column<ulong>(nullable: false),
                    User4 = table.Column<ulong>(nullable: false),
                    User5 = table.Column<ulong>(nullable: false),
                    User6 = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveMilestones", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_ActiveMilestones_Milestones_MilestoneId",
                        column: x => x.MilestoneId,
                        principalTable: "Milestones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveMilestones_MilestoneId",
                table: "ActiveMilestones",
                column: "MilestoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Clan_Members_ClanId",
                table: "Clan_Members",
                column: "ClanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveMilestones");

            migrationBuilder.DropTable(
                name: "Catalysts");

            migrationBuilder.DropTable(
                name: "Clan_Members");

            migrationBuilder.DropTable(
                name: "Gears");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "Milestones");

            migrationBuilder.DropTable(
                name: "Clans");
        }
    }
}