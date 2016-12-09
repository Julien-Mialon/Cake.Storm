using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Storm.Android
{
	internal class KeytoolCommand : Tool<ToolSettings>
	{
		private const int VALIDITY = 365 * 100;
		private ICakeContext _context;

		internal KeytoolCommand(ICakeContext context) : base(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools)
		{
			_context = context;
		}

		protected override string GetToolName() => "keytool";

		protected override IEnumerable<string> GetToolExecutableNames() => new[] { "keytool" };

		protected override IEnumerable<FilePath> GetAlternativeToolPaths(ToolSettings settings)
		{
			if (_context.Environment.Platform.Family == PlatformFamily.Windows)
			{
				//find it in jdk
				return new[] {
					new FilePath("keytool")
				}.Concat(_context.Globber.GetFiles("/Program Files/Java/*/bin/keytool"))
				 .Concat(_context.Globber.GetFiles("/Program Files/Java/*/jre/bin/keytool"));
			}
			else if (_context.Environment.Platform.Family == PlatformFamily.OSX)
			{
				return new[] {
					new FilePath("keytool"),
					new FilePath("/usr/bin/keytool"),
					new FilePath("/System/Library/Frameworks/JavaVM.framework/Versions/Current/Commands/keytool"),
				}.Concat(_context.Globber.GetFiles("/Library/Java/JavaVirtualMachines/*/Contents/Home/bin/keytool"))
				 .Concat(_context.Globber.GetFiles("/Library/Java/JavaVirtualMachines/*/Contents/Home/jre/bin/keytool"));
			}
			else
			{
				throw new CakeException($"Environment {_context.Environment.Platform} not supported, only Windows and OSX are supported");
			}
		}

		public bool IsRightPassword(FilePath keystore, string password)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();

			builder.Append("-list");
			builder.Append("-keystore");
			builder.AppendQuoted(keystore.MakeAbsolute(_context.Environment).FullPath);
			builder.Append("-storepass");
			builder.AppendQuotedSecret(password);

			IProcess process = RunProcess(new ToolSettings(), builder);
			process.WaitForExit();
			return (process.GetExitCode() == 0);
		}

		public bool HasAlias(FilePath keystore, string password, string alias)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder()
				.Append("-list")
				.Append("-keystore")
				.AppendQuoted(keystore.MakeAbsolute(_context.Environment).FullPath)
				.Append("-storepass")
				.AppendQuotedSecret(password)
				.Append("-alias")
				.Append(alias);

			IProcess process = RunProcess(new ToolSettings(), builder);
			process.WaitForExit();
			return (process.GetExitCode() == 0);
		}

		public bool CreateAlias(FilePath keystore, string password, string alias, string aliasPassword, string authority)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder()
				.Append("-genkey")
				.Append("-keystore")
				.AppendQuoted(keystore.MakeAbsolute(_context.Environment).FullPath)
				.Append("-storepass")
				.AppendQuotedSecret(password)
				.Append("-dname")
				.AppendQuoted(authority)
				.Append("-alias")
				.Append(alias)
				.Append("-keypass")
				.AppendQuoted(aliasPassword)
				.Append("-keyalg")
				.Append("RSA")
				.Append("-keysize")
				.Append("2048")
				.Append("-validity")
				.Append(VALIDITY.ToString());

			IProcess process = RunProcess(new ToolSettings(), builder);
			process.WaitForExit();
			return (process.GetExitCode() == 0);
		}
	}
}
