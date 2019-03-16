using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerPenAudio.Code;
using Microsoft.Extensions.DependencyInjection;
using penAudioInterfaces = ServerPenAudio.Code.Interfaces;
using ServerPenAudio.Models;

namespace ServerPenAudio.Controllers
{
	[Route("audio/[action]")]
	[ApiController]
	public class AudioController : ControllerBase
	{
		private penAudioInterfaces.IConfigurationProvider configurationProvider;
		private penAudioInterfaces.IAudioManager audioManager;

		public AudioController(IServiceProvider serviceProvider)
		{
			this.configurationProvider = serviceProvider.GetService<penAudioInterfaces.IConfigurationProvider>();
			this.audioManager = serviceProvider.GetService<penAudioInterfaces.IAudioManager>();
		}

		
		[HttpGet]
		public async Task<ActionResult> Test()
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
			return Ok();
		}
	}
}
