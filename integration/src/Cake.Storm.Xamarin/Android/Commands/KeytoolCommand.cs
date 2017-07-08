using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;
using Cake.Storm.Xamarin.Common.Commands;

namespace Cake.Storm.Xamarin.Android.Commands
{
	internal class KeytoolCommand : BaseCommand
	{
		private const int VALIDITY = 365 * 100;
		private readonly ICakeContext _context;

		internal KeytoolCommand(ICakeContext context) : base(context)
		{
			_context = context;
		}

		protected override string GetToolName() => "keytool";

		protected override IEnumerable<string> GetToolExecutableNames() => new[] { "keytool" };

		protected override IEnumerable<FilePath> GetAlternativeToolPaths(ToolSettings settings)
		{
			if (IsWindows)
			{
				//find it in jdk
				string programFiles = _context.Environment.GetSpecialPath(SpecialPath.ProgramFiles).FullPath;
				string programFilesX86 = _context.Environment.GetSpecialPath(SpecialPath.ProgramFilesX86).FullPath;

				return new[] {
						new FilePath("keytool.exe")
					}.Concat(_context.Globber.GetFiles($"{programFiles}/Java/*/bin/keytool.exe"))
					.Concat(_context.Globber.GetFiles($"{programFiles}/Java/*/jre/bin/keytool.exe"))
					.Concat(_context.Globber.GetFiles($"{programFilesX86}/Java/*/bin/keytool.exe"))
					.Concat(_context.Globber.GetFiles($"{programFilesX86}/Java/*/jre/bin/keytool.exe"));
			}
			if (IsOSX)
			{
				return new[] {
						new FilePath("keytool"),
						new FilePath("/usr/bin/keytool"),
						new FilePath("/System/Library/Frameworks/JavaVM.framework/Versions/Current/Commands/keytool"),
					}.Concat(_context.Globber.GetFiles("/Library/Java/JavaVirtualMachines/*/Contents/Home/bin/keytool"))
					.Concat(_context.Globber.GetFiles("/Library/Java/JavaVirtualMachines/*/Contents/Home/jre/bin/keytool"));
			}

			throw new CakeException($"Environment {_context.Environment.Platform} not supported, only Windows and OSX are supported");
		}

		public bool IsRightPassword(FilePath keystore, string password)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder()
				.Append("-list")
				.Append("-keystore")
				.AppendQuoted(keystore.MakeAbsolute(_context.Environment).FullPath)
				.Append("-storepass")
				.AppendQuotedSecret(password);

			return ExecuteProcess(builder);
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

			return ExecuteProcess(builder);
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

			return ExecuteProcess(builder);
		}
	}
}
