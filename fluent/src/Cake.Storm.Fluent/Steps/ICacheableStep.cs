using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Steps
{
	public interface ICacheableStep
	{
		string GetCacheId(IConfiguration configuration, StepType currentStep);
	}
}