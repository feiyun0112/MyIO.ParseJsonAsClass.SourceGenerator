


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyIO.ParseJsonAsClass.SourceGenerator.Implementation
{
    public class PropertyInfo
    {
        public PropertyInfo(string jsonName, ParsedType type)
        { 
            this.JsonName = jsonName;
            this.Name = jsonName.ToTitleCase(); 
            this.Type = type;
        }
 
        public string Name { get; private set; }
        public string JsonName { get; private set; }
        public ParsedType Type { get; private set; }
    }
}
