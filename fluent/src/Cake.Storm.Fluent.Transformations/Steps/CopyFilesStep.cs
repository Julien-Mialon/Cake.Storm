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
	internal class CopyFilesStep : BaseMultipleStep
	{
		private readonly string _source;
		private readonly PathItemType _sourceType;
		private readonly string _target;
		private readonly PathItemType _targetType;

		public CopyFilesStep(string source, PathItemType sourceType, string target, PathItemType targetType, StepType onStep) : base(onStep)
		{
			_source = source;
			_sourceType = sourceType;
			_target = target;
			_targetType = targetType;
		}

		protected override void Execute(IConfiguration configuration)
		{
			string sourcePath = configuration.AddRootDirectory(_source);
			string targetPath = configuration.AddRootDirectory(_target);

			ValidateParameters(configuration, sourcePath, targetPath);

			if (_sourceType == PathItemType.File)
			{
				FilePath source = new FilePath(sourcePath);
				if (_targetType == PathItemType.Directory)
				{
					DirectoryPath target = new DirectoryPath(targetPath);
					configuration.Context.CakeContext.CopyFileToDirectory(source, target);
				}
				else if (_targetType == PathItemType.File)
				{
					FilePath target = new FilePath(targetPath);
					configuration.Context.CakeContext.CopyFile(source, target);
				}
				else
				{
					throw new NotSupportedException($"Could not copy with source {_sourceType} to {_targetType}");
				}
			}
			else if (_sourceType == PathItemType.Directory)
			{
				DirectoryPath source = new DirectoryPath(sourcePath);
				if (_targetType == PathItemType.Directory)
				{
					DirectoryPath target = new DirectoryPath(targetPath);
					configuration.Context.CakeContext.CopyDirectory(source, target);
				}
				else
				{
					throw new NotSupportedException($"Could not copy with source {_sourceType} to {_targetType}");
				}
			}
			else if (_sourceType == PathItemType.Pattern)
			{
				IEnumerable<FilePath> sources = configuration.Context.CakeContext.Globber.GetFiles(sourcePath);
				if (_targetType == PathItemType.Directory)
				{
					DirectoryPath target = new DirectoryPath(targetPath);
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

		private void ValidateParameters(IConfiguration configuration, string sourcePath, string targetPath)
		{
			switch (_sourceType)
			{
				case PathItemType.File:
					if (!configuration.Context.CakeContext.FileExists(sourcePath))
					{
						configuration.LogAndThrow($"File {sourcePath} does not exists");
					}
					break;
				case PathItemType.Directory:
					if (!configuration.Context.CakeContext.DirectoryExists(sourcePath))
					{
						configuration.LogAndThrow($"Directory {sourcePath} does not exists");
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
					if (!configuration.Context.CakeContext.DirectoryExists(targetPath))
					{
						configuration.LogAndThrow($"Directory {targetPath} does not exists");
					}
					break;
				case PathItemType.Pattern:
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}