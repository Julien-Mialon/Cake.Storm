using Microsoft.CodeAnalysis;

namespace Cake.Storm.Fluent.Transformations.Interfaces
{
    public interface ICodeTransformation
    {
	    ICodeTransformation UpdateVariable(string variableName, string newValue);

	    ICodeTransformation UpdateVariable(string variableName, int newValue);
		
		SyntaxNode Execute(SyntaxNode node);
    }
}
