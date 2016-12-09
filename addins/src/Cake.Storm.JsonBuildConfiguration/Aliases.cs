using System;
using System.IO;
using System.Text;
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
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Json file not found {jsonFile.FullPath} {ex.Message}");
				throw new CakeException($"Json file not found {jsonFile.FullPath} {ex.Message}", ex);
			}

			try
			{
				RootConfiguration result = JsonConvert.DeserializeObject<RootConfiguration>(fileContent);
				context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Read json configuration file {jsonFile.FullPath} with success");
				return new ConfigurationEngine(context, result);
			}
			catch (Exception ex)
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Invalid json format {ex.Message}");
				throw new CakeException($"Invalid json format {ex.Message}", ex);
			}
		}

		[CakeMethodAlias]
		public static void LogJsonBuildConfiguration(this ICakeContext context, AndroidBuildConfiguration configuration)
		{
			StringBuilder builder = new StringBuilder();

			LogGeneralConfiguration(builder, configuration);

			builder.AppendLine("=== Android configuration ===");
			builder.AppendLine($"\tPackage: {configuration.Package}");
			builder.AppendLine($"\tVersion: {configuration.Version}");
			builder.AppendLine($"\tVersion Code: {configuration.VersionCode}");
			builder.AppendLine($"\tManifest file: {configuration.ManifestFile}");
			builder.AppendLine("\tKeystore: ");
			builder.AppendLine($"\t\tfile: {configuration.KeystoreFile}");
			builder.AppendLine($"\t\tpassword: {configuration.KeystorePassword}");
			builder.AppendLine($"\t\tauthority: {configuration.KeystoreAuthority}");
			builder.AppendLine($"\t\tkey alias: {configuration.KeystoreKeyAlias}");
			builder.AppendLine($"\t\tkey password: {configuration.KeystoreKeyPassword}");
			builder.AppendLine("=============================");


			context.Log.Write(Verbosity.Normal, LogLevel.Information, builder.ToString());
		}

		[CakeMethodAlias]
		public static void LogJsonBuildConfiguration(this ICakeContext context, iOSBuildConfiguration configuration)
		{
			StringBuilder builder = new StringBuilder();

			LogGeneralConfiguration(builder, configuration);

			builder.AppendLine("=== iOS configuration ===");
			builder.AppendLine($"\tBundle: {configuration.Bundle}");
			builder.AppendLine($"\tVersion: {configuration.Version}");
			builder.AppendLine($"\tBuild version: {configuration.BuildVersion}");
			builder.AppendLine("=============================");

			context.Log.Write(Verbosity.Normal, LogLevel.Information, builder.ToString());
		}

		[CakeMethodAlias]
		public static void LogJsonBuildConfiguration(this ICakeContext context, DotNetBuildConfiguration configuration)
		{
			StringBuilder builder = new StringBuilder();

			LogGeneralConfiguration(builder, configuration);

			builder.AppendLine("=== DotNet configuration ===");
			builder.AppendLine("=============================");

			context.Log.Write(Verbosity.Normal, LogLevel.Information, builder.ToString());
		}

		private static void LogGeneralConfiguration(StringBuilder builder, Configuration configuration)
		{
			builder.AppendLine($"Build configuration for platform={configuration.PlatformName}, target={configuration.TargetName}, app={configuration.AppName}");

			builder.AppendLine($"=== Build configuration === ");
			builder.AppendLine($"\tSolution file: {configuration.Solution}");
			builder.AppendLine($"\tProject file: {configuration.Project}");
			builder.AppendLine($"\tBuild properties: {(configuration.BuildProperties.Count == 0 ? "none" : "")}");
			foreach (var property in configuration.BuildProperties)
			{
				builder.AppendLine($"\t\t{property.Key}={property.Value}");
			}
		}
	}
}
