using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Storm.iOS
{
	internal class FastlaneSighCommand : BaseTool
	{
		private readonly ICakeContext _context;

		internal FastlaneSighCommand(ICakeContext context) : base(context)
		{
			_context = context;
		}

		protected override string GetToolName() => "fastlane";

		protected override IEnumerable<string> GetToolExecutableNames() => new[] { "fastlane" };

		protected override IEnumerable<FilePath> GetAlternativeToolPaths(ToolSettings settings)
		{
			if (IsWindows)
			{
				throw new CakeException("Fastlane tool is not supported on windows");
			}
			if (IsOSX)
			{
				string home = _context.Environment.GetEnvironmentVariable("HOME");
				return new[] {
					new FilePath("fastlane"),
					new FilePath($"{home}/.fastlane/bin/fastlane"),
					new FilePath("/usr/local/bin/fastlane")
				};
			}

			throw new CakeException($"Environment {_context.Environment.Platform} not supported, only Windows and OSX are supported");
		}

		public bool GetCertificate(string userName, string teamName, string bundleId, CertificateType type, string outputFile)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder()
				.Append("-u").Append(userName)
				.Append("--team_name").Append(teamName)
				.Append("--app_identifier").Append(bundleId)
				.Append("--filename").Append(outputFile);
			if(type == CertificateType.AdHoc)
			{
				builder.Append("--adhoc");
			}
			else if(type == CertificateType.Development)
			{
				builder.Append("--development");
			}

			IProcess process = RunProcess(new ToolSettings(), builder);
			process.WaitForExit();
			return (process.GetExitCode() == 0);
		}
	}
}
