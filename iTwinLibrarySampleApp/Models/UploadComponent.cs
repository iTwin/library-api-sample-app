/*--------------------------------------------------------------------------------------+
|
| Copyright (c) Bentley Systems, Incorporated. All rights reserved.
| See LICENSE.md in the catalog root for license terms and full copyright notice.
|
+--------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using Newtonsoft.Json;


namespace ItwinLibrarySampleApp.Models
    {
    public class UploadComponent
        {
        public string FileName { get; set; }
        public string ComponentName { get; set; }
        public string Description { get; set; }
        public string SessionId { get; set; }
        public List<string> SearchTags { get; set; }
        public bool IsTypeCatalog { get; set; }
        [JsonProperty("_links")]
        public UploadComponentLinks Links { get; set; }
        }

    public class UploadComponentLinks
        {
        public List<Link> Catalogs { get; set; }
        public Link Application { get; set; }
        public Link Manufacturer { get; set; }
        public Link Category { get; set; }
        public Link TypeCatalogUploadUrl { get; set; }
        public Link DesignFileUploadUrl { get; set; }
        }

    }

