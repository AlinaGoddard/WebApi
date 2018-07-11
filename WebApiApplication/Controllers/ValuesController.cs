using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace WebApiApplication.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage GetXmlLikePortions()
        {
            var emailText = Request.Content.ReadAsStringAsync().Result;            
            string totalTagText = "<total>";

            if (!emailText.Contains(totalTagText))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"Missing {totalTagText}.");

            var missingClosingTag = GetMissingClosingTag(emailText);
            if (!string.IsNullOrWhiteSpace(missingClosingTag))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"Closing tag missing for {missingClosingTag}.");

            return Request.CreateResponse(HttpStatusCode.Created, GetXmlResultString(emailText));
        }

        private string GetXmlResultString(string emailText)
        {
            string xmlStringSoFar = emailText;
            string xmlResult = string.Empty;

            while (xmlStringSoFar.Contains("<"))
            {
                int indexXmlTagStarts = xmlStringSoFar.IndexOf("<", StringComparison.Ordinal);
                int indexXmlTagEnds = xmlStringSoFar.IndexOf(">", StringComparison.Ordinal);
                string openingXmlTag = xmlStringSoFar.Substring(indexXmlTagStarts, indexXmlTagEnds - indexXmlTagStarts + 1);

                //If it is an email address it isn't an xml tag we are interested in
                if (openingXmlTag.Contains("@"))
                {
                    xmlStringSoFar = xmlStringSoFar.Substring(indexXmlTagStarts + openingXmlTag.Length);
                    continue;
                }

                string expectedClosingXmlTag = openingXmlTag.Replace("<", "</");
                xmlStringSoFar = xmlStringSoFar.Substring(indexXmlTagStarts);

                int lengthOfTag = xmlStringSoFar.IndexOf(expectedClosingXmlTag, StringComparison.Ordinal) + expectedClosingXmlTag.Length;
                var xmlPortionToAdd = xmlStringSoFar.Substring(0, lengthOfTag);

                xmlResult += xmlPortionToAdd;
                xmlStringSoFar = xmlStringSoFar.Substring(lengthOfTag);
            }

            if (!xmlResult.Contains("<cost_centre>"))
                xmlResult += "<cost_centre>UNKNOWN</cost_centre>";

            return xmlResult;
        }

        private string GetMissingClosingTag(string xmlPortion)
        {
            //Match anything inside triangle brackets
            string pattern = @"\<(.*?)\>";
            var matches = Regex.Matches(xmlPortion, pattern);

            foreach (var match in matches)
            {
                var matchValue = match.ToString();
                if (matchValue[1] != '/')
                {
                    var startingTag = matchValue;

                    //If it is an email address it isn't an xml tag we are interested in
                    if (startingTag.Contains("@"))
                        continue;

                        var expectedEndTag = $"</{startingTag.Substring(1)}";
                    if (matches.Cast<Match>().All(m => m.ToString() != expectedEndTag))
                        return startingTag;
                }
            }

            return string.Empty;
        }
    }
}
