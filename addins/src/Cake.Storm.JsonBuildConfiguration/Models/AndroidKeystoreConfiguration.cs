using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cake.Storm.JsonBuildConfiguration.Models
{

	internal class AndroidKeystoreConfiguration
	{
		[JsonProperty("file")]
		public string File { get; set; }

		[JsonProperty("authority")]
		public string Authority { get; set; }

		[JsonProperty("password")]
		public string Password { get; set; }

		[JsonProperty("key_alias")]
		public string KeyAlias { get; set; }

		[JsonProperty("key_password")]
		public string KeyPassword { get; set; }

		public static AndroidKeystoreConfiguration Merge(AndroidKeystoreConfiguration source, AndroidKeystoreConfiguration overwrite)
		{
			if (overwrite == null)
			{
				return source;
			}
			if (source == null)
			{
				return overwrite;
			}

			return new AndroidKeystoreConfiguration
			{
				File = overwrite.File ?? source.File,
				Authority = overwrite.Authority ?? source.Authority,
				Password = overwrite.Password ?? source.Password,
				KeyAlias = overwrite.KeyAlias ?? source.KeyAlias,
				KeyPassword = overwrite.KeyPassword ?? source.KeyPassword
			};
		}
	}
}
