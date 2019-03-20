using Microsoft.AspNetCore.Http;
using ServerPenAudio.Code.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace ServerPenAudio.Code
{
	public class AudioManager : IAudioManager
	{
		private const string AUDIONAME = "audio";
		private ConfigurationProvider provider;
		public AudioManager(IOptions<ConfigurationProvider> options)
		{
			this.provider = options.Value;
		}
		public async Task<byte[]> GetAudioAsync(string audioId)
		{
			var file = 
				Directory.GetFiles(Path.Combine(provider.AudioFolderLocation, audioId))
				.FirstOrDefault(dirFilePath => string.Equals(AUDIONAME, Path.GetFileNameWithoutExtension(dirFilePath)));

			if (string.IsNullOrEmpty(file))
				return null;

			using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
			{
				byte[] buffer = new byte[stream.Length];
				await stream.ReadAsync(buffer, 0, buffer.Length);
				return buffer;
			}
		}

		public async Task<string> SaveAudioAsync(IFormFile audio)
		{
			var audioId = Guid.NewGuid().ToString();
			var newDirectory = Directory.CreateDirectory(Path.Combine(provider.AudioFolderLocation, audioId));
			var filePath = Path.Combine(newDirectory.FullName, $"{AUDIONAME}{Path.GetExtension(audio.FileName)}");
			using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				await audio.CopyToAsync(stream);
			
			return audioId;
		}
	}
}
