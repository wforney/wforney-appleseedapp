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
using System.Diagnostics;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Appleseed.Framework.Web.UI.WebControls
{
    public class LanguagesTemplateDropDown : ITemplate
    {
        private ListItemType templateType;
        //		string columnName;
        private CultureTypes cultureTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LanguagesTemplateDropDown"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="colName">Name of the col.</param>
        /// <param name="types">The types.</param>
        [Obsolete("colName parameter was removed")]
        public LanguagesTemplateDropDown(ListItemType type, string colName, CultureTypes types)
        {
            templateType = type;
            //			this.columnName = colName;
            cultureTypes = types;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LanguagesTemplateDropDown"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="types">The types.</param>
        public LanguagesTemplateDropDown(ListItemType type, CultureTypes types)
        {
            templateType = type;
            cultureTypes = types;
        }

        /// <summary>
        /// When implemented by a class, defines the <see cref="T:System.Web.UI.Control"></see> object that child controls and templates belong to. These child controls are in turn defined within an inline template.
        /// </summary>
        /// <param name="container">The <see cref="T:System.Web.UI.Control"></see> object to contain the instances of controls from the inline template.</param>
        public void InstantiateIn(Control container)
        {
            switch (templateType)
            {
                case ListItemType.Header:
                    break;
                case ListItemType.Item:
                    DropDownList langList = new DropDownList();
                    langList.Width = new Unit("135px", CultureInfo.InvariantCulture);

                    langList.DataSource = CultureInfo.GetCultures(cultureTypes);
                    langList.DataTextField = "DisplayName";
                    langList.DataValueField = "Name";

                    Debug.Assert(langList.Items.Count > 0);
                    //					langList.DataBinding += new System.EventHandler(this.TemplateControl_DataBinding);
                    container.Controls.Add(langList);
                    break;
                case ListItemType.EditItem:
                    break;
                case ListItemType.Footer:
                    break;
            }
        }

        //		private void TemplateControl_DataBinding(object sender, System.EventArgs e)
        //		{
        //			DropDownList langList = (DropDownList) sender;
        //			System.Diagnostics.Debug.Assert(langList.Items.Count > 0);
        //
        //			//Get item value
        //			System.Web.UI.WebControls.DataGridItem container = (System.Web.UI.WebControls.DataGridItem) langList.NamingContainer;
        //			string currentValue = ((CultureInfo) System.Web.UI.DataBinder.Eval(container.DataItem, columnName)).Name;
        //
        //			//Find the value on DropDownList and select it
        //			if (langList.Items.FindByValue(currentValue) != null)
        //			{
        //				langList.Items.FindByValue(currentValue).Selected = true;
        //				System.Diagnostics.Debug.Assert(langList.SelectedItem != null);
        //			}
        //		}
    }
}