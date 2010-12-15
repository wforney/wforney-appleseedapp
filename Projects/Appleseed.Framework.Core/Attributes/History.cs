using System;

namespace Appleseed.Framework
{
    /// <summary>
    /// History can be used to track code authors and modification
    /// </summary>
    [AttributeUsage(
        AttributeTargets.All, 
        Inherited = false,
        AllowMultiple = true
        )]
    public class History : Attribute 
    {
        //Private fields.
        private string name;
        private string email;
        private string version;
        private DateTime date;
        private string comment;

        /// <summary>
        /// Requires all parameters
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="email">email</param>
        /// <param name="version">version</param>
        /// <param name="date">date</param>
        /// <param name="comment">comment</param>
        public History(string name, string email, string version, string date, string comment)
        {
            this.Name = name;
            this.Email = email; 
            this.Version = version;
            this.Date = DateTime.Parse(date);
            this.Comment = comment;
        }

        /// <summary>
        /// Requires name, date and comment
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="date">date</param>
        /// <param name="comment">comment</param>
        public History(string name, string date, string comment)
        {
            this.Name = name;
			try
			{
				this.Date = DateTime.Parse(date);
			}
			catch(FormatException ex)
			{
				throw new Exception("'" + date + "' is an invalid date", ex);
			}
            this.Comment = comment;
        }

        /// <summary>
        /// Requires name, date and comment
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="date">date</param>
        public History(string name, string date)
        {
            this.Name = name;
            this.Date = DateTime.Parse(date);
        }

        /// <summary>
        /// The Author name
        /// </summary>
        public virtual string Name
        {
            get {return name;}
            set {name = value;}
        }

        /// <summary>
        /// The Email of the Author
        /// </summary>
        public virtual string Email
        {
            get {return email;}
            set {email = value;}
        }

        /// <summary>
        /// Modification Version
        /// </summary>
        public virtual string Version
        {
            get {return version;}
            set {version = value;}
        }

        /// <summary>
        /// Modification date
        /// </summary>
        public virtual DateTime Date
        {
            get {return date;}
            set {date = value;}
        }

        /// <summary>
        /// Modification description
        /// </summary>
        public virtual string Comment
        {
            get {return comment;}
            set {comment = value;}
        }
    }
}