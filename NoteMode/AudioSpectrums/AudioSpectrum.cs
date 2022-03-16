// Audio spectrum component
// By Keijiro Takahashi, 2013
// https://github.com/keijiro/unity-audio-spectrum
using System;
using System.Linq;
using UnityEngine;
using System.Collections;

namespace NoteMode.AudioSpectrums
{
    public class AudioSpectrum : MonoBehaviour
    {
        #region Band type definition
        public enum BandType
        {
            FourBand,
            FourBandVisual,
            EightBand,
            TenBand,
            TwentySixBand,
            ThirtyOneBand
        };

        private static readonly float[][] middleFrequenciesForBands = {
            new float[]{ 125.0f, 500, 1000, 2000 },
            new float[]{ 250.0f, 400, 600, 800 },
            new float[]{ 63.0f, 125, 500, 1000, 2000, 4000, 6000, 8000 },
            new float[]{ 31.5f, 63, 125, 250, 500, 1000, 2000, 4000, 8000, 16000 },
            new float[]{ 25.0f, 31.5f, 40, 50, 63, 80, 100, 125, 160, 200, 250, 315, 400, 500, 630, 800, 1000, 1250, 1600, 2000, 2500, 3150, 4000, 5000, 6300, 8000 },
            new float[]{ 20.0f, 25, 31.5f, 40, 50, 63, 80, 100, 125, 160, 200, 250, 315, 400, 500, 630, 800, 1000, 1250, 1600, 2000, 2500, 3150, 4000, 5000, 6300, 8000, 10000, 12500, 16000, 20000 },
        };
        private static readonly float[] bandwidthForBands = {
            1.414f, // 2^(1/2)
            1.260f, // 2^(1/3)
            1.414f, // 2^(1/2)
            1.414f, // 2^(1/2)
            1.122f, // 2^(1/6)
            1.122f  // 2^(1/6)
        };
        #endregion

        public static BandType ConvertToBandtype(string bandTypeName) => Enum.GetValues(typeof(AudioSpectrum.BandType)).OfType<AudioSpectrum.BandType>().FirstOrDefault(x => string.Equals(x.ToString(), bandTypeName, StringComparison.CurrentCultureIgnoreCase));

        #region Public variables
        public int numberOfSamples = 1024;
        public float fallSpeed = 0.08f;
        public float sensibility = 8.0f;
        public event Action<AudioSpectrum> UpdateRawSpectrums;
        #endregion

        #region Private variables
        private BandType bandType = BandType.TenBand;
        private float[] rawSpectrum;
        private float[] levels;
        private float[] peakLevels;
        private float[] meanLevels;
        #endregion

        #region Public property
        public float[] Levels => this.levels;

        public float[] PeakLevels => this.peakLevels;

        public float[] MeanLevels => this.meanLevels;
        public BandType Band
        {
            get => this.bandType;

            set => this.SetBandType(ref this.bandType, value);
        }

        private bool SetBandType(ref BandType bt, BandType value)
        {
            if (bt == value)
            {
                return false;
            }

            bt = value;
            var bandCount = middleFrequenciesForBands[(int)bt].Length;
            if (this.levels.Length != bandCount)
            {
                this.levels = new float[bandCount];
                this.peakLevels = new float[bandCount];
                this.meanLevels = new float[bandCount];
            }
            return true;
        }

        #endregion

        #region Private functions
        private void CheckBuffers()
        {
            if (this.rawSpectrum == null || this.rawSpectrum.Length != this.numberOfSamples)
            {
                this.rawSpectrum = new float[this.numberOfSamples];
            }
            var bandCount = middleFrequenciesForBands[(int)this.Band].Length;
            if (this.levels == null || this.levels.Length != bandCount)
            {
                this.levels = new float[bandCount];
                this.peakLevels = new float[bandCount];
                this.meanLevels = new float[bandCount];
            }
        }

        private int FrequencyToSpectrumIndex(float f)
        {
            var i = Mathf.FloorToInt(f / AudioSettings.outputSampleRate * 2.0f * this.rawSpectrum.Length);
            return Mathf.Clamp(i, 0, this.rawSpectrum.Length - 1);
        }
        #endregion

        #region Monobehaviour functions
        private void Awake() => this.CheckBuffers();

        private void Update()
        {
            this.CheckBuffers();

            AudioListener.GetSpectrumData(this.rawSpectrum, 0, FFTWindow.BlackmanHarris);

            var middlefrequencies = middleFrequenciesForBands[(int)this.Band];
            var bandwidth = bandwidthForBands[(int)this.Band];

            var falldown = fallSpeed * Time.deltaTime;
            var filter = Mathf.Exp(-this.sensibility * Time.deltaTime);

            for (var bi = 0; bi < this.levels.Length; bi++)
            {
                int imin = this.FrequencyToSpectrumIndex(middlefrequencies[bi] / bandwidth);
                int imax = this.FrequencyToSpectrumIndex(middlefrequencies[bi] * bandwidth);

                var bandMax = 0.0f;
                for (var fi = imin; fi <= imax; fi++)
                {
                    bandMax = Mathf.Max(bandMax, this.rawSpectrum[fi]);
                }

                this.levels[bi] = bandMax;
                this.peakLevels[bi] = Mathf.Max(this.peakLevels[bi] - falldown, bandMax);
                this.meanLevels[bi] = bandMax - (bandMax - this.meanLevels[bi]) * filter;
            }
        }
        #endregion
    }
}
