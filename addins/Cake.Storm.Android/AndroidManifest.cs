using System;
using System.Xml.Linq;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

namespace Cake.Storm.Android
{
	public class AndroidManifest
	{
		private const string ANDROID_NAMESPACE = "http://schemas.android.com/apk/res/android";

		private readonly ICakeContext _context;
		private readonly string _path;
		private readonly XDocument _document;

		internal AndroidManifest(ICakeContext context, string path)
		{
			_context = context;
			_path = path;

			try
			{
				_document = XDocument.Load(path);
			}
			catch (Exception ex)
			{
				_context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Invalid manifest file {path}");
				throw new CakeException($"Invalid manifest file {path}", ex);
			}
		}

		public void SetPackage(string package)
		{
			if (string.IsNullOrEmpty(package))
			{
				return;
			}

			XName attributeName = XName.Get("package");

			_document.Root.SetAttributeValue(attributeName, package);
		}

		public void SetVersionName(string versionName)
		{
			if (string.IsNullOrEmpty(versionName))
			{
				return;
			}

			XName attributeName = XName.Get("versionName", ANDROID_NAMESPACE);

			_document.Root.SetAttributeValue(attributeName, versionName);
		}

		public void SetVersionCode(string versionCode)
		{
			if (string.IsNullOrEmpty(versionCode))
			{
				return;
			}

			int intVersionCode;
			if (!int.TryParse(versionCode, out intVersionCode))
			{
				_context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"VersionCode for android manifest must be an integer, got {versionCode}");
				throw new CakeException($"VersionCode for android manifest must be an integer, got {versionCode}");
			}

			XName attributeName = XName.Get("versionCode", ANDROID_NAMESPACE);

			_document.Root.SetAttributeValue(attributeName, intVersionCode);
		}

		public void Log()
		{
			_context.Log.Write(Verbosity.Normal, LogLevel.Information, _document.ToString());
		}

		public void Save()
		{
			_document.Save(_path);
		}

		public void Save(FilePath newPath)
		{
			_document.Save(newPath.FullPath);
		}
	}
}
