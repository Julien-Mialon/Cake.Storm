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
		private readonly StepType _onStep;

		protected BaseMultipleStep(StepType onStep)
		{
			_onStep = onStep;
		}

		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			if (currentStep == _onStep)
			{
				Execute(configuration);
			}
		}

		protected abstract void Execute(IConfiguration configuration);
	}
}