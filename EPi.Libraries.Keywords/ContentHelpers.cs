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
    using System.Collections.Generic;
    using System.Linq;

    using EPiServer;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.ServiceLocation;
    using EPiServer.SpecializedProperties;

    public class ContentHelpers
    {
        public ContentHelpers(IContentRepository contentRepository, IContentTypeRepository contentTypeRepository)
        {
            this.ContentRepository = contentRepository;
            this.ContentTypeRepository = contentTypeRepository;
        }

        /// <summary>
        ///     Gets or sets the content repository.
        /// </summary>
        /// <value>The content repository.</value>
        public IContentRepository ContentRepository { get; set; }

        /// <summary>
        ///     Gets or sets the content type repository.
        /// </summary>
        /// <value>The content type repository.</value>
        public IContentTypeRepository ContentTypeRepository { get; set; }

        /// <summary>
        /// Gets the additional search content from the <paramref name="contentArea"/>.
        /// </summary>
        /// <param name="contentArea">The content area.</param>
        /// <returns>The additional search content.</returns>
        public IEnumerable<string> GetContentAreaContent(ContentArea contentArea)
        {
            List<string> propertyValues = new List<string>();

            foreach (ContentAreaItem contentAreaItem in contentArea.Items)
            {
                if (!this.ContentRepository.TryGet(contentAreaItem.ContentLink, out IContent content))
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
        /// <param name="contentType">Type of the content.</param>
        /// <returns>A list of values.</returns>
        public IEnumerable<string> GetSearchablePropertyValues(IContentData contentData, ContentType contentType)
        {
            List<string> propertyValues = new List<string>();

            if (contentType == null)
            {
                return propertyValues;
            }

            foreach (PropertyDefinition current in from d in contentType.PropertyDefinitions
                                                   where d.Searchable
                                                         || typeof(IPropertyBlock).IsAssignableFrom(
                                                             c: d.Type.DefinitionType)
                                                   select d)
            {
                PropertyData propertyData = contentData.Property[name: current.Name];

                IPropertyBlock propertyBlock = propertyData as IPropertyBlock;

                if (propertyBlock != null)
                {
                    propertyValues.AddRange(
                        this.GetSearchablePropertyValues(
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
        ///     Gets the searchable property values.
        /// </summary>
        /// <param name="contentData">The content data.</param>
        /// <param name="contentTypeId">The content type identifier.</param>
        /// <returns>A list of values.</returns>
        public IEnumerable<string> GetSearchablePropertyValues(IContentData contentData, int contentTypeId)
        {
            return this.GetSearchablePropertyValues(contentData, this.ContentTypeRepository.Load(contentTypeId));
        }
    }
}