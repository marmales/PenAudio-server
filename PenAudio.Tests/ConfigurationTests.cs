using NUnit.Framework;
using ServerPenAudio.Code;
using ServerPenAudio.Code.Interfaces;
using System.IO;

namespace PenAudio.Tests
{
	public class ConfigurationTests
	{
		[Test]
		public void AudioFolderExist()
		{
			var configuration = new ConfigurationProvider();

			var folder = configuration.AudioFolderLocation;
			var files = Directory.GetFiles(folder);

			Assert.That(files, Is.Not.Null);
		}
	}
}
