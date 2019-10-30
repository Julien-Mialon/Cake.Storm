using System.Threading.Tasks;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Steps
{
	public abstract class BaseAsyncStep : IStep
	{
		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			ExecuteAsync(configuration, currentStep).Wait();
		}

		protected abstract Task ExecuteAsync(IConfiguration configuration, StepType currentStep);
	}
}