using System;
using System.Globalization;

namespace ET
{
    public static class BTreeValueUtility
    {
        public static object GetValue(BTreeSerializedValue value)
        {
            if (value == null)
            {
                return null;
            }

            return value.ValueType switch
            {
                BTreeValueType.Integer => value.IntValue,
                BTreeValueType.Long => value.LongValue,
                BTreeValueType.Float => value.FloatValue,
                BTreeValueType.Boolean => value.BoolValue,
                BTreeValueType.String => value.StringValue,
                _ => null,
            };
        }

        public static int GetInt(BTreeSerializedValue value, int defaultValue = 0)
        {
            object rawValue = GetValue(value);
            if (rawValue == null)
            {
                return defaultValue;
            }

            if (rawValue is int intValue)
            {
                return intValue;
            }

            return int.TryParse(rawValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out int result)
                    ? result
                    : defaultValue;
        }

        public static long GetLong(BTreeSerializedValue value, long defaultValue = 0)
        {
            object rawValue = GetValue(value);
            if (rawValue == null)
            {
                return defaultValue;
            }

            if (rawValue is long longValue)
            {
                return longValue;
            }

            return long.TryParse(rawValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out long result)
                    ? result
                    : defaultValue;
        }

        public static float GetFloat(BTreeSerializedValue value, float defaultValue = 0)
        {
            object rawValue = GetValue(value);
            if (rawValue == null)
            {
                return defaultValue;
            }

            if (rawValue is float floatValue)
            {
                return floatValue;
            }

            return float.TryParse(rawValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out float result)
                    ? result
                    : defaultValue;
        }

        public static bool GetBool(BTreeSerializedValue value, bool defaultValue = false)
        {
            object rawValue = GetValue(value);
            if (rawValue == null)
            {
                return defaultValue;
            }

            if (rawValue is bool boolValue)
            {
                return boolValue;
            }

            return bool.TryParse(rawValue.ToString(), out bool result) ? result : defaultValue;
        }

        public static string GetString(BTreeSerializedValue value, string defaultValue = "")
        {
            object rawValue = GetValue(value);
            return rawValue?.ToString() ?? defaultValue;
        }

        public static bool Compare(object currentValue, BTreeCompareOperator comparison, BTreeSerializedValue compareValue)
        {
            return comparison switch
            {
                BTreeCompareOperator.IsSet => currentValue != null,
                BTreeCompareOperator.IsNotSet => currentValue == null,
                BTreeCompareOperator.IsTrue => currentValue is bool boolValue && boolValue,
                BTreeCompareOperator.IsFalse => currentValue is not bool typedBool || !typedBool,
                BTreeCompareOperator.Equal => Equals(currentValue, GetValue(compareValue)),
                BTreeCompareOperator.NotEqual => !Equals(currentValue, GetValue(compareValue)),
                BTreeCompareOperator.Greater => CompareOrder(currentValue, compareValue) > 0,
                BTreeCompareOperator.GreaterOrEqual => CompareOrder(currentValue, compareValue) >= 0,
                BTreeCompareOperator.Less => CompareOrder(currentValue, compareValue) < 0,
                BTreeCompareOperator.LessOrEqual => CompareOrder(currentValue, compareValue) <= 0,
                _ => false,
            };
        }

        public static string ToDisplayString(BTreeSerializedValue value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.ValueType switch
            {
                BTreeValueType.Integer => value.IntValue.ToString(CultureInfo.InvariantCulture),
                BTreeValueType.Long => value.LongValue.ToString(CultureInfo.InvariantCulture),
                BTreeValueType.Float => value.FloatValue.ToString(CultureInfo.InvariantCulture),
                BTreeValueType.Boolean => value.BoolValue.ToString(),
                BTreeValueType.String => value.StringValue ?? string.Empty,
                _ => string.Empty,
            };
        }

        private static int CompareOrder(object currentValue, BTreeSerializedValue compareValue)
        {
            if (currentValue == null)
            {
                return -1;
            }

            object targetValue = GetValue(compareValue);
            if (targetValue == null)
            {
                return 1;
            }

            if (currentValue is int currentInt)
            {
                return currentInt.CompareTo(GetInt(compareValue));
            }

            if (currentValue is long currentLong)
            {
                return currentLong.CompareTo(GetLong(compareValue));
            }

            if (currentValue is float currentFloat)
            {
                return currentFloat.CompareTo(GetFloat(compareValue));
            }

            if (currentValue is double currentDouble)
            {
                return currentDouble.CompareTo(GetFloat(compareValue));
            }

            if (currentValue is string currentString)
            {
                return string.Compare(currentString, GetString(compareValue), StringComparison.OrdinalIgnoreCase);
            }

            return string.Compare(currentValue.ToString(), targetValue.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
