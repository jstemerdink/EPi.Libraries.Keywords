// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureExtractionService.cs" company="Jeroen Stemerdink">
//      Copyright © 2023 Jeroen Stemerdink.
//      Permission is hereby granted, free of charge, to any person obtaining a copy
//      of this software and associated documentation files (the "Software"), to deal
//      in the Software without restriction, including without limitation the rights
//      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//      copies of the Software, and to permit persons to whom the Software is
//      furnished to do so, subject to the following conditions:
//
//      The above copyright notice and this permission notice shall be included in all
//      copies or substantial portions of the Software.
//
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//      SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace EPi.Libraries.Keywords.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;

    using EPiServer.ServiceLocation;

    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     Class AzureExtractionService.
    /// </summary>
    [ServiceConfiguration(typeof(IExtractionService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class AzureExtractionService : IExtractionService
    {
        private const string EndpointMissingMessage = "[SEO Keywords : Azure] Api Url not configured";

        private const string KeyMissingMessage = "[SEO Keywords : Azure] Access key not configured";

        private readonly IConfiguration configuration;

        private readonly ILogger<AzureExtractionService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureExtractionService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public AzureExtractionService(IConfiguration configuration, ILogger<AzureExtractionService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the keywords.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="language">The language.</param>
        /// <param name="id">The identifier of the content.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}" /> of keywords.</returns>
        /// <exception cref="ConfigurationErrorsException">Setting could not be found.</exception>
        public ReadOnlyCollection<string> GetKeywords(string text, string language, string id)
        {
            string textAnalyticsKey = this.configuration.GetValue<string>("Azure:TextAnalytics:Key");

            if (string.IsNullOrWhiteSpace(value: textAnalyticsKey))
            {
                throw new ConfigurationErrorsException(message: KeyMissingMessage);
            }

            string textAnalysisEndpoint = this.configuration.GetValue<string>("Azure:TextAnalytics:Endpoint");

            if (string.IsNullOrWhiteSpace(value: textAnalysisEndpoint))
            {
                throw new ConfigurationErrorsException(message: EndpointMissingMessage);
            }

            try
            {
                ApiKeyServiceClientCredentials credentials =
                    new ApiKeyServiceClientCredentials(subscriptionKey: textAnalyticsKey);

                TextAnalyticsClient client = new TextAnalyticsClient(credentials: credentials)
                                                 {
                                                     Endpoint = textAnalysisEndpoint
                                                 };


                MultiLanguageBatchInput inputDocuments = new MultiLanguageBatchInput(
                    new List<MultiLanguageInput> { new MultiLanguageInput(language: language, id: id, text: text) });

                KeyPhraseBatchResult kpResults = client.KeyPhrasesBatch(multiLanguageBatchInput: inputDocuments);

                return new ReadOnlyCollection<string>(list: kpResults.Documents[0].KeyPhrases);
            }
            catch (Exception exception)
            {
                this.logger.Log(
                    logLevel: LogLevel.Error,
                    exception: exception,
                    "[SEO Keywords : Azure] Error getting keywords from Azure");

                return new ReadOnlyCollection<string>(new List<string>());
            }
        }
    }
}