using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Common.Tools.NuGet;
using Newtonsoft.Json;
using Cake.Common.IO;
using Cake.Common.Tools.NuGet.Pack;
using Cake.Common.Tools.NuGet.Push;

namespace Cake.Storm.Nuget
{
	public static class Aliases
	{
		[CakeMethodAlias]
		public static NugetAppConfiguration ReadNugetConfiguration(this ICakeContext context, FilePath file, string name)
		{
			List<NugetAppConfiguration> configurations = context.ReadNugetConfigurations(file);

			NugetAppConfiguration result = configurations.FirstOrDefault(x => x.Name == name);

			if (result == null)
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Nuget configuration {name} not found in file {file}");
				throw new CakeException($"Nuget configuration {name} not found in file {file}");
			}

			return result;
		}

		[CakeMethodAlias]
		public static List<NugetAppConfiguration> ReadNugetConfigurations(this ICakeContext context, FilePath file)
		{
			if (!context.FileSystem.Exist(file))
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Nuget configuration file {file} not found");
				throw new CakeException($"Nuget configuration file {file} not found");
			}

			string fileContent = null;
			try
			{
				fileContent = File.ReadAllText(file.FullPath);
			}
			catch (Exception ex)
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Json file not found {file.FullPath} {ex.Message}");
				throw new CakeException($"Json file not found {file.FullPath} {ex.Message}", ex);
			}

			try
			{
				List<NugetAppConfiguration> result = JsonConvert.DeserializeObject<List<NugetAppConfiguration>>(fileContent);
				context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Read json configuration file {file.FullPath} with success");
				return result;
			}
			catch (Exception ex)
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Invalid json format {ex.Message}");
				throw new CakeException($"Invalid json format {ex.Message}", ex);
			}
		}

		[CakeMethodAlias]
		public static void CreateNuspecForRelease(this ICakeContext context, NugetAppConfiguration configuration)
		{
			if (string.IsNullOrEmpty(configuration.NuspecFile))
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Nuspec file {configuration.NuspecFile} not found");
				throw new CakeException($"Nuspec file {configuration.NuspecFile} not found");
			}

			string output = System.IO.Path.Combine(configuration.Path, $"{configuration.Id}.nuspec");

			File.WriteAllText(output, File.ReadAllText(configuration.NuspecFile)
							  .Replace("{id}", configuration.Id)
			                  .Replace("{title}", configuration.Title ?? configuration.Id)
							  .Replace("{author}", configuration.Author)
							  .Replace("{version}", configuration.Version)
			                  .Replace("{release_notes}", string.IsNullOrEmpty(configuration.ReleaseNoteFile) ? "" : File.ReadAllText(configuration.ReleaseNoteFile))
			                 );
		}

		[CakeMethodAlias]
		public static void NuGetPack(this ICakeContext context, NugetAppConfiguration configuration)
		{
			if (string.IsNullOrEmpty(configuration.PackOutput))
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"packOutput not set in nuget configuration");
				throw new CakeException($"packOutput not set in nuget configuration");
			}

			DirectoryPath directory = new FilePath(configuration.PackOutput).GetDirectory();
			context.EnsureDirectoryExists(directory);

			context.NuGetPack(context.MakeAbsolute(new FilePath(System.IO.Path.Combine(configuration.Path, $"{configuration.Id}.nuspec"))), new NuGetPackSettings
			{
				OutputDirectory = directory
			});
		}

		[CakeMethodAlias]
		public static void NuGetPush(this ICakeContext context, NugetAppConfiguration configuration)
		{
			if (string.IsNullOrEmpty(configuration.PackOutput))
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"packOutput not set in nuget configuration");
				throw new CakeException($"packOutput not set in nuget configuration");
			}
			string apiKey = context.Environment.GetEnvironmentVariable("NUGET_API_KEY");
			if (string.IsNullOrEmpty(apiKey))
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"NUGET_API_KEY environment variable not set");
				throw new CakeException($"NUGET_API_KEY environment variable not set");
			}

			string outputDirectory = context.MakeAbsolute(new DirectoryPath(configuration.PackOutput)).FullPath;
			DirectoryInfo dir = new DirectoryInfo(outputDirectory);

			string searchPattern = System.IO.Path.Combine(dir.Parent.FullName, $"{configuration.Id}.*.nupkg");
			FilePath packageFile = context.Globber
			                              .GetFiles(searchPattern)
			                              .OrderBy(f => new FileInfo(f.FullPath).LastWriteTimeUtc)
			                              .FirstOrDefault();

			if (packageFile == null)
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"can not found package for {configuration.Name}, search pattern: {searchPattern}");
				throw new CakeException($"can not found package for {configuration.Name}, search pattern: {searchPattern}");
			}

			context.NuGetPush(packageFile, new NuGetPushSettings
			{
				ApiKey = apiKey,
				Source = configuration.PushSource ?? "https://www.nuget.org/api/v2/package"
			});
		}
	}
}
