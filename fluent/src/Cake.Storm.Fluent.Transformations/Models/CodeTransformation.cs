using System;
using System.Collections.Generic;
using Cake.Storm.Fluent.Transformations.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cake.Storm.Fluent.Transformations.Models
{
    internal class CodeTransformation : ICodeTransformationAction
    {
	    private readonly List<CSharpSyntaxRewriter> _transformations = new List<CSharpSyntaxRewriter>();

	    public ICodeTransformation UpdateVariable(string variableName, string newValue)
	    {
			_transformations.Add(new VariableTransformation(variableName, SyntaxKind.StringLiteralExpression, () => SyntaxFactory.Literal(newValue)));
		    return this;
	    }

	    public ICodeTransformation UpdateVariable(string variableName, int newValue)
	    {
		    _transformations.Add(new VariableTransformation(variableName, SyntaxKind.NumericLiteralExpression, () => SyntaxFactory.Literal(newValue)));
		    return this;
	    }

		public SyntaxNode Execute(SyntaxNode node)
	    {
		    foreach (CSharpSyntaxRewriter transformation in _transformations)
		    {
			    node = transformation.Visit(node);
		    }
		    return node;
	    }

	    private class VariableTransformation : CSharpSyntaxRewriter
	    {
		    private readonly string _variableName;
		    private readonly SyntaxKind _kind;
		    private readonly Func<SyntaxToken> _creator;

		    public VariableTransformation(string variableName, SyntaxKind kind, Func<SyntaxToken> creator)
		    {
			    _variableName = variableName;
			    _kind = kind;
			    _creator = creator;
		    }

			public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
		    {
			    if (node.Identifier.ValueText == _variableName)
			    {
				    return node.WithInitializer(
					    node.Initializer.WithValue(
						    SyntaxFactory.LiteralExpression(_kind, _creator())
					    )
				    );
			    }

			    return base.VisitVariableDeclarator(node);
		    }
	    }
	}
}
