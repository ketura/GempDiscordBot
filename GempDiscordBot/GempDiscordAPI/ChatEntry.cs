using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GempDiscordAPI
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum MessageType
	{ 
		User,
		Moderator,
		System,
		Announcement
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum OriginType
	{
		Gemp,
		Discord,
		System
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum TargetType
	{
		Gemp,
		Discord,
		All
	}

	public class ChatEntry
	{
		public DateTime Timestamp { get; set; }

		[Required]
		public string User { get; set; }

		[Required]
		public string Message { get; set; }

		[DefaultValue(MessageType.User)]
		
		public MessageType Level { get; set; }

		[DefaultValue(OriginType.System)]
		public OriginType Origin { get; set; }

		public TargetType GetTarget()
		{
			return GetTarget(Origin);
		}

		public static TargetType GetTarget(OriginType origin)
		{
			switch (origin)
			{
				case OriginType.Gemp:
					return TargetType.Discord;
				case OriginType.Discord:
					return TargetType.Gemp;
				default:
				case OriginType.System:
					return TargetType.All;
			}
		}
	}
}
