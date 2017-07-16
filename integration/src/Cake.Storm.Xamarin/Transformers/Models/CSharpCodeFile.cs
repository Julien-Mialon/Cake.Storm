using System.Text;
using Cake.Core;
using Cake.Core.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Cake.Storm.Xamarin.Transformers.Models
{
    public class CSharpCodeFile
    {
	    private readonly ICakeContext _context;
	    private readonly FilePath _path;

	    private SyntaxNode _rootNode;

	    public CSharpCodeFile(ICakeContext context, FilePath path)
	    {
		    _context = context;
		    _path = path;
	    }

	    public void SetVariableValue(string variableName, string newValue)
	    {
		    _rootNode = new VariableValueRewriter(variableName, newValue).Visit(LoadContent());
	    }

	    public void Save() => Save(_path);
	    
	    public void Save(FilePath outputPath)
	    {
		    if (_rootNode == null)
		    {
			    return;
		    }

		    using (var outputStream = _context.FileSystem.GetFile(outputPath).OpenWrite())
		    {
			    string content = _rootNode.ToFullString();
			    byte[] dataBytes = Encoding.UTF8.GetBytes(content);
			    outputStream.Write(dataBytes, 0, dataBytes.Length);
		    }
	    }

		private SyntaxNode LoadContent()
	    {
		    if (_rootNode != null)
		    {
			    return _rootNode;
		    }

			using (var inputStream = _context.FileSystem.GetFile(_path).OpenRead())
			{
				return _rootNode = CSharpSyntaxTree.ParseText(SourceText.From(inputStream)).GetRoot();
			}
	    }

	    private class VariableValueRewriter : CSharpSyntaxRewriter
	    {
		    private readonly string _variableName;
		    private readonly string _variableValue;

		    public VariableValueRewriter(string variableName, string variableValue)
		    {
			    _variableName = variableName;
			    _variableValue = variableValue;
		    }

		    public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
			{
				if (node.Identifier.ValueText == _variableName)
				{
					return node.WithInitializer(
						node.Initializer.WithValue(
							SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(_variableValue))
						)
					);
				}

				return base.VisitVariableDeclarator(node);
			}
		}
    }
}
