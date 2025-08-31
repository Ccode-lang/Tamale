using Silk.NET.Input;

namespace Tamale
{
    public class Input
    {
        // Dictionary to hold the state of each key
        internal static Dictionary<Key, bool> keyStates = new Dictionary<Key, bool>();

        internal static void Initialize(IInputContext input)
        {
            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                keyStates.Add(key, false);
            }
        }

        // Set key down
        internal static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
        {
            keyStates[key] = true;
        }

        // Set key up
        internal static void KeyUp(IKeyboard keyboard, Key key, int keyCode)
        {
            keyStates[key] = false;
        }

        // Check if a key is currently pressed
        public static bool GetKey(Key key)
        {
            if (keyStates.TryGetValue(key, out bool ret)) return ret;
            return false;
        }
    }
}
