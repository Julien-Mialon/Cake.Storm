using System;
using System.IO;
using System.Xml.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Transformations.Interfaces;

namespace Cake.Storm.Fluent.Transformations.Models
{
	internal class CsprojTransformation : ICsprojTransformationAction
	{
		private const string PARAMETER_KEY = "$PARAMETER$";

		private string _packageVersion = null; 
		
		public ICsprojTransformation UpdatePackageVersion(string newVersion)
		{
			_packageVersion = newVersion;
			return this;
		}

		public ICsprojTransformation UpdatePackageVersionFromParameter()
		{
			_packageVersion = PARAMETER_KEY;
			return this;
		}

		public void Execute(FilePath projectFile, IConfiguration configuration)
		{
			if (_packageVersion == null)
			{
				return;
			}

			XDocument document;
			using (Stream inputStream = configuration.Context.CakeContext.FileSystem.GetFile(projectFile).OpenRead())
			{
				document = XDocument.Load(inputStream);
			}
			
			if (document.Root == null)
			{
				configuration.Context.CakeContext.LogAndThrow($"Invalid csproj file format for {projectFile.FullPath}");
				throw new Exception();
			}

			string version = _packageVersion == PARAMETER_KEY ? configuration.GetSimple<string>(ConfigurationConstants.VERSION_KEY) : _packageVersion;
			foreach (XElement packageVersionNodes in document.Root.Descendants(XName.Get("PackageVersion")))
			{
				packageVersionNodes.SetValue(version);
			}
			
			using (Stream outputStream = configuration.Context.CakeContext.FileSystem.GetFile(projectFile).OpenWrite())
			{
				document.Save(outputStream);
			}
		}
	}
}