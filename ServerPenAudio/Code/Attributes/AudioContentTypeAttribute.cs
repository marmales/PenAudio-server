using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
namespace ServerPenAudio.Code.Attributes
{
	public class AudioContentTypeAttribute : ValidationAttribute
	{
		public AudioContentTypeAttribute(string message) : base(message)
		{

		}
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var formFile = (IFormFile)value;
			if (formFile == null)
				return new ValidationResult("Uploaded file is null");

			if (!AllowedContentTypes.Contains(formFile.ContentType))
				return new ValidationResult(string.Format(base.ErrorMessageString, $": \t{formFile.ContentType}"));//TODO: DISPLAY ERROR

			return ValidationResult.Success;
		}

		private static string[] AllowedContentTypes => new string[]
		{
			"audio/mpeg",
			"audio/basic",
			"audio/L24",
			"audio/mid",
			"audio/mp4",
			"audio/x-aiff",
			"audio/x-mpegurl",
			"audio/vnd.wav"
		};
	}
}
