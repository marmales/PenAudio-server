using Microsoft.AspNetCore.Http;
using ServerPenAudio.Code.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServerPenAudio.Code
{
	public class AudioManager : IAudioManager
	{
		private IConfigurationProvider provider;
		public AudioManager(IConfigurationProvider provider)
		{
			this.provider = provider;
		}
		public async Task<byte[]> GetAudioAsync(string audioId)
		{
			var file = Directory.GetFiles(provider.AudioFolderLocation).SingleOrDefault(x => Path.GetFileName(x).Equals(audioId, StringComparison.InvariantCulture));
			if (string.IsNullOrEmpty(file))
				return null;

			byte[] buffer = null;
			using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
				await stream.ReadAsync(buffer);
			return buffer;
		}

		public async Task<string> SaveAudioAsync(IFormFile audio)
		{
			var audioId = Guid.NewGuid().ToString();
			var filePath = Path.Combine(provider.AudioFolderLocation, $"{audioId}.{Path.GetExtension(audio.FileName)}");
			using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				await audio.CopyToAsync(stream);
			
			return audioId;
		}
	}
}
