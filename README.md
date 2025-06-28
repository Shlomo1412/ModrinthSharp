# ModrinthSharp

ModrinthSharp is a .NET 8 library for accessing the Modrinth API in C#. It provides easy-to-use methods for searching, retrieving, and analyzing Modrinth projects, versions, users, teams, dependencies, and files.

## Features
- Search for Modrinth projects with advanced filters
- Get detailed project info by ID or slug
- List all versions of a project
- Get version details and files
- Download project files
- Get user and team info
- Fetch project dependencies
- Strongly-typed models for all major Modrinth API objects
- Robust error handling with API error details

## Installation
Add the ModrinthSharp project to your solution or reference the DLL. Target .NET 8 or later.

## Usage

### Creating a Client
Create an instance of the client:
```
using ModrinthSharp;

var modrinth = new ModrinthSharp();
```
### Async Usage
All methods are async. Use them in an async context:
```
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var modrinth = new ModrinthSharp();
        // ... call methods below ...
    }
}
```
### Search for Projects
Search for projects by keyword:
```
var results = await modrinth.SearchProjectsAsync("sodium");
foreach (var project in results.Hits)
{
    Console.WriteLine($"{project.Title} ({project.Slug})");
}
```
### Advanced Search
Search with filters (categories, loaders, game versions):
```
var advanced = await modrinth.SearchProjectsAdvancedAsync(
    query: "optimization",
    categories: new[] { "performance" },
    loaders: new[] { "fabric" },
    gameVersions: new[] { "1.20.1" }
);
foreach (var project in advanced.Hits)
{
    Console.WriteLine($"{project.Title} - {project.Downloads} downloads");
}
```
### Get Project by ID or Slug
Get detailed info for a project:
```
var project = await modrinth.GetProjectAsync("sodium");
Console.WriteLine($"{project.Title}: {project.Description}");
```
### List Project Versions
Get all versions for a project:
```
var versions = await modrinth.GetProjectVersionsAsync("sodium");
foreach (var version in versions)
{
    Console.WriteLine($"{version.Name} ({version.VersionNumber})");
}
```
### Get Version Details and Files
Get details and files for a specific version:
```
var version = await modrinth.GetVersionAsync(versions[0].Id);
var files = await modrinth.GetVersionFilesAsync(version.Id);
foreach (var file in files)
{
    Console.WriteLine($"File: {file.Filename} ({file.Size} bytes)");
}
```
### Download a File
Download a file to disk:
```
await modrinth.DownloadFileAsync(files[0].Url, @"C:\\Downloads\\" + files[0].Filename);
```
### Get User and Team Info
Get user and team details:
```
var user = await modrinth.GetUserAsync("johndoe");
Console.WriteLine($"User: {user.Username} ({user.Id})");

var team = await modrinth.GetTeamAsync("teamid");
Console.WriteLine($"Team: {team.Name}");
### Get Project Dependencies
List dependencies for a project:var dependencies = await modrinth.GetProjectDependenciesAsync("sodium");
foreach (var dep in dependencies)
{
    Console.WriteLine($"Depends on: {dep.ProjectId} ({dep.DependencyType})");
}
```
### Error Handling
All methods throw `ModrinthApiException` on API errors, which includes the HTTP status and any error message from Modrinth.

**Example:**
```
try
{
    var project = await modrinth.GetProjectAsync("nonexistent_project");
}
catch (ModrinthApiException ex)
{
    Console.WriteLine($"API Error: {ex.StatusCode} - {ex.ApiError}");
}
```
### Complete Example
A typical workflow:
```
using ModrinthSharp;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var modrinth = new ModrinthSharp();
        var results = await modrinth.SearchProjectsAsync("sodium");
        var project = await modrinth.GetProjectAsync(results.Hits[0].Slug);
        var versions = await modrinth.GetProjectVersionsAsync(project.Id);
        var version = await modrinth.GetVersionAsync(versions[0].Id);
        var files = await modrinth.GetVersionFilesAsync(version.Id);
        await modrinth.DownloadFileAsync(files[0].Url, @"C:\\Downloads\\" + files[0].Filename);
        Console.WriteLine($"Downloaded {files[0].Filename} for {project.Title}");
    }
}

```
### Model Reference
- `ModrinthProject`: All project metadata (title, description, downloads, categories, etc.)
- `ModrinthVersion`: Version info (name, changelog, supported game versions, loaders, etc.)
- `ModrinthFile`: File info (filename, size, URL, hashes, etc.)
- `User`, `Team`: User/team info
- `Dependency`: Project dependencies
- `SearchResult`: Search results (list of projects, total hits, etc.)

## License
MIT