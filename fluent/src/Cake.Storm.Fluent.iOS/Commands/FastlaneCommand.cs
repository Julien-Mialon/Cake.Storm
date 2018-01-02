using System.Collections.Generic;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;
using Cake.Storm.Fluent.Commands;
using Cake.Storm.Fluent.iOS.Models;

namespace Cake.Storm.Fluent.iOS.Commands
{
	internal class FastlaneCommand : BaseCommand
	{
		private readonly ICakeContext _context;

		internal FastlaneCommand(ICakeContext context) : base(context)
		{
			_context = context;
		}

		protected override string GetToolName() => "fastlane";

		protected override IEnumerable<string> GetToolExecutableNames() => new[] { "fastlane" };

		protected override IEnumerable<FilePath> GetAlternativeToolPaths(ToolSettings settings)
		{
			if (IsOSX)
			{
				string home = _context.Environment.GetEnvironmentVariable("HOME");
				return new[] {
					new FilePath("fastlane"),
					new FilePath($"{home}/.fastlane/bin/fastlane"),
					new FilePath("/usr/local/bin/fastlane")
				};
			}

			throw new CakeException($"Environment {_context.Environment.Platform} not supported, only OSX is supported");
		}

		public bool SynchronizeProvisionningProfile(string userName, string teamName, string bundleId, CertificateType type, string outputFile, bool isAdmin = true)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder()
				.Append("sigh")
				.Append("-u").Append(userName)
				.Append("--team_name").AppendQuoted(teamName)
				.Append("--app_identifier").Append(bundleId)
				.Append("--filename").Append(outputFile);
			if (type == CertificateType.AdHoc)
			{
				builder.Append("--adhoc");
			}
			else if (type == CertificateType.Development)
			{
				builder.Append("--development");
			}
			//appstore provisioning profile type is default

			if (isAdmin)
			{
				//nothing to add
			}
			else
			{
				builder.Append("--readonly");
				builder.Append("--skip_certificate_verification");
			}

			return ExecuteProcess(builder);
		}
	}
}
