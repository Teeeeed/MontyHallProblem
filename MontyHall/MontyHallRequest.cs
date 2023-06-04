using System;
using Amazon.Lambda.APIGatewayEvents;

namespace MontyHall;

public class MontyHallRequest
{
    public int NumOfSimulations;
    public bool AlwaysChangeChoice;
}

public static class APIGatewayProxyRequestUtility
{
    public static bool TryGetQueryParam<T>(APIGatewayProxyRequest request, string key, out T value)
    {
        value = default!;
        if (request.QueryStringParameters is null)
        {
            return false;
        }

        var strValue = request.QueryStringParameters.TryGetValue(key, out var val) ? val : null;
        if (string.IsNullOrWhiteSpace(strValue))
        {
            return false;
        }

        if (TryParse(strValue, out T parsed))
        {
            value = parsed;
            return true;
        }

        return false;
    }
    
    private static bool TryParse<T>(string strValue, out T value)
    {
        value = default!;

        var result = Enum.TryParse(typeof(TypeCode), Type.GetTypeCode(typeof(T)).ToString(), out var objType);
        if (!result || objType == null)
        {
            throw new InvalidOperationException($"Type {typeof(T)} is not supported.");
        }

        switch ((TypeCode)objType)
        {
            case TypeCode.Int16:
            case TypeCode.Int32:
                if (!int.TryParse(strValue, out var intValue))
                {
                    return false;
                }

                value = (T)Convert.ChangeType(intValue, typeof(T));
                break;
            case TypeCode.Boolean:
                if (!bool.TryParse(strValue, out var boolValue))
                {
                    return false;
                }

                value = (T)Convert.ChangeType(boolValue, typeof(T));
                break;
            default:
                value = (T)Convert.ChangeType(strValue, typeof(T));
                break;
        }

        return true;
    }
}