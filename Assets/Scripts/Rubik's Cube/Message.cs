using System;

namespace Tausi.RubiksCube
{
    [Serializable]
    public class Message
    {
        public MessageType type;
        public Axis axis;
        public int index;
        public bool clockwise;
    }
}