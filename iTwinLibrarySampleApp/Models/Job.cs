/*--------------------------------------------------------------------------------------+
|
| Copyright (c) Bentley Systems, Incorporated. All rights reserved.
| See LICENSE.md in the catalog root for license terms and full copyright notice.
|
+--------------------------------------------------------------------------------------*/

using Newtonsoft.Json;

namespace ItwinLibrarySampleApp.Models
    {
    public class Job 
        {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public int JobSize { get; set; }
        public double Progress { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        [JsonProperty("_links")]
        public JobLinks Links { get; set; }
        }

    public class JobLinks
        {
        public Link Component { get; set; }
        }

    }

