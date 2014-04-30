using System;
using System.Collections.Generic;

namespace nex.Utilities.Serialization
{
    [Serializable]
    public class SerializationData
    {
        /// <summary>
        /// Key of package
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Actual data
        /// </summary>
        public Dictionary<string, object> Data { get; set; }

        public SerializationData(string key, Dictionary<string, object> data)
        {
            Key = key;
            Data = data;
        }
    }
}