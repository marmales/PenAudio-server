using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ServerPenAudio.Code.Extensions;
using ServerPenAudio.Code.Interfaces;


/* 
 * Free FFT and convolution (C#)
 * 
 * Copyright (c) 2017 Project Nayuki. (MIT License)
 * https://www.nayuki.io/page/free-small-fft-in-multiple-languages
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
 * the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 * - The above copyright notice and this permission notice shall be included in
 *   all copies or substantial portions of the Software.
 * - The Software is provided "as is", without warranty of any kind, express or
 *   implied, including but not limited to the warranties of merchantability,
 *   fitness for a particular purpose and noninfringement. In no event shall the
 *   authors or copyright holders be liable for any claim, damages or other
 *   liability, whether in an action of contract, tort or otherwise, arising from,
 *   out of or in connection with the Software or the use or other dealings in the
 *   Software.
 */
namespace ServerPenAudio.Code
{
    public class FrequencyManager : IFrequencyManager
    {
        /// <summary>
        /// Process 16 bit sample
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IEnumerable<ChannelFrequency>> GetFrequencyDomainAsync(FrequencyDomainOptions options)
        {
            return Task.Factory.StartNew<IEnumerable<ChannelFrequency>>(() =>
            {
                var sampleSize = options.SampleSize;
                var channelLength = options.Data.Length / 4;
                var floorSamplesCount = RoundUpToPreviousPowerOf2(channelLength);
                var ceilingSamplesCount = RoundUpToNextPowerOf2(channelLength);
                
                var left = new Complex[channelLength];
                var right = new Complex[channelLength];
                for (int i = 0; i < channelLength; i++)
                {
                    left[i] = BitConverter.ToInt16(options.Data, i * 4) / 32768d;
                    right[i] = BitConverter.ToInt16(options.Data, i * 4 + 2) / 32768d;
                }

                var leftChannel = new ChannelFrequency(Channel.Left);
                var rightChannel = new ChannelFrequency(Channel.Right);
                for (int i = 0; i <= floorSamplesCount; i += sampleSize)
                {
                    CreateFrequencyWindow(leftChannel, left.Skip(i).Take(sampleSize).ToArray());
                    CreateFrequencyWindow(rightChannel, right.Skip(i).Take(sampleSize).ToArray());
                }

                if (ceilingSamplesCount > channelLength)
                { 
                    CreateFrequencyWindow(leftChannel, left.Skip(floorSamplesCount).Take(channelLength - floorSamplesCount, sampleSize).ToArray());
                    CreateFrequencyWindow(rightChannel, right.Skip(floorSamplesCount).Take(channelLength - floorSamplesCount, sampleSize).ToArray());
                }

                return new List<ChannelFrequency>() {leftChannel, rightChannel};
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void CreateFrequencyWindow(ChannelFrequency result, Complex[] data)
        {
            Fft.Transform(data);
            result.Append(data
                .Take(data.Length / 2)
                .Select(x => 20 * Math.Log10(x.Magnitude))
                .ToArray()
            );
        }

        private int RoundUpToNextPowerOf2(int v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;
        }

        private int RoundUpToPreviousPowerOf2(int x)
        {
            x = x | (x >> 1);
            x = x | (x >> 2);
            x = x | (x >> 4);
            x = x | (x >> 8);
            x = x | (x >> 16);
            return x - (x >> 1);
        }
        private static class Fft
        {
            /* 
             * Computes the discrete Fourier transform (DFT) of the given complex vector, storing the result back into the vector.
             */
            public static void Transform(Complex[] vector)
            {
                int n = vector.Length;
                if (n == 0 || (n & (n - 1)) == 0)
                    throw new ArgumentException("Provided audio samples are not power of 2");

                TransformRadix2(vector);
            }


            /* 
             * Computes the discrete Fourier transform (DFT) of the given complex vector, storing the result back into the vector.
             * The vector's length must be a power of 2. Uses the Cooley-Tukey decimation-in-time radix-2 algorithm.
             */
            private static void TransformRadix2(Complex[] vector)
            {
                // Length variables
                int n = vector.Length;
                int levels = 0; // compute levels = floor(log2(n))
                for (int temp = n; temp > 1; temp >>= 1)
                    levels++;
                if (1 << levels != n)
                    throw new ArgumentException("Length is not a power of 2");

                // Trigonometric table
                Complex[] expTable = new Complex[n / 2];
                double coef = 2 * Math.PI / n * -1;
                for (int i = 0; i < n / 2; i++)
                    expTable[i] = Complex.Exp(new Complex(0, i * coef));

                // Bit-reversed addressing permutation
                for (int i = 0; i < n; i++)
                {
                    int j = (int) ((uint) ReverseBits(i) >> (32 - levels));
                    if (j > i)
                    {
                        Complex temp = vector[i];
                        vector[i] = vector[j];
                        vector[j] = temp;
                    }
                }

                // Cooley-Tukey decimation-in-time radix-2 FFT
                for (int size = 2; size <= n; size *= 2)
                {
                    int halfsize = size / 2;
                    int tablestep = n / size;
                    for (int i = 0; i < n; i += size)
                    {
                        for (int j = i, k = 0; j < i + halfsize; j++, k += tablestep)
                        {
                            Complex temp = vector[j + halfsize] * expTable[k];
                            vector[j + halfsize] = vector[j] - temp;
                            vector[j] += temp;
                        }
                    }

                    if (size == n) // Prevent overflow in 'size *= 2'
                        break;
                }
            }


            private static int ReverseBits(int val)
            {
                int result = 0;
                for (int i = 0; i < 32; i++, val >>= 1)
                    result = (result << 1) | (val & 1);
                return result;
            }
        }

    }
}