using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerPenAudio.Code.Extensions
{
	public static class GetServiceExtension
	{
		public static T Cast<T>(this object obj) where T : class
		{
			return (T)obj;
		}
	}
}
