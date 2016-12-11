using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Storm.Android
{
	internal abstract class BaseTool : Tool<ToolSettings>
	{
		private readonly ICakeContext _context;

		internal BaseTool(ICakeContext context) : base(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools)
		{
			_context = context;
		}

		protected bool IsWindows => _context.Environment.Platform.Family == PlatformFamily.Windows;

		protected bool IsOSX => _context.Environment.Platform.Family == PlatformFamily.OSX ||
										(_context.Environment.Platform.Family == PlatformFamily.Linux && _context.Environment.Platform.Is64Bit && _context.Environment.Platform.IsUnix());
	}
	
}
