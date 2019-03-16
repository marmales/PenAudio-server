using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServerPenAudio.Code;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using penAudioInterfaces = ServerPenAudio.Code.Interfaces;
using ServerPenAudio.Models;

namespace ServerPenAudio.Controllers
{
	[Route("audio/[action]")]
	[ApiController]
	public class AudioController : ControllerBase
	{
		private ConfigurationProvider configurationProvider;
		private penAudioInterfaces.IAudioManager audioManager;

		public AudioController(IServiceProvider serviceProvider)
		{
			this.audioManager = serviceProvider.GetService<penAudioInterfaces.IAudioManager>();
			this.configurationProvider = serviceProvider.GetService<IOptions<ConfigurationProvider>>().Value;
		}

		
		[HttpGet]
		public ActionResult Test()
		{
			return Ok();
		}
		[HttpPost]
		public async Task<ActionResult> Upload([FromForm] AudioModel audio)
		{
			if (audio == null || audio.HttpFile.Length <= 0)
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
