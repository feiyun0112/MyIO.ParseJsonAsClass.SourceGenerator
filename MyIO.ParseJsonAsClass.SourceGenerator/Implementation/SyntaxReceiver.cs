using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
 

namespace MyIO.ParseJsonAsClass.SourceGenerator.Implementation
{
    public class SyntaxReceiver : ISyntaxReceiver
    {
        public List<MemberDeclarationSyntax> MemberSyntaxes { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is MemberDeclarationSyntax member &&
                member.SyntaxTree != null &&
                (member is ClassDeclarationSyntax) &&
                member.AttributeLists.Count>0)
            {
                this.MemberSyntaxes.Add(member);
            }
        }
         
    }
}
