namespace Cake.Storm.Fluent
{
	public enum StepType
	{
		PreClean,
		Clean,
		PostClean,
		
		PreBuild,
		Build,
		PostBuild,
		
		PreRelease,
		Release,
		PostRelease,
		
		PreDeploy,
		Deploy,
		PostDeploy
	}
}