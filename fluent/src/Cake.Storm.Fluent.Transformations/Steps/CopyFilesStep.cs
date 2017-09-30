using System;
using System.Collections.Generic;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;
using Cake.Storm.Fluent.Transformations.Models;

namespace Cake.Storm.Fluent.Transformations.Steps
{
	[PreBuildStep]
	[MultiStep]
	internal class CopyFilesStep : IStep
	{
		private string _source;
		private readonly PathItemType _sourceType;
		private string _target;
		private readonly PathItemType _targetType;

		public CopyFilesStep(string source, PathItemType sourceType, string target, PathItemType targetType)
		{
			_source = source;
			_sourceType = sourceType;
			_target = target;
			_targetType = targetType;
		}

		public void Execute(IConfiguration configuration)
		{
			_source = configuration.AddRootDirectory(_source);
			_target = configuration.AddRootDirectory(_target);
			
			ValidateParameters(configuration);
			
			if (_sourceType == PathItemType.File)
			{
				FilePath source = new FilePath(_source);
				if (_targetType == PathItemType.Directory)
				{
					DirectoryPath target = new DirectoryPath(_target);
					configuration.Context.CakeContext.CopyFileToDirectory(source, target);
				}
				else if (_targetType == PathItemType.File)
				{
					FilePath target = new FilePath(_target);
					configuration.Context.CakeContext.CopyFile(source, target);
				}
				else
				{
					throw new NotSupportedException($"Could not copy with source {_sourceType} to {_targetType}");
				}
			}
			else if (_sourceType == PathItemType.Directory)
			{
				DirectoryPath source = new DirectoryPath(_source);
				if (_targetType == PathItemType.Directory)
				{
					DirectoryPath target = new DirectoryPath(_target);
					configuration.Context.CakeContext.CopyDirectory(source, target);
				}
				else
				{
					throw new NotSupportedException($"Could not copy with source {_sourceType} to {_targetType}");
				}
			}
			else if (_sourceType == PathItemType.Pattern)
			{
				IEnumerable<FilePath> sources = configuration.Context.CakeContext.Globber.GetFiles(_source);
				if (_targetType == PathItemType.Directory)
				{
					DirectoryPath target = new DirectoryPath(_target);
					configuration.Context.CakeContext.CopyFiles(sources, target);
				}
				else
				{
					throw new NotSupportedException($"Could not copy with source {_sourceType} to {_targetType}");
				}
			}
			else
			{
				throw new NotSupportedException($"Could not copy with source {_sourceType} to {_targetType}");
			}
		}

		private void ValidateParameters(IConfiguration configuration)
		{
			switch (_sourceType)
			{
				case PathItemType.File:
					if (!configuration.Context.CakeContext.FileExists(_source))
					{
						configuration.LogAndThrow($"File {_source} does not exists");
					}
					break;
				case PathItemType.Directory:
					if (!configuration.Context.CakeContext.DirectoryExists(_source))
					{
						configuration.LogAndThrow($"Directory {_source} does not exists");
					}
					break;
				case PathItemType.Pattern:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			switch (_targetType)
			{
				case PathItemType.File:
					break;
				case PathItemType.Directory:
					if (!configuration.Context.CakeContext.DirectoryExists(_target))
					{
						configuration.LogAndThrow($"Directory {_target} does not exists");
					}
					break;
				case PathItemType.Pattern:
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}