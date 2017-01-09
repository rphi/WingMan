using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingMan.Objects
{
    public class Input
    {
        public InputType Type { get; set; }
        public int Id { get; set; }
        public int Value { get; set; }

        public Input(InputType type, int id, int value)
        {
            Type = type;
            Id = id;
            Value = value;
        }
    }

    public enum InputType
    {
        Button, Fader
    }

    public class InputComparer : IEqualityComparer<Input>
    {
        public bool Equals(Input x, Input y)
        {
            return x.Value == y.Value;
        }

        public int GetHashCode(Input x)
        {
            return x.GetHashCode();
        }
    }
}
