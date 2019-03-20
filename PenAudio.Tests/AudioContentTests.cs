using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using ServerPenAudio.Code;
using ServerPenAudio.Code.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PenAudio.Tests
{
	public class AudioContentTests
	{
		IAudioManager audioReader;
		private string containerDirectory = Path.Combine(Directory.GetCurrentDirectory(), "FileContainer");
		[SetUp]
		public void Init()
		{
			var mockedOptions = new Mock<IOptions<ConfigurationProvider>>();

			mockedOptions.SetupGet(x => x.Value.AudioFolderLocation)
				.Returns(containerDirectory);
			audioReader = new AudioManager(mockedOptions.Object);
		}
		private static IFormFile AsMockIFormFile(FileInfo physicalFile)
		{
			var fileMock = new Mock<IFormFile>();
			var ms = new MemoryStream();
			var writer = new StreamWriter(ms);
			writer.Write(physicalFile.OpenRead());
			writer.Flush();
			ms.Position = 0;
			var fileName = physicalFile.Name;
			//Setup mock file using info from physical file
			fileMock.Setup(_ => _.FileName).Returns(fileName);
			fileMock.Setup(_ => _.Length).Returns(ms.Length);
			fileMock.Setup(m => m.OpenReadStream()).Returns(ms);
			fileMock.Setup(m => m.ContentDisposition).Returns(string.Format("inline; filename={0}", fileName));
			//...setup other members (code removed for brevity)


			return fileMock.Object;
		}

		[Test]
		public async Task IsFolderCreatedOnFileUpload()
		{
			var audio = AsMockIFormFile(new FileInfo(Path.Combine(containerDirectory, "test.mp3")));
			var audioCopyName = "audio.mp3";

			var result = await audioReader.SaveAudioAsync(audio);
			var createdDirectory = Path.Combine(containerDirectory, result);
			var filePath = Path.Combine(createdDirectory, audioCopyName);

			Assert.That(createdDirectory, Is.Not.Null.And.Not.Empty);
			Assert.That(filePath, Is.Not.Null.And.Not.Empty);
			Assert.That(Directory.Exists(createdDirectory));
			Assert.That(File.Exists(filePath));

			//Delete
			TryRemoveDirectory(createdDirectory);
		}


		[Test]
		public async Task GetAudioWithValidId()
		{
			var guidid = "guid";
			var audioDirectory = Path.Combine(containerDirectory, guidid);
			try
			{
				TryRemoveDirectory(audioDirectory);
				var directory = Directory.CreateDirectory(audioDirectory);
				File.Copy(Path.Combine(containerDirectory, "test.mp3"), Path.Combine(directory.FullName, "audio.mp3"));

				var file = await audioReader.GetAudioAsync(guidid);

				Assert.That(file, Is.Not.Null.And.Not.Empty);

			}
			catch { Assert.Fail(); }
			finally { TryRemoveDirectory(audioDirectory); }
		}
		private void TryRemoveDirectory(string path)
		{
			if (Directory.Exists(path))
				Directory.Delete(path, recursive: true);
		}
	}
}
