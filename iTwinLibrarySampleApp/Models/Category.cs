/*--------------------------------------------------------------------------------------+
|
| Copyright (c) Bentley Systems, Incorporated. All rights reserved.
| See LICENSE.md in the catalog root for license terms and full copyright notice.
|
+--------------------------------------------------------------------------------------*/

using System;

namespace ItwinLibrarySampleApp.Models
    {
    public class Category : EntityBase
        {
        public Category ()
            {
            DisplayName = $"iTwinSample Name {Guid.NewGuid()}";
            }
        }
    }

