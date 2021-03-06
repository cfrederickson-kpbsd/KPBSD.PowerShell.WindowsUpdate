namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Filter terms for the <see cref="WindowsUpdateJob"/> when the job will perform a search for updates.
    /// </summary>
    public class SearchJobFilter
    {
        public SearchJobFilter(
            bool includePotentiallySupersededUpdates,
            bool searchOffline,
            ServerSelection serverSelection,
            string? serviceId,
            string[]? title,
            string[]? updateId,
            string[]? categoryName,
            string[]? categoryId,
            bool includeHidden,
            bool includeInstalled,
            UpdateType? type,
            bool IsSearchForUninstallations,
            bool? assignedForAutomaticUpdates,
            bool? browseOnly,
            bool? autoSelectOnWebSites,
            bool? isPresent,
            bool? rebootRequired
        )
        {
            this.IncludePotentiallySupersededUpdates = includePotentiallySupersededUpdates;
            this.SearchOffline = searchOffline;
            this.ServerSelection = serverSelection;
            this.ServiceId = serviceId;
            if (title != null)
            {
                this.Title = new ReadOnlyCollection<string>((string[])title.Clone());
            }
            if (updateId != null)
            {
                this.UpdateId = new ReadOnlyCollection<string>((string[])updateId.Clone());
            }
            if (categoryName != null)
            {
                this.CategoryName = new ReadOnlyCollection<string>((string[])categoryName.Clone());
            }
            if (categoryId != null)
            {
                this.CategoryId =  new ReadOnlyCollection<string>((string[])categoryId.Clone());
            }
            this.IncludeHidden = includeHidden;
            this.IncludeInstalled = includeInstalled;
            this.Type = type;
            this.IsSearchForUninstallations = IsSearchForUninstallations;
            this.AssignedForAutomaticUpdates = assignedForAutomaticUpdates;
            this.BrowseOnly = browseOnly;
            this.AutoSelectOnWebSites = autoSelectOnWebSites;
            this.IsPresent = isPresent;
            this.RebootRequired = rebootRequired;
        }

        public bool IncludePotentiallySupersededUpdates { get; }
        public bool SearchOffline { get; }
        public ServerSelection ServerSelection { get; }
        public string? ServiceId { get; }

        /// <summary>
        /// Filters by any title pattern. While this criteria cannot be filtered on the server,
        /// it makes sense to include it here instead of in the associated PowerShell command
        /// in case the -AsJob parameter is used.
        /// </summary>
        /// <value></value>
        public ReadOnlyCollection<string>? Title { get; }
        /// <summary>
        /// Filters by any UpdateID. While this criteria cannot be filtered on the server (unless
        /// only one UpdateId is provided), it makes sense to include it here instead of in the
        /// associated PowerShell command in case the -AsJob parameter is used.
        /// </summary>
        /// <value></value>
        public ReadOnlyCollection<string>? UpdateId { get; }
        /// <summary>
        /// Filters by any CategoryName. While this criteria cannot be filtered on the server,
        /// it makes sense to include it here instead of in the associated PowerShell command
        /// in case the -AsJob parameter is used.
        /// </summary>
        /// <value></value>
        public ReadOnlyCollection<string>? CategoryName { get; }
        /// <summary>
        /// Filters by any CategoryId. While this criteria cannot be filtered on the server,
        /// it makes sense to include it here instead of in the associated PowerShell command
        /// in case the -AsJob parameter is used.
        /// </summary>
        /// <value></value>
        public ReadOnlyCollection<string>? CategoryId { get; }
        /// <summary>
        /// Indicates thaat 
        /// </summary>
        /// <value></value>
        public bool IncludeHidden { get; }
        /// <summary>
        /// Indicates that updates should be included even if they are already installed on the computer.
        /// </summary>
        /// <value></value>
        public bool IncludeInstalled { get; }
        /// <summary>
        /// Filters by UpdateType unless null. Accepted values are "Driver" and "Software".
        /// </summary>
        /// <value></value>
        public UpdateType? Type { get; }
        /// <summary>
        /// Searches for Uninstallation updates instead of the default search for Installation type updates.
        /// </summary>
        /// <value></value>
        public bool IsSearchForUninstallations { get; }
        /// <summary>
        /// Filters by IsAssigned.
        /// </summary>
        /// <value></value>
        public bool? AssignedForAutomaticUpdates { get; }
        /// <summary>
        /// True finds updates that are considered optional.
        /// False finds updates that are not considered optional.
        /// </summary>
        /// <remarks>I believe BrowseOnly means the update will not be installed by Automatic Updates.</remarks>
        public bool? BrowseOnly { get; }
        /// <summary>
        /// True to find updates that are flagged to be automatically selected by Windows Updates.
        /// False finds updates that are not flagged for Automatic Updates.
        /// </summary>
        public bool? AutoSelectOnWebSites { get; }
        /// <summary>
        /// True finds updates that are present on a computer. If the update is valid for one or more products,
        /// the update is considered present if it is installed for one or more of the products.
        /// </summary>
        public bool? IsPresent { get; }
        /// <summary>
        /// True finds only updates that require a computer to be restarted to complete.
        /// False finds updates that do not require a computer to be restarted to complete an installation or uninstallation.
        /// </summary>
        /// <value></value>
        public bool? RebootRequired { get; }
    }
}