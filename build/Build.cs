using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;


// ReSharper disable InconsistentNaming

partial class Build : NukeBuild
{
    public static int Main () => Execute<Build>(
        x=> x.Publish_RepositriesContracts,
        x=> x.Publish_Repositries
        );
    
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    string GitHubSource => $"https://nuget.pkg.github.com/{GitHubOwner}/index.json";
    
    [Parameter]public string GitHubOwner;
    [Parameter]public string GitHubToken;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    
    Target LoadSettings => x => x
        .Executes(() =>
        {
            string json = File.ReadAllText(RootDirectory/"build"/ "secrets.json");
            var secrets = System.Text.Json.JsonSerializer.Deserialize<Secrets>(json);
            GitHubOwner=secrets.GitHubOwner;
            GitHubToken=secrets.GitHubToken;
        });
    
    void SetBuildParameters(BuildParameters parameters)
    {
        var projectfile = SourceDirectory / parameters.ProjectFilePath;
        parameters.Version= XmlTasks.XmlPeek(projectfile, "/Project/PropertyGroup/Version").First();
        parameters.PackageName = XmlTasks.XmlPeek(projectfile, "/Project/PropertyGroup/PackageId").First();
    }
    
    
    
    void Clean(BuildParameters buildParameters)
    {
        var folder = OutputDirectory / buildParameters.PackageName;
        folder.CreateOrCleanDirectory();
    }
    
    void Restore(BuildParameters buildParameters, bool useCache)
    {
        if (useCache)
        {
            DotNetTasks.DotNetRestore(s => s
                .SetProjectFile(SourceDirectory / buildParameters.ProjectFilePath)
            );
        }
        else
        {
            DotNetTasks.DotNetRestore(s=>s
                .SetProcessWorkingDirectory(SourceDirectory / buildParameters.ProjectFolder)
                .SetNoCache(true)
                .SetIgnoreFailedSources(false)
            );    
        }
    }
    
    void Pack(BuildParameters buildParameters)
    {
        DotNetTasks.DotNetPack(s=> s
            .SetProject(SourceDirectory / buildParameters.ProjectFilePath)
            .SetConfiguration(Configuration)
            .SetOutputDirectory(OutputDirectory / buildParameters.PackageName)
            .EnableIncludeSymbols()
            .EnableIncludeSource()
            .SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg)
            .SetVersion(buildParameters.Version)
            .SetProcessWorkingDirectory(SourceDirectory/ buildParameters.ProjectFolder)
        );
    }

    void Publish(BuildParameters buildParameters)
    {
        var nupkgs = OutputDirectory / buildParameters.PackageName / buildParameters.PackageName+"." +buildParameters.Version +".nupkg";
        var snupkgs = OutputDirectory / buildParameters.PackageName / buildParameters.PackageName+"."+buildParameters.Version +".snupkg";
        Push(nupkgs);
        Push(snupkgs);

        void Push(string packageName)
        {
            DotNetTasks.DotNetNuGetPush(s => s
                .SetTargetPath(packageName)
                .SetSource(GitHubSource)
                .SetApiKey(GitHubToken)
                .EnableSkipDuplicate()
            );
        }
    }


    
}
