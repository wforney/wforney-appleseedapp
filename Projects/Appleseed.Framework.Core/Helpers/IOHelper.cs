/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Coder: Mario Hartmann [mario@hartmann.net // http://mario.hartmann.net/]
 * Original version: C#
 * Original product name: Appleseed
 * Official site: http://www.Appleseedportal.net
 * Last updated Date: 28/MAY/2004
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
*/

using System.IO;

namespace Appleseed.Framework.Helpers
{
	/// <summary>
	/// Summary description for IOHelper.
	/// </summary>
	public class IOHelper
	{
		/// <summary>
		/// Returns the names of files in the specified directory that match the specified searchpatterns.
		/// </summary>
		/// <param name="path">the directory to search.</param>
		/// <param name="searchPatterns">the search strings to match against the names of files in the path deliminated by a ';'. For example:"*.gif;*.xl?;my*.txt"</param>
		/// <returns></returns>
		public static string [] GetFiles(string path,string searchPatterns)
		{
			//declare the return array
			string[] returnArray = new string[0];
			
			if (Directory.Exists(path))
			{
				//loop throuht the givven searchpatterns
				foreach (string ext in searchPatterns.Split(';'))
				{

					string [] tmpArray ;
					tmpArray = Directory.GetFiles(path,ext);
					if (tmpArray.Length >0 )
					{
						string [] newArray = new string[returnArray.Length + tmpArray.Length];
						returnArray.CopyTo(newArray,0);
						tmpArray.CopyTo(newArray,returnArray.Length);
						returnArray = newArray;
					}
				}
			}
			return returnArray;						
		}
		/// <summary>
		/// Creates all directories and subdirectories as specified and return false in case of an error.
		/// </summary>
		/// <param name="physicalPath"></param>
		/// <returns></returns>
		public static bool CreateDirectory(string physicalPath)
		{
			bool returnValue = true;

			if (!Directory.Exists(physicalPath)) 
			{
				try { Directory.CreateDirectory(physicalPath);}
				catch { returnValue = false; }
			}
			return returnValue;
		}
	}
}
