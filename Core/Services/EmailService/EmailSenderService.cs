using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Win32;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.EmailService
{
	public class EmailSender : IEmailSender
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly EmailConfiguration _emailConfig;
		private readonly ILogService _logService;
		private readonly IPnkUserService _pnkUserService;
		private readonly IMemoryCache _memoryCache;
		public EmailSender(AppConfig config,
						   IWebHostEnvironment webHostEnvironment,
						   IPnkUserService pnkUserService,
						   ILogService logService,
						   IMemoryCache memoryCache)
		{
			_emailConfig = config.EmailConfiguration ?? throw new NullReferenceException();
			_pnkUserService = pnkUserService;
			_webHostEnvironment = webHostEnvironment;
			_logService = logService;
			_memoryCache = memoryCache;
		}

		public async Task SendEmail(Message message)
		{
			using (var emailMessage = CreateEmailMessage(message))
			{
				try
				{
					await Send(emailMessage);
				}
				catch (Exception e)
				{
					await Logger.Instance.LogErrorAsync(nameof(EmailSender) + Constants.Arrow + nameof(SendEmail), e);
                }
				finally
				{
					emailMessage.Dispose();
				}
			}
		}

		public async Task SendEmail<T>(T model) where T : EmailMessageBase
		{
			try
			{
				var content = await renderTemplate(model, model.TemplatePath!);
				var message = new Message(new string[] { model.UserEmail! }, model.Title!, content);

				_ = Logger.Instance.LogInfoAsync(nameof(EmailSender) + Constants.Arrow + nameof(SendEmail), content);

				using (var emailMessage = CreateEmailMessage(message))
				{
					await Send(emailMessage);
				}
			}
			catch (Exception e)
			{
                _ = Logger.Instance.LogErrorAsync(nameof(EmailSender) + Constants.Arrow + nameof(SendEmail), e);
				throw;
            }
		}

		private MailMessage CreateEmailMessage(Message message)
		{
			var emailMessage = new MailMessage();
			emailMessage.From = new MailAddress(_emailConfig.From);
			foreach (var to in message.To)
			{
				emailMessage.To.Add(new MailAddress(to.Address));

			}
			emailMessage.Subject = message.Subject;
			emailMessage.Body = message.Content;
			emailMessage.IsBodyHtml = true;

			return emailMessage;
		}

		private async Task<bool> Send(MailMessage mailMessage)
		{
			using (var client = new SmtpClient(_emailConfig.SmtpServer, _emailConfig.Port))
			{
				try
				{
					client.UseDefaultCredentials = false;
					client.Credentials = new NetworkCredential(_emailConfig.UserName, _emailConfig.Password);
					client.EnableSsl = true;
					client.Send(mailMessage);
					return true;
				}
				catch (Exception e)
				{
                    _ = Logger.Instance.LogErrorAsync(nameof(EmailSender) + Constants.Arrow + nameof(SendEmail), e);
					throw;
                }
				finally
				{
					client.Dispose();
				}
			}
		}

		private async Task<string> renderTemplate<T>(T model, string templatePath) where T : class
		{
			var emailContext = ReadFile(templatePath);

			RazorLightEngine engine = new RazorLightEngineBuilder()
				.UseEmbeddedResourcesProject(Assembly.GetEntryAssembly())
				.Build();

			return await engine.CompileRenderStringAsync(templatePath, emailContext, model);
		}

		private string ReadFile(string path)
		{
            if (_memoryCache.TryGetValue(path, out string? emailContext))
            {
                return emailContext!;
            }

            string contentRootPath = _webHostEnvironment.ContentRootPath;
            var fullPath = Path.Combine(contentRootPath, path);
            emailContext = File.ReadAllText(fullPath);
			var options = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(5));
            
			_memoryCache.Set(path, emailContext, options);
            return emailContext;
        }
	}
}
