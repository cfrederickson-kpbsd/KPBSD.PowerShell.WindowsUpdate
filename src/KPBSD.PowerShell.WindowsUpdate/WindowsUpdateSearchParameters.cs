using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace KPBSD.PowerShell.WindowsUpdate
{
    public sealed class WindowsUpdateSearchParameters : IEquatable<WindowsUpdateSearchParameters> {
        private WildcardPattern[] _title;
        private string[] _updateId;
        private bool _includeHidden;

        public WindowsUpdateSearchParameters(WildcardPattern[] title, string[] updateId, bool includeHidden) {
            _title = title;
            if (_title == null) {
                _title = Array.Empty<WildcardPattern>();
            }
            _updateId = updateId;
            if (_updateId == null) {
                _updateId = Array.Empty<string>();
            }
            _includeHidden = includeHidden;
        }
        public WildcardPattern[] Title { get { return (WildcardPattern[])_title.Clone(); } }
        public string[] UpdateId { get { return (string[])_updateId; } }
        public bool IncludeHidden { get { return _includeHidden; } }
        public override string ToString()
        {
            return string.Format("Filters: [ Titles: {0} ] [ UpdateIds: {1} ] [ IncludeHidden: {2} ]", this.Title.Length, this.UpdateId.Length, this.IncludeHidden);
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
        public string GetServerFilter() {

            if (this.UpdateId.Length == 1) {
                if (this.IncludeHidden) {
                    return string.Format("IsInstalled = 0 and UpdateId = '{0}'", this.UpdateId[0]);
                }
                else {
                    return string.Format("IsInstalled = 0 and UpdateId = '{0}' and IsHidden = 0", this.UpdateId[0]);
                }
            }
            else {
                if (this.IncludeHidden) {
                    return "IsInstalled = 0";
                }
                else {
                    return "IsInstalled = 0 and IsHidden = 0";
                }
            }
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