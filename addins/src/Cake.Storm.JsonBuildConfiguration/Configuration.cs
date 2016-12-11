using System.Linq;
using System.Collections.Generic;
using Cake.Storm.JsonBuildConfiguration.Models;

namespace Cake.Storm.JsonBuildConfiguration
{
	public abstract class Configuration
	{
		//global info
		public string PlatformName { get; internal set; }

		public string TargetName { get; internal set; }

		public string AppName { get; internal set; }

		//from BuildConfiguration
		public string Solution { get; internal set; }

		public string Project { get; internal set; }

		public Dictionary<string, string> BuildProperties { get; }

		internal Configuration(BuildConfiguration build, PlatformConfiguration platform, TargetConfiguration target, AppConfiguration app)
		{
			PlatformName = platform.Name.ToString();
			TargetName = target.Name;
			AppName = app.Name;

			Solution = build.Solution;
			Project = build.Project;
			BuildProperties = build.Properties.ToDictionary(x => x.Key, x => x.Value);
		}
	}
}
