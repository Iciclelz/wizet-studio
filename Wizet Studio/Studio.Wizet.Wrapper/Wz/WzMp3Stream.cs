/*
    Copyright (C) 2016 Ryukuo

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using Studio.Wizet.Audio.Wave;
using Studio.Wizet.Library.Wz.WzProperties;
using System;
using System.IO;

namespace Wizet_Studio.Studio.Wizet.Wrapper.Wz
{
    class WzMp3Stream
    {
        private Stream byteStream;

        private Mp3FileReader mpegStream;

        private WaveOut wavePlayer;

        private WzSoundProperty sound;

        private bool repeat;

        private bool disposed;

        public bool Disposed
        {
            get
            {
                return this.disposed;
            }
        }

        public int Length
        {
            get
            {
                return this.sound.Length / 1000;
            }
        }

        public int Position
        {
            get
            {
                return (int)(this.mpegStream.Position / (long)this.mpegStream.WaveFormat.AverageBytesPerSecond);
            }
            set
            {
                this.mpegStream.Seek((long)(value * this.mpegStream.WaveFormat.AverageBytesPerSecond), SeekOrigin.Begin);
            }
        }

        public bool Repeat
        {
            get
            {
                return this.repeat;
            }
            set
            {
                this.repeat = value;
            }
        }

        public WzMp3Stream(WzSoundProperty sound, bool repeat)
        {
            this.repeat = repeat;
            this.sound = sound;
            this.byteStream = new MemoryStream(sound.GetBytes(false));
            this.mpegStream = new Mp3FileReader(this.byteStream);
            this.wavePlayer = new WaveOut(WaveCallbackInfo.FunctionCallback());
            this.wavePlayer.Init(this.mpegStream);
            this.wavePlayer.PlaybackStopped += new EventHandler<StoppedEventArgs>(wavePlayer_PlaybackStopped);
        }

        private void wavePlayer_PlaybackStopped(object sender, EventArgs e)
        {
            if (this.repeat)
            {
                this.mpegStream.Seek((long)0, SeekOrigin.Begin);
                this.wavePlayer.Pause();
                this.wavePlayer.Play();
            }
        }

        public void Dispose()
        {
            this.disposed = true;
            this.wavePlayer.Dispose();
            this.wavePlayer.Stop();
            this.mpegStream.Dispose();
            this.byteStream.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void Pause()
        {
            this.wavePlayer.Pause();
        }

        public void Play()
        {
            this.wavePlayer.Play();
        }


    }
}
