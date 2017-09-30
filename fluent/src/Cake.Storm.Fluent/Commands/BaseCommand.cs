using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Storm.Fluent.Commands
{
    public abstract class BaseCommand<TSettings> : Tool<TSettings> 
		where TSettings : ToolSettings
    {
	    private readonly ICakeContext _context;

	    protected BaseCommand(ICakeContext context) : base(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools)
	    {
		    _context = context;
	    }

	    protected bool IsWindows => _context.Environment.Platform.Family == PlatformFamily.Windows;

	    protected bool IsOSX => _context.Environment.Platform.Family == PlatformFamily.OSX ||
	                            (_context.Environment.Platform.Family == PlatformFamily.Linux && _context.Environment.Platform.Is64Bit && _context.Environment.Platform.IsUnix());

	    protected bool ExecuteProcess(ProcessArgumentBuilder processArgumentBuilder, TSettings settings)
	    {
		    IProcess process = RunProcess(settings, processArgumentBuilder);
			process.WaitForExit();
		    return process.GetExitCode() == 0;
	    }
	}

	public abstract class BaseCommand : BaseCommand<ToolSettings>
	{
		protected BaseCommand(ICakeContext context) : base(context)
		{
		}

		protected bool ExecuteProcess(ProcessArgumentBuilder processArgumentBuilder) => ExecuteProcess(processArgumentBuilder, new ToolSettings());
	}
}
