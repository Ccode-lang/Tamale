using Silk.NET.Maths;
using Tamale.Behaviour;

namespace Tamale.Audio
{
    public class AudioSource : Component
    {
        public uint source;

        internal Vector3D<float> _position;
        public Vector3D<float> position
        {
            get => _position;
            set
            {
                _position = value;
                AudioVars.al.SetSourceProperty(source, Silk.NET.OpenAL.SourceVector3.Position, _position.ToSystem());
            }
        }

        public AudioSource()
        {
            source = AudioVars.al.GenSource();
            AudioVars.al.SetSourceProperty(source, Silk.NET.OpenAL.SourceBoolean.Looping, false);
            AudioVars.al.SetSourceProperty(source, Silk.NET.OpenAL.SourceVector3.Position, _position.ToSystem());
        }

        public void Play(Sound sound)
        {
            AudioVars.al.SetSourceProperty(source, Silk.NET.OpenAL.SourceInteger.Buffer, sound.buffer);
            AudioVars.al.SourcePlay(source);
        }

        public override void Update(double delta)
        {
            if (position != gameObject.Position)
            {
                position = gameObject.Position;
            }
        }
    }
}
