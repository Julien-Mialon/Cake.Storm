using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;
using Cake.Storm.Fluent.Commands;

namespace Cake.Storm.Fluent.Android.Commands
{
	internal class ApksignerCommand : BaseCommand
	{
		private readonly ICakeContext _context;

		internal ApksignerCommand(ICakeContext context) : base(context)
		{
			_context = context;
		}

		protected override string GetToolName() => "apksigner";

		protected override IEnumerable<string> GetToolExecutableNames() => new[] { "apksigner" };

		protected override IEnumerable<FilePath> GetAlternativeToolPaths(ToolSettings settings)
		{
			if (IsWindows)
			{
				//find it in jdk
				string programFiles = _context.Environment.GetSpecialPath(SpecialPath.ProgramFiles).FullPath;
				string programFilesX86 = _context.Environment.GetSpecialPath(SpecialPath.ProgramFilesX86).FullPath;

				return new[] {
						new FilePath("apksigner.bat")
					}.Concat(
						_context.Globber.GetFiles($"{programFiles}/Android/android-sdk/build-tools/*/apksigner.bat")
							.Concat(_context.Globber.GetFiles($"{programFiles}/Android/build-tools/*/apksigner.bat"))
							.Concat(_context.Globber.GetFiles($"{programFilesX86}/Android/android-sdk/build-tools/*/apksigner.bat"))
							.Concat(_context.Globber.GetFiles($"{programFilesX86}/Android/build-tools/*/apksigner.bat"))
							.Reverse() //Reverse to take the most recent version of build-tools if more than one is installed
					);
			}
			if (IsOSX)
			{
				string home = _context.Environment.GetEnvironmentVariable("HOME");
				return new[] {
					new FilePath("apksigner"),
				}.Concat(_context.Globber.GetFiles("/Library/Developer/Xamarin/android-sdk-macosx/build-tools/*/apksigner"))
				.Concat(_context.Globber.GetFiles($"{home}/Library/Developer/Xamarin/android-sdk-macosx/build-tools/*/apksigner"))
				.Reverse();
			}

			throw new CakeException($"Environment {_context.Environment.Platform} not supported, only Windows and OSX are supported");
		}

		public bool SignApk(FilePath inputApk, FilePath outputApk, FilePath keystore, string password, string alias, string aliasPassword)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder()
					.Append("sign")
					.Append("--ks")
					.AppendQuoted(keystore.MakeAbsolute(_context.Environment).FullPath)
					.Append("--ks-pass")
					.AppendQuotedSecret($"pass:{password}")
					.Append("--ks-key-alias")
					.Append(alias)
					.Append("--key-pass")
					.AppendQuotedSecret($"pass:{aliasPassword}")
					.Append("--out")
					.AppendQuoted(outputApk.MakeAbsolute(_context.Environment).FullPath)
					.AppendQuoted(inputApk.MakeAbsolute(_context.Environment).FullPath)
				;

			return ExecuteProcess(builder);
		}

		public bool VerifyApk(FilePath apkFile)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder()
				.Append("verify")
				.Append("--print-certs")
				.Append("-v")
				.AppendQuoted(apkFile.MakeAbsolute(_context.Environment).FullPath);

			return ExecuteProcess(builder);
		}
	}
}
