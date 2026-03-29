using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MailerSendNetCore.Common;
using MailerSendNetCore.Common.Extensions;
using ThirdPartyServices.Interfaces;
using ThirdPartyServices.Configs;

namespace ThirdPartyServices.Extensions
{
	public static class WebAppExtension
	{
		public static void RegisterThridParties(this IServiceCollection services, IConfiguration configuration)
		{
			//METHOD #1: Read options from configuration (RECOMMENDED)
			services.AddMailerSendEmailClient(configuration.GetSection("MailerSend"));

			//METHOD #2: Set options from configuration manually
			services.AddMailerSendEmailClient(options =>
			{
				options.ApiUrl = configuration["MailerSend:ApiUrl"];
				options.ApiToken = configuration["MailerSend:ApiToken"];
			});

			//METHOD #3: Add custom options instance
			services.AddMailerSendEmailClient(new MailerSendEmailClientOptions
			{
				ApiUrl = configuration["MailerSend:ApiUrl"],
				ApiToken = configuration["MailerSend:ApiToken"]
			});


			services.AddScoped<TwilioSMSConfig>(sp =>
			{
				return configuration.GetRequiredSection(nameof(TwilioSMSConfig)).Get<TwilioSMSConfig>()!;
			});
			services.AddScoped<TwilioWhatsAppConfig>(sp =>
			{
				return configuration.GetRequiredSection(nameof(TwilioWhatsAppConfig)).Get<TwilioWhatsAppConfig>()!;
			});

			services.AddScoped<IMailerSendService, MailerSendService>();
			services.AddScoped<ITextMessageService, TextMessageService>();
		}
	}
}
