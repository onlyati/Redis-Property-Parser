using System;
using System.Linq;

namespace RedisPropertyDeserializer
{
    public static class RedisPropertyDeserializer
    {
        public static TValue Deserializer<TValue>(string? text) where TValue : new()
        {
            TValue DeserObject = (TValue)Activator.CreateInstance(typeof(TValue));

            if (string.IsNullOrEmpty(text))
                return DeserObject;

            // Split the input string into a dictionary
            string[] lines = text.Split(Environment.NewLine);
            var input = lines.Where(w => !string.IsNullOrEmpty(w))
                             .Where(w => w.Substring(0, 1) != "#" && w != Environment.NewLine)
                             .Select(s => new { Name = s.Substring(0, s.IndexOf(':')), Value = s.Substring(s.IndexOf(':') + 1) })
                             .ToDictionary(d => d.Name, d => d.Value);

            var parseClass = DeserObject.GetType(); // Get type of the result

            foreach (var item in parseClass.GetProperties())
            {
                string name = item.Name;
                var value = item.GetValue(DeserObject, null);

                // Check each attribute on the type
                var attrs = item.GetCustomAttributes(true);
                foreach (var attr in attrs)
                {
                    var rp = attr as RedisPropertyName;
                    if (rp != null)
                    {
                        // If it is RedisPropertyName then set its value
                        string? inputValue = null;
                        if (input.TryGetValue(rp.Name, out inputValue))
                        {
                            try
                            {
                                item.SetValue(DeserObject, Convert.ChangeType(inputValue, item.PropertyType));
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(inputValue, ex);
                            }

                        }
                    }
                }
            }

            return DeserObject;
        }
    }
}
