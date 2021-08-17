using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GempDiscordAPI
{
	public class RabbitMQOptions
	{
		public const string RabbitMQ = "RabbitMQ";

		public string Host { get; set; }
		public string User { get; set; }
		public string Pass { get; set; }

		public string ExchangeName { get; set; }
		public string DiscordQueueName { get; set; }
		public string GempQueueName { get; set; }


	}
}
