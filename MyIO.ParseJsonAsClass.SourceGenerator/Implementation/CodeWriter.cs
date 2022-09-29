
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyIO.ParseJsonAsClass.SourceGenerator.Implementation
{
    public static class CodeWriter
    {  
        private static  string GetTypeName(ParsedType type)
        {
            switch (type.Type)
            { 
                case TypeEnum.Array: return "IList<" + GetTypeName(type.InternalType) + ">" ;
                case TypeEnum.Boolean: return "bool";
                case TypeEnum.Decimal: return "decimal";
                case TypeEnum.Integer: return "int";
                case TypeEnum.Long: return "long";
                case TypeEnum.DateTime: return "DateTime";
                case TypeEnum.NullableBoolean: return "bool?";
                case TypeEnum.NullableDecimal: return "decimal?";
                case TypeEnum.NullableInteger: return "int?";
                case TypeEnum.NullableLong: return "long?";
                case TypeEnum.NullableDateTime: return "DateTime?";
                case TypeEnum.Object: return type.Name;
                case TypeEnum.String: return "string";
                default: return "object";
            }
        }

         
        public static void WriteFileStart(TextWriter sw, string name_space, string class_name)
        {
            sw.WriteLine();
            sw.WriteLine("using System;");
            sw.WriteLine("using System.Collections.Generic;");
            sw.WriteLine("using System.Reflection;");

            sw.WriteLine("using System.Text.Json.Serialization;");

            sw.WriteLine("namespace " + name_space);
            sw.WriteLine("{");
            sw.WriteLine("    {0} partial class {1}", "", class_name);
            sw.WriteLine("    {");
        }

        public static void WriteFileEnd(TextWriter sw)
        {
            sw.WriteLine("    }");
            sw.WriteLine("}");
        }



        public static void WriteClass(TextWriter sw, ParsedType type)
        {
            if (!type.IsRoot)
            {
                sw.WriteLine("        {0} class {1}", "public", type.Name);
                sw.WriteLine("        {");
            }

            var prefix = "                ";

            WriteClassMembers(sw, type, prefix);

            if (!type.IsRoot)
                sw.WriteLine("        }");

            sw.WriteLine();
        }



        private static void WriteClassMembers(TextWriter sw, ParsedType type, string prefix)
        {
            foreach (var field in type.Properties)
            {
                sw.WriteLine(prefix + "[JsonPropertyName(\"{0}\")]", field.JsonName);

                sw.WriteLine(prefix + "public {0} {1} {{ get; set; }}", GetTypeName(field.Type), field.Name);
            }
        }
    }
}
