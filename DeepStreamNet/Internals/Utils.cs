﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DeepStreamNet
{
    static class Utils
    {
        public static string BuildCommand(Topic topic, Action action, params object[] args)
        {
            var sb = StringBuilderPool.Aquire();

            sb.Append(topic.ToString());
            sb.Append(Constants.RecordSeperator);
            sb.Append(action.ToString());
            sb.Append(Constants.RecordSeperator);

            for (int i = 0; i < args.Length; i++)
            {
                sb.Append(args[i]);
                sb.Append(Constants.RecordSeperator);
            }

            sb[sb.Length - 1] = Constants.GroupSeperator;

            return StringBuilderPool.ToStringAndRelease(sb);
        }

        public static string ConvertAndPrefixData<T>(T data)
        {
            if (data is JToken)
            {
                return ConvertAndPrefixDataFromJToken(data as JToken);
            }

            if (data is string)
            {
                return Constants.Types.STRING.ToString() + data;
            }

            if (data is bool)
            {
                if (bool.Parse(data.ToString()))
                    return Constants.Types.TRUE.ToString();

                return Constants.Types.FALSE.ToString();
            }

            if (data is int || data is double || data is float || data is decimal)
            {
                return Constants.Types.NUMBER.ToString() + data.ToString().Replace(',', '.');
            }

            if (!(data is object))
            {
                return Constants.Types.NULL.ToString();
            }

            try
            {
                return Constants.Types.OBJECT.ToString() + JsonConvert.SerializeObject(data);
            }
            catch
            {
                return Constants.Types.UNDEFINED.ToString();
            }
        }

        static string ConvertAndPrefixDataFromJToken(JToken data)
        {
            switch (data.Type)
            {
                case JTokenType.String:
                    return Constants.Types.STRING.ToString() + data.ToObject<string>();
                case JTokenType.Boolean:
                    return data.ToObject<bool>() ? Constants.Types.TRUE.ToString() : Constants.Types.FALSE.ToString();
                case JTokenType.Float:
                case JTokenType.Integer:
                    return Constants.Types.NUMBER.ToString() + data.ToObject<string>();
                case JTokenType.Null:
                    return Constants.Types.NULL.ToString();
                default:
                    try
                    {
                        return Constants.Types.OBJECT.ToString() + JsonConvert.SerializeObject(data);
                    }
                    catch
                    {
                        return Constants.Types.UNDEFINED.ToString();
                    }
            }
        }

        public static KeyValuePair<Type, JToken> ConvertPrefixedData(string dataWithTypePrefix)
        {
            var evtData = dataWithTypePrefix.Substring(1);

            switch (dataWithTypePrefix[0])
            {
                case Constants.Types.STRING:
                    return new KeyValuePair<Type, JToken>(typeof(string), JToken.FromObject(evtData));

                case Constants.Types.NUMBER:
                    return new KeyValuePair<Type, JToken>(typeof(double), JToken.Parse(evtData));

                case Constants.Types.TRUE:
                    return new KeyValuePair<Type, JToken>(typeof(bool), JToken.FromObject(true));

                case Constants.Types.FALSE:
                    return new KeyValuePair<Type, JToken>(typeof(bool), JToken.FromObject(false));

                case Constants.Types.NULL:
                    return new KeyValuePair<Type, JToken>(typeof(object), JValue.CreateNull());

                case Constants.Types.OBJECT:
                    return new KeyValuePair<Type, JToken>(typeof(object), JToken.Parse(evtData));

                default:
                    return new KeyValuePair<Type, JToken>(typeof(string), JToken.Parse(evtData));
            }
        }

        static readonly Dictionary<Type, JTokenType> typeMappings = new Dictionary<Type, JTokenType>
        {
            [typeof(bool)] = JTokenType.Boolean,
            [typeof(string)] = JTokenType.String,
            [typeof(Guid)] = JTokenType.Guid,
            [typeof(DateTime)] = JTokenType.Date,
            [typeof(TimeSpan)] = JTokenType.TimeSpan,
            [typeof(double)] = JTokenType.Float,
            [typeof(float)] = JTokenType.Float,
            [typeof(decimal)] = JTokenType.Float,
            [typeof(int)] = JTokenType.Integer,
            [typeof(short)] = JTokenType.Integer,
            [typeof(byte)] = JTokenType.Integer
        };

        public static bool IsJTokenTypeEqualNetType(JTokenType tokenType, Type type)
        {
            if (typeMappings.ContainsKey(type))
            {
                return typeMappings[type] == tokenType;
            }

            return tokenType == JTokenType.Object || tokenType == JTokenType.Null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumeric(Type type)
        {
            if (type == null)
                return false;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == NullableType)
                    {
                        return IsNumeric(type.GetGenericArguments()[0]);
                    }
                    return false;
            }
            return false;
        }

        static readonly Type NullableType = typeof(Nullable<>);

        public static string CreateUid() => Guid.NewGuid().ToString("N");
    }
}