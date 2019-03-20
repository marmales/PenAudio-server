using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerPenAudio.Code.Interfaces
{
	public interface IAudioManager
	{
		Task<string> SaveAudioAsync(IFormFile file);
		Task<byte[]> GetAudioAsync(string audioId);
	}
}
