using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MyIO.ParseJsonAsClass.SourceGenerator.Implementation
{
    internal class Processor
    {
        private readonly GeneratorExecutionContext context;

        public Processor(GeneratorExecutionContext context)
        {
            this.context = context;
        }

        public void Process()
        {
            if (this.context.SyntaxReceiver is SyntaxReceiver syntaxReceiver)
                this.ProcessMemberSyntaxes(syntaxReceiver.MemberSyntaxes);
        }

        private void ProcessMemberSyntaxes(IEnumerable<MemberDeclarationSyntax> memberSyntaxes)
        {

            var visitedTypes = new HashSet<INamedTypeSymbol>();
            foreach (var memberSyntax in memberSyntaxes)
            {
                if (memberSyntax is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    var name_space = GetNamespace(classDeclarationSyntax);
                    var class_name = classDeclarationSyntax.Identifier.ValueText;

                    string json = GetJson(classDeclarationSyntax);

                    if (json == null)
                    {
                        continue;
                    }

                    var sourceText = GenerateSource(name_space, class_name, json);

                    if (sourceText != null)
                    {
                        this.context.AddSource("MyIO.ParseJsonAsClass." + classDeclarationSyntax.Identifier.ValueText + ".g", sourceText);
                    }
                }
                this.context.CancellationToken.ThrowIfCancellationRequested();
            }
        }

        private string GetJson(ClassDeclarationSyntax classDeclarationSyntax)
        {
            AttributeSyntax attributeSyntax = null;
            foreach (var attributeList in classDeclarationSyntax.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    if (attribute.Name.ToFullString() == "ParseJsonAsClass")
                    {
                        attributeSyntax = attribute;
                        break;
                    }
                }
            }
            if (attributeSyntax == null)
            {
                return null;
            }

            var argument = attributeSyntax.ArgumentList?.Arguments.First();
            var expression = argument?.Expression as LiteralExpressionSyntax;

            var mainSyntaxTree = context.Compilation.SyntaxTrees
                  .First(x => x.HasCompilationUnitRoot);

            var directory = Path.GetDirectoryName(mainSyntaxTree.FilePath);
            var json = File.ReadAllText(Path.Combine(directory, expression?.Token.ValueText));
            return json;
        }

        private string GenerateSource(string name_space, string class_name, string json)
        {
            try
            {
                var jsonParser = new JsonParser(name_space, class_name, json);

                return jsonParser.Parse();
            }
            catch (Exception ex)
            {
                this.context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(Guid.NewGuid().ToString(), ex.Message, "", "MyIO.ParseJsonAsClass.SourceGenerator", DiagnosticSeverity.Warning, true), null));
                return null;
            }
        }
         
        static string GetNamespace(BaseTypeDeclarationSyntax syntax)
        {
            string nameSpace = string.Empty;

            SyntaxNode? potentialNamespaceParent = syntax.Parent;

            while (potentialNamespaceParent != null &&
                    potentialNamespaceParent is not NamespaceDeclarationSyntax
                    && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
            {
                potentialNamespaceParent = potentialNamespaceParent.Parent;
            }

            if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
            {
                nameSpace = namespaceParent.Name.ToString();

                while (true)
                {
                    if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                    {
                        break;
                    }

                    nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                    namespaceParent = parent;
                }
            }

            return nameSpace;
        }
       
    }
}
