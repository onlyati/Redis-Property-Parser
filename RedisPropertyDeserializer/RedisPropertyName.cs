using System;
using System.Collections.Generic;
using System.Text;

namespace OnlyAti
{
    public class RedisPropertyName : Attribute
    {
        public string Name { get; set; }

        public RedisPropertyName(string name)
        {
            this.Name = name;
        }
    }
}
