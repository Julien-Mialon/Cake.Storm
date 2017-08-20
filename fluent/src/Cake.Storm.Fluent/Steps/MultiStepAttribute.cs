using System;

namespace Cake.Storm.Fluent.Steps
{
	/// <summary>
	/// This allow a step to be used multiple times in the same build
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class MultiStepAttribute : Attribute
	{
		
	}
}