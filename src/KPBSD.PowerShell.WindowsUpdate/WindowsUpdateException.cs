using System;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace KPBSD.PowerShell.WindowsUpdate
{
    [Serializable]
    public class WindowsUpdateException : Exception {
        private UpdateExceptionContext _context;
        private ErrorRecord? _errorRecord;
        public ErrorRecord? ErrorRecord { get { return _errorRecord; } }
        public UpdateExceptionContext Context { get { return _context; } }
        public WindowsUpdateException() : base("The Windows Update API experienced an error.") {

        }
        public WindowsUpdateException(string message) : base(message) {

        }
        public WindowsUpdateException(string message, Exception inner) : base (message, inner) {

        }
        internal WindowsUpdateException(IUpdateException updateException) : base(updateException.Message, updateException as Exception) {
            this.HResult = (int)updateException.HResult;
            this._context = (UpdateExceptionContext)(int)updateException.Context;
            this._errorRecord = ComErrorCodes.CreateErrorRecord((int)updateException.HResult, this, null);
        }
        protected WindowsUpdateException(SerializationInfo info, StreamingContext context) : base(info, context) {

        }
    }
}