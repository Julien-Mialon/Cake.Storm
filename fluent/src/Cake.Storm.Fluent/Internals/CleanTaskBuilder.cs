using System.Collections.Generic;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.Internals
{
	public class CleanTaskBuilder : IBuilder
	{
		public const string TASK_NAME = "clean";

		private readonly BuilderParameters _parameters;

		public CleanTaskBuilder(BuilderParameters parameters)
		{
			_parameters = parameters;
		}

		public void Build()
		{
			IEnumerable<IStep> preCleanSteps = _parameters.RootConfiguration.StepsOf<PreCleanStepAttribute>();
			IEnumerable<IStep> cleanSteps = _parameters.RootConfiguration.StepsOf<CleanStepAttribute>();
			IEnumerable<IStep> postCleanSteps = _parameters.RootConfiguration.StepsOf<PostCleanStepAttribute>();

			_parameters.Context.Task(TASK_NAME).Does(() =>
			{
				_parameters.Runner.ExecuteStepsOfTypes(_parameters.RootConfiguration, StepType.PreClean, StepType.Clean, StepType.PostClean);
			});
		}

		public void Help()
		{
			_parameters.Context.CakeContext.Log.Information("clean: run preclean, clean and postclean steps");
		}
	}
}