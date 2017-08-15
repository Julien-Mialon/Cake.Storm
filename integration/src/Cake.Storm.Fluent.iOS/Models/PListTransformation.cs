using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.iOS.Common;
using Cake.Storm.Fluent.iOS.Interfaces;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;

namespace Cake.Storm.Fluent.iOS.Models
{
	internal class PListTransformation : IPListTransformationAction
	{
		private const string PARAMETER_KEY = "$PARAMETER$";

		private const string BUNDLE_KEY = "CFBundleIdentifier";
		private const string VERSION_KEY = "CFBundleShortVersionString";
		private const string BUILD_VERSION_KEY = "CFBundleVersion";

		//default to be used from parameter informations
		private string _version = PARAMETER_KEY;

		private string _bundleId = PARAMETER_KEY;

		public IPListTransformation WithVersion(string version)
		{
			_version = version;
			return this;
		}

		public IPListTransformation WithVersionFromParameter()
		{
			_version = PARAMETER_KEY;
			return this;
		}

		public IPListTransformation WithBundleId(string bundleId)
		{
			_bundleId = bundleId;
			return this;
		}

		public IPListTransformation WithBundleIdFromParameter()
		{
			_bundleId = PARAMETER_KEY;
			return this;
		}

		public void Execute(FilePath filePath, IConfiguration configuration)
		{
			XDocument document;
			using (Stream inputStream = configuration.Context.CakeContext.FileSystem.GetFile(filePath).OpenRead())
			{
				document = XDocument.Load(inputStream);
			}

			string bundleId = _bundleId == PARAMETER_KEY ? configuration.GetSimple<string>(iOSConstants.BUNDLE_ID_KEY) : _bundleId;
			if (string.IsNullOrEmpty(bundleId))
			{
				configuration.Context.CakeContext.LogAndThrow("Missing bundleId for iOS PlistTransformation");
				throw new Exception();
			}

			GetValueElementForKey(document, BUNDLE_KEY)?.SetValue(bundleId);

			string versionString = _version == PARAMETER_KEY ? configuration.GetSimple<string>(ConfigurationConstants.VERSION_KEY) : _version;
			if (string.IsNullOrEmpty(versionString) || !Version.TryParse(versionString, out Version version))
			{
				configuration.Context.CakeContext.LogAndThrow($"Invalid version for iOS {versionString}");
				throw new Exception();
			}

			GetValueElementForKey(document, VERSION_KEY)?.SetValue($"{version.Major}.{version.Minor}");
			GetValueElementForKey(document, BUILD_VERSION_KEY)?.SetValue($"{version.Major}.{version.Minor}.{(version.Build > 0 ? version.Build : 0)}");

			using (Stream outputStream = configuration.Context.CakeContext.FileSystem.GetFile(filePath).OpenWrite())
			{
				document.Save(outputStream);
			}
		}

		private XElement GetValueElementForKey(XDocument document, string key)
		{
			XElement dict = document.Root?.Elements().FirstOrDefault();
			if (dict == null)
			{
				return null;
			}

			List<XElement> values = dict.Elements().ToList();
			for (int i = 0; i < values.Count; ++i)
			{
				XElement item = values[i];
				if (item.Name.LocalName == "key" && item.Value == key)
				{
					return values[i + 1];
				}
			}

			return null;
		}
	}
}