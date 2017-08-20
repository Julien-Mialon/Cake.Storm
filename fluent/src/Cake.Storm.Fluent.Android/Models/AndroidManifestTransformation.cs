using System;
using System.IO;
using System.Xml.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Android.Common;
using Cake.Storm.Fluent.Android.Interfaces;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;

namespace Cake.Storm.Fluent.Android.Models
{
	internal class AndroidManifestTransformation : IAndroidManifestTransformationAction
	{
		private const string ANDROID_NAMESPACE = "http://schemas.android.com/apk/res/android";
		private const string PARAMETER_KEY = "$PARAMETER$";
		
		private string _package = PARAMETER_KEY;
		private string _versionName = PARAMETER_KEY;
		private int _versionCode = -1;
		
		public IAndroidManifestTransformation WithPackage(string package)
		{
			_package = package;
			return this;
		}

		public IAndroidManifestTransformation WithVersionName(string versionName)
		{
			_versionName = versionName;
			return this;
		}

		public IAndroidManifestTransformation WithVersionCode(int versionCode)
		{
			_versionCode = versionCode;
			return this;
		}

		public IAndroidManifestTransformation WithPackageFromParameter()
		{
			_package = PARAMETER_KEY;
			return this;
		}

		public IAndroidManifestTransformation WithVersionNameFromParameter()
		{
			_versionName = PARAMETER_KEY;
			return this;
		}

		public IAndroidManifestTransformation WithVersionCodeFromParameter()
		{
			_versionCode = -1;
			return this;
		}

		public void Execute(FilePath manifestFile, IConfiguration configuration)
		{
			configuration.FileExistsOrThrow(manifestFile.FullPath);

			XElement root = null;
			XDocument document = null;
			try
			{
				using (Stream inputStream = configuration.Context.CakeContext.FileSystem.GetFile(manifestFile).OpenRead())
				{
					document = XDocument.Load(inputStream);
				}

				root = document.Root;
			}
			catch (Exception ex)
			{
				configuration.Context.CakeContext.LogAndThrow($"Invalid manifest file {manifestFile.FullPath}", ex);
			}

			if (root == null)
			{
				configuration.Context.CakeContext.LogAndThrow($"Invalid manifest file {manifestFile.FullPath}");
				throw new Exception();
			}

			XAttribute packageAttribute = GetAttribute(root, XName.Get("package"));
			XAttribute versionNameAttribute = GetAttribute(root, XName.Get("versionName", ANDROID_NAMESPACE));
			XAttribute versionCodeAttribute = GetAttribute(root, XName.Get("versionCode", ANDROID_NAMESPACE));

			XAttribute GetAttribute(XElement element, XName name)
			{
				XAttribute attribute = element.Attribute(name);
				if (attribute == null)
				{
					configuration.Context.CakeContext.LogAndThrow($"Invalid manifest file {manifestFile.FullPath} missing {name.LocalName} attribute on root");
				}
				return attribute;
			}

			string package = _package != PARAMETER_KEY ? _package : configuration.GetSimple<string>(AndroidConstants.ANDROID_PACKAGE_KEY);
			string versionName = _versionName;
			int versionCode = _versionCode;

			if (versionName == PARAMETER_KEY || versionCode < 0)
			{
				string versionFromConfiguration = configuration.GetSimple<string>(ConfigurationConstants.VERSION_KEY);
				if (!Version.TryParse(versionFromConfiguration, out Version configurationVersion))
				{
					configuration.Context.CakeContext.LogAndThrow($"Invalid version {versionFromConfiguration} for android application");
				}

				if (versionName == PARAMETER_KEY)
				{
					versionName = $"{configurationVersion.Major}.{configurationVersion.Minor}";
				}

				if (versionCode < 0)
				{
					versionCode = configurationVersion.Build;
				}
			}

			if (versionCode < 1)
			{
				versionCode = 1; //version code of 0 is invalid, set to 1 minimum
			}

			packageAttribute.SetValue(package);
			versionNameAttribute.SetValue(versionName);
			versionCodeAttribute.SetValue(versionCode);

			using (Stream outputStream = configuration.Context.CakeContext.FileSystem.GetFile(manifestFile).OpenWrite())
			{
				document.Save(outputStream);
			}
		}
	}
}
