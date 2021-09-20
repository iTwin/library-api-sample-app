/*--------------------------------------------------------------------------------------+
|
| Copyright (c) Bentley Systems, Incorporated. All rights reserved.
| See LICENSE.md in the catalog root for license terms and full copyright notice.
|
+--------------------------------------------------------------------------------------*/

using System;

namespace ItwinLibrarySampleApp.Models
    {
   
    public class Manufacturer : EntityBase
        {
        public Manufacturer ()
            {
            DisplayName = $"iTwinSample Name {Guid.NewGuid()}";
            Phone = $"1 999 8765";
            Address = "999 Main St, iTwin Sample Address 12345";
            Website = "www.itwinsamplewebportal.com";
            }

        public string Phone { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        }
    }

