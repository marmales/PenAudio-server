using System;
using System.Linq;
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
		private IFrequencyManager frequencyManager;
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
			[AudioContentType("NotSupported MIME type")] IFormFile audio)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			var result = await audioManager.SaveAudioAsync(audio);
			CookieManager.AddCookie(Response, result.AudioId);

			return Ok(result.FileInformation);
		}

		[HttpGet]
		public async Task<ActionResult> GetFrequencyDomain(FrequencyDomainOptions options)
		{
			var id = Request.GetAudioCookie();
			if (string.IsNullOrEmpty(id))
				return BadRequest($"Cannot find audio cookie");

			var audio = await audioManager.GetAudioAsync(id);
			if (audio == null)
				return NoContent();
			options.Data = audio.Data;

			var liveGraphsData = await frequencyManager.GetFrequencyDomainAsync(options);
			var response = new FrequencyGraphModel()
			{
				LeftData = liveGraphsData.First(x => x.Side == Channel.Left).Get(),
				RightData = liveGraphsData.First(x => x.Side == Channel.Right).Get(),
			};
			return Ok(response);
		}
		
		[HttpGet]
		public async Task<ActionResult> Get()
		{
			var audioId = Request.GetAudioCookie();
			if (string.IsNullOrEmpty(audioId))
				return BadRequest($"Cannot find {CookieManager.AudioKey} cookie");

			var response = await audioManager.GetAudioAsync(audioId);

			if (response == null)
				return NoContent();

			return Ok(response.Information);
		}
	}
}
