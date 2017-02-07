using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

namespace Cake.Storm.iOS
{
	public class PList
	{
		private const string BUNDLE_KEY = "CFBundleIdentifier";
		private const string VERSION_KEY = "CFBundleShortVersionString";
		private const string BUILD_VERSION_KEY = "CFBundleVersion";

		private readonly ICakeContext _context;
		private readonly string _path;
		private readonly XDocument _document;

		public PList(ICakeContext context, string path)
		{
			_context = context;
			_path = path;

			try
			{
				_document = XDocument.Load(path);
			}
			catch (Exception ex)
			{
				_context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Invalid plist file {path}");
				throw new CakeException($"Invalid plist file {path}", ex);
			}
		}

		public string BundleId
		{
			get
			{
				return GetValueElementForKey(BUNDLE_KEY).Value;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}

				XElement item = GetValueElementForKey(BUNDLE_KEY);
				item.SetValue(value);
			}
		}

		public string Version
		{
			get
			{
				return GetValueElementForKey(VERSION_KEY).Value;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}

				XElement item = GetValueElementForKey(VERSION_KEY);
				item.SetValue(value);
			}
		}

		public string BuildVersion
		{
			get
			{
				return GetValueElementForKey(BUILD_VERSION_KEY).Value;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}

				XElement item = GetValueElementForKey(BUILD_VERSION_KEY);
				item.SetValue(value);
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
			_document.Save(newPath.MakeAbsolute(_context.Environment).FullPath);
		}

		private XElement GetValueElementForKey(string key)
		{
			XElement dict = _document.Root.Elements().FirstOrDefault();
			List<XElement> values = dict.Elements().ToList();
			for (int i = 0; i < values.Count; ++i)
			{
				XElement item = values[i];
				if (item.Name.LocalName == "key" && item.Value == key)
				{
					return values[i + 1];
				}
			}

			_context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Missing key {key} in plist file");
			throw new CakeException($"Missing key {key} in plist file");
		}
	}
}
