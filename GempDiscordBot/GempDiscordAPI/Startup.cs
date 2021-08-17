using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Converters;

using Swashbuckle.AspNetCore.Newtonsoft;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GempDiscordAPI
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
			services.AddControllers()
				.AddNewtonsoftJson()
				.AddJsonOptions(c =>
				{
					c.JsonSerializerOptions.AllowTrailingCommas = true;
					//c.JsonSerializerOptions.Converters.Add(new StringEnumConverter());
				}); ;

			services.AddSwaggerGen();
			services.AddSwaggerGenNewtonsoftSupport();

			var section = Configuration.GetSection(RabbitMQOptions.RabbitMQ).Get<RabbitMQOptions>();
			services.Configure<RabbitMQOptions>(x => Configuration.GetSection(RabbitMQOptions.RabbitMQ).Bind(x));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
