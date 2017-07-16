using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Cake.Core.IO;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.iOS.Common;
using Cake.Storm.Fluent.iOS.Interfaces;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;

namespace Cake.Storm.Fluent.iOS.Models
{
	internal class PListTransformation : IPListTransformation
	{
		private const string PARAMETER_KEY = "$PARAMETER$";

		private const string BUNDLE_KEY = "CFBundleIdentifier";
		private const string VERSION_KEY = "CFBundleShortVersionString";
		private const string BUILD_VERSION_KEY = "CFBundleVersion";

		private string _version;
		private string _bundleId;

		public IPListTransformation UseVersion(string version)
		{
			_version = version;
			return this;
		}

		public IPListTransformation UseVersionFromParameter()
		{
			_version = PARAMETER_KEY;
			return this;
		}

		public IPListTransformation UseBundleId(string bundleId)
		{
			_bundleId = bundleId;
			return this;
		}

		public IPListTransformation UseBundleIdFromParameter()
		{
			_bundleId = PARAMETER_KEY;
			return this;
		}

		public void Execute(FilePath filePath, IConfiguration configuration)
		{
			XDocument document = XDocument.Load(filePath.FullPath);

			string bundleId = _bundleId == PARAMETER_KEY ? configuration.GetSimple<string>(iOSConstants.BUNDLE_ID_KEY) : _bundleId;
			GetValueElementForKey(document, BUNDLE_KEY)?.SetValue(bundleId);

			Version version = Version.Parse(_version == PARAMETER_KEY ? configuration.GetSimple<string>(ConfigurationConstants.VERSION_KEY) : _version);
			GetValueElementForKey(document, VERSION_KEY)?.SetValue($"{version.Major}.{version.Minor}");
			GetValueElementForKey(document, BUILD_VERSION_KEY)?.SetValue($"{version.Major}.{version.Minor}.{(version.Build > 0 ? version.Build : 0)}");
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
