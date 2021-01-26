using Studio.Wizet.Audio.CoreAudioApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Studio.Wizet.Audio.CoreAudioApi
{
    internal class AudioSessionNotification : IAudioSessionNotification
    {
        private AudioSessionManager parent;

        internal AudioSessionNotification(AudioSessionManager parent)
        {
            this.parent = parent;
        }

        [PreserveSig]
        public int OnSessionCreated(IAudioSessionControl newSession)
        {
            parent.FireSessionCreated(newSession);
            return 0;
        }
    }
}
