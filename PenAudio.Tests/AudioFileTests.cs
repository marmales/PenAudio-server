using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using ServerPenAudio.Code;
using ServerPenAudio.Code.Interfaces;
using ServerPenAudio.Controllers;

namespace PenAudio.Tests
{
    public class AudioFileTests
    {
        private IServiceProvider provider;
        [SetUp]
        public void Init()
        {
            var mockedOptions = new Mock<IOptions<ConfigurationProvider>>();
            var mockedProvider = new Mock<IServiceProvider>();

            mockedOptions.SetupGet(x => x.Value.AudioFolderLocation)
                .Returns(Path.Combine(Directory.GetCurrentDirectory(), "FileContainer"));

            mockedProvider.Setup(x => x.GetService<IOptions<ConfigurationProvider>>())
                .Returns(mockedOptions.Object);
            mockedProvider.Setup(x => x.GetService<IAudioManager>())
                .Returns(MockedAudioManager(mockedOptions.Object.Value));

            provider = mockedProvider.Object;
        }

        private IAudioManager MockedAudioManager(ConfigurationProvider configuration)
        {
            var mockedAudio = new Mock<IAudioManager>();
            string audioName = "test.mp3";

            mockedAudio.Setup(x => x.SaveAudioAsync(
                    It.Is<IFormFile>(y => Path.GetExtension(y.FileName).Contains("mp3"))))
                .ReturnsAsync(Path.GetFileNameWithoutExtension(audioName));
            mockedAudio.Setup(x => x.SaveAudioAsync(
                    It.Is<IFormFile>(y => !Path.GetExtension(y.FileName).Contains("mp3"))))
                .ThrowsAsync(new InvalidDataException());
            mockedAudio.Setup(x => x.GetAudioAsync(audioName)).ReturnsAsync(File.ReadAllBytes(audioName));

            return mockedAudio.Object;
        }

        [Test]
        public void Mp3Upload()
        {
            var audioController = new AudioController(provider);
            
            //TODO
        }

        [Test]
        public void TryGetAudio()
        {
            
        }
        
        [Test]
        public void ThrowBadRequestForNonAudioUpload()
        {
            
        }

        [Test]
        public void ThrowBadRequestForNotFoundAudio()
        {
            
        }

        [Test]
        public void NotExistingAudioFolder()
        {
            
        }
    }
}