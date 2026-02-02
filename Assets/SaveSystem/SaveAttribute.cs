using System;

namespace SaveSystem
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SaveAttribute : Attribute
    {
        public string Name { get; private set; }

        public SaveAttribute()
        {
        }

        public SaveAttribute(string name)
        {
            Name = name;
        }
    }
}