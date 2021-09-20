/*--------------------------------------------------------------------------------------+
|
| Copyright (c) Bentley Systems, Incorporated. All rights reserved.
| See LICENSE.md in the catalog root for license terms and full copyright notice.
|
+--------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using JsonConverter = Newtonsoft.Json.JsonConverter;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;

namespace ItwinLibrarySampleApp.Models
    {
    public class Component : EntityBase
        {
        public string Description { get; set; }
        public List<string> Hashtags { get; set; }
        public ComponentState State { get; set; }
        public List<string> SupportedFileTypes { get; set; }
        [JsonProperty("_links")]
        public ComponentLinks Links { get; set; }
        }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ComponentState
        {
        Draft = 1,
        Published = 2,
        Checked = 3,
        Approved = 4,
        Archived = 5
        }

    public class ComponentLinks
        {
        public List<Link> Catalogs { get; set; }
        public Link Application { get; set; }
        public Link Manufacturer { get; set; }
        public Link Category { get; set; }
        }

    public static class ComponentExtension
        {
        public static ComponentPost ConvertToComponentPost (this Component component)
            {
            var componentPost = new ComponentPost()
                {
                DisplayName = component.DisplayName,
                Description = component.Description,
                Hashtags = component.Hashtags,
                State = component.State
                };

            if ( component?.Links != null )
                {
                componentPost.Category = GetLastSegmentFromUrl(component.Links.Category?.Href);
                componentPost.Manufacturer = GetLastSegmentFromUrl(component.Links.Manufacturer?.Href);
                componentPost.Application = GetLastSegmentFromUrl(component.Links.Application?.Href);

                if ( component.Links.Catalogs != null && component.Links.Catalogs.Count > 0 )
                    {
                    var catalogs = new List<string>();
                    foreach ( var catalog in component.Links.Catalogs )
                        {
                        var catalogId = GetLastSegmentFromUrl(catalog.Href);
                        if ( catalogId != null )
                            catalogs.Add(catalogId);
                        }
                    if ( catalogs.Count > 0 )
                        componentPost.Catalogs = catalogs;
                    }

                }

            return componentPost;
            }

        private static string GetLastSegmentFromUrl (string url)
            {
            if ( Uri.TryCreate(url, UriKind.Absolute, out Uri uri) )
                return uri.Segments.Last();

            return null;
            }
        }
    }

