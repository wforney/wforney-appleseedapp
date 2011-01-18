using System;
using System.IO;
using System.Text;
using System.Web;
using Appleseed.Framework.Site.Configuration;

namespace Appleseed.Framework.Helpers
{
    /// <summary>
    /// CssHelper object (Jes1111)
    /// </summary>
    public class CssHelper
    {
        /// <summary>
        ///     
        /// </summary>
        private const bool ALLOW_IMPORTS = true;

        /// <summary>
        ///     
        /// </summary>
        private const bool INCLUDE_COMMENTS = true;

        /// <summary>
        ///     
        /// </summary>
        private const bool PARSE_IMPORTS = true;

        /// <summary>
        ///     
        /// </summary>
        private const string SELECTOR_PREFIX = "";

        /// <summary>
        ///     
        /// </summary>
        private PortalSettings portalSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CssHelper"/> class.
        /// </summary>
        /// <returns>
        /// A void value...
        /// </returns>
        public CssHelper()
        {
            if (HttpContext.Current != null)
            {
                portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
            }
        }

        /// <summary>
        /// Parses the CSS.
        /// </summary>
        /// <param name="cssFileName">Name of the CSS file.</param>
        /// <returns>A string value...</returns>
        public string ParseCss(string cssFileName)
        {
            return ParseCss(cssFileName, SELECTOR_PREFIX);
        }

        /// <summary>
        /// Parses the CSS.
        /// </summary>
        /// <param name="cssFileName">Name of the CSS file.</param>
        /// <param name="selectorPrefix">The selector prefix.</param>
        /// <param name="allowImports">if set to <c>true</c> [allow imports].</param>
        /// <param name="parseImports">if set to <c>true</c> [parse imports].</param>
        /// <param name="includeComments">if set to <c>true</c> [include comments].</param>
        /// <returns>A string value...</returns>
        public string ParseCss(string cssFileName, string selectorPrefix, bool allowImports, bool parseImports,
                               bool includeComments)
        {
            return (ParseCss(cssFileName, selectorPrefix, allowImports, parseImports, includeComments, null));
        }

