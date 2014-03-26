using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    public struct Identifier
    {
        private int? _value;

        public Identifier(object value)
        {
            if (value.GetType() == typeof(long))
            {
                _value = (int)(long)value;
            }
            else if (value.GetType() == typeof(int))
            {
                _value = (int)value;
            }
            else
            {
                throw new ArgumentException("Unsupported key value");
            }
        }

        internal int? Value 
        {
            get { return _value; }
            set { _value = value; }
        }

        public bool IsEmpty()
        {
            return Value == null;
        }
    }
}
