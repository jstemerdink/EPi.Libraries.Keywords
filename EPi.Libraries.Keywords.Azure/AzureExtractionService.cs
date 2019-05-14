// Copyright © 2019 Jeroen Stemerdink.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
namespace EPi.Libraries.Keywords.Azure
{
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

    using EPiServer;
    using EPiServer.Logging;
    using EPiServer.ServiceLocation;

    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class AzureExtractionService.
    /// </summary>
    [ServiceConfiguration(typeof(IExtractionService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class AzureExtractionService : IExtractionService
    {
        /// <summary>
        ///     The logger
        /// </summary>
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(KeywordsInitialization));

        private readonly TextAnalyticsClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureExtractionService"/> class.
        /// </summary>
        public AzureExtractionService()
        {
            ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(subscriptionKey: TextAnalyticsKey);

            this.client = new TextAnalyticsClient(credentials: credentials)
                              {
                                  Endpoint = TextAnalysisEndpoint
                              };
        }

        /// <summary>
        ///     Gets the textanalysis key.
        /// </summary>
        /// <value>The textanalysis key.</value>
        private static string TextAnalyticsKey
        {
            get
            {
                return ConfigurationManager.AppSettings["seo.textanalytics.key"];
            }
        }

        private static string TextAnalysisEndpoint
        {
            get
            {
                return ConfigurationManager.AppSettings["seo.textanalytics.endpoint"];
            }
        }

        /// <summary>
        /// Gets the keywords.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="language">The language.</param>
        /// <param name="id">The identifier of the content.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}" /> of keywords.</returns>
        public ReadOnlyCollection<string> GetKeywords(string text, string language, string id)
        {
            if (string.IsNullOrWhiteSpace(value: TextAnalyticsKey))
            {
                return new ReadOnlyCollection<string>(new List<string>());
            }

            try
            {
                MultiLanguageBatchInput inputDocuments = new MultiLanguageBatchInput(
                    new List<MultiLanguageInput>
                        {
                            new MultiLanguageInput(language: language, id: id, text: text)
                        });

                KeyPhraseBatchResult kpResults = this.client.KeyPhrasesAsync(false, multiLanguageBatchInput: inputDocuments).Result;

                return new ReadOnlyCollection<string>(list: kpResults.Documents[0].KeyPhrases);
            }
            catch (Exception exception)
            {
                Logger.Error("[SEO] Error getting keywords from Azure", exception: exception);
                return new ReadOnlyCollection<string>(new List<string>());
            }
        }
    }
}