        /// <summary>
        /// Parses the CSS.
        /// </summary>
        /// <param name="cssFileName">Name of the CSS file.</param>
        /// <param name="selectorPrefix">The selector prefix.</param>
        /// <param name="allowImports">if set to <c>true</c> [allow imports].</param>
        /// <param name="parseImports">if set to <c>true</c> [parse imports].</param>
        /// <param name="includeComments">if set to <c>true</c> [include comments].</param>
        /// <param name="sb">The sb.</param>
        /// <returns>A string value...</returns>
        public string ParseCss(string cssFileName, string selectorPrefix, bool allowImports, bool parseImports,
                               bool includeComments, StringBuilder sb)
        {
            using (StreamReader sr = new StreamReader(cssFileName))
            {
                StringTokenizer st = new StringTokenizer(sr);

                if (sb == null)
                    sb = new StringBuilder();
                Token token;

                try
                {
                    do
                    {
                        token = st.Next();

                        switch (token.Kind)
                        {
                            case TokenKind.Comment:

                                if (includeComments)
                                {
                                    sb.Append(token.Value);
                                    //sb.Append("\n");
                                }
                                break;

                            case TokenKind.Selector:

                                if (selectorPrefix == string.Empty)
                                    sb.Append(token.Value);

                                else
                                {
                                    sb.Append(selectorPrefix);
                                    sb.Append(" ");
                                    sb.Append(token.Value);
                                }
                                break;

                            case TokenKind.AtRule:

                            case TokenKind.Block:
                                sb.Append(token.Value);
                                break;

                            case TokenKind.ImportRule:

                                if (allowImports && parseImports)
                                {
                                    // temp
                                    //sb.Append(token.Value);
                                    string _filename = token.Value.Replace("@import", string.Empty);
                                    _filename = _filename.Replace("url", string.Empty);
                                    _filename = _filename.Replace("(", string.Empty);
                                    _filename = _filename.Replace(")", string.Empty);
                                    _filename = _filename.Replace("'", string.Empty);
                                    _filename = _filename.Replace("\"", string.Empty);
                                    _filename = _filename.Replace(";", string.Empty).Trim();
                                    _filename =
                                        string.Concat(cssFileName.Substring(0, cssFileName.LastIndexOf(@"\")).Trim(),
                                                      "\\", _filename);
                                    CssHelper _loop = new CssHelper();
                                    _loop.ParseCss(_filename, selectorPrefix, allowImports, parseImports,
                                                   includeComments, sb);
                                }

                                else if (allowImports && !parseImports)
                                {
                                    sb.Append(token.Value);
                                }
                                break;
                            default:
                                sb.Append(token.Value);
                                break;
                        }
                    } while (token.Kind != TokenKind.EOF);
                }

                catch (Exception ex)
                {
                    ErrorHandler.Publish(LogLevel.Error,
                                         "Error in parsing CSS file: " + cssFileName + " Message was: " + ex.Message);
                }

                finally
                {
                    sr.Close();
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Parses the CSS.
        /// </summary>
        /// <param name="cssFileName">Name of the CSS file.</param>
        /// <param name="selectorPrefix">The selector prefix.</param>
        /// <param name="allowImports">if set to <c>true</c> [allow imports].</param>
        /// <param name="parseImports">if set to <c>true</c> [parse imports].</param>
        /// <returns>A string value...</returns>
        public string ParseCss(string cssFileName, string selectorPrefix, bool allowImports, bool parseImports)
        {
            return (ParseCss(cssFileName, selectorPrefix, allowImports, parseImports, INCLUDE_COMMENTS));
        }

        /// <summary>
        /// Parses the CSS.
        /// </summary>
        /// <param name="cssFileName">Name of the CSS file.</param>
        /// <param name="selectorPrefix">The selector prefix.</param>
        /// <param name="allowImports">if set to <c>true</c> [allow imports].</param>
        /// <returns>A string value...</returns>
        public string ParseCss(string cssFileName, string selectorPrefix, bool allowImports)
        {
            return ParseCss(cssFileName, selectorPrefix, allowImports, PARSE_IMPORTS);
        }

        /// <summary>
        /// Parses the CSS.
        /// </summary>
        /// <param name="cssFileName">Name of the CSS file.</param>
        /// <param name="selectorPrefix">The selector prefix.</param>
        /// <returns>A string value...</returns>
        public string ParseCss(string cssFileName, string selectorPrefix)
        {
            return ParseCss(cssFileName, selectorPrefix, ALLOW_IMPORTS);
        }
    }

    /// <summary>
    /// StringTokenizer tokenized string (or stream) into tokens.
    /// 
    /// ********************************************************
    /// *	Author: Andrew Deren
    /// *	Date: July, 2004
    /// *	http://www.adersoftware.com
    /// * 
    /// *	StringTokenizer class. You can use this class in any way you want
    /// * as long as this header remains in this file.
    /// * 
    /// **********************************************************
    /// </summary>
    /// <remarks>
    /// modified by Jes1111 to be specific to CSS
    /// </remarks>
    public class StringTokenizer
    {
        private const char EOF = (char) 0;
        private int column;
        private string data;
        private bool ignoreWhiteSpace;
        private int line;
        private int pos; // position within data
        private int saveCol;
        private int saveLine;
        private int savePos;
        private char[] symbolChars;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:StringTokenizer"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>
        /// A void value...
        /// </returns>
        public StringTokenizer(string data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            this.data = data;
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:StringTokenizer"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>
        /// A void value...
        /// </returns>
        public StringTokenizer(StreamReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            data = reader.ReadToEnd();
            Reset();
        }

        /// <summary>
        /// Nexts this instance.
        /// </summary>
        /// <returns>
        /// A Appleseed.Framework.Helpers.Token value...
        /// </returns>
        public Token Next()
        {
            ReadToken:
            char ch = LA(0);

            switch (ch)
            {
                case EOF:
                    return CreateToken(TokenKind.EOF, string.Empty);

                case ' ':

                case '\t':
                    {
                        if (ignoreWhiteSpace)
                        {
                            Consume();
                            goto ReadToken;
                        }

                        else
                            return ReadWhitespace();
                    }

                case '\r':
                    {
                        StartRead();
                        Consume();

                        if (LA(0) == '\n')
                            Consume(); // on DOS/Windows we have \r\n for new line
                        line++;
                        column = 1;
                        return CreateToken(TokenKind.EOL);
                    }

                case '\n':
                    {
                        StartRead();
                        Consume();
                        line++;
                        column = 1;
                        return CreateToken(TokenKind.EOL);
                    }

                case '@':
                    {
                        if (LA(1) == 'i' || LA(1) == 'I') // import rule
                            return ReadImportRule();

                        else
                            return ReadAtRule();
                    }

                case '/':
                    {
                        if (LA(1) == '*') // comment
                            return ReadComment();

                        else
                        {
                            Consume();
                            return CreateToken(TokenKind.Symbol);
                        }
                    }

                case '{': // block
                    {
                        return ReadBlock();
                    }
                default: // selector
                    {
                        return ReadSelector();
                        //					if (Char.IsLetter(ch) || ch == '_')
                        //						return ReadWord();
                        //					else if (IsSymbol(ch))
                        //					{
                        //						StartRead();
                        //						Consume();
                        //						return CreateToken(TokenKind.Symbol);
                        //					}
                        //					else
                        //					{
                        //						StartRead();
                        //						Consume();
                        //						return CreateToken(TokenKind.Unknown);						
                        //					}
                    }
            }
        }

        /// <summary>
        /// Consumes this instance.
        /// </summary>
        /// <returns>A char value...</returns>
        protected char Consume()
        {
            char ret = data[pos];
            pos++;
            column++;
            return ret;
        }

        /// <summary>
        /// Creates the token.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <returns>
        /// A Appleseed.Framework.Helpers.Token value...
        /// </returns>
        protected Token CreateToken(TokenKind kind)
        {
            string tokenData = data.Substring(savePos, pos - savePos);
            return new Token(kind, tokenData, saveLine, saveCol);
        }

        /// <summary>
        /// Creates the token.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A Appleseed.Framework.Helpers.Token value...
        /// </returns>
        protected Token CreateToken(TokenKind kind, string value)
        {
            return new Token(kind, value, line, column);
        }

        /// <summary>
        /// checks whether c is a symbol character.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>
        /// 	<c>true</c> if the specified c is symbol; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsSymbol(char c)
        {
            for (int i = 0; i < symbolChars.Length; i++)

                if (symbolChars[i] == c)
                    return true;
            return false;
        }

        /// <summary>
        /// LAs the specified count.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>A char value...</returns>
        protected char LA(int count)
        {
            if (pos + count >= data.Length)
                return EOF;

            else
                return data[pos + count];
        }

        /// <summary>
        /// Reads at rule.
        /// </summary>
        /// <returns>
        /// A Appleseed.Framework.Helpers.Token value...
        /// </returns>
        protected Token ReadAtRule()
        {
            StartRead();
            Consume(); // read '@'

            while (true)
            {
                char ch = LA(0);

                if (ch == EOF)
                    break;

                else if (ch == '\r') // handle CR in strings
                {
                    Consume();

                    if (LA(0) == '\n') // for DOS & windows
                        Consume();
                    line++;
                    column = 1;
                }

                else if (ch == '\n') // new line in quoted string
                {
                    Consume();
                    line++;
                    column = 1;
                }

                else if (ch == ';')
                {
                    Consume(); // read ';'
                    break;
                }

                else if (ch == '{')
                    break;

                else
                    Consume();
            }
            return CreateToken(TokenKind.AtRule);
        }

        /// <summary>
        /// Reads the block.
        /// </summary>
        /// <returns>
        /// A Appleseed.Framework.Helpers.Token value...
        /// </returns>
        protected Token ReadBlock()
        {
            StartRead();
            Consume(); // read '{'

            while (true)
            {
                char ch = LA(0);

                if (ch == EOF)
                    break;

                else if (ch == '\r') // handle CR in strings
                {
                    Consume();

                    if (LA(0) == '\n') // for DOS & windows
                        Consume();
                    line++;
                    column = 1;
                }

                else if (ch == '\n') // new line in quoted string
                {
                    Consume();
                    line++;
                    column = 1;
                }

                else if (ch == '}')
                {
                    Consume();
                    break;
                }

                else
                    Consume();
            }
            return CreateToken(TokenKind.Block);
        }

        /// <summary>
        /// Reads the comment.
        /// </summary>
        /// <returns>
        /// A Appleseed.Framework.Helpers.Token value...
        /// </returns>
        protected Token ReadComment()
        {
            StartRead();
            Consume(); // read '/'
            Consume(); // read '*'

            while (true)
            {
                char ch = LA(0);

                if (ch == EOF)
                    break;

                else if (ch == '\r') // handle CR in strings
                {
                    Consume();

                    if (LA(0) == '\n') // for DOS & windows
                        Consume();
                    line++;
                    column = 1;
                }

                else if (ch == '\n') // new line in quoted string
                {
                    Consume();
                    line++;
                    column = 1;
                }

                else if (ch == '*' && LA(1) == '/')
                {
                    Consume(); // read '*'
                    Consume(); // read '/'
                    break;
                }

                else
                    Consume();
            }
            return CreateToken(TokenKind.Comment);
        }

        /// <summary>
        /// Reads the import rule.
        /// </summary>
        /// <returns>
        /// A Appleseed.Framework.Helpers.Token value...
        /// </returns>
        protected Token ReadImportRule()
        {
            StartRead();
            Consume(); // read '@'

            while (true)
            {
                char ch = LA(0);

                if (ch == EOF)
                    break;

                else if (ch == '\r') // handle CR in strings
                {
                    Consume();

                    if (LA(0) == '\n') // for DOS & windows
                        Consume();
                    line++;
                    column = 1;
                }

                else if (ch == '\n') // new line in quoted string
                {
                    Consume();
                    line++;
                    column = 1;
                }

                else if (ch == ';')
                {
                    Consume(); // read ';'
                    break;
                }

                else
                    Consume();
            }
            return CreateToken(TokenKind.ImportRule);
        }

        /// <summary>
        /// reads number. Number is: DIGIT+ ("." DIGIT*)?
        /// </summary>
        /// <returns></returns>
        protected Token ReadNumber()
        {
            StartRead();
            bool hadDot = false;
            Consume(); // read first digit

            while (true)
            {
                char ch = LA(0);

                if (Char.IsDigit(ch))
                    Consume();

                else if (ch == '.' && !hadDot)
                {
                    hadDot = true;
                    Consume();
                }

                else
                    break;
            }
            return CreateToken(TokenKind.Number);
        }

        /// <summary>
        /// Reads the selector.
        /// </summary>
        /// <returns>
        /// A Appleseed.Framework.Helpers.Token value...
        /// </returns>
        protected Token ReadSelector()
        {
            StartRead();

            while (true)
            {
                char ch = LA(0);

                if (ch == EOF)
                {
                    return CreateToken(TokenKind.Error); // shouldn't encounter this
                }

                else if (ch == '\r') // handle CR in strings
                {
                    Consume();

                    if (LA(0) == '\n') // for DOS & windows
                        Consume();
                    line++;
                    column = 1;
                }

                else if (ch == '\n') // new line in quoted string
                {
                    Consume();
                    line++;
                    column = 1;
                }

                else if (ch == ';')
                {
                    Consume(); // read ';' - shouldn't encounter this
                    return CreateToken(TokenKind.Error);
                }

                else if (ch == '{')
                    break;

                else
                    Consume();
            }
            return CreateToken(TokenKind.Selector);
        }

        /// <summary>
        /// reads all characters until next " is found.
        /// If string.Empty (2 quotes) are found, then they are consumed as
        /// part of the string
        /// </summary>
        /// <returns></returns>
        protected Token ReadString()
        {
            StartRead();
            Consume(); // read "

            while (true)
            {
                char ch = LA(0);

                if (ch == EOF)
                    break;

                else if (ch == '\r') // handle CR in strings
                {
                    Consume();

                    if (LA(0) == '\n') // for DOS & windows
                        Consume();
                    line++;
                    column = 1;
                }

                else if (ch == '\n') // new line in quoted string
                {
                    Consume();
                    line++;
                    column = 1;
                }

                else if (ch == '"')
                {
                    Consume();

                    if (LA(0) != '"')
                        break; // done reading, and this quotes does not have escape character

                    else
                        Consume(); // consume second ", because first was just an escape
                }

                else
                    Consume();
            }
            return CreateToken(TokenKind.QuotedString);
        }

        /// <summary>
        /// reads all whitespace characters (does not include newline)
        /// </summary>
        /// <returns></returns>
        protected Token ReadWhitespace()
        {
            StartRead();
            Consume(); // consume the looked-ahead whitespace char

            while (true)
            {
                char ch = LA(0);

                if (ch == '\t' || ch == ' ')
                    Consume();

                else
                    break;
            }
            return CreateToken(TokenKind.WhiteSpace);
        }

        /// <summary>
        /// reads word. Word contains any alpha character or _
        /// </summary>
        /// <returns></returns>
        protected Token ReadWord()
        {
            StartRead();
            Consume(); // consume first character of the word

            while (true)
            {
                char ch = LA(0);

                if (Char.IsLetter(ch) || ch == '_')
                    Consume();

                else
                    break;
            }
            return CreateToken(TokenKind.Word);
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        private void Reset()
        {
            ignoreWhiteSpace = false;
            symbolChars =
                new char[]
                    {
                        '=', '+', '-', '/', ',', '.', '*', '~', '!', '@', '#', '$', '%', '^', '&', '(', ')', '{', '}', '[',
                        ']', ':', ';', '<', '>', '?', '|', '\\'
                    };
            line = 1;
            column = 1;
            pos = 0;
        }

        /// <summary>
        /// save read point positions so that CreateToken can use those
        /// </summary>
        private void StartRead()
        {
            saveLine = line;
            saveCol = column;
            savePos = pos;
        }

        /// <summary>
        /// if set to true, white space characters will be ignored,
        /// but EOL and whitespace inside of string will still be tokenized
        /// </summary>
        /// <value><c>true</c> if [ignore white space]; otherwise, <c>false</c>.</value>
        public bool IgnoreWhiteSpace
        {
            get { return ignoreWhiteSpace; }
            set { ignoreWhiteSpace = value; }
        }

        /// <summary>
        /// gets or sets which characters are part of TokenKind.Symbol
        /// </summary>
        public char[] SymbolChars
        {
            get { return symbolChars; }
            set { symbolChars = value; }
        }
    }

    /// <summary>
    ///     ********************************************************
    ///     *	Author: Andrew Deren
    ///     *	Date: July, 2004
    ///     *	http://www.adersoftware.com
    ///     * 
    ///     *	StringTokenizer class. You can use this class in any way you want
    ///     * as long as this header remains in this file.
    ///     * 
    ///     **********************************************************
    /// </summary>
    /// <remarks>
    ///     modified by Jes1111 to be specific to CSS
    /// </remarks>
    public enum TokenKind
    {
        /// <summary>
        ///     
        /// </summary>
        Unknown,
        /// <summary>
        ///     
        /// </summary>
        Word,
        /// <summary>
        ///     
        /// </summary>
        Number,
        /// <summary>
        ///     
        /// </summary>
        QuotedString,
        /// <summary>
        ///     
        /// </summary>
        WhiteSpace,
        /// <summary>
        ///     
        /// </summary>
        Symbol,
        /// <summary>
        ///     
        /// </summary>
        EOL,
        /// <summary>
        ///     
        /// </summary>
        EOF,
        /// <summary>
        ///     
        /// </summary>
        Comment,
        /// <summary>
        ///     
        /// </summary>
        Selector,
        /// <summary>
        ///     
        /// </summary>
        Block,
        /// <summary>
        ///     
        /// </summary>
        AtRule,
        /// <summary>
        ///     
        /// </summary>
        ImportRule,
        /// <summary>
        ///     
        /// </summary>
        Error
    }

    /// <summary>
    ///     
    /// </summary>
    /// <remarks>
    ///     
    /// </remarks>
    public class Token
    {
        /// <summary>
        ///     
        /// </summary>
        private int column;

        /// <summary>
        ///     
        /// </summary>
        private TokenKind kind;

        /// <summary>
        ///     
        /// </summary>
        private int line;

        /// <summary>
        ///     
        /// </summary>
        private string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Token"/> class.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="value">The value.</param>
        /// <param name="line">The line.</param>
        /// <param name="column">The column.</param>
        /// <returns>
        /// A void value...
        /// </returns>
        public Token(TokenKind kind, string value, int line, int column)
        {
            this.kind = kind;
            this.value = value;
            this.line = line;
            this.column = column;
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>The column.</value>
        /// <remarks>
        /// </remarks>
        public int Column
        {
            get { return column; }
        }

        /// <summary>
        /// Gets the kind.
        /// </summary>
        /// <value>The kind.</value>
        /// <remarks>
        /// </remarks>
        public TokenKind Kind
        {
            get { return kind; }
        }

        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <value>The line.</value>
        /// <remarks>
        /// </remarks>
        public int Line
        {
            get { return line; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        /// <remarks>
        /// </remarks>
        public string Value
        {
            get { return value; }
        }
    }
}