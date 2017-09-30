using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Steps
{
	[MultiStep]
	[PreCleanStep] [CleanStep] [PostCleanStep]
	[PreBuildStep] [BuildStep] [PostBuildStep]
	[PreReleaseStep] [ReleaseStep] [PostReleaseStep]
	[PreDeployStep] [DeployStep] [PostDeployStep]
	public abstract class BaseMultipleStep : IStep
	{
		private readonly StepType _executeOn;

		protected BaseMultipleStep(StepType executeOn)
		{
			_executeOn = executeOn;
		}

		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			if (currentStep == _executeOn)
			{
				Execute(configuration);
			}
		}

		protected abstract void Execute(IConfiguration configuration);
	}
}