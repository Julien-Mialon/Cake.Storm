using System;
using System.IO;
using System.Xml.Linq;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.Storm.Xamarin.Android.Models
{
	public class AndroidManifest
	{
		private const string ANDROID_NAMESPACE = "http://schemas.android.com/apk/res/android";

		private readonly ICakeContext _context;
		private readonly FilePath _path;
		private readonly XDocument _document;
		private readonly XAttribute _packageAttribute;
		private readonly XAttribute _versionNameAttribute;
		private readonly XAttribute _versionCodeAttribute;
		
		internal AndroidManifest(ICakeContext context, FilePath path)
		{
			_context = context;
			_path = path;

			XElement root = null;
			try
			{
				using (var inputStream = context.FileSystem.GetFile(path).OpenRead())
				{
					_document = XDocument.Load(inputStream);
				}
				root = _document.Root;
			}
			catch (Exception ex)
			{
				_context.LogAndThrow($"Invalid manifest file {path}", ex);
			}

			if (root == null)
			{
				context.LogAndThrow($"Invalid manifest file {path}");
				throw new Exception(); //to make static analyser happy, this could not be executed anyway
			}

			_packageAttribute = root.Attribute(XName.Get("package"));
			if (_packageAttribute == null)
			{
				_context.LogAndThrow($"Invalid manifest file {path} missing package attribute on root");
			}

			_versionNameAttribute = root.Attribute(XName.Get("versionName", ANDROID_NAMESPACE));
			if (_versionNameAttribute == null)
			{
				_context.LogAndThrow($"Invalid manifest file {path} missing versionName attribute on root");
			}

			_versionCodeAttribute = root.Attribute(XName.Get("versionCode", ANDROID_NAMESPACE));
			if (_versionCodeAttribute == null)
			{
				_context.LogAndThrow($"Invalid manifest file {path} missing version code attribute on root");
			}
		}

		public string Package
		{
			get => _packageAttribute.Value;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}
				_packageAttribute.SetValue(value);
			}
		}

		public string VersionName
		{
			get => _versionNameAttribute.Value;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}
				_versionNameAttribute.SetValue(value);
			}
		}

		public string VersionCode
		{
			get => _versionCodeAttribute.Value;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}
				
				if (int.TryParse(value, out int versionCode) && versionCode > 0)
				{
					_versionCodeAttribute.SetValue(value);
				}

				_context.LogAndThrow($"VersionCode for android manifest must be an integer greater than 0, got {value}");
			}
		}

		public void Save() => Save(_path);
		
		public void Save(FilePath outputPath)
		{
			using (Stream outputStream = _context.FileSystem.GetFile(outputPath).OpenWrite())
			{
				_document.Save(outputStream);
			}
		}
	}
}
