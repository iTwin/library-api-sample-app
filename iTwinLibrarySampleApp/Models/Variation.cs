/*--------------------------------------------------------------------------------------+
|
| Copyright (c) Bentley Systems, Incorporated. All rights reserved.
| See LICENSE.md in the catalog root for license terms and full copyright notice.
|
+--------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using JsonConverter = Newtonsoft.Json.JsonConverter;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;

namespace ItwinLibrarySampleApp.Models
    {
    public class Variation : EntityBase
        {
        public Variation (string documentId)
            {
            AssociatedDesignDocument = documentId;
            DisplayName = $"iTwinSample Name {Guid.NewGuid()}";
            AdHocProperties = new List<AdHocProperty>();
            }
        public string AssociatedDesignDocument { get; set; }
        public List<AdHocProperty> AdHocProperties { get; set; }
        [JsonProperty("_links")]
        public VariationLinks Links { get; set; }
        }

    public class AdHocProperty
        {
        public AdHocProperty (string name, PropertyType type, string value, string unitOFMeasure )
            {
            DisplayName = name;
            Type = type;
            Value = value;
            UnitOfMeasure = unitOFMeasure;
            }
        public string DisplayName { get; set; }
        public string Value { get; set; }
        public PropertyType Type { get; set; }
        public string UnitOfMeasure { get; set; }
        }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PropertyType
        {
        StringType = 1,
        IntegerType = 2,
        DoubleType = 3,
        FloatType = 4,
        BooleanType = 5
        }

    public class VariationLinks
        {
        public Link AssociatedDesignDocument { get; set; }
        }

    }

