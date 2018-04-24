using System.IO;
using System.Net;
using System.Text;

namespace WindowsFormsApplication1
{
    class Report
    {

        public static string SensorUpdate(string pm25, string gatewayId, string userkey)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(
                $@"http://www.lewei50.com/api/v1/gateway/updatesensors/{gatewayId}");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers["userkey"] = userkey;

            var data = Encoding.ASCII.GetBytes($"[{{\"Name\":\"PM25\",\"Value\":\"{pm25}\"}}]");

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            using (Stream mResponseStream = response.GetResponseStream())
            {
                if (mResponseStream != null)
                {
                    StreamReader mStreamReader = new StreamReader(mResponseStream, Encoding.GetEncoding("utf-8"));
                    string returnString = mStreamReader.ReadToEnd();

                    mStreamReader.Close();
                    mResponseStream.Close();

                    return returnString;
                }
                else
                {
                    return "Error";
                }
            }
        }
    }
}
