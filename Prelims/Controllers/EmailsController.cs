

using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Mvc;

namespace Prelims.Controllers
{
    public class EmailsController : Controller
    {
        [HttpGet]
        public dynamic ReadMails()
        {
            string responseText = "[]";

            try
            {

                var userName = ConfigurationManager.AppSettings.Get("EMAIL-USERNAME").ToString();
                var password = ConfigurationManager.AppSettings.Get("EMAIL-PASSWORD").ToString();

                string url = ConfigurationManager.AppSettings.Get("EMAIL-AUTHURL").ToString();

                // Create a request using a URL that can receive a post.   
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "&name=" + userName);
                // Set the Method property of the request to POST. 
                request.CookieContainer = new CookieContainer();
                request.Method = "POST";

                // Create POST data and convert it to a byte array.  
                string postData = string.Format("password={0}", password);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                // Set the ContentType property of the WebRequest.  
                request.ContentType = "application/x-www-form-urlencoded";
                // Set the ContentLength property of the WebRequest.  
                request.ContentLength = byteArray.Length;
                // Get the request stream.  
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.  
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.  
                dataStream.Close();
                // Get the response.  
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // Display the status.  
                //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.  
                dataStream = response.GetResponseStream();

                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();
                // Display the content.  
                // Clean up the streams.  

                var jsonObject = JsonConvert.DeserializeObject<AuthResponse>(responseFromServer);
                reader.Close();
                dataStream.Close();
                response.Close();

                if (jsonObject != null)
                {
                    string getRequestUrl = string.Format("http://webmail.ftbpo.com/ajax/mail?action=all&session={0}&folder=Inbox&columns=603,604,607,615,610,600&limit=100&order=desc", jsonObject.session);
                    HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(getRequestUrl);

                    getRequest.CookieContainer = new CookieContainer();

                    foreach (Cookie cookie in response.Cookies)
                    {
                        getRequest.CookieContainer.Add(cookie);
                    }

                    Session.Add("EMAIL-COOKIES", response.Cookies);
                    Session.Add("EMAIL-SESSIONID", jsonObject.session);

                    WebResponse getResponse = getRequest.GetResponse();

                    Stream getDataStream = getResponse.GetResponseStream();
                    // Open the stream using a StreamReader for easy access.  
                    StreamReader getReader = new StreamReader(getDataStream);
                    // Read the content.  
                    responseText = getReader.ReadToEnd();
                    getReader.Close();
                    getResponse.Close();
                }
            }
            catch (Exception exe) { }

            return responseText;
        }

        [HttpGet]
        public dynamic GetEmailDetails(int id)
        {
            string responseText = string.Empty;
            string sessionId = Session["EMAIL-SESSIONID"].ToString();

            string getRequestUrl = string.Format("http://webmail.ftbpo.com/ajax/mail?action=get&session={0}&folder=Inbox&id={1}", sessionId, id);
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(getRequestUrl);

            getRequest.CookieContainer = new CookieContainer();

            var cookies = (CookieCollection)Session["EMAIL-COOKIES"];

            foreach (Cookie cookie in cookies)
            {
                getRequest.CookieContainer.Add(cookie);
            }

            WebResponse getResponse = getRequest.GetResponse();

            Stream getDataStream = getResponse.GetResponseStream();
            // Open the stream using a StreamReader for easy access.  
            StreamReader getReader = new StreamReader(getDataStream);
            // Read the content.  
            responseText = getReader.ReadToEnd();

            //Replace URLs

            responseText = responseText.Replace(@"/ajax/image/mail/picture", "http://webmail.ftbpo.com/ajax/image/mail/picture");

            getReader.Close();
            getResponse.Close();

            return responseText;
        }
    }

    
    //{"session":"0bdfa3575e1b4701a8b96688218ba419","user":"si@fnfsocal.com","user_id":7,"context_id":30457287,"locale":"en_US"}

    public class AuthResponse
    {
        public string session { get; set; }

        public string user { get; set; }
    }

}
