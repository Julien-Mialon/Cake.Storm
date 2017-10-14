using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Steps
{
	public interface ICacheableStep : IStep
	{
		string GetCacheId(IConfiguration configuration, StepType currentStep);
	}
}