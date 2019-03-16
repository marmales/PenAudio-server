using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerPenAudio.Code.Interfaces
{
	public interface IConfigurationProvider
	{
		string AudioFolderLocation { get; set; }
	}
}
