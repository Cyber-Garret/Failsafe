﻿namespace Neuromatrix.Models.Db
{
    public class Guild
    {
        public ulong ID { get; set; }
        public string Name { get; set; }
        public ulong OwnerId { get; set; }
        public ulong NotificationChannel { get; set; }
        public ulong LoggingChannel { get; set; }
        public bool EnableLogging { get; set; }
    }
}
