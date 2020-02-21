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
namespace EPi.Libraries.Keywords.Commerce
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    
    using EPiServer;
    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Core;
    using EPiServer.Core.Html;
    using EPiServer.DataAbstraction;
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.Logging;
    
    /// <summary>
    ///     Class KeywordsInitialization.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(FrameworkInitialization))]
    public class KeywordsInitialization : IInitializableModule
    {
        /// <summary>
        ///     The logger
        /// </summary>
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(KeywordsInitialization));

        /// <summary>
        ///     Gets or sets the content events.
        /// </summary>
        /// <value>The content events.</value>
        protected IContentEvents ContentEvents { get; set; }

        /// <summary>
        ///     Gets or sets the content repository.
        /// </summary>
        /// <value>The content repository.</value>
        protected IContentRepository ContentRepository { get; set; }

        /// <summary>
        ///     Gets or sets the content type repository.
        /// </summary>
        /// <value>The content type repository.</value>
        protected IContentTypeRepository ContentTypeRepository { get; set; }

        /// <summary>
        ///     Gets or sets the extraction service.
        /// </summary>
        /// <value>The extraction service.</value>
        protected IExtractionService ExtractionService { get; set; }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <remarks>
        ///     Gets called as part of the EPiServer Framework initialization sequence. Note that it will be called
        ///     only once per AppDomain, unless the method throws an exception. If an exception is thrown, the initialization
        ///     method will be called repeatedly for each request reaching the site until the method succeeds.
        /// </remarks>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there is are errors resolving the service instance.</exception>
        public void Initialize(InitializationEngine context)
        {
            this.ContentEvents = context.Locate.Advanced.GetInstance<IContentEvents>();
            this.ContentRepository = context.Locate.Advanced.GetInstance<IContentRepository>();
            this.ContentTypeRepository = context.Locate.Advanced.GetInstance<IContentTypeRepository>();
            this.ExtractionService = context.Locate.Advanced.GetInstance<IExtractionService>();

            this.ContentEvents.PublishingContent += this.OnPublishingContent;
        }

        /// <summary>
        ///     Resets the module into an uninitialized state.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <remarks>
        ///     <para>
        ///         This method is usually not called when running under a web application since the web app may be shut down very
        ///         abruptly, but your module should still implement it properly since it will make integration and unit testing
        ///         much simpler.
        ///     </para>
        ///     <para>
        ///         Any work done by
        ///         <see
        ///             cref="M:EPiServer.Framework.IInitializableModule.Initialize(EPiServer.Framework.Initialization.InitializationEngine)" />
        ///         as well as any code executing on
        ///         <see cref="E:EPiServer.Framework.Initialization.InitializationEngine.InitComplete" /> should be reversed.
        ///     </para>
        /// </remarks>
        public void Uninitialize(InitializationEngine context)
        {
            this.ContentEvents.PublishingContent -= this.OnPublishingContent;
        }

        private void OnPublishingContent(object sender, ContentEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            // Check if the content that is published is a page. If it's not, don't do anything.
            if (!(e.Content is CatalogContentBase catalogContent))
            {
                return;
            }

            if (catalogContent.IsReadOnly)
            {
                catalogContent = catalogContent.CreateWritableClone();
                e.Content = catalogContent;
            }

            ContentHelpers contentHelpers = new ContentHelpers(this.ContentRepository, this.ContentTypeRepository);
            IEnumerable<string> props = contentHelpers.GetSearchablePropertyValues(contentData: catalogContent, contentTypeId: catalogContent.ContentTypeID);

            string textToAnalyze = TextIndexer.StripHtml(string.Join(" ", values: props), 0).ToLower(culture: catalogContent.Language);

            ReadOnlyCollection<string> keywordList;

            try
            {
                keywordList = this.ExtractionService.GetKeywords(text: textToAnalyze, language: catalogContent.Language.TwoLetterISOLanguageName, id: catalogContent.ContentLink.ID.ToString());
            }
            catch (Exception exception)
            {
                Logger.Error("[SEO] No extraction service available", exception: exception);
                return;
            }

            if (keywordList.Count == 0)
            {
                return;
            }

            switch (catalogContent)
            {
                case EntryContentBase entryContent:
                    entryContent.SeoInformation.Keywords = string.Join(", ", values: keywordList);
                    return;
                case NodeContent nodeContent:
                    nodeContent.SeoInformation.Keywords = string.Join(", ", values: keywordList);
                    break;
            }
        }
    }
}