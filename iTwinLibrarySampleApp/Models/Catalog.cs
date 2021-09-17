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
    public class Catalog : EntityBase
        {
        public Catalog ()
            {
            DisplayName = $"iTwinSample Name {Guid.NewGuid()}";
            Description = $"iTwinSample Description {Guid.NewGuid()}";
            Region = "US";
            Hashtags = new List<string> { $"iTwinSampleHashTag_{Guid.NewGuid().ToString()}".Substring(0, 50) };
            }

        public string Description { get; set; }
        public string Region { get; set; }
        public List<string> Hashtags { get; set; }
        }
    }

