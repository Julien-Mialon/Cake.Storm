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

	internal class StepRunner : IStepRunner
	{
		private readonly Dictionary<Type, HashSet<string>> _cachedStepsInstructions = new Dictionary<Type, HashSet<string>>();
		
		public void ExecuteStepsOfTypes(IConfiguration configuration, params StepType[] types)
		{
			foreach (StepType type in types)
			{
				foreach (IStep step in GetStepsForType(configuration, type))
				{
					if (step is ICacheableStep cacheableStep)
					{
						string cacheId = cacheableStep.GetCacheId(configuration, type);
						Type stepType = step.GetType();
						if (ExistsInCache(stepType, cacheId))
						{
							continue;
						}
						AddToCache(stepType, cacheId);
					}
					step.Execute(configuration, type);
				}
			}
		}

		private bool ExistsInCache(Type stepType, string cacheId)
		{
			return _cachedStepsInstructions.ContainsKey(stepType) && _cachedStepsInstructions[stepType].Contains(cacheId);
		}

		private void AddToCache(Type stepType, string cacheId)
		{
			if (!_cachedStepsInstructions.TryGetValue(stepType, out HashSet<string> instructions))
			{
				instructions = new HashSet<string>();
				_cachedStepsInstructions.Add(stepType, instructions);
			}

			instructions.Add(cacheId);
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