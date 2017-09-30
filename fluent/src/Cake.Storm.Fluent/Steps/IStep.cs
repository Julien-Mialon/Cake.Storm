using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Steps
{
    public interface IStep
    {
	    void Execute(IConfiguration configuration, StepType currentStep);
    }
}
