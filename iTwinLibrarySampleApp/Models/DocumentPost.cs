/*--------------------------------------------------------------------------------------+
|
| Copyright (c) Bentley Systems, Incorporated. All rights reserved.
| See LICENSE.md in the catalog root for license terms and full copyright notice.
|
+--------------------------------------------------------------------------------------*/

using System;

namespace ItwinLibrarySampleApp.Models
    {
    public class DocumentPost
        {
        public DocumentPost (Purpose purpose, string ext)
            {
            DisplayName = $"iTwinSample Name {Guid.NewGuid()}";
            Purpose = purpose;
            Extension = ext;
            Available = false;
            IsActive = true;
            Version = "v1";
            }
        public string DisplayName { get; set; }
        public string Extension { get; set; }
        public Purpose Purpose { get; set; }
        public bool Available { get; set; }
        public bool IsActive { get; set; }
        public string Version { get; set; }
        public string PreviousVersionId { get; set; }
        public string AssociatedDesignDocument { get; set; }
        }

    }

