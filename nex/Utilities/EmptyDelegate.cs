using System;

namespace nex.Utilities
{
    /// <summary>
    /// Contains instance of EmptyDelagate of Action type
    /// </summary>
    public class EmptyDelegate
    {
        /// <summary>
        /// Instance of EmptyDelagate of Action type
        /// </summary>
        public static Action Instance = delegate() { };
    }
}