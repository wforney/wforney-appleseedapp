using System.IO;
using System.Web;
using System.Web.Caching;
using System.Xml.Serialization;

namespace Appleseed.Framework.Helpers
{

	/// <summary>
	/// WebPart
	/// Added by Jakob Hansen.
	/// </summary>
	[XmlRoot(Namespace="urn:schemas-microsoft-com:webpart:", IsNullable=false)]
	public class WebPart
	{

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public int      AllowMinimize;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public int      AllowRemove;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement]
		public int      AutoUpdate;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public int      CacheBehavior;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public int      CacheTimeout;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   Content;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   ContentLink;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public int      ContentType;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   CustomizationLink;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   Description;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   DetailLink;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public int      FrameState;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public int      HasFrame;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   Height;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   HelpLink;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public int      IsIncluded;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public int      IsVisible;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   LastModified;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   MasterPartLink;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   Namespace;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   PartImageLarge;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   PartImageSmall;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public int      PartOrder;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   PartStorage;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public int      RequiresIsolation;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   Title;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   Width;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   XSL;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public string   XSLLink;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement ]
		public int      Zone;
		/*

		[XmlElementAttribute ]
		public string   Resource;  // Its an array thingy!!
		*/
	}

	/// <summary>
	/// WebPartParser
	/// Added by Jakob Hansen.
	/// </summary>
	public class WebPartParser
	{
		/// <summary>
		///     
		/// </summary>
		/// <param name="fileName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A Appleseed.Framework.Helpers.WebPart value...
		/// </returns>
		public static WebPart Load(string fileName)
		{
			HttpContext context = HttpContext.Current;
			WebPart partData = (WebPart) context.Cache[fileName];

			if (partData == null)
			{

				if (File.Exists(fileName))
				{
					StreamReader reader = File.OpenText(fileName);

					try
					{
						XmlSerializer serializer = new XmlSerializer(typeof(WebPart));
						partData = (WebPart) serializer.Deserialize(reader);
					}

					finally
					{
						reader.Close();
					}
					context.Cache.Insert(fileName, partData, new CacheDependency(fileName));
				}
			}
			return partData;
		}
	}
}
