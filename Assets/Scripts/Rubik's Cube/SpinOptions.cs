namespace Tausi.RubiksCube
{
    public struct SpinOptions
    {
        public float speed;
        public bool invokeMessageEvent;
        public Frontside frontside;
        public bool ignoreInactive;

        public static SpinOptions Default => new()
        {
            speed = 1,
            invokeMessageEvent = true
        };

        public static SpinOptions Fast => new()
        {
            speed = 2,
            invokeMessageEvent = true
        };

        public static SpinOptions InstantAndWithoutMessageEvent => new()
        {
            speed = 0,
            invokeMessageEvent = false
        };

        public static SpinOptions WithFrontside(Frontside frontside)
        {
            var options = Default;
            options.frontside = frontside;
            return options;
        }
    }
}