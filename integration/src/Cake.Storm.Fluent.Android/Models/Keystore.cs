using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Android.Commands;
using Cake.Storm.Fluent.Android.Interfaces;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Android.Models
{
	internal class Keystore : IKeystoreAction
	{
		private string _authority;
		private string _password;
		private string _keyAlias;
		private string _keyPassword;
		private bool _allowCreate;

		public IKeystore WithAuthority(string authority)
		{
			_authority = authority;
			return this;
		}

		public IKeystore WithPassword(string password)
		{
			_password = password;
			return this;
		}

		public IKeystore WithKeyAlias(string alias)
		{
			_keyAlias = alias;
			return this;
		}

		public IKeystore WithKeyPassword(string password)
		{
			_keyPassword = password;
			return this;
		}

		public IKeystore WithAllowCreate(bool allowCreate)
		{
			_allowCreate = allowCreate;
			return this;
		}

		public IKeystore WithKey(string alias, string password) => WithKeyAlias(alias).WithKeyPassword(password);

		public void Execute(FilePath keyStoreFile, IConfiguration configuration)
		{
			if (string.IsNullOrEmpty(_password))
			{
				configuration.Context.CakeContext.LogAndThrow($"Missing password for keystore {keyStoreFile}");
			}
			if (string.IsNullOrEmpty(_keyAlias))
			{
				configuration.Context.CakeContext.LogAndThrow($"Missing key alias for keystore {keyStoreFile}");
			}
			if (string.IsNullOrEmpty(_keyPassword))
			{
				configuration.Context.CakeContext.LogAndThrow($"Missing key password for keystore {keyStoreFile}");
			}

			if (configuration.Context.CakeContext.FileExists(keyStoreFile))
			{
				KeytoolCommand command = new KeytoolCommand(configuration.Context.CakeContext);
				if (!command.IsRightPassword(keyStoreFile.FullPath, _password))
				{
					configuration.Context.CakeContext.LogAndThrow($"Invalid password for keystore {keyStoreFile}");
				}

				if (!command.HasAlias(keyStoreFile.FullPath, _password, _keyAlias))
				{
					if (_allowCreate)
					{
						if (string.IsNullOrEmpty(_authority))
						{
							configuration.Context.CakeContext.LogAndThrow($"Missing authority to create alias {_keyAlias} in keystore {keyStoreFile}");
						}
						command.CreateAlias(keyStoreFile, _password, _keyAlias, _keyPassword, _authority);
					}
					else
					{
						configuration.Context.CakeContext.LogAndThrow($"Missing alias {_keyAlias} for keystore {keyStoreFile} and we are not allowed to create it");
					}
				}
			}
			else if (_allowCreate)
			{
				if (string.IsNullOrEmpty(_authority))
				{
					configuration.Context.CakeContext.LogAndThrow($"Missing authority to create keystore {keyStoreFile}");
				}
				KeytoolCommand command = new KeytoolCommand(configuration.Context.CakeContext);
				command.CreateAlias(keyStoreFile, _password, _keyAlias, _keyPassword, _authority);
			}
			else
			{
				configuration.Context.CakeContext.LogAndThrow($"Keystore {keyStoreFile} does not exists and we are not allowed to create it");
			}
		}

		public void Sign(FilePath keyStoreFile, IConfiguration configuration, string sourceApk, string destinationApk)
		{
			JarsignerCommand command = new JarsignerCommand(configuration.Context.CakeContext);
			command.SignApk(sourceApk, destinationApk, keyStoreFile, _password, _keyAlias, _keyPassword);
		}
	}
}