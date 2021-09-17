/*--------------------------------------------------------------------------------------+
|
| Copyright (c) Bentley Systems, Incorporated. All rights reserved.
| See LICENSE.md in the catalog root for license terms and full copyright notice.
|
+--------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;

namespace ItwinLibrarySampleApp.Models
    {
    public class UploadComponentPost
        {
        public UploadComponentPost (string fileName, List<string> catalogs)
            {
            FileName = fileName;
            ComponentName = $"iTwinSample Name {Guid.NewGuid()}";
            Description = $"iTwinSample Description {Guid.NewGuid()}";
            SearchTags = new List<string> { $"iTwinSampleHashTag_{Guid.NewGuid()}".Substring(0, 50) };
            Catalogs = catalogs;
            }

        public string FileName { get; set; }
        public string ComponentName { get; set; }
        public string Description { get; set; }
        public List<string> SearchTags { get; set; }
        public List<string> Catalogs { get; set; }
        public string Category { get; set; }
        public string Application { get; set; }
        public string Manufacturer { get; set; }
        public bool IsTypeCatalog { get; set; }
        }
    }

