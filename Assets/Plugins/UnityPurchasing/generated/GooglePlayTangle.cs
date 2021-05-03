#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("PA6P8DPmM/7afz1yQ2IOznDTCrEio3lZ9wV29lXD6qmiCQYf9zp1SnGwSYIJxpcEWs6Te2yc7FYtaNJdNLy69wt2TuZxjKIdt9GOCBcjkM7oWtn66NXe0fJekF4v1dnZ2d3Y2zYA4wDY3KUlK1g5Z6jPe1f/vwz7NQ8cynETtQmL+S3tRSY7ejttBMn056Q65K2qYV4b4yPVV7wehHAQKRZFtxmdbCzN+If9TYYiCrO4pbnayE49wO15V2782XcQaNbV21N1KdbV3+MKh7Qna0qMCsn9ZW/l9bkVizdKAlb7gZFkNcRRIgw22QCmTMSjWtnX2Oha2dLaWtnZ2FYpfh6q/qCVicoUP6MFj8NrGfhFdkT7GM4ghPpcBAs8irpCr9rb2djZ");
        private static int[] order = new int[] { 4,13,7,12,4,5,10,7,12,12,12,11,13,13,14 };
        private static int key = 216;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
