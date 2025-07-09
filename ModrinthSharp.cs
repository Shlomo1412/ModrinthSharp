using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.IO;
using System.Net;
using System.Linq;

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

    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        public string GithubId { get; set; }
        public string Role { get; set; }
        public string Created { get; set; }
    }

    public class Team
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public List<User> Members { get; set; }
    }

    public class Dependency
    {
        public string VersionId { get; set; }
        public string ProjectId { get; set; }
        public string DependencyType { get; set; }
    }

    public class ModrinthFile
    {
        public string Url { get; set; }
        public string Filename { get; set; }
        public string Hashes { get; set; }
        public long Size { get; set; }
        public string Primary { get; set; }
        public string FileType { get; set; }
    }

    public class ModrinthApiException : HttpRequestException
    {
        public HttpStatusCode StatusCode { get; }
        public string ApiError { get; }
        public ModrinthApiException(string message, HttpStatusCode statusCode, string apiError = null)
            : base(message + (apiError != null ? $" | API Error: {apiError}" : ""))
        {
            StatusCode = statusCode;
            ApiError = apiError;
        }
    }

    public class ModrinthSharp
    {
        private static readonly HttpClient _httpClient = new HttpClient { BaseAddress = new System.Uri("https://api.modrinth.com/v2/") };

        private static async Task ThrowIfNotSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string apiError = null;
                try { apiError = await response.Content.ReadAsStringAsync(); } catch { }
                throw new ModrinthApiException($"Request failed: {response.StatusCode}", response.StatusCode, apiError);
            }
        }

        public async Task<ModrinthProject?> GetProjectAsync(string idOrSlug)
        {
            var response = await _httpClient.GetAsync($"project/{idOrSlug}");
            await ThrowIfNotSuccess(response);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ModrinthProject>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<SearchResult> SearchProjectsAsync(string query, int limit = 10, int offset = 0)
        {
            var url = $"search?query={System.Net.WebUtility.UrlEncode(query)}&limit={limit}&offset={offset}";
            var response = await _httpClient.GetAsync(url);
            await ThrowIfNotSuccess(response);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SearchResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<ModrinthVersion>> GetProjectVersionsAsync(string projectId)
        {
            var response = await _httpClient.GetAsync($"project/{projectId}/version");
            await ThrowIfNotSuccess(response);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ModrinthVersion>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ModrinthVersion> GetVersionAsync(string versionId)
        {
            var response = await _httpClient.GetAsync($"version/{versionId}");
            await ThrowIfNotSuccess(response);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ModrinthVersion>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<User> GetUserAsync(string usernameOrId)
        {
            var response = await _httpClient.GetAsync($"user/{usernameOrId}");
            await ThrowIfNotSuccess(response);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<Team> GetTeamAsync(string teamId)
        {
            var response = await _httpClient.GetAsync($"team/{teamId}");
            await ThrowIfNotSuccess(response);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Team>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<Dependency>> GetProjectDependenciesAsync(string projectId)
        {
            var response = await _httpClient.GetAsync($"project/{projectId}/dependencies");
            await ThrowIfNotSuccess(response);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Dependency>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<ModrinthFile>> GetVersionFilesAsync(string versionId)
        {
            var response = await _httpClient.GetAsync($"version/{versionId}/files");
            await ThrowIfNotSuccess(response);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ModrinthFile>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task DownloadFileAsync(string fileUrl, string destinationPath)
        {
            var response = await _httpClient.GetAsync(fileUrl);
            await ThrowIfNotSuccess(response);
            using (var fs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await response.Content.CopyToAsync(fs);
            }
        }

        public async Task<SearchResult> SearchProjectsAdvancedAsync(string query, string[] categories = null, string[] loaders = null, string[] gameVersions = null, int limit = 10, int offset = 0)
        {
            var url = $"search?query={System.Net.WebUtility.UrlEncode(query)}&limit={limit}&offset={offset}";
            if (categories != null && categories.Length > 0)
                url += "&categories=" + string.Join(",", categories);
            if (loaders != null && loaders.Length > 0)
                url += "&loaders=" + string.Join(",", loaders);
            if (gameVersions != null && gameVersions.Length > 0)
                url += "&game_versions=" + string.Join(",", gameVersions);
            var response = await _httpClient.GetAsync(url);
            await ThrowIfNotSuccess(response);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SearchResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<ModrinthProject>> GetRandomProjectsAsync(int count = 10, string projectType = null)
        {
            if (count > 100) count = 100; // API limit
            if (count < 1) count = 1;
            
            var url = $"projects_random?count={count}";
            var response = await _httpClient.GetAsync(url);
            await ThrowIfNotSuccess(response);
            var json = await response.Content.ReadAsStringAsync();
            var projects = JsonSerializer.Deserialize<List<ModrinthProject>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            // Filter by project type if specified (since the API doesn't support this parameter)
            if (!string.IsNullOrEmpty(projectType))
            {
                projects = projects.Where(p => p.ProjectType?.Equals(projectType, StringComparison.OrdinalIgnoreCase) == true).ToList();
            }
            
            return projects ?? new List<ModrinthProject>();
        }

        public async Task<SearchResult> GetTrendingProjectsAsync(int limit = 10, string projectType = null)
        {
            var url = $"search?limit={limit}&index=downloads&offset=0";
            
            if (!string.IsNullOrEmpty(projectType))
                url += $"&project_type={projectType}";
            
            var response = await _httpClient.GetAsync(url);
            await ThrowIfNotSuccess(response);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SearchResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<SearchResult> GetPopularProjectsAsync(int limit = 10, string projectType = null)
        {
            var url = $"search?limit={limit}&index=follows&offset=0";
            
            if (!string.IsNullOrEmpty(projectType))
                url += $"&project_type={projectType}";
            
            var response = await _httpClient.GetAsync(url);
            await ThrowIfNotSuccess(response);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SearchResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<SearchResult> GetRecentProjectsAsync(int limit = 10, string projectType = null)
        {
            var url = $"search?limit={limit}&index=updated&offset=0";
            
            if (!string.IsNullOrEmpty(projectType))
                url += $"&project_type={projectType}";
            
            var response = await _httpClient.GetAsync(url);
            await ThrowIfNotSuccess(response);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SearchResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<SearchResult> GetNewProjectsAsync(int limit = 10, string projectType = null)
        {
            var url = $"search?limit={limit}&index=newest&offset=0";
            
            if (!string.IsNullOrEmpty(projectType))
                url += $"&project_type={projectType}";
            
            var response = await _httpClient.GetAsync(url);
            await ThrowIfNotSuccess(response);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SearchResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Helper methods for specific project types - updated to work with new return type
        public async Task<List<ModrinthProject>> GetRandomModsAsync(int count = 10) =>
            await GetRandomProjectsAsync(count, "mod");

        public async Task<List<ModrinthProject>> GetRandomModpacksAsync(int count = 10) =>
            await GetRandomProjectsAsync(count, "modpack");

        public async Task<List<ModrinthProject>> GetRandomResourcePacksAsync(int count = 10) =>
            await GetRandomProjectsAsync(count, "resourcepack");

        public async Task<List<ModrinthProject>> GetRandomShadersAsync(int count = 10) =>
            await GetRandomProjectsAsync(count, "shader");

        public async Task<SearchResult> GetTrendingModsAsync(int limit = 10) =>
            await GetTrendingProjectsAsync(limit, "mod");

        public async Task<SearchResult> GetTrendingModpacksAsync(int limit = 10) =>
            await GetTrendingProjectsAsync(limit, "modpack");

        public async Task<SearchResult> GetTrendingResourcePacksAsync(int limit = 10) =>
            await GetTrendingProjectsAsync(limit, "resourcepack");

        public async Task<SearchResult> GetTrendingShadersAsync(int limit = 10) =>
            await GetTrendingProjectsAsync(limit, "shader");
    }
}
