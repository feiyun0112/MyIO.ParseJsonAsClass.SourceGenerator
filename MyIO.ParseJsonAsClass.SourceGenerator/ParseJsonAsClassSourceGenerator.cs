using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using MyIO.ParseJsonAsClass.SourceGenerator.Implementation;
using System.Diagnostics;

namespace MyIO.ParseJsonAsClass.SourceGenerator
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            const string attributeText = @"using System;

namespace MyIO
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ParseJsonAsClassAttribute : Attribute
    {
        public ParseJsonAsClassAttribute(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; set; }
    }
}
";

            context.AddSource("MyIO.ParseJsonAsClassAttribute.g", SourceText.From(attributeText, System.Text.Encoding.UTF8));
              

            new Processor(context).Process();
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }
    }
}
