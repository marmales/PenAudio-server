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

        public async Task<FileModel> GetAudioAsync(string audioId)
        {
            var targetFolder = Path.Combine(provider.AudioFolderLocation, audioId);

            if (!Directory.Exists(targetFolder))
                return null;

            var file = Directory.GetFiles(
                    targetFolder,
                    "*.*",
                    SearchOption.TopDirectoryOnly)
                .First();

            if (string.IsNullOrEmpty(file))
                return null;

            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                return new FileModel()
                {
                    Information = new FileInformationModel
                    {
                        Title = Path.GetFileNameWithoutExtension(stream.Name)
                    },
                    Data = buffer
                };
            }
        }


        public async Task<UploadedModel> SaveAudioAsync(IFormFile audio)
        {
            var audioId = Guid.NewGuid().ToString();
            var fileTitle = Path.GetFileNameWithoutExtension(audio.FileName);
            var parentDirectory = Directory.CreateDirectory(Path.Combine(provider.AudioFolderLocation, audioId));
            var filePath = Path.Combine(parentDirectory.FullName, $"{fileTitle}{Path.GetExtension(audio.FileName)}");

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                await audio.CopyToAsync(stream);

            return new UploadedModel()
            {
                AudioId = audioId,
                FileInformation = new FileInformationModel()
                {
                    Title = fileTitle
                }
            };
        }
    }
}