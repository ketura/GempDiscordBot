using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using RabbitMQ.Client;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GempDiscordAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ChatQueueController : ControllerBase
	{

		private readonly ILogger<ChatQueueController> _logger;

		public RabbitMQOptions RabbitOptions { get; private set; }

		public ChatQueueController(ILogger<ChatQueueController> logger, IOptions<RabbitMQOptions> options, IWebHostEnvironment hostingEnv, IConfiguration config)
		{
			_logger = logger;
			RabbitOptions = options.Value;
		}

		/// <summary>
		/// Adds a chat message to the queue, to be consumed either by the GEMP client or the Discord bot.
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		/// 
		/// <returns></returns>
		/// <response code="200">The chat message was successfully queued.</response>
		/// <response code="400">A required field was missing, or the request is otherwise malformed.</response>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult AddChatEntry([FromBody] ChatEntry entry)
		{
			var factory = new ConnectionFactory() 
			{ 
				HostName = RabbitOptions.Host, 
				UserName = RabbitOptions.User,
				Password = RabbitOptions.Pass
			};
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: RabbitOptions.ExchangeName, type: "direct");

				var target = entry.GetTarget();

				// Gemp-to-Discord
				channel.QueueDeclare(queue: RabbitOptions.DiscordQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
				channel.QueueBind(RabbitOptions.DiscordQueueName, exchange: RabbitOptions.ExchangeName, routingKey: TargetType.Discord.ToString());
				channel.QueueBind(RabbitOptions.DiscordQueueName, exchange: RabbitOptions.ExchangeName, routingKey: TargetType.All.ToString());

				// Discord-to-Gemp
				channel.QueueDeclare(queue: RabbitOptions.GempQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
				channel.QueueBind(RabbitOptions.GempQueueName, exchange: RabbitOptions.ExchangeName, routingKey: TargetType.Gemp.ToString());
				channel.QueueBind(RabbitOptions.GempQueueName, exchange: RabbitOptions.ExchangeName, routingKey: TargetType.All.ToString());

				//channel.BasicQos(0, 1, false);

				var message = JsonConvert.SerializeObject(entry);
				var body = Encoding.UTF8.GetBytes(message);
				channel.BasicPublish(exchange: RabbitOptions.ExchangeName,
														 routingKey: target.ToString(),
														 basicProperties: null,
														 body: body);
			}

			return Ok();
		}
	}
}
