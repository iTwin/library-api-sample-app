/*--------------------------------------------------------------------------------------+
|
| Copyright (c) Bentley Systems, Incorporated. All rights reserved.
| See LICENSE.md in the catalog root for license terms and full copyright notice.
|
+--------------------------------------------------------------------------------------*/

using System;

namespace ItwinLibrarySampleApp.Models
    {
    public class WebLink : EntityBase
        {
        public WebLink ()
            {
            DisplayName = $"iTwinSample Name {Guid.NewGuid()}".Substring(0, 50);
            Uri = "http://itwinsampleeblink.com/samplelink";
            }

        public string Uri { get; set; }
        }
    }

