using Microsoft.AspNetCore.Http;

namespace ServerPenAudio.Models
{
	public class AudioModel
	{
		public IFormFile HttpFile { get; set; }
	}
}
