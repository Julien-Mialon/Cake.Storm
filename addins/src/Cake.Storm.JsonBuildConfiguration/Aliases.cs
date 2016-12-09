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
		public static ConfigurationEngine ReadJsonConfigurationFile(this ICakeContext context, FilePath jsonFile)
		{
			string fileContent = null;
			try
			{
				fileContent = File.ReadAllText(jsonFile.FullPath);
			}
			catch (Exception ex)
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Json file not found {jsonFile.FullPath}");
				throw new CakeException($"Json file not found {jsonFile.FullPath}");
			}

			try
			{
				RootConfiguration result = JsonConvert.DeserializeObject<RootConfiguration>(fileContent);
				context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Read json configuration file {jsonFile.FullPath} with success");
				return new ConfigurationEngine(context, result);
			}
			catch (Exception ex)
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Invalid json format {ex}");
				throw new CakeException($"Invalid json format {ex}");
			}
		}
	}
}
