using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ServerPenAudio.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AudioController : ControllerBase
	{
		[HttpPost]
		public ActionResult Upload(string[] data)
		{
			return new JsonResult("ok");
		}
	}
}
