using System;

namespace CHxP8
{
    public struct Int4
    {
        private Int4(int val)
        {
            if (val >= (1 << 4)) throw new ArgumentOutOfRangeException("val");
            this._value = val;
        }
        private int _value;
        public static explicit operator Int4(int value)
        {
            return new Int4(value);
        }

        public static implicit operator int(Int4 me)
        {
            return me._value;
        }
    }
    public struct Int12
    {
        private Int12(int val)
        {
            if (val >= (1 << 12)) throw new ArgumentOutOfRangeException("val");
            this._value = val;
        }
        private int _value;
        public static explicit operator Int12(int value)
        {
            return new Int12(value);
        }

        public static implicit operator int(Int12 me)
        {
            return me._value;
        }
    }
}