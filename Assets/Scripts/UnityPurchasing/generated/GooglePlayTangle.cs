// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("SYB+T0HdxVUlMgvUMeWhjZNsaPcE1+wly+12HTaypsfg+HJF9k4o92aaOPYm5KJFL8Iff+U9Gpvoqjwd1t3bJtjwXNz9f/DbF+lUZLoixMU4zHAUh+r/azs//H3P7O3CYfDYRGfpAKP1wd3fa83wg2KiIG3bQ/G0Qazr3hTzPcSdLg64vUpzc+X+c8ggsZx24it+J0DVXyOVUoVAn5tH9QbjDJaoFUgbY96IAS1/ESVXAlTsdMZFZnRJQk1uwgzCs0lFRUVBREeaKt5nBlr7K2wJlmk7Z1lWo+tZMoJJS/b9GbjMmRdQli/Ee+hcdDesLYGf7Q4+Nz7JmUnloe6bRwmBjnnGRUtEdMZFTkbGRUVE1NbY3AfkHN49I06BkhbTJUZHRURF");
        private static int[] order = new int[] { 3,7,5,8,10,13,6,10,11,11,12,13,13,13,14 };
        private static int key = 68;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
