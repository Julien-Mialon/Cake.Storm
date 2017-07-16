using System;
using Cake.Core.Diagnostics;

// ReSharper disable once CheckNamespace
namespace Cake.Core
{
    public static class CakeContextExtensions
    {
	    public static void LogAndThrow(this ICakeContext context, string message)
	    {
			context.Log.Write(Verbosity.Quiet, LogLevel.Fatal, message);
			throw new CakeException(message);
	    }

		public static void LogAndThrow(this ICakeContext context, string message, Exception innerException)
	    {
			context.Log.Write(Verbosity.Quiet, LogLevel.Fatal, message);
		    throw new CakeException(message, innerException);
		}
    }
}
