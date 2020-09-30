﻿namespace Failsafe.Models
{
	public class MilestoneInfo
	{
		public string Name { get; set; }
		public string Alias { get; set; }
		public string Type { get; set; }
		public string Icon { get; set; }
		public byte MaxSpace { get; set; }
		public MilestoneType MilestoneType { get; set; }
		public GameName Game { get; set; }
	}

	public enum MilestoneType
	{
		Raid,
		Nightfall,
		Other
	}

	public enum GameName
	{
		Destiny,
		Division,
		Warzone
	}
}
