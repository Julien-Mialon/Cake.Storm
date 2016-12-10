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

		private XName PackageAttribute { get; } = XName.Get("package");

		private XName VersionNameAttribute { get; } = XName.Get("versionName", ANDROID_NAMESPACE);

		private XName VersionCodeAttribute { get; } = XName.Get("versionCode", ANDROID_NAMESPACE);

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

		public string Package
		{
			get
			{
				return _document.Root.Attribute(PackageAttribute).Value;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}

				_document.Root.SetAttributeValue(PackageAttribute, value);
			}
		}

		public string VersionName
		{
			get
			{
				return _document.Root.Attribute(VersionNameAttribute).Value;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}

				_document.Root.SetAttributeValue(VersionNameAttribute, value);
			}
		}

		public string VersionCode
		{
			get
			{
				return _document.Root.Attribute(VersionCodeAttribute).Value;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}

				int versionCode;
				if (!int.TryParse(value, out versionCode))
				{
					_context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"VersionCode for android manifest must be an integer, got {value}");
					throw new CakeException($"VersionCode for android manifest must be an integer, got {value}");
				}

				_document.Root.SetAttributeValue(VersionCodeAttribute, versionCode);
			}
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
