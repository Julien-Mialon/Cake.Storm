using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace Cake.Storm.Fluent.AppCenter.Commands
{
	public class AppCenterUploader
	{
		private readonly IConfiguration _configuration;
		private readonly string _ownerName;
		private readonly string _applicationName;
		private readonly string _apiToken;
		private readonly string _distributionGroupName;
		private readonly HttpClient _httpClient;

		private ReleaseUpload _upload;
		private NewReleaseItem _newRelease;
		private SymbolUpload _symbolUpload;

		public AppCenterUploader(IConfiguration configuration, string ownerName, string applicationName, string apiToken, string distributionGroupName)
		{
			_configuration = configuration;
			_ownerName = ownerName;
			_applicationName = applicationName;
			_apiToken = apiToken;
			_distributionGroupName = distributionGroupName;
			_httpClient = new HttpClient();
		}

		public async Task<bool> UploadAndroidPackage(string packagePath)
		{
			_configuration.FileExistsOrThrow(packagePath);

			if (!await CreateUploadResource())
			{
				_configuration.LogAndThrow("[AppCenter] Unable to create upload resource");
			}

			if (!await UploadPackage(packagePath))
			{
				_configuration.LogAndThrow("[AppCenter] Unable to upload package");
			}

			if (!await CommitPackage())
			{
				_configuration.LogAndThrow("[AppCenter] Unable to commit release");
			}

			if (!await DistributeToGroup())
			{
				_configuration.LogAndThrow("[AppCenter] Unable to distribute release to groups");
			}

			return true;
		}

		public async Task<bool> UploadiOSPackage(string ipaPath, string symbolsPath)
		{
//			_configuration.FileExistsOrThrow(ipaPath);
//			_configuration.FileExistsOrThrow(symbolsPath);

			if (!await CreateUploadResource())
			{
				_configuration.LogAndThrow("[AppCenter] Unable to create upload resource");
			}

			if (!await UploadPackage(ipaPath))
			{
				_configuration.LogAndThrow("[AppCenter] Unable to upload package");
			}

			if (!await CommitPackage())
			{
				_configuration.LogAndThrow("[AppCenter] Unable to commit release");
			}

			if (!await DistributeToGroup())
			{
				_configuration.LogAndThrow("[AppCenter] Unable to distribute release to groups");
			}

			if (!await CreateUploadSymbolResource())
			{
				_configuration.LogAndThrow("[AppCenter] Unable to create symbol upload resource");
			}

			if (!await UploadSymbols(symbolsPath))
			{
				_configuration.LogAndThrow("[AppCenter] Unable to upload symbols");
			}

			if (!await CommitSymbols())
			{
				_configuration.LogAndThrow("[AppCenter] Unable to commit symbols");
			}


			return true;
		}

		private async Task<bool> DistributeToGroup()
		{
			if (await GetDistributionGroupId() is string distributionGroupId)
			{
				HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, $"https://api.appcenter.ms/v0.1/apps/{_ownerName}/{_applicationName}/releases/{_newRelease.ReleaseId}/groups");
				requestMessage.Headers.Add("Accept", "application/json");
				requestMessage.Headers.Add("X-API-TOKEN", _apiToken);
				requestMessage.Content = new StringContent($"{{\"id\": \"{distributionGroupId}\", \"mandatory_update\":false, \"notify_testers\":true}}", Encoding.UTF8, "application/json");

				using (HttpResponseMessage response = await _httpClient.SendAsync(requestMessage))
				{
					if (response.IsSuccessStatusCode)
					{
						string responseContent = await response.Content.ReadAsStringAsync();
						return true;
					}
					else
					{
						string responseContent = await response.Content.ReadAsStringAsync();
						_configuration.Context.CakeContext.Log.Error(responseContent);
					}
				}

				return false;
			}

			_configuration.LogAndThrow($"[AppCenter] Distribution group {_distributionGroupName} not found");
			throw new Exception();
		}

		private async Task<string> GetDistributionGroupId()
		{
			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api.appcenter.ms/v0.1/apps/{_ownerName}/{_applicationName}/distribution_groups/{_distributionGroupName}");
			requestMessage.Headers.Add("Accept", "application/json");
			requestMessage.Headers.Add("X-API-TOKEN", _apiToken);

			using (HttpResponseMessage response = await _httpClient.SendAsync(requestMessage))
			{
				if (response.IsSuccessStatusCode)
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					DistributionGroupItem distributionGroup = JsonConvert.DeserializeObject<DistributionGroupItem>(responseContent);

					return distributionGroup.Id;
				}
				else
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					_configuration.Context.CakeContext.Log.Error(responseContent);
				}
			}

			return null;
		}

		private async Task<bool> CommitPackage()
		{
			HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), $"https://api.appcenter.ms/v0.1/apps/{_ownerName}/{_applicationName}/release_uploads/{_upload.UploadId}");
			requestMessage.Headers.Add("Accept", "application/json");
			requestMessage.Headers.Add("X-API-TOKEN", _apiToken);
			requestMessage.Content = new StringContent("{\"status\": \"committed\"}", Encoding.UTF8, "application/json");

			using (HttpResponseMessage response = await _httpClient.SendAsync(requestMessage))
			{
				if (response.IsSuccessStatusCode)
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					_newRelease = JsonConvert.DeserializeObject<NewReleaseItem>(responseContent);

					return true;
				}
				else
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					_configuration.Context.CakeContext.Log.Error(responseContent);
				}
			}

			return false;
		}

		private async Task<bool> UploadPackage(string packagePath)
		{
			//string filePath = _configuration.Context.CakeContext.MakeAbsolute((FilePath)packagePath);
			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, _upload.UploadUrl);
			requestMessage.Headers.Add("Accept", "application/json");
			requestMessage.Headers.Add("X-API-TOKEN", _apiToken);
			requestMessage.Content = new MultipartFormDataContent {{new ByteArrayContent(File.ReadAllBytes(packagePath)), "ipa", Path.GetFileName(packagePath)}};

			using (HttpResponseMessage response = await _httpClient.SendAsync(requestMessage))
			{
				if (response.IsSuccessStatusCode)
				{
					return true;
				}

				string responseContent = await response.Content.ReadAsStringAsync();
				_configuration.Context.CakeContext.Log.Error(responseContent);
			}

			return false;
		}

		private async Task<bool> CreateUploadResource()
		{
			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, $"https://api.appcenter.ms/v0.1/apps/{_ownerName}/{_applicationName}/release_uploads");
			requestMessage.Headers.Add("Accept", "application/json");
			requestMessage.Headers.Add("X-API-TOKEN", _apiToken);
			requestMessage.Content = new StringContent("{}", Encoding.UTF8, "application/json");

			using (HttpResponseMessage response = await _httpClient.SendAsync(requestMessage))
			{
				if (response.IsSuccessStatusCode)
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					_upload = JsonConvert.DeserializeObject<ReleaseUpload>(responseContent);

					return true;
				}
				else
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					_configuration.Context.CakeContext.Log.Error(responseContent);
				}
			}

			return false;
		}

		private async Task<bool> CreateUploadSymbolResource()
		{
			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, $"https://api.appcenter.ms/v0.1/apps/{_ownerName}/{_applicationName}/symbol_uploads");
			requestMessage.Headers.Add("Accept", "application/json");
			requestMessage.Headers.Add("X-API-TOKEN", _apiToken);
			requestMessage.Content = new StringContent("{\"symbol_type\":\"Apple\"}", Encoding.UTF8, "application/json");

			using (HttpResponseMessage response = await _httpClient.SendAsync(requestMessage))
			{
				if (response.IsSuccessStatusCode)
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					_symbolUpload = JsonConvert.DeserializeObject<SymbolUpload>(responseContent);

					return true;
				}
				else
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					_configuration.Context.CakeContext.Log.Error(responseContent);
				}
			}

			return false;
		}

		private async Task<bool> UploadSymbols(string symbolsFile)
		{
			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, _symbolUpload.UploadUrl);
			requestMessage.Headers.Add("Accept", "application/json");
			requestMessage.Headers.Add("x-ms-blob-type", "BlockBlob");
			requestMessage.Content = new ByteArrayContent(File.ReadAllBytes(symbolsFile));

			using (HttpResponseMessage response = await _httpClient.SendAsync(requestMessage))
			{
				if (response.IsSuccessStatusCode)
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					return true;
				}
				else
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					_configuration.Context.CakeContext.Log.Error(responseContent);
				}
			}

			return false;
		}

		private async Task<bool> CommitSymbols()
		{
			HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), $"https://api.appcenter.ms/v0.1/apps/{_ownerName}/{_applicationName}/symbol_uploads/{_symbolUpload.SymbolUploadId}");
			requestMessage.Headers.Add("Accept", "application/json");
			requestMessage.Headers.Add("X-API-TOKEN", _apiToken);
			requestMessage.Content = new StringContent("{\"status\": \"committed\"}", Encoding.UTF8, "application/json");

			using (HttpResponseMessage response = await _httpClient.SendAsync(requestMessage))
			{
				if (response.IsSuccessStatusCode)
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					return true;
				}
				else
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					_configuration.Context.CakeContext.Log.Error(responseContent);
				}
			}

			return false;
		}

		private class ReleaseItem
		{
			[JsonProperty("id")] public int Id { get; set; }
		}

		private class ReleaseUpload
		{
			[JsonProperty("upload_id")] public string UploadId { get; set; }

			[JsonProperty("upload_url")] public string UploadUrl { get; set; }
		}

		private class SymbolUpload
		{
			[JsonProperty("symbol_upload_id")] public string SymbolUploadId { get; set; }

			[JsonProperty("upload_url")] public string UploadUrl { get; set; }
		}

		private class NewReleaseItem
		{
			[JsonProperty("release_id")] public int ReleaseId { get; set; }

			[JsonProperty("release_url")] public string ReleaseUrl { get; set; }
		}

		private class DistributionGroupItem
		{
			[JsonProperty("id")]
			public string Id { get; set; }

			[JsonProperty("name")]
			public string Name { get; set; }
		}
	}
}