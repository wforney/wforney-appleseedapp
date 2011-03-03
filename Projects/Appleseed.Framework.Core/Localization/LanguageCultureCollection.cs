// Esperantus - The Web translator
// Copyright (C) 2003 Emmanuele De Andreis
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Emmanuele De Andreis (manu-dea@hotmail dot it)

using System;
using System.Reflection;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Collections;

namespace Appleseed.Framework.Web.UI.WebControls
{
	/// <summary>
	/// LanguageCultureCollection
	/// </summary>
	[TypeConverter(typeof(TypeConverterLanguageCultureCollection))]
	public class LanguageCultureCollection : System.Collections.CollectionBase, ICollection
	{
		private readonly char[] itemsSeparator = {';'};
		private readonly char[] keyValueSeparator = {'='};

		public LanguageCultureCollection()
		{
		}

		public LanguageCultureCollection(string LanguageCultureCollection)
		{
			LanguageCultureCollection mylist = (LanguageCultureCollection) LanguageCultureCollection;

			foreach(LanguageCultureItem l in mylist)
				Add(l);
		}

		public LanguageCultureItem this[Int32 i]
		{
			get{return (LanguageCultureItem) InnerList[i];}
//			set{InnerList[i] = value;}		
		}

		public void Add(LanguageCultureItem item)
		{
			InnerList.Add(item);
		}

		public void Insert(Int32 index, LanguageCultureItem item)
		{
			InnerList.Insert(index, item);
		}

		public virtual bool Contains(LanguageCultureItem item)
		{
			return InnerList.Contains(item);
		}

		public virtual int IndexOf(LanguageCultureItem item)
		{
			return InnerList.IndexOf(item);
		}

		public void Remove(LanguageCultureItem item)
		{
			InnerList.Remove(item);
		}

		// Provide the explicit interface member for ICollection.
		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			InnerList.CopyTo(array, arrayIndex);
		}

		// Provide the strongly typed member for ICollection.
		public void CopyTo(LanguageCultureItem[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		public override string ToString()
		{
			System.Text.StringBuilder s = new System.Text.StringBuilder();
			foreach(LanguageCultureItem item in InnerList)
			{
				s.Append(item.UICulture.Name);
				s.Append(keyValueSeparator);
				s.Append(item.Culture.Name);
				s.Append(itemsSeparator);
			}
			return s.ToString();
		}

		/// <summary>
		/// Returns the best possible LanguageCultureItem 
		/// matching the provided culture
		/// </summary>
		/// <param name="culture"></param>
		/// <returns></returns>
		public LanguageCultureItem GetBestMatching(CultureInfo culture)
		{
			return GetBestMatching(new CultureInfo[] {culture});		
		}

		/// <summary>
		/// Returns the best possible LanguageCultureItem 
		/// matching cultures in provided list
		/// </summary>
		/// <param name="cultures"></param>
		/// <returns></returns>
		public LanguageCultureItem GetBestMatching(CultureInfo[] cultures)
		{
			//If null return default
			if (cultures == null || cultures.Length == 0 || cultures[0] == null)
				return (LanguageCultureItem) InnerList[0];

			//First pass, exact match
			foreach(CultureInfo culture in cultures)
			{
				for(Int32 i = 0; i < InnerList.Count; i++)
				{
					if (culture.Name == ((LanguageCultureItem) InnerList[i]).Culture.Name) // switched from UICulture to culture
						return (LanguageCultureItem) InnerList[i];
				}
			}
			//Second pass, we may accept a parent match
			foreach(CultureInfo culture in cultures)
			{
				for(Int32 i = 0; i < InnerList.Count; i++)
				{
                    //if ((culture.Name == ((LanguageCultureItem)InnerList[i]).UICulture.Name) ||
                    //    (culture.Parent.Name == ((LanguageCultureItem)InnerList[i]).UICulture.Name))
                        if ((culture.Name == ((LanguageCultureItem)InnerList[i]).Culture.Parent.Name) ||
                            (culture.Parent.Name == ((LanguageCultureItem)InnerList[i]).Culture.Name) ||
                            (culture.Parent.Name == ((LanguageCultureItem)InnerList[i]).Culture.Parent.Name))
                            return (LanguageCultureItem)InnerList[i];
				}
			}
			return null; //no applicable match
		}

		/// <summary>
		/// Returns a CultureInfo list matching language property
		/// </summary>
		/// <param name="addInvariantCulture">If true adds a row containing invariant culture</param>
		/// <returns></returns>
		public CultureInfo[] ToUICultureArray(bool addInvariantCulture)
		{
			ArrayList cultures = new ArrayList();
			if (addInvariantCulture)
				cultures.Add(CultureInfo.InvariantCulture);

			for(Int32 i = 0; i < InnerList.Count; i++)
			{
				cultures.Add(((LanguageCultureItem) InnerList[i]).UICulture);
			}
			return (CultureInfo[]) cultures.ToArray(typeof(CultureInfo));
		}

		/// <summary>
		/// Returns a CultureInfo list matching language property
		/// </summary>
		/// <returns></returns>
		public CultureInfo[] ToUICultureArray()
		{
			return ToUICultureArray(false);
		}

		/// <summary>
		/// Explicitly converts String to LanguageCultureCollection value
		/// </summary>
		/// <returns></returns>
		static public explicit operator LanguageCultureCollection(string languageList)
		{
			return (LanguageCultureCollection) TypeDescriptor.GetConverter(typeof(LanguageCultureCollection)).ConvertTo(languageList, typeof(LanguageCultureCollection));
		}

		/// <summary>
		/// Explicitly converts LanguageCultureCollection to String value
		/// </summary>
		/// <returns></returns>
		static public explicit operator String(LanguageCultureCollection langList)
		{
			return (string) TypeDescriptor.GetConverter(typeof(LanguageCultureCollection)).ConvertTo(langList, typeof(string));
		}
	}
}
