

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.IO;
 
using System.Globalization;
using System.Text.Json;

namespace MyIO.ParseJsonAsClass.SourceGenerator.Implementation
{
    public class JsonParser
    {
        public string Parse()
        {
            List<JsonElement> jsonElements = new List<JsonElement>();
            using (var jsonDocument = JsonDocument.Parse(this.json))
            {
                var jsonElement = jsonDocument.RootElement;
                if (jsonElement.ValueKind == JsonValueKind.Array)
                {
                    foreach(var item in jsonElement.EnumerateArray())
                    {
                        jsonElements.Add(item);
                    }
                }
                else
                {
                    jsonElements.Add(jsonElement);
                }

                types = new List<ParsedType>();
                names.Add(this.class_name);
                var rootType = new ParsedType(jsonElements[0]);
                rootType.IsRoot = true;
                rootType.AssignName(this.class_name);
                Parse(jsonElements.ToArray(), rootType);
            }
              
            using(var writer = new StringWriter())
            {
                GenerateClass(writer, types,name_space,class_name);
                return writer.ToString();
            }

        }
         
        private void GenerateClass(TextWriter sw, IEnumerable<ParsedType> types, string name_space,string class_name)
        {
            CodeWriter.WriteFileStart(sw,name_space,class_name);
            foreach (var type in types)
            {
                CodeWriter.WriteClass(sw, type);
            }

            CodeWriter.WriteFileEnd(sw);
        }


        private void Parse(JsonElement[] jsonElements, ParsedType type)
        {
            var jsonFields = new Dictionary<string, ParsedType>();

            foreach (var obj in jsonElements)
            {
                foreach (var prop in obj.EnumerateObject())
                {
                    ParsedType fieldType;
                    var currentType = new ParsedType(prop.Value);
                    var propName = prop.Name;
                    if (jsonFields.TryGetValue(propName, out fieldType))
                    {
                        currentType = fieldType.GetNullableType(currentType);

                        jsonFields[propName] = currentType;
                    }
                    else
                    {
                        jsonFields.Add(propName, currentType);
                    }
                }
            }


            foreach (var field in jsonFields)
            {
                names.Add(field.Key.ToLower());
            }

            foreach (var field in jsonFields)
            {
                var fieldType = field.Value;
                if (fieldType.Type == TypeEnum.Object)
                {
                    var subexamples = new List<JsonElement>(jsonElements.Length);
                    foreach (var obj in jsonElements)
                    {
                        JsonElement value;
                        if (obj.TryGetProperty(field.Key,out value))
                        {
                            if (value.ValueKind == JsonValueKind.Object)
                            {
                                subexamples.Add(value);
                            }
                        }
                    }

                    fieldType.AssignName(CreateUniqueClassName(field.Key));
                    Parse(subexamples.ToArray(), fieldType);
                }

                if (fieldType.InternalType != null && fieldType.InternalType.Type == TypeEnum.Object)
                {
                    var subexamples = new List<JsonElement>(jsonElements.Length);
                    foreach (var obj in jsonElements)
                    {
                        JsonElement value;
                        if (obj.TryGetProperty(field.Key, out value))
                        {
                            if (value.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var item in value.EnumerateArray())
                                {
                                    subexamples.Add(item);
                                }
                            }
                            else if (value.ValueKind == JsonValueKind.Object)
                            {
                                foreach (var item in value.EnumerateObject())
                                {
                                    subexamples.Add(item.Value);
                                }
                            }
                        }
                    }

                    field.Value.InternalType.AssignName(CreateUniqueClassName(field.Key));
                    Parse(subexamples.ToArray(), field.Value.InternalType);
                }
            }

            type.Properties = jsonFields.Select(x => new PropertyInfo( x.Key, x.Value)).ToArray();

            types.Add(type);
        }

        private IList<ParsedType> types = new List<ParsedType>();
        private List<string> names = new List<string>();
        private string name_space;
        private string class_name;
        private string json;

        private string CreateUniqueClassName(string name)
        { 
            var finalName = name.ToTitleCase();
            var i = 2;
            while (names.Any(x => x.Equals(finalName, StringComparison.OrdinalIgnoreCase)))
            {
                finalName = finalName + i.ToString();
                i++;
            }

            names.Add(finalName);
            return finalName;
        }
         

        public JsonParser(string name_space, string class_name, string json)
        {
            this.name_space = name_space;
            this.class_name = class_name;
            this.json = json;
        }
    }
}
