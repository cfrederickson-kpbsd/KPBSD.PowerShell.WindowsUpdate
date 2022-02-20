using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace KPBSD.PowerShell.WindowsUpdate
{
    public sealed class WindowsUpdateSearchParameters : IEquatable<WindowsUpdateSearchParameters> {
        private readonly WildcardPattern[] _title;
        private readonly string[] _updateId;
        private readonly bool _includeHidden;
        private readonly bool _includeInstalled;

        public WindowsUpdateSearchParameters(WildcardPattern[] title, string[] updateId, bool includeHidden, bool includeInstalled) {
            _title = title;
            if (_title == null) {
                _title = Array.Empty<WildcardPattern>();
            }
            _updateId = updateId;
            if (_updateId == null) {
                _updateId = Array.Empty<string>();
            }
            _includeHidden = includeHidden;
            _includeInstalled = includeInstalled;
        }
        public WildcardPattern[] Title { get { return (WildcardPattern[])_title.Clone(); } }
        public string[] UpdateId { get { return (string[])_updateId; } }
        public bool IncludeHidden { get { return _includeHidden; } }
        public bool IncludeInstalled { get { return _includeInstalled; } }
        public override string ToString()
        {
            return string.Format("Filters: [ Titles: {0} ] [ UpdateIds: {1} ] [ IncludeHidden: {2} ] [ IncludeInstalled: {3} ]", this.Title.Length, this.UpdateId.Length, this.IncludeHidden, this.IncludeInstalled);
        }
        
        public bool Filter(dynamic update) {
            if (UpdateId.Length > 0 && !System.Linq.Enumerable.Contains(UpdateId, update.Identity.UpdateId, StringComparer.OrdinalIgnoreCase)) {
                return false;
            }
            if (Title.Length > 0) {
                var titleMatched = false;
                foreach (var title in Title) {
                    if (title.IsMatch(update.Title)) {
                        titleMatched = true;
                        break;
                    }
                }
                if (!titleMatched) {
                    return false;
                }
            }
            return true;
        }
        private static StringBuilder AppendAnd(StringBuilder sb, string str, params object[] formatData) {
            if (sb.Length > 0) {
                sb.Append(" and ");
            }
            return sb.AppendFormat(str, formatData);
        }
        private int IncludeInstalledAsNumber() {
            if (this.IncludeInstalled) {
                return 1;
            }
            else {
                return 0;
            }
        }
        public string GetServerFilter() {
            var sb = new StringBuilder();
            if (!this.IncludeInstalled) {
                AppendAnd(sb, "IsInstalled = 0");
            }
            if (!this.IncludeHidden) {
                AppendAnd(sb, "IsHidden = 0");
            }
            if (this.UpdateId.Length == 1) {
                AppendAnd(sb, "UpdateId = '{0}'", this.UpdateId[0]);
            }
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is WindowsUpdateSearchParameters) {
                return this.Equals((WindowsUpdateSearchParameters)obj);
            }
            else {
                return false;
            }
        }
        public bool Equals(WindowsUpdateSearchParameters parameters) {
            return System.Linq.Enumerable.SequenceEqual(this._title, parameters._title) &&
                    System.Linq.Enumerable.SequenceEqual(this._updateId, parameters._updateId) &&
                    _includeHidden == parameters._includeHidden;
        }

        public override int GetHashCode()
        {
            int hashCode = 722471353;
            hashCode = hashCode * -1521134295 + EqualityComparer<WildcardPattern[]>.Default.GetHashCode(_title);
            hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(_updateId);
            hashCode = hashCode * -1521134295 + _includeHidden.GetHashCode();
            return hashCode;
        }
    }
}