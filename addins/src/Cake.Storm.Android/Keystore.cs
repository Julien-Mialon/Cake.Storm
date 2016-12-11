using Cake.Core;
using Cake.Core.IO;

namespace Cake.Storm.Android
{
	public class Keystore
	{
		private readonly FilePath _path;
		private readonly KeytoolCommand _keytool;

		internal Keystore(ICakeContext context, FilePath path)
		{
			_path = path;
			_keytool = new KeytoolCommand(context);
		}

		public bool IsRightPassword(string password)
		{
			return _keytool.IsRightPassword(_path, password);
		}

		public bool HasAlias(string password, string alias)
		{
			return _keytool.HasAlias(_path, password, alias);
		}

		public bool CreateAlias(string password, string alias, string aliasPassword, string authority)
		{
			return _keytool.CreateAlias(_path, password, alias, aliasPassword, authority);
		}
	}
}
