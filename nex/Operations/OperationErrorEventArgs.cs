using System;

namespace nex.Operations
{
    public sealed class OperationErrorEventArgs : EventArgs
    {
        public bool Error { get; set; }
    }
}
