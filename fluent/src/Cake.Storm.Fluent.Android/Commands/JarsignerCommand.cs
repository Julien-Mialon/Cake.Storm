using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;
using Cake.Storm.Fluent.Commands;

namespace Cake.Storm.Fluent.Android.Commands
{
    internal class JarsignerCommand : BaseCommand
    {
	    private readonly ICakeContext _context;

	    internal JarsignerCommand(ICakeContext context) : base(context)
	    {
		    _context = context;
	    }

	    protected override string GetToolName() => "jarsigner";

	    protected override IEnumerable<string> GetToolExecutableNames() => new[] { "jarsigner" };

	    protected override IEnumerable<FilePath> GetAlternativeToolPaths(ToolSettings settings)
	    {
		    if (IsWindows)
		    {
			    //find it in jdk
			    string programFiles = _context.Environment.GetSpecialPath(SpecialPath.ProgramFiles).FullPath;
			    string programFilesX86 = _context.Environment.GetSpecialPath(SpecialPath.ProgramFilesX86).FullPath;

			    return new[] {
					    new FilePath("jarsigner.exe")
				    }.Concat(_context.Globber.GetFiles($"{programFiles}/Java/*/bin/jarsigner.exe"))
				    .Concat(_context.Globber.GetFiles($"{programFilesX86}/Java/*/bin/jarsigner.exe"))
				    .Concat(_context.Globber.GetFiles($"{programFiles}/Android/jdk/*/bin/jarsigner.exe"))//Microsoft openjdk location
				    .Concat(_context.Globber.GetFiles($"{programFilesX86}/Android/jdk/*/bin/jarsigner.exe"));
		    }
		    if (IsOSX)
		    {
			    return new[] {
				    new FilePath("jarsigner"),
				    new FilePath("/usr/bin/jarsigner"),
				    new FilePath("/System/Library/Frameworks/JavaVM.framework/Versions/Current/Commands/jarsigner"),
			    }.Concat(_context.Globber.GetFiles("/Library/Java/JavaVirtualMachines/*/Contents/Home/bin/jarsigner"))
				.Concat(_context.Globber.GetFiles("/Users/*/Library/Developer/Xamarin/jdk/*/bin/jarsigner"));
		    }

		    throw new CakeException($"Environment {_context.Environment.Platform} not supported, only Windows and OSX are supported");
	    }

	    public bool SignApk(FilePath inputApk, FilePath outputApk, FilePath keystore, string password, string alias, string aliasPassword)
	    {
		    ProcessArgumentBuilder builder = new ProcessArgumentBuilder()
				    .Append("-sigalg")
				    .Append("SHA1withRSA")
				    .Append("-digestalg")
				    .Append("SHA1")
				    .Append("-keystore")
				    .AppendQuoted(keystore.MakeAbsolute(_context.Environment).FullPath)
				    .AppendQuoted(inputApk.MakeAbsolute(_context.Environment).FullPath)
				    .Append(alias)
				    .Append("-storepass")
				    .AppendQuotedSecret(password)
				    .Append("-keypass")
				    .AppendQuotedSecret(aliasPassword)
				    .Append("-signedjar")
				    .AppendQuoted(outputApk.MakeAbsolute(_context.Environment).FullPath)
			    ;

		    return ExecuteProcess(builder);
	    }

	    public bool VerifyApk(FilePath apkFile)
	    {
		    ProcessArgumentBuilder builder = new ProcessArgumentBuilder()
			    .Append("-verify")
			    .Append("-certs")
			    .AppendQuoted(apkFile.MakeAbsolute(_context.Environment).FullPath);

			return ExecuteProcess(builder);
		}
	}
}
