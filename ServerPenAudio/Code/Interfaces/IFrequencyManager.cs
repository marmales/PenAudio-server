using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerPenAudio.Code.Interfaces
{
    public interface IFrequencyManager
    {
        Task<IEnumerable<ChannelFrequency>> GetFrequencyDomainAsync(FrequencyDomainOptions options);
    }

    public class ChannelFrequency
    {
        private IList<double[]> data;
        public Channel Side { get; }
        public ChannelFrequency(Channel channel)
        {
            Side = channel;
            data = new List<double[]>();
        }

        public void Append(double[] data)
        {
            this.data.Append(data);
        }

        public IEnumerable<double[]> Get()
        {
            return data;
        }
    }

    public enum Channel
    {
        Left,
        Right
    }
    public class FrequencyDomainOptions
    {
        public int SampleSize { get; set; }
        public int SampleRate { get; set; }

        public byte[] Data { get; set; }
        //rest of the options
    }
}