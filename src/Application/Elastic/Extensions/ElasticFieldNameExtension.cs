using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Elastic.Extensions;

internal static class ElasticFieldName
{
    public static string FromProperty(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return propertyName;
        }

        return char.ToLowerInvariant(propertyName[0]) + propertyName[1..];
    }
}
