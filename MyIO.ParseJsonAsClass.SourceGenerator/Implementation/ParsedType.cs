

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.Json;

namespace MyIO.ParseJsonAsClass.SourceGenerator.Implementation
{
    public class ParsedType
    { 
        public ParsedType(JsonElement element)
        {
            Type = GetFirstTypeEnum(element);

            if (Type == TypeEnum.Array)
            { 
                InternalType = GetNullableType(element.EnumerateArray().ToArray());
            }
        }

        internal static ParsedType GetNull()
        {
            return new ParsedType(TypeEnum.NullableObject);
        }
         

        internal ParsedType(TypeEnum type)
        {
            this.Type = type;
        }



        private static ParsedType GetNullableType(JsonElement[] elements)
        {

            if (elements.Length == 0) return new ParsedType(TypeEnum.Object);

            var result = new ParsedType(elements[0]);

            for (int i = 1; i < elements.Length; i++)
            {
                var current = new ParsedType(elements[i]);
                result = result.GetNullableType(current);
            }

            return result;
        }

        public TypeEnum Type { get; private set; }
        public ParsedType InternalType { get; private set; }
        public string Name { get; private set; }


        public void AssignName(string name)
        {
            Name = name;
        }

        public ParsedType GetNullableType(ParsedType type2)
        {
            var commonType = GetNullableTypeEnum(this.Type, type2.Type);

            if (commonType == TypeEnum.Array)
            {
                if (type2.Type == TypeEnum.NullableObject) return this;
                if (this.Type == TypeEnum.NullableObject) return type2;
                var internalType = InternalType.GetNullableType(type2.InternalType);
                if (internalType != InternalType) return new ParsedType(TypeEnum.Array) { InternalType = internalType };
            }


            if (this.Type == commonType) return this;
            return new ParsedType(commonType);
        }


        private static bool IsNull(TypeEnum type)
        {
            return type == TypeEnum.NullableObject;
        }
         

        private TypeEnum GetNullableTypeEnum(TypeEnum type1, TypeEnum type2)
        {
            switch (type1)
            {
                case TypeEnum.Boolean:
                    if (IsNull(type2)) return TypeEnum.NullableBoolean;
                    if (type2 == TypeEnum.Boolean) return type1;
                    break;
                case TypeEnum.NullableBoolean:
                    if (IsNull(type2)) return type1;
                    if (type2 == TypeEnum.Boolean) return type1;
                    break;
                case TypeEnum.Integer:
                    if (IsNull(type2)) return TypeEnum.NullableInteger;
                    if (type2 == TypeEnum.Decimal) return TypeEnum.Decimal;
                    if (type2 == TypeEnum.Long) return TypeEnum.Long;
                    if (type2 == TypeEnum.Integer) return type1;
                    break;
                case TypeEnum.NullableInteger:
                    if (IsNull(type2)) return type1;
                    if (type2 == TypeEnum.Decimal) return TypeEnum.NullableDecimal;
                    if (type2 == TypeEnum.Long) return TypeEnum.NullableLong;
                    if (type2 == TypeEnum.Integer) return type1;
                    break;
                case TypeEnum.Decimal:
                    if (IsNull(type2)) return TypeEnum.NullableDecimal;
                    if (type2 == TypeEnum.Decimal) return type1;
                    if (type2 == TypeEnum.Integer) return type1;
                    if (type2 == TypeEnum.Long) return type1;
                    break;
                case TypeEnum.NullableDecimal:
                    if (IsNull(type2)) return type1;
                    if (type2 == TypeEnum.Decimal) return type1;
                    if (type2 == TypeEnum.Integer) return type1;
                    if (type2 == TypeEnum.Long) return type1;
                    break;
                case TypeEnum.Long:
                    if (IsNull(type2)) return TypeEnum.NullableLong;
                    if (type2 == TypeEnum.Decimal) return TypeEnum.Decimal;
                    if (type2 == TypeEnum.Integer) return type1;
                    break;
                case TypeEnum.NullableLong:
                    if (IsNull(type2)) return type1;
                    if (type2 == TypeEnum.Decimal) return TypeEnum.NullableDecimal;
                    if (type2 == TypeEnum.Integer) return type1;
                    if (type2 == TypeEnum.Long) return type1;
                    break;
                case TypeEnum.DateTime:
                    if (IsNull(type2)) return TypeEnum.NullableDateTime;
                    if (type2 == TypeEnum.DateTime) return TypeEnum.DateTime;
                    break;
                case TypeEnum.NullableDateTime:
                    if (IsNull(type2)) return type1;
                    if (type2 == TypeEnum.DateTime) return type1;
                    break;
                case TypeEnum.NullableObject:
                    if (IsNull(type2)) return type1;
                    if (type2 == TypeEnum.String) return TypeEnum.String;
                    if (type2 == TypeEnum.Integer) return TypeEnum.NullableInteger;
                    if (type2 == TypeEnum.Decimal) return TypeEnum.NullableDecimal;
                    if (type2 == TypeEnum.Long) return TypeEnum.NullableLong;
                    if (type2 == TypeEnum.Boolean) return TypeEnum.NullableBoolean;
                    if (type2 == TypeEnum.DateTime) return TypeEnum.NullableDateTime;
                    if (type2 == TypeEnum.Array) return TypeEnum.Array;
                    if (type2 == TypeEnum.Object) return TypeEnum.Object;
                    break;
                case TypeEnum.Object:
                    if (IsNull(type2)) return type1;
                    if (type2 == TypeEnum.Object) return type1;
                    break;
                case TypeEnum.Array:
                    if (IsNull(type2)) return type1;
                    if (type2 == TypeEnum.Array) return type1;
                    break;
                case TypeEnum.String:
                    if (IsNull(type2)) return type1;
                    if (type2 == TypeEnum.String) return type1;
                    break;
            }

            return TypeEnum.Object;

        }


        private static TypeEnum GetFirstTypeEnum(JsonElement element)
        {
            var type = element.ValueKind;
            if (type == JsonValueKind.Number)
            {
                if (element.TryGetDecimal(out _))
                {
                    if (element.TryGetInt64(out long value1))
                    {
                    }
                    else
                    {
                        return TypeEnum.Decimal;
                    }
                }
                if (element.TryGetInt64(out long value2))
                {
                    if (value2 < int.MaxValue) return TypeEnum.Integer;
                    else return TypeEnum.Long;
                }
                
                return TypeEnum.Integer;
            }

            if (type == JsonValueKind.String)
            {
                if (element.TryGetDateTime(out _))
                {
                    return TypeEnum.DateTime;
                }
                return TypeEnum.String;
            }

            switch (type)
            {
                case JsonValueKind.Array: return TypeEnum.Array;
                case JsonValueKind.True: return TypeEnum.Boolean;
                case JsonValueKind.Null: return TypeEnum.NullableObject;
                case JsonValueKind.Undefined: return TypeEnum.NullableObject;
                case JsonValueKind.Object: return TypeEnum.Object;
                default: return TypeEnum.Object;
            }
        }
        public ParsedType GetInnermostType()
        {
            if (Type != TypeEnum.Array) throw new InvalidOperationException();
            if (InternalType.Type != TypeEnum.Array) return InternalType;
            return InternalType.GetInnermostType();
        }

        public IList<PropertyInfo> Properties { get; internal set; }
        public bool IsRoot { get; internal set; }
    }
}
