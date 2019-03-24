using Microsoft.AspNetCore.Http;
using ServerPenAudio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerPenAudio.Code.Interfaces
{
	public interface IAudioManager
	{
		Task<string> SaveAudioAsync(IFormFile file);
		Task<FileInformationResponse> GetAudioAsync(string audioId);
	}
}
