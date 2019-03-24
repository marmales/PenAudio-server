using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ServerPenAudio.Code.Interfaces;
using ServerPenAudio.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServerPenAudio.Code
{
	public class AudioManager : IAudioManager
	{
		private const string AUDIONAME = "audio";
		private ConfigurationProvider provider;
		public AudioManager(IOptions<ConfigurationProvider> options)
		{
			provider = options.Value;
		}
		public async Task<FileInformationResponse> GetAudioAsync(string audioId)
		{
			var targetFolder = Path.Combine(provider.AudioFolderLocation, audioId);

			if (!Directory.Exists(targetFolder))
			{
				return null;
			}

			var file =
				Directory.GetFiles(Path.Combine(provider.AudioFolderLocation, audioId)).
				FirstOrDefault(dirFilePath => string.Equals(AUDIONAME, Path.GetFileNameWithoutExtension(dirFilePath)));

			if (string.IsNullOrEmpty(file))
			{
				return null;
			}

			using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
			{
				byte[] buffer = new byte[stream.Length];
				await stream.ReadAsync(buffer, 0, buffer.Length);
				var response = new FileInformationResponse()
				{
					FileName = Path.GetFileNameWithoutExtension(stream.Name),
					Content = buffer
				};
				return response;
			}
		}


		public async Task<string> SaveAudioAsync(IFormFile audio)
		{
			var audioId = Guid.NewGuid().ToString();
			var newDirectory = Directory.CreateDirectory(Path.Combine(provider.AudioFolderLocation, audioId));
			var filePath = Path.Combine(newDirectory.FullName, $"{Path.GetFileName(audio.FileName)}");
			using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			{
				await audio.CopyToAsync(stream);
			}

			return audioId;
		}
	}
}
