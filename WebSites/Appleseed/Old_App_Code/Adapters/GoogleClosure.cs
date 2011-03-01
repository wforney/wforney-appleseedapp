using System.IO;
using System.Net;
using System.Web;
using System.Xml;
using System;
using Appleseed.Framework;

/// <summary>
/// A C# wrapper around the Google Closure Compiler web service.
/// </summary>
public class GoogleClosure
{
    private const string PostData = "{0}output_format=xml&output_info=compiled_code&compilation_level=SIMPLE_OPTIMIZATIONS";
    private const string ApiEndpoint = "http://closure-compiler.appspot.com/compile";




    public static string CallApi(string source)
    {
        using (WebClient client = new WebClient())
        {
            client.Headers.Add("content-type", "application/x-www-form-urlencoded");
            string data = string.Format(PostData,source);
            string result = client.UploadString(ApiEndpoint, data);
            ErrorHandler.Publish(LogLevel.Debug, result);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            return doc.SelectSingleNode("//compiledCode").InnerText;
        }
    }
}