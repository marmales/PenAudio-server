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
		Task<UploadedModel> SaveAudioAsync(IFormFile file);
		Task<FileModel> GetAudioAsync(string audioId);
	}
}
