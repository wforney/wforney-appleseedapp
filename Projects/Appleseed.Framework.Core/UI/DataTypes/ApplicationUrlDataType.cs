namespace Appleseed.Framework.DataTypes
{
	/// <summary>
	/// ApplicationUrlDataType
	/// </summary>
	public class ApplicationUrlDataType : UrlDataType
	{
		/// <summary>
		/// ApplicationUrlDataType
		/// </summary>
		public ApplicationUrlDataType()
		{
			InnerDataType = PropertiesDataType.String;
		}

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
		public override string Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				base.Value = value;
			}
		}

        /// <summary>
        /// Url relative to Application
        /// </summary>
        /// <value>The description.</value>
		public override string Description
		{
			get
			{
				return "Url relative to Application";
			}
		}
	}

}