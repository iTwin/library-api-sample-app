/*--------------------------------------------------------------------------------------+
|
| Copyright (c) Bentley Systems, Incorporated. All rights reserved.
| See LICENSE.md in the catalog root for license terms and full copyright notice.
|
+--------------------------------------------------------------------------------------*/

using System;

namespace ItwinLibrarySampleApp.Models
    {
    public class Application : EntityBase
        {
        public Application ()
            {
            DisplayName = $"iTwinSample Name {Guid.NewGuid()}";
            Version = $"iTwinSample Version {Guid.NewGuid()}";
            }

        public string Version { get; set; }
        }
    }

