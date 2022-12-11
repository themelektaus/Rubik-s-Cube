using System;
using System.Collections.Generic;

namespace Tausi.RubiksCube
{
    [Serializable]
    public class History
    {
        public List<Message> messages = new();

        public void AddSpin(Axis axis, int index, bool clockwise)
        {
            messages.Add(new Message
            {
                type = MessageType.Spin,
                axis = axis,
                index = index,
                clockwise = clockwise
            });
        }

        public void Run(Cube cube, bool reverse, SpinOptions options)
        {
            options.ignoreInactive = true;

            var messages = new List<Message>(this.messages);

            if (reverse)
                messages.Reverse();

            foreach (var message in messages)
                if (message.type == MessageType.Spin)
                    cube.AddSpin(message.axis, message.index, message.clockwise, options);
        }
    }
}