using System;
using System.Collections.Generic;
using System.Text;

namespace OnlyAti
{
    public class RedisPropertyName : Attribute
    {
        public string Name { get; set; }

        /// <summary>
        /// Redis property name which value will be assigned to variable after deserilization
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        public RedisPropertyName(string name)
        {
            this.Name = name;
        }
    }
}
