using Tamale.Audio;
using Tamale.Behaviour;

namespace Tamale.Testing
{
    internal class PlaySoundEverySecond : Component
    {
        Sound sound;
        AudioSource source;
        double timer = 0;

        public PlaySoundEverySecond(Sound sound, AudioSource source)
        {
            this.sound = sound;
            this.source = source;
        }

        public override void Update(double delta)
        {
            timer += delta;

            if (timer >= 1.0)
            {
                timer = 0;
                source.Play(sound);
            }
        }
    }
}
