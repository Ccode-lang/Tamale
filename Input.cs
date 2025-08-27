using Silk.NET.Input;

namespace Tamale
{
    public class Input
    {
        internal static Dictionary<Key, bool> keyStates = new Dictionary<Key, bool>();

        internal static void Initialize(IInputContext input)
        {
            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                keyStates.Add(key, false);
            }
        }

        internal static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
        {
            keyStates[key] = true;
        }

        internal static void KeyUp(IKeyboard keyboard, Key key, int keyCode)
        {
            keyStates[key] = false;
        }

        public static bool GetKey(Key key)
        {
            if (keyStates.TryGetValue(key, out bool ret)) return ret;
            return false;
        }
    }
}
