using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;

namespace SmartAdminMvc.Models.EWAY
{
    public class PinDistanceApiCall
    {
        protected PinDistanceApiCall()
        {
            //
        }

        public static void GetDistance(long pin1, int pin2,out int pin)
        {
            PinDistance pn = new PinDistance();

            string url = String.Format("https://maps.googleapis.com/maps/api/distancematrix/xml?units=imperial&origins={0}&destinations={1}&key=AIzaSyAApz3ROoSsGV-9E86XOrH0_youG_g_go8", pin1, pin2);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader sreader = new StreamReader(dataStream);
            string responsereader = sreader.ReadToEnd();
            response.Close();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(responsereader);

            if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OK")
            {
                XmlNodeList distance = xmldoc.GetElementsByTagName("distance");
                if (xmldoc.GetElementsByTagName("origin_address").Count > 0)
                {
                    pn.origin = xmldoc.GetElementsByTagName("origin_address")[0].InnerText;
                }
                if (xmldoc.GetElementsByTagName("destination_address").Count > 0)

                {
                    pn.destination = xmldoc.GetElementsByTagName("destination_address")[0].InnerText;

                }
                if (distance.Count > 0)
                {
                    var text = distance[0].ChildNodes[1].InnerText;
                    if (text.Contains("mi"))
                    {
                        var s1 = Convert.ToDouble(text.Replace(" mi", ""));
                        var s2 = s1 * 1.609344;
                        pn.distance = Convert.ToInt32(s2);
                    }
                    else
                    {
                        pn.distance = Convert.ToInt32(text);
                    }
                    pn.status = "OK";
                }
                else
                {

                    XmlNodeList element = xmldoc.GetElementsByTagName("element");
                    pn.status = element[0].ChildNodes[0].InnerText;
                    pn.distance = 0;
                }

            }

            pin = pn.distance;

        }
    }
}