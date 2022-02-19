using System;
using System.Runtime.Serialization;

namespace KPBSD.PowerShell.WindowsUpdate
{
    [Serializable]
    public class WindowsUpdateException : Exception {
        private UpdateExceptionContext _context;
        public UpdateExceptionContext Context { get { return _context; } }
        public WindowsUpdateException() : base("The Windows Update API experienced an error.") {

        }
        public WindowsUpdateException(string message) : base(message) {

        }
        public WindowsUpdateException(string message, Exception inner) : base (message, inner) {

        }
        internal WindowsUpdateException(dynamic iUpdateException) : base((string)iUpdateException.Message, iUpdateException as Exception) {
            this.HResult = iUpdateException.HResult;
            this._context = (UpdateExceptionContext)(int)iUpdateException.Context;
        }
        protected WindowsUpdateException(SerializationInfo info, StreamingContext context) : base(info, context) {

        }
    }
}