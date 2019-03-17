using Microsoft.AspNetCore.Http;
using ServerPenAudio.Code.Attributes;

namespace ServerPenAudio.Models
{
	public class AudioModel
	{
		[AudioContentType("NotSupported MIME type")]
		public IFormFile HttpFile { get; set; }

	}
}
