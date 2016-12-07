using System;
using System.IO;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Storm.JsonBuildConfiguration.Models;
using Newtonsoft.Json;

namespace Cake.Storm.JsonBuildConfiguration
{
	[CakeAliasCategory("Cake.Storm.JsonBuildConfiguration")]
	public static class Aliases
	{
		[CakeMethodAlias]
		[CakeNamespaceImport("Cake.Storm.JsonBuildConfiguration.Models")]
		public static Configuration ReadJsonConfigurationFile(this ICakeContext context, FilePath jsonFile)
		{
			string fileContent = null;
			try
			{
				fileContent = File.ReadAllText(jsonFile.FullPath);
			}
			catch (Exception ex)
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Json file not found {jsonFile.FullPath}");
			}

			try
			{
				return JsonConvert.DeserializeObject<Configuration>();
			}
			catch (Exception ex)
			{

			}
			jsonFile.FullPath
			        
		}
	}
}
