// Copyright© 2015 Jeroen Stemerdink. All Rights Reserved.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

using EPi.Libraries.Keywords.Alchemy.Models;

using EPiServer;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

using Newtonsoft.Json;

namespace EPi.Libraries.Keywords.Alchemy
{
    /// <summary>
    ///     Class AlchemyExtractionService.
    /// </summary>
    [ServiceConfiguration(typeof(IExtractionService))]
    public class AlchemyExtractionService : IExtractionService
    {
        #region Static Fields

        /// <summary>
        ///     The logger
        /// </summary>
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(KeywordsInitialization));

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the content repository.
        /// </summary>
        /// <value>The content repository.</value>
        protected Injected<IContentRepository> ContentRepository { get; set; }

        /// <summary>
        ///     Gets the alchemy key.
        /// </summary>
        /// <value>The alchemy key.</value>
        private static string AlchemyKey
        {
            get
            {
                return ConfigurationManager.AppSettings["seo.alchemy.key"];
            }
        }

        /// <summary>
        ///     Gets the maximum items.
        /// </summary>
        /// <value>The maximum items.</value>
        private static int MaxItems
        {
            get
            {
                int maxItems;

                if (!int.TryParse(ConfigurationManager.AppSettings["seo.alchemy.maxitems"], out maxItems))
                {
                    maxItems = 20;
                }

                return maxItems;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the keywords.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>ReadOnlyCollection&lt;System.String&gt;.</returns>
        public ReadOnlyCollection<string> GetKeywords(string text)
        {
            if (string.IsNullOrWhiteSpace(AlchemyKey))
            {
                return new ReadOnlyCollection<string>(new List<string>());
            }
            try
            {
                Uri uri;

                try
                {
                    uri =
                        new Uri(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "http://access.alchemyapi.com/calls/text/TextGetRankedKeywords?apikey={0}&text={1}&maxRetrieve={2}&keywordExtractMode=strict&outputMode=json",
                                AlchemyKey,
                                HttpUtility.UrlEncode(text),
                                MaxItems));
                }
                catch (UriFormatException uriFormatException)
                {
                    Logger.Error("[Localization] URI for Bing in wrong format.", uriFormatException);
                    return null;
                }

                WebRequest translationWebRequest = WebRequest.Create(uri);
                translationWebRequest.Method = "POST";

                Encoding encode = Encoding.GetEncoding("utf-8");

                string json = string.Empty;

                using (WebResponse response = translationWebRequest.GetResponse())
                {
                    Stream responseStream = response.GetResponseStream();

                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream, encode))
                        {
                            json = reader.ReadToEnd();
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new ReadOnlyCollection<string>(new List<string>());
                }

                AlchemyResponse alchemyResponse = JsonConvert.DeserializeObject<AlchemyResponse>(json);

                List<string> keywords = alchemyResponse.status.Equals("error", StringComparison.OrdinalIgnoreCase)
                                            ? new List<string>()
                                            : alchemyResponse.keywords.Where(k => k.relevance >= 0.5)
                                                  .OrderByDescending(k => k.relevance)
                                                  .Select(keyword => keyword.text)
                                                  .ToList();

                return new ReadOnlyCollection<string>(keywords);
            }
            catch (Exception exception)
            {
                Logger.Error("[SEO] Error getting keywords from Alchemy", exception);
                return new ReadOnlyCollection<string>(new List<string>());
            }
        }

        #endregion
    }
}