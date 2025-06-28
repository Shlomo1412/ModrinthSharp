using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ModrinthSharp
{
    public class DonationUrl
    {
        public string Platform { get; set; }
        public string Url { get; set; }
    }

    public class License
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class GalleryImage
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Created { get; set; }
        public int? Ordering { get; set; }
    }

    public class ModrinthVersion
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string VersionNumber { get; set; }
        public string Changelog { get; set; }
        public string DatePublished { get; set; }
        public List<string> GameVersions { get; set; }
        public List<string> Loaders { get; set; }
        public string ProjectId { get; set; }
    }

    public class ModrinthProject
    {
        public string Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Categories { get; set; }
        public string ClientSide { get; set; }
        public string ServerSide { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }
        public string RequestedStatus { get; set; }
        public List<string> AdditionalCategories { get; set; }
        public string IssuesUrl { get; set; }
        public string SourceUrl { get; set; }
        public string WikiUrl { get; set; }
        public string DiscordUrl { get; set; }
        public List<DonationUrl> DonationUrls { get; set; }
        public string ProjectType { get; set; }
        public int Downloads { get; set; }
        public string IconUrl { get; set; }
        public int? Color { get; set; }
        public string ThreadId { get; set; }
        public string MonetizationStatus { get; set; }
        public string Team { get; set; }
        public string BodyUrl { get; set; }
        public string ModeratorMessage { get; set; }
        public string Published { get; set; }
        public string Updated { get; set; }
        public string Approved { get; set; }
        public string Queued { get; set; }
        public int Followers { get; set; }
        public License License { get; set; }
        public List<string> Versions { get; set; }
        public List<string> GameVersions { get; set; }
        public List<string> Loaders { get; set; }
        public List<GalleryImage> Gallery { get; set; }
    }

    public class SearchResult
    {
        [JsonPropertyName("hits")]
        public List<ModrinthProject> Hits { get; set; }
        [JsonPropertyName("total_hits")]
        public int TotalHits { get; set; }
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        [JsonPropertyName("offset")]
        public int Offset { get; set; }
    }

    public class ModrinthSharp
    {
        private static readonly HttpClient _httpClient = new HttpClient { BaseAddress = new System.Uri("https://api.modrinth.com/v2/") };

        public async Task<ModrinthProject?> GetProjectAsync(string idOrSlug)
        {
            var response = await _httpClient.GetAsync($"project/{idOrSlug}");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to get project: {response.StatusCode}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ModrinthProject>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<SearchResult> SearchProjectsAsync(string query, int limit = 10, int offset = 0)
        {
            var url = $"search?query={System.Web.HttpUtility.UrlEncode(query)}&limit={limit}&offset={offset}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to search projects: {response.StatusCode}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SearchResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<ModrinthVersion>> GetProjectVersionsAsync(string projectId)
        {
            var response = await _httpClient.GetAsync($"project/{projectId}/version");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to get project versions: {response.StatusCode}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ModrinthVersion>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ModrinthVersion> GetVersionAsync(string versionId)
        {
            var response = await _httpClient.GetAsync($"version/{versionId}");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to get version: {response.StatusCode}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ModrinthVersion>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
