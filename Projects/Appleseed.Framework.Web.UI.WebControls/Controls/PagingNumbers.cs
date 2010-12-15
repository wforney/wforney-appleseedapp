namespace Appleseed.Framework.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    /// The paging control is used to display paging options for controls that
    /// page through data. This control is necessary since some data rendering 
    /// controls, such as the DataList, do not support paging and it must be 
    /// a custom implementation.
    /// </summary>
    // ********************************************************************/
    public class PagingNumbers : WebControl, INamingContainer, IPaging
    {
        // TODO: Upgrade to comosite control... :-)

        // Paging defaults
        private int m_pageNumber = 0;
        private int totalPages = 0;
        private const int defaultRecordCount = 0;
        private int pageSize = -1;

        // Controls
        private System.Web.UI.WebControls.Label currentPage;
        private PlaceHolder previousButton;
        private PlaceHolder nextButton;
        private LinkButton prev;
        private LinkButton next;
        private System.Web.UI.WebControls.LinkButton[] numericalLinkButtons;
        private PlaceHolder numericalPaging;

        // *********************************************************************
        //  CreateChildControls
        //
        /// <summary>
        /// This event handler adds the children controls and is resonsible
        /// for determining the template type used for the control.
        /// </summary>
        // ********************************************************************/ 
        protected override void CreateChildControls()
        {
            // If the total number of records is less than the
            // number of records we display in a page, we'll display page 1 of 1.
            if ((RecordCount <= RecordsPerPage) && (RecordCount != 0))
            {
                Controls.Add(NavigationDisplay(true));
                DisplayCurrentPage();
                return;
            }

            // Quick check to ensure the PageIndex is not greater than the Page Size
            if ((PageIndex > RecordsPerPage) || (PageIndex < 0))
            {
                PageIndex = 0;
            }

            // How many link buttons do we need?
            numericalLinkButtons = new System.Web.UI.WebControls.LinkButton[TotalPages];

            // Add the control to display navigation
            Controls.Add(NavigationDisplay(false));
        }

        // *********************************************************************
        //  DisplayPager
        //
        /// <summary>
        /// Used to display the pager. Is public so that the parent control can
        /// reset the pager when a post back occurs that the pager did not raise.
        /// </summary>
        // ********************************************************************/ 
        private void DisplayPager()
        {
            DisplayCurrentPage();
            DisplayNumericalPaging();
            DisplayPrevNext();
        }

        // *********************************************************************
        //  DisplayCurrentPage
        //
        /// <summary>
        /// Displays the current page that the user is viewing
        /// </summary>
        // ********************************************************************/ 
        private void DisplayCurrentPage()
        {
            currentPage.Text = General.GetString("PAGE", "Page", null) + " " + (PageIndex + 1).ToString("n0") + " " +
                               General.GetString("OF", "of", null) + " " + TotalPages.ToString("n0");
        }


        // *********************************************************************
        //  DisplayNumericalPaging
        //
        /// <summary>
        /// Controls how the numerical link buttons get rendered
        /// </summary>
        // ********************************************************************/ 
        private void DisplayNumericalPaging()
        {
            int itemsToDisplay = 30;
            int lowerBoundPosition = 1;
            int upperBoundPosition = (TotalPages - 1);
            System.Web.UI.WebControls.Label label;

            // Clear out the controls
            numericalPaging.Controls.Clear();

            // If we have less than 6 items we don't need the fancier paging display
            if ((upperBoundPosition + 1) < (itemsToDisplay + 3))
            {
                for (int i = 0; i < (upperBoundPosition + 1); i++)
                {
                    // Don't display a link button for the existing page
                    if (i == PageIndex)
                    {
                        label = new System.Web.UI.WebControls.Label();
                        label.CssClass = "normalTextSmallBold";
                        label.Text = "[" + (PageIndex + 1).ToString("n0") + "]";
                        numericalPaging.Controls.Add(label);
                    }
                    else
                    {
                        numericalPaging.Controls.Add(numericalLinkButtons[i]);
                    }

                    if (i + 1 != numericalLinkButtons.Length)
                    {
                        label = new System.Web.UI.WebControls.Label();
                        label.CssClass = "normalTextSmallBold";
                        label.Text = ", ";

                        numericalPaging.Controls.Add(label);
                    }
                }

                return;
            }

            // Always display the first 3 if available
            if (numericalLinkButtons.Length < itemsToDisplay)
            {
                itemsToDisplay = numericalLinkButtons.Length;
            }

            for (int i = 0; i < itemsToDisplay; i++)
            {
                numericalPaging.Controls.Add(numericalLinkButtons[i]);

                if (i + (itemsToDisplay / 2) != itemsToDisplay)
                {
                    label = new System.Web.UI.WebControls.Label();
                    label.CssClass = "normalTextSmallBold";
                    label.Text = ", ";

                    numericalPaging.Controls.Add(label);
                }
            }

            // Handle the lower end first
            if ((PageIndex - lowerBoundPosition) <= (upperBoundPosition - PageIndex))
            {
                for (int i = itemsToDisplay; i < PageIndex + 2; i++)
                {
                    label = new System.Web.UI.WebControls.Label();
                    label.CssClass = "normalTextSmallBold";
                    label.Text = ", ";

                    numericalPaging.Controls.Add(label);
                    numericalPaging.Controls.Add(numericalLinkButtons[i]);
                }
            }

            // Insert the ellipses or a trailing comma if necessary
            label = new System.Web.UI.WebControls.Label();
            label.CssClass = "normalTextSmallBold";
            if (upperBoundPosition == 3)
            {
                label.Text = ", ";
            }
            else if (upperBoundPosition >= 4)
            {
                label = new System.Web.UI.WebControls.Label();
                label.CssClass = "normalTextSmallBold";
                label.Text = " ... ";
            }
            numericalPaging.Controls.Add(label);

            // Handle the upper end
            if ((PageIndex - lowerBoundPosition) > (upperBoundPosition - PageIndex))
            {
                for (int i = PageIndex - 1; i < upperBoundPosition; i++)
                {
                    label = new System.Web.UI.WebControls.Label();
                    label.CssClass = "normalTextSmallBold";
                    label.Text = ", ";

                    if (i > PageIndex - 1)
                    {
                        numericalPaging.Controls.Add(label);
                    }

                    numericalPaging.Controls.Add(numericalLinkButtons[i]);
                }
            }

            // Always display the last 2 if available
            if ((numericalLinkButtons.Length > 3) && (TotalPages > 5))
            {
                itemsToDisplay = 2;

                for (int i = itemsToDisplay; i > 0; i--)
                {
                    numericalPaging.Controls.Add(numericalLinkButtons[(upperBoundPosition + 1) - i]);

                    if (i + 1 != itemsToDisplay)
                    {
                        System.Web.UI.WebControls.Label tmp = new System.Web.UI.WebControls.Label();
                        tmp.CssClass = "normalTextSmallBold";
                        tmp.Text = ", ";
                        numericalPaging.Controls.Add(tmp);
                    }
                }
            }
        }

        // *********************************************************************
        //  DisplayPrevNext
        //
        /// <summary>
        /// Controls how the previous next link buttons get rendered
        /// </summary>
        // ********************************************************************/ 
        private void DisplayPrevNext()
        {
            prev.CommandArgument = (PageIndex - 1).ToString();
            next.CommandArgument = (PageIndex + 1).ToString();

            // Control what gets displayed
            if ((PageIndex > 0) && ((PageIndex + 1) < TotalPages))
            {
                nextButton.Visible = true;
                previousButton.Visible = true;
            }
            else if (PageIndex == 0)
            {
                nextButton.Visible = true;
                previousButton.Visible = false;
            }
            else if ((PageIndex + 1) == TotalPages)
            {
                nextButton.Visible = false;
                previousButton.Visible = true;
            }
        }

        // *********************************************************************
        //  NavigationDisplay
        //
        /// <summary>
        /// Control that contains all the navigation display details.
        /// </summary>
        /// <param name="singlePage">if set to <c>true</c> [single page].</param>
        /// <returns>The navigation control</returns>
        // ********************************************************************/ 
        private Control NavigationDisplay(bool singlePage)
        {
            Table table;
            TableRow tr;
            TableCell td;
            System.Web.UI.WebControls.Label navigation;
            Label navigationText;

            // Create a new table
            table = new Table();
            table.CellPadding = 0;
            table.CellSpacing = 0;
            table.Width = Unit.Percentage(100);

            // We only have a single row
            tr = new TableRow();

            // Two columns. One for the current page and one for navigation
            td = new TableCell();

            // Display the current page
            td.Controls.Add(CreateCurrentPage());
            tr.Controls.Add(td);

            // Do we have multiple pages to display?
            if (!singlePage)
            {
                // Create page navigation
                td = new TableCell();
                td.HorizontalAlign = HorizontalAlign.Right;
                navigation = new System.Web.UI.WebControls.Label();
                navigationText = new Label();
                navigationText.CssClass = "normalTextSmallBold";
                navigationText.TextKey = "GOTO_PAGE";
                navigationText.Text = "Goto to page: ";

                navigation.Controls.Add(navigationText);

                // Numerical Paging
                navigation.Controls.Add(CreateNumericalNavigation());

                // Prev Next Paging
                navigation.Controls.Add(CreatePrevNextNavigation());

                td.Controls.Add(navigation);
                tr.Controls.Add(td);
            }

            table.Controls.Add(tr);

            return table;
        }

        // *********************************************************************
        //  CreateCurrentPage
        //
        /// <summary>
        /// Display the page n of n+1 text
        /// </summary>
        /// <returns>the control</returns>
        // ********************************************************************/ 
        private Control CreateCurrentPage()
        {
            currentPage = new System.Web.UI.WebControls.Label();

            currentPage.CssClass = "normalTextSmallBold";

            return currentPage;
        }

        // *********************************************************************
        //  CreateNumericalNavigation
        //
        /// <summary>
        /// Creates numerical navigation link buttons
        /// </summary>
        /// <returns>A Control woth the navigation links</returns>
        // ********************************************************************/ 
        private Control CreateNumericalNavigation()
        {
            numericalPaging = new PlaceHolder();
            int linkButtonsToCreate;

            if (TotalPages > numericalLinkButtons.Length)
            {
                linkButtonsToCreate = numericalLinkButtons.Length;
            }
            else
            {
                linkButtonsToCreate = TotalPages;
            }

            // Create all the link buttons
            for (int i = 0; i < linkButtonsToCreate; i++)
            {
                numericalLinkButtons[i] = new System.Web.UI.WebControls.LinkButton();
                numericalLinkButtons[i].CssClass = "normalTextSmallBold";
                numericalLinkButtons[i].Click += new EventHandler(PageIndex_Click);
                numericalLinkButtons[i].Text = (i + 1).ToString("n0");
                numericalLinkButtons[i].CommandArgument = i.ToString();
                numericalPaging.Controls.Add(numericalLinkButtons[i]);
            }

            return numericalPaging;
        }

        // *********************************************************************
        //  CreatePrevNextNavigation
        //
        /// <summary>
        /// Creates previous/next navigation link buttons
        /// </summary>
        /// <returns>The Control</returns>
        // ********************************************************************/ 
        private Control CreatePrevNextNavigation()
        {
            PlaceHolder prevNext = new PlaceHolder();
            System.Web.UI.WebControls.Label whitespace;

            // Create the previous button
            previousButton = new PlaceHolder();
            whitespace = new System.Web.UI.WebControls.Label();
            whitespace.CssClass = "normalTextSmallBold";
            whitespace.Text = "&nbsp;";
            prev = new LinkButton();
            prev.CssClass = "normalTextSmallBold";
            prev.TextKey = "PREVIOUS";
            prev.Text = "Prev";
            prev.ID = "Prev";
            prev.Click += new EventHandler(PrevNext_Click);
            previousButton.Controls.Add(whitespace);
            previousButton.Controls.Add(prev);
            prevNext.Controls.Add(previousButton);

            // Create the next button
            nextButton = new PlaceHolder();
            whitespace = new System.Web.UI.WebControls.Label();
            whitespace.CssClass = "normalTextSmallBold";
            whitespace.Text = "&nbsp;";
            next = new LinkButton();
            next.CssClass = "normalTextSmallBold";
            next.TextKey = "NEXT";
            next.Text = "Next";
            next.ID = "Next";
            next.Click += new EventHandler(PrevNext_Click);
            nextButton.Controls.Add(whitespace);
            nextButton.Controls.Add(next);
            prevNext.Controls.Add(nextButton);

            return prevNext;
        }

        // *********************************************************************
        //  PageIndex_Changed
        //
        /// <summary>
        /// Event raised when a an index has been selected by the end user
        /// </summary>
        // ********************************************************************/
        public event EventHandler OnMove;

        // *********************************************************************
        //  PageIndex_Click
        //
        /// <summary>
        /// Event raised when a new index is selected from the paging control
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        // ********************************************************************/
        private void PageIndex_Click(Object sender, EventArgs e)
        {
            string requestedPage = ((System.Web.UI.WebControls.LinkButton)sender).CommandArgument;

            if (requestedPage.Length > 0)
            {
                PageIndex = Convert.ToInt32(requestedPage);

                if (null != OnMove)
                {
                    OnMove(sender, e);
                }
            }
        }

        // *********************************************************************
        //  PrevNext_Click
        //
        /// <summary>
        /// Event raised when a new index is selected from the paging control
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        // ********************************************************************/
        private void PrevNext_Click(Object sender, EventArgs e)
        {
            string requestedPage = ((System.Web.UI.WebControls.LinkButton)sender).CommandArgument;

            if (requestedPage.Length > 0)
            {
                PageIndex = Convert.ToInt32(requestedPage);

                if (null != OnMove)
                {
                    OnMove(sender, e);
                }
            }
        }

        // *********************************************************************
        //  OnPreRender
        //
        /// <summary>
        /// Override OnPreRender and databind
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        // ********************************************************************/ 
        protected override void OnPreRender(EventArgs e)
        {
            // If the total number of records is less than the
            // number of records we display in a page, we'll simply
            // return.
            if (RecordCount <= RecordsPerPage)
            {
                return;
            }

            // Control what gets displayed
            DisplayPager();
        }

        /// <summary>
        /// Gets or sets the page size used in paging.
        /// </summary>
        /// <value>The records per page.</value>
        [
            Category("Required"),
                Description("Specifies the page size used in paging.")
            ]
        public int RecordsPerPage
        {
            get
            {
                if (pageSize == -1)
                    return 10; //default

                return pageSize;
            }
            set { pageSize = value; }
        }

        /// <summary>
        /// Gets or sets the current page in the index.
        /// </summary>
        /// <value>The index of the page.</value>
        [
            Description("Specifies the current page in the index.")
            ]
        private int PageIndex
        {
            get
            {
                if (ViewState["PageIndex"] == null)
                    return m_pageNumber;

                return Convert.ToInt32(ViewState["PageIndex"]);
            }
            set { ViewState["PageIndex"] = value; }
        }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        /// <value>The page number.</value>
        [
            Description("Specifies the current page in the index.")
            ]
        public int PageNumber
        {
            get
            {
                //Internal is 0 based, external is 1 based
                return PageIndex + 1;
            }
            set { PageIndex = value - 1; }
        }

        /// <summary>
        /// Gets or sets the Forum's posts you want to view.
        /// </summary>
        /// <value>The total pages.</value>
        public int TotalPages
        {
            get { return totalPages + 1; }
            set { totalPages = value - 1; }
        }

        /// <summary>
        /// Gets or sets the Record Count
        /// </summary>
        /// <value>The record count.</value>
        public int RecordCount
        {
            get
            {
                // the RecordCount is stuffed in the ViewState so that
                // it is persisted across postbacks.
                if (ViewState["totalRecords"] == null)
                    return defaultRecordCount; // if it's not found in the ViewState, return the default value

                return Convert.ToInt32(ViewState["totalRecords"].ToString());
            }
            set
            {
                TotalPages = CalculateTotalPages(value, RecordsPerPage);

                // set the viewstate
                ViewState["totalRecords"] = value;
            }
        }

        // *********************************************************************
        //  CalculateTotalPages
        //
        /// <summary>
        /// Static that caculates the total pages available.
        /// </summary>
        /// <param name="totalRecords">The total records.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>Total number of pages available</returns>
        // ********************************************************************/
        public static int CalculateTotalPages(int totalRecords, int pageSize)
        {
            int totalPagesAvailable;

            // First calculate the division
            totalPagesAvailable = totalRecords / pageSize;

            // Now do a mod for any remainder
            if ((totalRecords % pageSize) > 0)
                totalPagesAvailable++;

            return totalPagesAvailable;
        }
    }
}