using System;
using System.Globalization;

namespace ET
{
    public static class BehaviorTreeValueUtility
    {
        public static object GetValue(BehaviorTreeSerializedValue value)
        {
            if (value == null)
            {
                return null;
            }

            return value.ValueType switch
            {
                BehaviorTreeValueType.Integer => value.IntValue,
                BehaviorTreeValueType.Long => value.LongValue,
                BehaviorTreeValueType.Float => value.FloatValue,
                BehaviorTreeValueType.Boolean => value.BoolValue,
                BehaviorTreeValueType.String => value.StringValue,
                _ => null,
            };
        }

        public static int GetInt(BehaviorTreeSerializedValue value, int defaultValue = 0)
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

        public static long GetLong(BehaviorTreeSerializedValue value, long defaultValue = 0)
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

        public static float GetFloat(BehaviorTreeSerializedValue value, float defaultValue = 0)
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

        public static bool GetBool(BehaviorTreeSerializedValue value, bool defaultValue = false)
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

        public static string GetString(BehaviorTreeSerializedValue value, string defaultValue = "")
        {
            object rawValue = GetValue(value);
            return rawValue?.ToString() ?? defaultValue;
        }

        public static bool Compare(object currentValue, BehaviorTreeCompareOperator comparison, BehaviorTreeSerializedValue compareValue)
        {
            return comparison switch
            {
                BehaviorTreeCompareOperator.IsSet => currentValue != null,
                BehaviorTreeCompareOperator.IsNotSet => currentValue == null,
                BehaviorTreeCompareOperator.IsTrue => currentValue is bool boolValue && boolValue,
                BehaviorTreeCompareOperator.IsFalse => currentValue is not bool typedBool || !typedBool,
                BehaviorTreeCompareOperator.Equal => Equals(currentValue, GetValue(compareValue)),
                BehaviorTreeCompareOperator.NotEqual => !Equals(currentValue, GetValue(compareValue)),
                BehaviorTreeCompareOperator.Greater => CompareOrder(currentValue, compareValue) > 0,
                BehaviorTreeCompareOperator.GreaterOrEqual => CompareOrder(currentValue, compareValue) >= 0,
                BehaviorTreeCompareOperator.Less => CompareOrder(currentValue, compareValue) < 0,
                BehaviorTreeCompareOperator.LessOrEqual => CompareOrder(currentValue, compareValue) <= 0,
                _ => false,
            };
        }

        public static string ToDisplayString(BehaviorTreeSerializedValue value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.ValueType switch
            {
                BehaviorTreeValueType.Integer => value.IntValue.ToString(CultureInfo.InvariantCulture),
                BehaviorTreeValueType.Long => value.LongValue.ToString(CultureInfo.InvariantCulture),
                BehaviorTreeValueType.Float => value.FloatValue.ToString(CultureInfo.InvariantCulture),
                BehaviorTreeValueType.Boolean => value.BoolValue.ToString(),
                BehaviorTreeValueType.String => value.StringValue ?? string.Empty,
                _ => string.Empty,
            };
        }

        private static int CompareOrder(object currentValue, BehaviorTreeSerializedValue compareValue)
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
