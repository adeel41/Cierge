using System;
using System.Collections.Generic;
using System.Linq;
using Cierge.Data;

namespace Cierge.Options
{
    public class ClaimsOptions
    {
        public class Claim
        {
            public enum ChangedPropertyType
            {
                Name,
                Caption,
                Url
            }

            public class ChangedPropertyResult
            {
                public ChangedPropertyType Type { get; }
                public Delegate PropertySetter { get; }
                public object UpdatedValue { get; }

                public ChangedPropertyResult(ChangedPropertyType type, Delegate propertySetter, object updatedValue)
                {
                    Type = type;
                    PropertySetter = propertySetter;
                    UpdatedValue = updatedValue;
                }
            }

            private static readonly Delegate NameSetter;
            private static readonly Delegate CaptionSetter;
            private static readonly Delegate UrlSetter;

            public string Name { get; set; }
            public string Caption { get; set; }
            public string Url { get; set; }

            static Claim()
            {
                var type = typeof(CustomClaim);
                NameSetter = type.GetProperty(nameof(CustomClaim.Name)).GetSetMethod().CreateDelegate(typeof(Action<CustomClaim, string>));
                CaptionSetter = type.GetProperty(nameof(CustomClaim.Caption)).GetSetMethod().CreateDelegate(typeof(Action<CustomClaim, string>));
                UrlSetter = type.GetProperty(nameof(CustomClaim.Url)).GetSetMethod().CreateDelegate(typeof(Action<CustomClaim, string>));
            }

            public IEnumerable<ChangedPropertyResult> GetChangedProperties(CustomClaim dbObject)
            {
                var nameComparison = string.Compare(Name, dbObject.Name, StringComparison.Ordinal);
                if (nameComparison != 0)
                    yield return new ChangedPropertyResult(ChangedPropertyType.Name, NameSetter, Name);

                var captionComparison = string.Compare(Caption, dbObject.Caption, StringComparison.Ordinal);
                if (captionComparison != 0)
                    yield return new ChangedPropertyResult(ChangedPropertyType.Caption, CaptionSetter, Caption);

                var urlComparison = string.Compare(Url, dbObject.Url, StringComparison.Ordinal);
                if (urlComparison != 0)
                    yield return new ChangedPropertyResult(ChangedPropertyType.Url, UrlSetter, Url);
            }
        }

        public List<Claim> Claims { get; set; }
    }
}
