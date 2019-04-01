using ServerPenAudio.Code.Interfaces;

namespace ServerPenAudio.Models
{
    public class UploadedModel : IAudio
    {
        public string AudioId { get; set; }
        public FileInformationModel FileInformation { get; set; }
    }
}