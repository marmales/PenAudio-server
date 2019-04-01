using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ServerPenAudio.Code.Extensions
{
    public static class AudioExtensions
    {
        public static IEnumerable<T> Take<T>(this IEnumerable<T> collection, int elementsCount, int targetLength) 
        {
            for (int i = 0; i < targetLength; i++)
            {
                yield return i > elementsCount ? default(T) : collection.ElementAt(i);
            }
        }
    }
}