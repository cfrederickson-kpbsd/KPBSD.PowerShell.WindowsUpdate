namespace KPBSD.PowerShell.WindowsUpdate
{
    public sealed class JobResult<TJobResult>
    {
        public TJobResult Result { get { return _result; } }
        public IUpdateCollection Updates { get { return _updates; } }
        public JobResult(TJobResult result, IUpdateCollection updates)
        {
            this._result = result;
            this._updates = updates;
        }
        private readonly TJobResult _result;
        private readonly IUpdateCollection _updates;
    }
}