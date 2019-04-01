using System.Collections;
using System.Collections.Generic;

namespace ServerPenAudio.Models
{
    public class FrequencyGraphModel
    {
        public IEnumerable<double[]> LeftData { get; set; }
        public IEnumerable<double[]> RightData { get; set; }
    }
}