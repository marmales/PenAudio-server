using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using ServerPenAudio.Code;
using ServerPenAudio.Code.Interfaces;
using ServerPenAudio.Controllers;
using ServerPenAudio.Models;

namespace PenAudio.Tests
{
    public class ControllerStatusResultTests
    {
		private HttpContext context;
        private IServiceProvider provider;
		private string directory = "FileContainer";
		private string audioName = "test.mp3";
		[SetUp]
        public void Init()
        {
            var mockedOptions = new Mock<IOptions<ConfigurationProvider>>();
            var mockedProvider = new Mock<IServiceProvider>();

            mockedOptions.SetupGet(x => x.Value.AudioFolderLocation)
                .Returns(Path.Combine(Directory.GetCurrentDirectory(), directory));

			mockedProvider.Setup(x => x.GetService(typeof(IAudioManager)))
				.Returns(MockedAudioManager(mockedOptions.Object.Value));
			mockedProvider.Setup(x => x.GetService(typeof(IOptions<ConfigurationProvider>)))
                .Returns(mockedOptions.Object);
            provider = mockedProvider.Object;

			var mockedContext = new Mock<HttpContext>();
			var mockedResponse = new Mock<HttpResponse>();
			var mockedResponseCookies = new Mock<IResponseCookies>();
			var mockedModelState = new Mock<ModelBinderAttribute>();
			mockedResponseCookies.Setup(x => x.Append(It.IsAny<string>(), It.IsAny<string>()));
			mockedResponse.SetupGet(x => x.Cookies).Returns(mockedResponseCookies.Object);
			mockedContext.SetupGet(x => x.Response).Returns(mockedResponse.Object);
			context = mockedContext.Object;
        }

        private IAudioManager MockedAudioManager(ConfigurationProvider configuration)
        {
            var mockedAudio = new Mock<IAudioManager>();

			mockedAudio.Setup(x => x.SaveAudioAsync(
				It.Is<IFormFile>(y => Path.GetExtension(y.FileName).Equals("mp3"))))
				.ReturnsAsync("valid");
			mockedAudio.Setup(x => x.SaveAudioAsync(
				It.Is<IFormFile>(y => Path.GetExtension(y.FileName).Equals("mp3"))))
				.ThrowsAsync(new InvalidDataException());
			mockedAudio.Setup(x => x.GetAudioAsync(audioName))
				.ReturnsAsync(
					new FileInformationResponse
					{
						Content = File.ReadAllBytes(Path.Combine(directory, audioName)),
						FileName = Path.GetFileNameWithoutExtension(audioName)
					}
				);

            return mockedAudio.Object;
        }

        [Test]
        public async Task Mp3Upload()
        {
			var audioController = new AudioController(provider)
			{
				ControllerContext = new ControllerContext() { HttpContext = context }
			};
			var fileMock = new Mock<IFormFile>();
			fileMock.SetupGet(x => x.FileName).Returns("mp3");
			fileMock.SetupGet(x => x.Length).Returns(420);

			var codeResult = await audioController.Upload(new AudioModel() { HttpFile = fileMock.Object }) as StatusCodeResult;

			Assert.That(codeResult, Is.Not.Null);
			Assert.That(codeResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task GetExistingAudioFile()
        {

		}
        private Mock<HttpRequest> CreateValidRequest()
		{
			//var cookieCollection = new Mock<IRequestCookieCollection>();
			//cookieCollection.Setup(x => x.ContainsKey(It.IsAny<string>()))
			//	.Returns(true);
			//cookieCollection.Setup(x => x.TryGetValue()) //TODO: check how to mock out parametres
			//var mock = new Mock<HttpRequest>();
			return null;
		}
        [Test]
        public async Task ThrowBadRequestForNonAudioUpload()
        {//TODO test AudioContentType attribute
			var audioController = new AudioController(provider)
			{
				ControllerContext = new ControllerContext() { HttpContext = context }
			};
			audioController.ModelState.AddModelError("TestError", "Invalid content type");

			var codeResult = await audioController.Upload(new AudioModel()) as StatusCodeResult;

			Assert.That(codeResult, Is.Not.Null);
			Assert.That(codeResult.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
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