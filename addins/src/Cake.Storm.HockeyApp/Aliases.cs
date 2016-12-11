using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.HockeyApp;
using Newtonsoft.Json;

namespace Cake.Storm.HockeyApp
{
	public static class Aliases
	{
		[CakeMethodAlias]
		public static HockeyAppConfiguration ReadHockeyAppConfiguration(this ICakeContext context, FilePath file, string name)
		{
			List<HockeyAppConfiguration> configurations = context.ReadHockeyAppConfigurations(file);

			HockeyAppConfiguration result = configurations.FirstOrDefault(x => x.Name == name);

			if (result == null)
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Nuget configuration {name} not found in file {file}");
				throw new CakeException($"Nuget configuration {name} not found in file {file}");
			}

			return result;
		}

		[CakeMethodAlias]
		public static List<HockeyAppConfiguration> ReadHockeyAppConfigurations(this ICakeContext context, FilePath file)
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
				List<HockeyAppConfiguration> result = JsonConvert.DeserializeObject<List<HockeyAppConfiguration>>(fileContent);
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
		public static void UploadToHockeyApp(this ICakeContext context, HockeyAppConfiguration configuration)
		{
			string releaseNote = string.Empty;
			try
			{
				releaseNote = File.ReadAllText(configuration.ReleaseNoteFile);
			}
			catch (Exception)
			{
				//ignored
			}

			if (configuration.Type == AppType.Android)
			{
				if (string.IsNullOrEmpty(configuration.ApkFile))
				{
					context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"ApkFile not set in hockeyapp configuration");
					throw new CakeException($"ApkFile not set in hockeyapp configuration");
				}

				context.UploadToHockeyApp(configuration.ApkFile, new HockeyAppUploadSettings
				{
					Version = configuration.Version,
					NoteType = NoteType.Markdown,
					Notes = releaseNote,
					BuildServerUrl = "Version coming from cakebuild",
					AppId = configuration.AppId
				});
			}
			else if (configuration.Type == AppType.iOS)
			{
				if (string.IsNullOrEmpty(configuration.IpaFile))
				{
					context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"IpaFile not set in hockeyapp configuration");
					throw new CakeException($"IpaFile not set in hockeyapp configuration");
				}

				if (string.IsNullOrEmpty(configuration.DsymFile))
				{
					context.UploadToHockeyApp(configuration.ApkFile, new HockeyAppUploadSettings
					{
						NoteType = NoteType.Markdown,
						Notes = releaseNote,
						BuildServerUrl = "Version coming from cakebuild",
						AppId = configuration.AppId
					});
				}
				else
				{
					context.UploadToHockeyApp(configuration.ApkFile, configuration.DsymFile, new HockeyAppUploadSettings
					{
						NoteType = NoteType.Markdown,
						Notes = releaseNote,
						BuildServerUrl = "Version coming from cakebuild",
						AppId = configuration.AppId
					});
				}
			}
		}
	}
}
