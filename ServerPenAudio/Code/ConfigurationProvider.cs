using ServerPenAudio.Code.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerPenAudio.Code
{
	public class ConfigurationProvider : IConfigurationProvider
	{
		public string AudioFolderLocation { get; set; }
	}
}
