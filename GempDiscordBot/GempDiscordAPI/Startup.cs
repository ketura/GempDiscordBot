using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

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

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "GEMP / Discord Conversion API GUI", Version = "v1" });

			});
			services.AddSwaggerGenNewtonsoftSupport();

			//var section = Configuration.GetSection(RabbitMQOptions.RabbitMQ).Get<RabbitMQOptions>();
			services.Configure<RabbitMQOptions>(x => Configuration.GetSection(RabbitMQOptions.RabbitMQ).Bind(x));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseDeveloperExceptionPage();

			app.UseHttpsRedirection();

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger(c =>
			{
				c.RouteTemplate = "discord/{documentname}/swagger.json";
			});

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/discord/v1/swagger.json", "Gemp Discord API V1");
				c.DocumentTitle = "GEMP / Discord API GUI";
				c.RoutePrefix = "discord";
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
