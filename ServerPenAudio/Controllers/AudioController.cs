using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerPenAudio.Code;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ServerPenAudio.Code.Attributes;
using ServerPenAudio.Code.Interfaces;
using ServerPenAudio.Models;
using ServerPenAudio.Code.Extensions;

namespace ServerPenAudio.Controllers
{
	[Route("audio/[action]")]
	[AllowAnonymous]
	public class AudioController : ControllerBase
	{
		private ConfigurationProvider configurationProvider;
		private IAudioManager audioManager;

		public AudioController(IServiceProvider serviceProvider)
		{
			this.audioManager = serviceProvider.GetService(typeof(IAudioManager))
				.Cast<IAudioManager>();
			this.configurationProvider = serviceProvider.GetService(typeof(IOptions<ConfigurationProvider>))
				.Cast<IOptions<ConfigurationProvider>>()?.Value;
		}

		
		[HttpGet]
		public ActionResult Test()
		{
			return Ok();
		}
		[HttpPost]
		public async Task<ActionResult> Upload(
			[AudioContentType("NotSupported MIME type")] IFormFile audio
			)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			var result = await audioManager.SaveAudioAsync(audio);
			CookieManager.AddCookie(Respo nse, result.AudioId);

			return Ok(result.File);
		}
		[HttpGet]
		public async Task<ActionResult> Get()
		{
			if (!Request.Cookies.ContainsKey(CookieManager.AudioKey))
				return BadRequest();

			var response = await audioManager.GetAudioAsync(Request.Cookies[CookieManager.AudioKey]);

			if (response == null)
				return NoContent();

			return Ok(response);
		}
	}
}
