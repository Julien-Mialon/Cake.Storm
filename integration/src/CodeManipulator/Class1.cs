using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeManipulator
{
    public class Class1
    {
	    public void Test()
	    {
		    var tree = CSharpSyntaxTree.ParseText(@"
using System;
namespace Sample 
{
	public static class Constants
	{
		public const string API_ROOT = ""http://api-dev.azurewebsites.net/"";
		public static string GCM = ""1234567"";
		public static CustomType MyVar;
	}
}
			");

		    SyntaxNode newSource = new Transform().Visit(tree.GetRoot());

		    string source = newSource.ToFullString();

		 //   SyntaxNode rootNode = tree.GetRoot();

			//foreach (var classDeclaration in GetClassDeclarations(rootNode).OfType<ClassDeclarationSyntax>())
			//{
			//	foreach (var fieldDeclarationSyntax in classDeclaration.Members.OfType<FieldDeclarationSyntax>())
			//	{
			//		foreach (var variableDeclaratorSyntax in fieldDeclarationSyntax.Declaration.Variables)
			//		{
			//			if (variableDeclaratorSyntax.Identifier.ValueText == "API_ROOT")
			//			{
			//				variableDeclaratorSyntax.Initializer.WithValue(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("https://plop.net/")));
			//			}
			//		}
			//	}
			//}

		 //   text = tree.ToString();
		 //   source = tree.GetText().ToString();
		}

	    private IEnumerable<SyntaxNode> GetClassDeclarations(SyntaxNode root)
	    {
		    if (root.IsKind(SyntaxKind.ClassDeclaration))
		    {
			    yield return root;
		    }

		    foreach (var syntaxNode in root.ChildNodes())
		    {
			    foreach (var result in GetClassDeclarations(syntaxNode))
			    {
				    yield return result;
			    }
		    }
	    }
    }

	public class Transform : CSharpSyntaxRewriter
	{
		public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
		{
			if (node.Identifier.ValueText == "API_ROOT")
			{
				return node.WithInitializer(
					node.Initializer.WithValue(
						SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("https://plop.net/"))
					)
				);
			}

			return base.VisitVariableDeclarator(node);
		}
	}
}
