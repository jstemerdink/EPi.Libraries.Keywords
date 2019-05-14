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
namespace EPi.Libraries.Keywords
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using EPi.Libraries.Keywords.DataAnnotations;

    using EPiServer;
    using EPiServer.Core;
    using EPiServer.Core.Html;
    using EPiServer.DataAbstraction;
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.Globalization;
    using EPiServer.Logging;
    using EPiServer.ServiceLocation;
    using EPiServer.SpecializedProperties;

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

        /// <summary>
        ///     Gets the name of the key word property.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The <see cref="PropertyInfo"/>.</returns>
        private static PropertyInfo GetKeyWordProperty(IContent page)
        {
            PropertyInfo keywordsMetatagProperty =
                page.GetType().GetProperties().Where(predicate: HasAttribute<KeywordsMetaTagAttribute>).FirstOrDefault();

            return keywordsMetatagProperty;
        }

        /// <summary>
        ///     Determines whether the specified self has attribute.
        /// </summary>
        /// <typeparam name="T">The attribute to look for.</typeparam>
        /// <param name="propertyInfo">The propertyInfo.</param>
        /// <returns><c>true</c> if the specified self has attribute; otherwise, <c>false</c>.</returns>
        private static bool HasAttribute<T>(PropertyInfo propertyInfo) where T : Attribute
        {
            T attr = (T)Attribute.GetCustomAttribute(element: propertyInfo, attributeType: typeof(T));

            return attr != null;
        }

        /// <summary>
        ///     Gets the searchable property values.
        /// </summary>
        /// <param name="contentData">The content data.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>A list of values.</returns>
        private IEnumerable<string> GetSearchablePropertyValues(IContentData contentData, ContentType contentType)
        {
            List<string> propertyValues = new List<string>();

            if (contentType == null)
            {
               return propertyValues;
            }

            foreach (PropertyDefinition current in
                from d in contentType.PropertyDefinitions
                where d.Searchable || typeof(IPropertyBlock).IsAssignableFrom(c: d.Type.DefinitionType)
                select d)
            {
                PropertyData propertyData = contentData.Property[name: current.Name];

                IPropertyBlock propertyBlock = propertyData as IPropertyBlock;

                if (propertyBlock != null)
                {
                    propertyValues.AddRange(this.GetSearchablePropertyValues(
                        contentData: propertyBlock.Block,
                        contentTypeId: propertyBlock.BlockPropertyDefinitionTypeID));

                    continue;
                }

                ContentArea contentArea = propertyData.Value as ContentArea;

                if (contentArea != null)
                {
                    propertyValues.AddRange(this.GetContentAreaContent(contentArea: contentArea));

                    continue;
                }

                propertyValues.Add(propertyData.ToWebString());
            }

            return propertyValues;
        }

        /// <summary>
        /// Gets the additional search content from the <paramref name="contentArea"/>.
        /// </summary>
        /// <param name="contentArea">The content area.</param>
        /// <returns>The additional search content.</returns>
        private IEnumerable<string> GetContentAreaContent(ContentArea contentArea)
        {
            List<string> propertyValues = new List<string>();

            foreach (ContentAreaItem contentAreaItem in contentArea.Items)
            {
                if (!this.ContentRepository.TryGet(contentLink: contentAreaItem.ContentLink, content: out IContent content))
                {
                    continue;
                }

                // content area item can be null when duplicating a page
                if (content == null)
                {
                    continue;
                }

                // Check if the content is indeed a block, and not a page used in a content area
                BlockData blockData = content as BlockData;

                // Content area is not a block, but probably a page used as a teaser.
                if (blockData == null)
                {
                    continue;
                }

                IEnumerable<string> props = this.GetSearchablePropertyValues(
                    contentData: content,
                    contentTypeId: content.ContentTypeID);

                propertyValues.AddRange(collection: props);
            }

            return propertyValues;
        }

        /// <summary>
        ///     Gets the searchable property values.
        /// </summary>
        /// <param name="contentData">The content data.</param>
        /// <param name="contentTypeId">The content type identifier.</param>
        /// <returns>A list of values.</returns>
        private IEnumerable<string> GetSearchablePropertyValues(IContentData contentData, int contentTypeId)
        {
            return this.GetSearchablePropertyValues(contentData: contentData, contentType: this.ContentTypeRepository.Load(id: contentTypeId));
        }

        private void OnPublishingContent(object sender, ContentEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            // Check if the content that is published is a page. If it's not, don't do anything.
            if (!(e.Content is PageData pageData))
            {
                return;
            }

            if (pageData.IsReadOnly)
            {
                pageData = pageData.CreateWritableClone();
                e.Content = pageData;
            }

            PropertyInfo keywordsMetatagProperty = GetKeyWordProperty(page: pageData);

            if (keywordsMetatagProperty == null)
            {
                return;
            }

            IEnumerable<string> props = this.GetSearchablePropertyValues(contentData: pageData, contentTypeId: pageData.ContentTypeID);

            ////ILocalizable localizable = e.Content as ILocalizable;
            ////CultureInfo currentLanguage = ContentLanguage.PreferredCulture;

            ////if (localizable != null)
            ////{
            ////    currentLanguage = localizable.Language;
            ////}

            string textToAnalyze = TextIndexer.StripHtml(string.Join(" ", values: props), 0).ToLower(culture: pageData.Language);

            ReadOnlyCollection<string> keywordList;

            try
            {
                keywordList = this.ExtractionService.GetKeywords(text: textToAnalyze, language: pageData.Language.TwoLetterISOLanguageName, id: pageData.ContentLink.ID.ToString());
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

            if (keywordsMetatagProperty.PropertyType == typeof(string[]))
            {
                pageData[index: keywordsMetatagProperty.Name] = keywordList.ToArray();
            }
            else if (keywordsMetatagProperty.PropertyType == typeof(List<string>))
            {
                pageData[index: keywordsMetatagProperty.Name] = keywordList;
            }
            else if (keywordsMetatagProperty.PropertyType == typeof(string))
            {
                pageData[index: keywordsMetatagProperty.Name] = string.Join(",", values: keywordList);
            }
        }
    }
}