using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xServer.Core.Utilities
{
    class TransferCalculator
    {
        public int AverageSpeed
        {
            get { return (int)_speedAvgs.Average(); }
        }

        public int AverageTimeLeft
        {
            get { return (int) _timeLeftAvgs.Average(); }
        }

        private readonly List<int> _speedAvgs;
        private readonly List<int> _timeLeftAvgs;

        private int _speedCounter, _timeLeftCounter;
        // TODO: Figure out the optimal count
        private const int AvgCollectionCount = 125;

        public TransferCalculator()
        {
            _speedAvgs = new List<int>();
            _timeLeftAvgs = new List<int>();
            _speedCounter = 0;
            _timeLeftCounter = 0;
        }

        public void Reset()
        {
            _speedAvgs.Clear();
            _timeLeftAvgs.Clear();
            _speedCounter = 0;
            _timeLeftCounter = 0;
        }

        public void AddSpeed(int speed)
        {
            if (_speedCounter + 1 > AvgCollectionCount)
                _speedCounter = 0;
            else
            {
                if (_speedAvgs.Count >= AvgCollectionCount)
                    _speedAvgs[_speedCounter++] = speed;
                else
                {
                    _speedAvgs.Add(speed);
                    _speedCounter++;
                }
            }
        }

        public void AddTimeLeft(int timeLeft)
        {
            if (_timeLeftCounter + 1 > AvgCollectionCount)
                _timeLeftCounter = 0;
            else
            {
                if (_timeLeftAvgs.Count >= AvgCollectionCount)
                    _timeLeftAvgs[_timeLeftCounter++] = timeLeft;
                else
                {
                    _timeLeftAvgs.Add(timeLeft);
                    _timeLeftCounter++;
                }
            }
        }

        private static readonly string[] SizeSuffixes =
                   { "bytes", "kb", "mb", "gb" };

        public static string GetSizeSuffix(long value)
        {
            if (value < 0) { return "-" + GetSizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n2} {1}/s", adjustedSize, SizeSuffixes[mag]);
        }
    }
}
