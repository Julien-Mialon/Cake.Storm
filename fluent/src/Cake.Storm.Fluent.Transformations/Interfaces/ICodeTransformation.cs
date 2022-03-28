using Microsoft.CodeAnalysis;

namespace Cake.Storm.Fluent.Transformations.Interfaces
{
    public interface ICodeTransformation
    {
	    ICodeTransformation UpdateVariable(string variableName, string newValue);

	    ICodeTransformation UpdateVariable(string variableName, int newValue);

	    ICodeTransformation UpdateVariable(string variableName, bool newValue);
    }

	internal interface ICodeTransformationAction : ICodeTransformation
	{
		SyntaxNode Execute(SyntaxNode node);
	}
}
