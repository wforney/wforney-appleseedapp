using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// Allows upload a file on current portal folder
    /// </summary>
    public class UploadedFileDataType : PortalUrlDataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadedFileDataType"/> class.
        /// </summary>
        public UploadedFileDataType()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadedFileDataType"/> class.
        /// </summary>
        /// <param name="portalPath">The portal path.</param>
        public UploadedFileDataType(string portalPath)
            : base(portalPath)
        {
        }

        /// <summary>
        /// String
        /// </summary>
        /// <value></value>
        public override string Description
        {
            get { return "A file that can be uploaded to server"; }
        }

        /// <summary>
        /// InitializeComponents
        /// </summary>
        protected override void InitializeComponents()
        {
            //UploadDialogTextBox
            using (UploadDialogTextBox upload = new UploadDialogTextBox())
            {
                upload.AllowEditTextBox = true;
                upload.Width = new Unit(controlWidth);
                upload.CssClass = "NormalTextBox";

                innerControl = upload;
            }
        }

        /// <summary>
        /// EditControl
        /// </summary>
        /// <value>The edit control.</value>
        public override Control EditControl
        {
            get
            {
                if (innerControl == null)
                    InitializeComponents();

                //Update value in control
                UploadDialogTextBox upload = (UploadDialogTextBox) innerControl;
                upload.UploadDirectory = PortalPathPrefix;
                upload.Text = Value;

                //Return control
                return innerControl;
            }
            set
            {
                if (value.GetType().Name == "UploadDialogTextBox")
                {
                    innerControl = value;
                    //Update value from control
                    UploadDialogTextBox upload = (UploadDialogTextBox) innerControl;
                    Value = upload.Text;
                }
                else
                    throw new ArgumentException(
                        "A UploadDialogTextBox values is required, a '" + value.GetType().Name + "' is given.",
                        "EditControl");
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public override string Value
        {
            get { return (innerValue); }
            set
            {
                //Remove portal path if present
                if (value.StartsWith(PortalPathPrefix))
                    innerValue = value.Substring(PortalPathPrefix.Length);
                else
                    innerValue = value;

                innerValue = innerValue.TrimStart('/');
            }
        }
    }
}