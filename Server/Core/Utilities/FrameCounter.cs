using System;
using System.Collections.Generic;
using System.Linq;

namespace xServer.Core.Utilities
{
    public class FrameUpdatedEventArgs : EventArgs
    {
        public float CurrentFramesPerSecond { get; private set; }

        public FrameUpdatedEventArgs(float _CurrentFramesPerSecond)
        {
            CurrentFramesPerSecond = _CurrentFramesPerSecond;
        }
    }

    public delegate void FrameUpdatedEventHandler(FrameUpdatedEventArgs e);

    public class FrameCounter
    {
        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }

        public const int MAXIMUM_SAMPLES = 100;

        private Queue<float> _sampleBuffer = new Queue<float>();

        public event FrameUpdatedEventHandler FrameUpdated;

        public void Update(float deltaTime)
        {
            float currentFramesPerSecond = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(currentFramesPerSecond);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = currentFramesPerSecond;
            }

            OnFrameUpdated(new FrameUpdatedEventArgs(AverageFramesPerSecond));

            TotalFrames++;
            TotalSeconds += deltaTime;
        }

        protected virtual void OnFrameUpdated(FrameUpdatedEventArgs e)
        {
            FrameUpdatedEventHandler handler = FrameUpdated;
            if (handler != null)
                handler(e);
        }
    }
}