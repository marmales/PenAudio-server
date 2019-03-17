using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServerPenAudio.Code;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ServerPenAudio.Code.Interfaces;
using ServerPenAudio.Models;
using ServerPenAudio.Code.Extensions;

namespace ServerPenAudio.Controllers
{
	[Route("audio/[action]")]
	[ApiController]
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
		public async Task<ActionResult> Upload([FromForm] AudioModel audio)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			var audioId = await audioManager.SaveAudioAsync(audio.HttpFile);
			CookieManager.AddCookie(Response, audioId);
			return Ok();
		}
		[HttpGet]
		public async Task<ActionResult> Get()
		{
			if (!Request.Cookies.ContainsKey(CookieManager.AudioKey))
				return BadRequest();

			var audioStream = await audioManager.GetAudioAsync(Request.Cookies[CookieManager.AudioKey]);

			return Ok();
		}
	}
}
