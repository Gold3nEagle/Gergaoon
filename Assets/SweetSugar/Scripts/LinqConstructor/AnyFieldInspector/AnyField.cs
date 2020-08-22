using System;
using System.Collections.Generic;

namespace SweetSugar.Scripts.LinqConstructor.AnyFieldInspector
{
    [Serializable]
    public class AnyField
    {
        public object obj;
        public int index;
        public bool expand;
        public string nameObj;
        public List<AnyField> objectPath = new List<AnyField>();
        public object value;
        public AnyField(object obj_ = null, int index_ = 0, object _value = null)
        {
            obj = obj_;
            index = index_;
            if (obj != null)
                nameObj = obj.ToString();
            value = _value;
        }
    }
}