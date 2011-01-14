/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * COPYRIGHT (c) 2003 by DUEMETRI and contributors.
 * Coder: Emmanuele De Andreis
 * Original version: C#
 * Original product name: Appleseed
 * Official site: http://www.Appleseedportal.net
 * Last updated Date: 15/APR/2003
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
*/

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Appleseed.Framework.Helpers
{
    /// <summary>
    /// Nice HTMLText helper. Contains both the HTML string
    /// and a tag-free version of the same string.
    /// by Manu
    /// </summary>
    public struct HTMLText
    {
        private string _InnerHTML;
        private string _InnerXHTML; //Same as _InnerHTML but converted to XHTML
        private string _InnerText; //Same as _InnerHTML but completely tag-free 

        /// <summary>
        /// HTMLText
        /// </summary>
        /// <param name="myText">My text.</param>
        public HTMLText(string myText)
        {
            _InnerHTML = myText;
            _InnerText = string.Empty;
            _InnerXHTML = string.Empty;
        }

        /// <summary>
        /// InnerHTML
        /// </summary>
        /// <value>The inner HTML.</value>
        public string InnerHTML
        {
            get { return _InnerHTML; }
            set { _InnerHTML = value; }
        }

        /// <summary>
        /// InnerXHTML
        /// </summary>
        /// <value>The inner XHTML.</value>
        public string InnerXHTML
        {
            get
            {
                _InnerXHTML = GetXHtml(_InnerHTML);
                return _InnerXHTML;
            }
        }

        /// <summary>
        /// InnerText
        /// Same as InnerHTML but completely tag-free
        /// </summary>
        /// <value>The inner text.</value>
        public string InnerText
        {
            get
            {
                _InnerText = CleanHTML(_InnerHTML);
                return _InnerText;
            }
        }

        /// <summary>
        /// Removes any HTML tags
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private string CleanHTML(string input)
        {
            string output;
            output = Regex.Replace(input,
                                   @"(\<[^\<]*\>)", string.Empty);
            output = output.Replace("<", "&lt;");
            output = output.Replace(">", "&gt;");
            return output;
        }

        /// <summary>
        /// Returns XHTML
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private string GetXHtml(string input)
        {
            if (input == string.Empty || input == null)
                return string.Empty;

            string output;
            string regex;

            // Open tags (no attributes)
            regex = "<[A-Za-z0-9:]*[>,\\s]";
            output = Regex.Replace(input, regex, new MatchEvaluator(MatchToLower));

            // Closed tags
            regex = "</[A-Za-z0-9]*>";
            output = Regex.Replace(output, regex, new MatchEvaluator(MatchToLower));

            // Open tags (with attributes)
            regex =
                "<[a-zA-Z]+[a-zA-Z0-9:]*(\\s+[a-zA-Z]+[a-zA-Z0-9:]*((=[^\\s\"'<>]+)|(=\"[^\"]*\")|(='[^']*')|()))*\\s*\\/?\\s*>";
            output = Regex.Replace(output, regex, new MatchEvaluator(ProcessAttribute));

            // HR
            output = output.Replace("<hr>", "<hr />");

            // BR
            output = output.Replace("<br>", "<br />");
            return output;
        }

        /// <summary>
        /// Transforms the match to lowercase
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private static string MatchToLower(Match m)
        {
            return m.ToString().ToLower();
        }

        /// <summary>
        /// Quote the result
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private static string QuoteAttibute(Match m)
        {
            string str = m.ToString().Remove(0, 1).Trim();
            return "=\"" + str + "\" ";
        }

        /// <summary>
        /// Quote the result (end tag)
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private static string QuoteAttibuteEnd(Match m)
        {
            string str = m.ToString().Remove(0, 1);
            str = str.Remove(str.Length - 1, 1);
            return "=\"" + str + "\">";
        }

        /// <summary>
        /// Processes the attribute.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private static string ProcessAttribute(Match m)
        {
            string output;
            string regex;

            //Attribute value (no quote) to Quoted Attribute Value
            output = "=([^\",^\\s,.]*)[\\s]";
            regex = Regex.Replace(m.ToString(), output, new MatchEvaluator(QuoteAttibute));

            //Attribute to lowercase
            output = "\\s[^=\"]*=";
            regex = Regex.Replace(regex, output, new MatchEvaluator(MatchToLower));

            //Attribute value (no quote) to Quoted Attribute Value (end of tag)
            output = "=([^\",^\\s,.]*)[>]";
            return Regex.Replace(regex, output, new MatchEvaluator(QuoteAttibuteEnd));
        }

        /// <summary>
        /// Gets an abstract HTML of maxLenght characters maximum
        /// </summary>
        /// <param name="maxLenght">The max lenght.</param>
        /// <returns></returns>
        public string GetAbstractHTML(int maxLenght)
        {
            if (maxLenght >= InnerHTML.Length || InnerHTML == string.Empty)
            {
                return InnerHTML;
            }
            else
            {
                string abstr = InnerHTML.Substring(0, maxLenght);
                abstr = abstr.Substring(0, abstr.LastIndexOf(' ')) + "...";
                return abstr;
            }
        }

        /// <summary>
        /// Gets an abstract text (no HTML tags!) of maxLenght characters maximum
        /// </summary>
        /// <param name="maxLenght">The max lenght.</param>
        /// <returns></returns>
        public string GetAbstractText(int maxLenght)
        {
            string abstr = InnerText;
            if (maxLenght >= abstr.Length || abstr == string.Empty)
            {
                return InnerText;
            }
            else
            {
                abstr = abstr.Substring(0, maxLenght);
                int l = abstr.LastIndexOf(' ');
                l = (l > 0) ? l : abstr.Length;
                abstr = abstr.Substring(0, l) + "...";
                return abstr.Trim();
            }
        }

        /// <summary>
        /// Break the text in rows of rowLenght characters maximum
        /// using HTML content
        /// </summary>
        /// <param name="rowLenght">The row lenght.</param>
        /// <returns></returns>
        public string GetBreakedHTML(int rowLenght)
        {
            return (breakThis(InnerHTML, rowLenght));
        }

        /// <summary>
        /// Break the text in rows of rowLenght characters maximum,
        /// using text content, useful for emails
        /// </summary>
        /// <param name="rowLenght"></param>
        /// <returns></returns>
        public string GetBreakedText(int rowLenght)
        {
            return (breakThis(InnerText, rowLenght));
        }

        /// <summary>
        /// Breaks the this.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="rowLenght">The row lenght.</param>
        /// <returns></returns>
        private string breakThis(string input, int rowLenght)
        {
            //Clean up input
            char[] RemoveChar = {' ', '\n'};
            input = input.Trim(RemoveChar);

            StringBuilder s = new StringBuilder();

            string row;
            int index = 0;
            int last;
            int len = input.Length;
            int count = 0;

            while (index <= len)
            {
                if ((index + rowLenght) < len)
                    row = input.Substring(index, rowLenght);
                else
                {
                    //Last row
                    s.Append(input.Substring(index));
                    s.Append(Environment.NewLine);
                    break;
                }

                //Seach for end of line
                last = row.IndexOf('\n');
                if (last == 0)
                {
                    row = Environment.NewLine;
                }
                else if (last > 0)
                {
                    row = row.Substring(0, last);
                }
                else
                {
                    last = row.LastIndexOf(' ');
                    if (last > 0)
                    {
                        row = row.Substring(0, last) + Environment.NewLine;
                    }
                }
                s.Append(row);
                index += row.Length;

                count++;

                //Avoid loop
                if (row == string.Empty || index == len || count > len)
                    break;
            }
            //Clean up output
            return s.ToString().Trim(RemoveChar);
        }

        /// <summary>
        /// Converts the struct to a string value
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static implicit operator string(HTMLText value)
        {
            return (value.InnerHTML);
        }

        /// <summary>
        /// Converts the struct from a string value
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static implicit operator HTMLText(string value)
        {
            HTMLText h = new HTMLText(value);
            return h;
        }
    }
}