using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.Storm.Xamarin.iOS.Models
{
	public class ApplicationPList
	{
		private const string BUNDLE_KEY = "CFBundleIdentifier";
		private const string VERSION_KEY = "CFBundleShortVersionString";
		private const string BUILD_VERSION_KEY = "CFBundleVersion";

		private readonly ICakeContext _context;
		private readonly string _path;
		private readonly XDocument _document;

		public ApplicationPList(ICakeContext context, string path)
		{
			_context = context;
			_path = path;

			try
			{
				_document = XDocument.Load(path);
			}
			catch (Exception ex)
			{
				_context.LogAndThrow($"Invalid plist file {path}", ex);
			}
		}

		public string BundleId
		{
			get => GetValueElementForKey(BUNDLE_KEY).Value;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}

				GetValueElementForKey(BUNDLE_KEY).SetValue(value);
			}
		}

		public string Version
		{
			get => GetValueElementForKey(VERSION_KEY).Value;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}

				GetValueElementForKey(VERSION_KEY).SetValue(value);
			}
		}

		public string BuildVersion
		{
			get => GetValueElementForKey(BUILD_VERSION_KEY).Value;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}

				GetValueElementForKey(BUILD_VERSION_KEY).SetValue(value);
			}
		}
		
		public void Save()
		{
			using (Stream outputStream = _context.FileSystem.GetFile(_path).OpenWrite())
			{
				_document.Save(outputStream);	
			}
		}

		public void Save(FilePath newPath)
		{
			using (Stream outputStream = _context.FileSystem.GetFile(newPath).OpenWrite())
			{
				_document.Save(outputStream);
			}
		}

		private XElement GetValueElementForKey(string key)
		{
			XElement dict = _document.Root?.Elements().FirstOrDefault();
			if (dict == null)
			{
				_context.LogAndThrow($"Invalid plist file {_path}");
				throw new Exception(); //only to make static analyzer happy
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

			_context.LogAndThrow($"Missing key {key} in plist file {_path}");
			throw new Exception(); //only to make the compiler happy
		}
	}
}
