using System;
using System.Globalization;

namespace ET
{
    public static class BTValueUtility
    {
        public static object GetValue(BTSerializedValue value)
        {
            if (value == null)
            {
                return null;
            }

            return value.ValueType switch
            {
                BTValueType.Integer => value.IntValue,
                BTValueType.Long => value.LongValue,
                BTValueType.Float => value.FloatValue,
                BTValueType.Boolean => value.BoolValue,
                BTValueType.String => value.StringValue,
                _ => null,
            };
        }

        public static int GetInt(BTSerializedValue value, int defaultValue = 0)
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

        public static long GetLong(BTSerializedValue value, long defaultValue = 0)
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

        public static float GetFloat(BTSerializedValue value, float defaultValue = 0)
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

        public static bool GetBool(BTSerializedValue value, bool defaultValue = false)
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

        public static string GetString(BTSerializedValue value, string defaultValue = "")
        {
            object rawValue = GetValue(value);
            return rawValue?.ToString() ?? defaultValue;
        }

        public static bool Compare(object currentValue, BTCompareOperator comparison, BTSerializedValue compareValue)
        {
            return comparison switch
            {
                BTCompareOperator.IsSet => currentValue != null,
                BTCompareOperator.IsNotSet => currentValue == null,
                BTCompareOperator.IsTrue => currentValue is bool boolValue && boolValue,
                BTCompareOperator.IsFalse => currentValue is not bool typedBool || !typedBool,
                BTCompareOperator.Equal => Equals(currentValue, GetValue(compareValue)),
                BTCompareOperator.NotEqual => !Equals(currentValue, GetValue(compareValue)),
                BTCompareOperator.Greater => CompareOrder(currentValue, compareValue) > 0,
                BTCompareOperator.GreaterOrEqual => CompareOrder(currentValue, compareValue) >= 0,
                BTCompareOperator.Less => CompareOrder(currentValue, compareValue) < 0,
                BTCompareOperator.LessOrEqual => CompareOrder(currentValue, compareValue) <= 0,
                _ => false,
            };
        }

        public static string ToDisplayString(BTSerializedValue value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.ValueType switch
            {
                BTValueType.Integer => value.IntValue.ToString(CultureInfo.InvariantCulture),
                BTValueType.Long => value.LongValue.ToString(CultureInfo.InvariantCulture),
                BTValueType.Float => value.FloatValue.ToString(CultureInfo.InvariantCulture),
                BTValueType.Boolean => value.BoolValue.ToString(),
                BTValueType.String => value.StringValue ?? string.Empty,
                _ => string.Empty,
            };
        }

        private static int CompareOrder(object currentValue, BTSerializedValue compareValue)
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
