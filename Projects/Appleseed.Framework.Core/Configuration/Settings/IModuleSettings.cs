namespace Appleseed.Framework.Site.Configuration
{
    using System;
    using System.Collections;

    public interface IModuleSettings
    {
        /// <summary>
        ///   The admin.
        /// </summary>
        bool Admin { get; set; }

        /// <summary>
        ///   The authorized add roles.
        /// </summary>
        string AuthorizedAddRoles { get; set; }

        /// <summary>
        ///   The authorized approve roles.
        /// </summary>
        string AuthorizedApproveRoles { get; set; }

        /// <summary>
        ///   The authorized delete module roles.
        /// </summary>
        string AuthorizedDeleteModuleRoles { get; set; }

        /// <summary>
        ///   The authorized delete roles.
        /// </summary>
        string AuthorizedDeleteRoles { get; set; }

        /// <summary>
        ///   The authorized edit roles.
        /// </summary>
        string AuthorizedEditRoles { get; set; }

        /// <summary>
        ///   The authorized move module roles.
        /// </summary>
        string AuthorizedMoveModuleRoles { get; set; }

        /// <summary>
        ///   The authorized properties roles.
        /// </summary>
        string AuthorizedPropertiesRoles { get; set; }

        /// <summary>
        ///   The authorized publishing roles.
        /// </summary>
        string AuthorizedPublishingRoles { get; set; }

        /// <summary>
        ///   The authorized view roles.
        /// </summary>
        string AuthorizedViewRoles { get; set; }

        /// <summary>
        ///   The cache dependency.
        /// </summary>
        ArrayList CacheDependency { get; set; }

        /// <summary>
        ///   The cache time.
        /// </summary>
        int CacheTime { get; set; }

        /// <summary>
        ///   The cacheable.
        /// </summary>
        bool Cacheable { get; set; }

        /// <summary>
        ///   The desktop source.
        /// </summary>
        string DesktopSrc { get; set; }

        /// <summary>
        ///   The guid id.
        /// </summary>
        Guid GuidID { get; set; }

        /// <summary>
        ///   The mobile source.
        /// </summary>
        string MobileSrc { get; set; }

        /// <summary>
        ///   The module def id.
        /// </summary>
        int ModuleDefID { get; set; }

        /// <summary>
        ///   The module id.
        /// </summary>
        int ModuleID { get; set; }

        /// <summary>
        ///   The module order.
        /// </summary>
        int ModuleOrder { get; set; }

        /// <summary>
        ///   The module title.
        /// </summary>
        string ModuleTitle { get; set; }

        /// <summary>
        ///   The page id.
        /// </summary>
        int PageID { get; set; }

        /// <summary>
        ///   The pane name.
        /// </summary>
        string PaneName { get; set; }

        /// <summary>
        ///   The show every where.
        /// </summary>
        bool ShowEveryWhere { get; set; }

        /// <summary>
        ///   The show mobile.
        /// </summary>
        bool ShowMobile { get; set; }

        /// <summary>
        ///   The support collapsible.
        /// </summary>
        bool SupportCollapsable { get; set; }

        /// <summary>
        ///   The support workflow.
        /// </summary>
        bool SupportWorkflow { get; set; }

        /// <summary>
        ///   The workflow status.
        /// </summary>
        WorkflowState WorkflowStatus { get; set; }
    }
}