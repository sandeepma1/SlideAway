#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("DYWDzjJPd99ItZskjui3MS4aqfdIiXC7MP+uPWP3qkJVpdVvFFHrZM3enQPdlJNYZyLaGuxuhSe9SSkQDznaOeHlnBwSYQBekfZCbsaGNcKssPMtBpo8tvpSIMF8T33CIfcZvWPg7uHRY+Dr42Pg4OFvEEcnk8eZBTe2yQrfCsfjRgRLels390nqM4gMNiXzSCqMMLLAFNR8HwJDAlQ98C98jiCkVRX0wb7EdL8bM4qBnIDjG5pAYM48T89s+tOQmzA/Js4DTHPRY+DD0ezn6MtnqWcW7ODg4OTh4g5zO2/CuKhdDP1oGzUP4Dmfdf2a7ObaM76NHlJztTPwxFxW3MyALLLxdwT51EBuV8XgTilR7+ziakwQ78NlPTIFs4N7luPi4OHg");
        private static int[] order = new int[] { 12,7,2,5,6,7,6,10,10,13,12,11,13,13,14 };
        private static int key = 225;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
