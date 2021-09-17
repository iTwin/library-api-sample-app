/*--------------------------------------------------------------------------------------+
|
| Copyright (c) Bentley Systems, Incorporated. All rights reserved.
| See LICENSE.md in the catalog root for license terms and full copyright notice.
|
+--------------------------------------------------------------------------------------*/

using System;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using JsonConverter = Newtonsoft.Json.JsonConverter;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;

namespace ItwinLibrarySampleApp.Models
    {
    public class Document : EntityBase
        {
        public string Extension { get; set; }
        public Purpose Purpose { get; set; }
        public int Size { get; set; }
        public bool Available { get; set; }
        public bool IsActive { get; set; }
        public string Version { get; set; }
        public string PreviousVersionId { get; set; }
        [JsonProperty("_links")]
        public DocumentLinks Links { get; set; }
        }

    public class DocumentLinks
        {
        public Link AssociatedDesignDocument { get; set; }
        public Link FileUrl { get; set; }
        }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Purpose
        {
        Design,
        Thumbnail,
        Reference,
        GalleryImage,
        TypeCatalog
        }

    public static class DocumentExtension
        {
        public static DocumentPost ConvertToDocumentPost (this Document document)
            {
            var documentPost = new DocumentPost(document.Purpose, document.Extension)
                {
                DisplayName = document.DisplayName,
                Available = document.Available,
                IsActive = document.IsActive,
                Version = document.Version
                };

            if ( document?.Links != null )
                {
                documentPost.AssociatedDesignDocument = GetLastSegmentFromUrl(document.Links.AssociatedDesignDocument?.Href);
                }

            return documentPost;
            }

        private static string GetLastSegmentFromUrl (string url)
            {
            if ( Uri.TryCreate(url, UriKind.Absolute, out Uri uri) )
                return uri.Segments.Last();

            return null;
            }
        }
    }

