﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace API.Bungie.Models.Account
{
    public partial class AccountResult : RootResult
    {
        [JsonProperty("Response")]
        public Response Response { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("activities")]
        public Activity[] Activities { get; set; }
    }

    public partial class Activity
    {
        [JsonProperty("period")]
        public DateTimeOffset Period { get; set; }

        [JsonProperty("activityDetails")]
        public ActivityDetails ActivityDetails { get; set; }

        [JsonProperty("values")]
        public Dictionary<string, Value> Values { get; set; }
    }

    public partial class ActivityDetails
    {
        [JsonProperty("referenceId")]
        public long ReferenceId { get; set; }

        [JsonProperty("directorActivityHash")]
        public long DirectorActivityHash { get; set; }

        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }

        [JsonProperty("mode")]
        public long Mode { get; set; }

        [JsonProperty("modes")]
        public long[] Modes { get; set; }

        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }
    }

    public partial class Value
    {
        [JsonProperty("statId")]
        public StatId StatId { get; set; }

        [JsonProperty("basic")]
        public Basic Basic { get; set; }
    }

    public partial class Basic
    {
        [JsonProperty("value")]
        public double Value { get; set; }

        [JsonProperty("displayValue")]
        public string DisplayValue { get; set; }
    }

    public enum StatId { ActivityDurationSeconds, Assists, AverageScorePerKill, AverageScorePerLife, Completed, CompletionReason, Deaths, Efficiency, FireteamId, Kills, KillsDeathsAssists, KillsDeathsRatio, OpponentsDefeated, PlayerCount, Score, Standing, StartSeconds, Team, TeamScore, TimePlayedSeconds };

    public partial class AccountResult
    {
        public static AccountResult FromJson(string json) => JsonConvert.DeserializeObject<AccountResult>(json, API.Bungie.Models.GroupV2.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this AccountResult self) => JsonConvert.SerializeObject(self, API.Bungie.Models.GroupV2.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                StatIdConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class StatIdConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(StatId) || t == typeof(StatId?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "activityDurationSeconds":
                    return StatId.ActivityDurationSeconds;
                case "assists":
                    return StatId.Assists;
                case "averageScorePerKill":
                    return StatId.AverageScorePerKill;
                case "averageScorePerLife":
                    return StatId.AverageScorePerLife;
                case "completed":
                    return StatId.Completed;
                case "completionReason":
                    return StatId.CompletionReason;
                case "deaths":
                    return StatId.Deaths;
                case "efficiency":
                    return StatId.Efficiency;
                case "fireteamId":
                    return StatId.FireteamId;
                case "kills":
                    return StatId.Kills;
                case "killsDeathsAssists":
                    return StatId.KillsDeathsAssists;
                case "killsDeathsRatio":
                    return StatId.KillsDeathsRatio;
                case "opponentsDefeated":
                    return StatId.OpponentsDefeated;
                case "playerCount":
                    return StatId.PlayerCount;
                case "score":
                    return StatId.Score;
                case "standing":
                    return StatId.Standing;
                case "startSeconds":
                    return StatId.StartSeconds;
                case "team":
                    return StatId.Team;
                case "teamScore":
                    return StatId.TeamScore;
                case "timePlayedSeconds":
                    return StatId.TimePlayedSeconds;
            }
            throw new Exception("Cannot unmarshal type StatId");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (StatId)untypedValue;
            switch (value)
            {
                case StatId.ActivityDurationSeconds:
                    serializer.Serialize(writer, "activityDurationSeconds");
                    return;
                case StatId.Assists:
                    serializer.Serialize(writer, "assists");
                    return;
                case StatId.AverageScorePerKill:
                    serializer.Serialize(writer, "averageScorePerKill");
                    return;
                case StatId.AverageScorePerLife:
                    serializer.Serialize(writer, "averageScorePerLife");
                    return;
                case StatId.Completed:
                    serializer.Serialize(writer, "completed");
                    return;
                case StatId.CompletionReason:
                    serializer.Serialize(writer, "completionReason");
                    return;
                case StatId.Deaths:
                    serializer.Serialize(writer, "deaths");
                    return;
                case StatId.Efficiency:
                    serializer.Serialize(writer, "efficiency");
                    return;
                case StatId.FireteamId:
                    serializer.Serialize(writer, "fireteamId");
                    return;
                case StatId.Kills:
                    serializer.Serialize(writer, "kills");
                    return;
                case StatId.KillsDeathsAssists:
                    serializer.Serialize(writer, "killsDeathsAssists");
                    return;
                case StatId.KillsDeathsRatio:
                    serializer.Serialize(writer, "killsDeathsRatio");
                    return;
                case StatId.OpponentsDefeated:
                    serializer.Serialize(writer, "opponentsDefeated");
                    return;
                case StatId.PlayerCount:
                    serializer.Serialize(writer, "playerCount");
                    return;
                case StatId.Score:
                    serializer.Serialize(writer, "score");
                    return;
                case StatId.Standing:
                    serializer.Serialize(writer, "standing");
                    return;
                case StatId.StartSeconds:
                    serializer.Serialize(writer, "startSeconds");
                    return;
                case StatId.Team:
                    serializer.Serialize(writer, "team");
                    return;
                case StatId.TeamScore:
                    serializer.Serialize(writer, "teamScore");
                    return;
                case StatId.TimePlayedSeconds:
                    serializer.Serialize(writer, "timePlayedSeconds");
                    return;
            }
            throw new Exception("Cannot marshal type StatId");
        }

        public static readonly StatIdConverter Singleton = new StatIdConverter();
    }
}