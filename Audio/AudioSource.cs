using Silk.NET.Maths;
using Tamale.Behaviour;

namespace Tamale.Audio
{
    // Plays a sound in 3D space
    public class AudioSource : Component
    {
        public uint source;

        internal Vector3D<float> _position;

        // Position of the audio source in 3D space
        public Vector3D<float> position
        {
            get => _position;
            set
            {
                _position = value;
                // Update the position in OpenAL
                AudioVars.al.SetSourceProperty(source, Silk.NET.OpenAL.SourceVector3.Position, _position.ToSystem());
            }
        }

        public AudioSource()
        {
            // Generate a new OpenAL source
            source = AudioVars.al.GenSource();
            AudioVars.al.SetSourceProperty(source, Silk.NET.OpenAL.SourceBoolean.Looping, false);
            AudioVars.al.SetSourceProperty(source, Silk.NET.OpenAL.SourceVector3.Position, _position.ToSystem());
        }

        public void Play(Sound sound)
        {
            // Attach the sound buffer to the source and play it
            AudioVars.al.SetSourceProperty(source, Silk.NET.OpenAL.SourceInteger.Buffer, sound.buffer);
            AudioVars.al.SourcePlay(source);
        }

        public override void Update(double delta)
        {
            // Keep position updated
            if (position != gameObject.Position)
            {
                position = gameObject.Position;
            }
        }
    }
}
