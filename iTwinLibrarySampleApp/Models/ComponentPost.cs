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
    public class ComponentPost
        {
        public ComponentPost ()
            {
            DisplayName = $"iTwinSample Name {Guid.NewGuid()}";
            Description = $"iTwinSample Description {Guid.NewGuid()}";
            Hashtags = new List<string> { $"iTwinSampleHashTag_{Guid.NewGuid()}".Substring(0, 50) };
            State = ComponentState.Draft;
            }

        public ComponentPost (List<string> catalogs, string category, string application, string manufacturer): this()
            {
            Catalogs = catalogs;
            Category = category;
            Application = application;
            Manufacturer = manufacturer;
            }

        public string DisplayName { get; set; }
        public string Description { get; set; }
        public ComponentState State { get; set; }
        public List<string> Hashtags { get; set; }
        public List<string> Catalogs { get; set; }
        public string Category { get; set; }
        public string Application { get; set; }
        public string Manufacturer { get; set; }
        }
    }

