using System;
using System.Collections.Generic;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.Internals
{
	public interface IStepRunner
	{
		void ExecuteStepsOfTypes(IConfiguration configuration, params StepType[] types);
	}

	public class StepRunner : IStepRunner
	{
		public void ExecuteStepsOfTypes(IConfiguration configuration, params StepType[] types)
		{
			foreach (StepType type in types)
			{
				foreach (IStep step in GetStepsForType(configuration, type))
				{
					step.Execute(configuration, type);
				}
			}
		}

		private IEnumerable<IStep> GetStepsForType(IConfiguration configuration, StepType type)
		{
			switch (type)
			{
				case StepType.PreClean:
					return configuration.StepsOf<PreCleanStepAttribute>();
				case StepType.Clean:
					return configuration.StepsOf<CleanStepAttribute>();
				case StepType.PostClean:
					return configuration.StepsOf<PostCleanStepAttribute>();
				case StepType.PreBuild:
					return configuration.StepsOf<PreBuildStepAttribute>();
				case StepType.Build:
					return configuration.StepsOf<BuildStepAttribute>();
				case StepType.PostBuild:
					return configuration.StepsOf<PostBuildStepAttribute>();
				case StepType.PreRelease:
					return configuration.StepsOf<PreReleaseStepAttribute>();
				case StepType.Release:
					return configuration.StepsOf<ReleaseStepAttribute>();
				case StepType.PostRelease:
					return configuration.StepsOf<PostReleaseStepAttribute>();
				case StepType.PreDeploy:
					return configuration.StepsOf<PreDeployStepAttribute>();
				case StepType.Deploy:
					return configuration.StepsOf<DeployStepAttribute>();
				case StepType.PostDeploy:
					return configuration.StepsOf<PostDeployStepAttribute>();
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
	}
}