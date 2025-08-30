using Silk.NET.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamale.Audio
{
    public static unsafe class AudioVars
    {
        internal static AL al;
        internal static ALContext alc;

        internal static Device* device;
        internal static Context* audioContext;
        public static uint source;
    }
}
