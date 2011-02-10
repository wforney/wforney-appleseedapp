// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanDataType.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   Boolean Data Type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.DataTypes
{
    using System;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Boolean Data Type
    /// </summary>
    public class BooleanDataType : BaseDataType<bool, CheckBox>
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BooleanDataType" /> class.
        /// </summary>
        public BooleanDataType()
        {
            this.Type = PropertiesDataType.Boolean;
            this.InitializeComponents();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the description.
        /// </summary>
        public override string Description
        {
            get
            {
                return "Boolean";
            }
        }

        /// <summary>
        ///   EditControl
        /// </summary>
        /// <value>The edit control.</value>
        public override CheckBox EditControl
        {
            get
            {
                if (this.InnerControl == null)
                {
                    this.InitializeComponents();
                }

                // Update value in control
                this.InnerControl.Checked = this.Value;

                // Return control
                return this.InnerControl;
            }

            set
            {
                if (value.GetType().Name != "CheckBox")
                {
                    throw new ArgumentException(
                        "EditControl",
                        string.Format("A CheckBox values is required, a '{0}' is given.", value.GetType().Name));
                }
                
                this.InnerControl = value;

                // Update value from control
                this.Value = this.InnerControl.Checked;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the components.
        /// </summary>
        protected override void InitializeComponents()
        {
            // Checkbox
            this.InnerControl = new CheckBox();
        }

        #endregion
    }
}