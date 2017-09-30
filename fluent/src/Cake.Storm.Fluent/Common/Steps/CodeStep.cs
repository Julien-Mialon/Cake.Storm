using System;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.Common.Steps
{
	internal class CodeStep : BaseMultipleStep
	{
		private readonly Action<IConfiguration> _action;

		public CodeStep(Action<IConfiguration> action, StepType onStep) : base(onStep)
		{
			_action = action;
		}

		protected override void Execute(IConfiguration configuration)
		{
			_action(configuration);
		}
	}
}