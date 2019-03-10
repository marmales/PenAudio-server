using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServerPenAudio.Controllers
{
	[Route("audio/[action]")]
	[ApiController]
	public class AudioController : ControllerBase
	{
		[HttpGet]
		public async Task<ActionResult> Test()
		{
			return Ok();
		}
		[HttpPost]
		public async Task<ActionResult> Upload([FromForm] AudioModel audio)
		{
			if (audio == null || audio.Audio.Length <= 0)
				return BadRequest();

			var audioId = Guid.NewGuid().ToString();
			var filePath = Path.Combine(Path.GetTempPath(), $"{audioId}.{Path.GetExtension(audio.Audio.FileName)}");
			using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				await audio.Audio.CopyToAsync(stream);

			return Ok();
		}
	}

	public class AudioModel
	{
		public IFormFile Audio { get; set; }
	}
}
