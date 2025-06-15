using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;

// ReSharper disable InconsistentNaming

partial class Build : NukeBuild
{
    public static int Main () => Execute<Build>(
        x=> x.Publish_RepositriesContracts,
        x=> x.Publish_Repositries
        );
    
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath OutputDirectory => RootDirectory / "packages";
    
    
    void SetBuildParameters(BuildParameters parameters)
    {
        var projectfile = SourceDirectory / parameters.ProjectFolder / $"{parameters.ProjectName}.csproj";
        parameters.Version= XmlTasks.XmlPeek(projectfile, "/Project/PropertyGroup/Version").First();
        parameters.PackageName = XmlTasks.XmlPeek(projectfile, "/Project/PropertyGroup/PackageId").First();
    }
    
    void IncreaseVersion(BuildParameters parameters)
    {
        if(Version.TryParse(parameters.Version, out var version))
        {
            var newVersion = new Version(version.Major, version.Minor, version.Build + 1);
            parameters.Version = newVersion.ToString();
            var projectfile = SourceDirectory / parameters.ProjectFolder / $"{parameters.ProjectName}.csproj";
            XmlTasks.XmlPoke(projectfile, "/Project/PropertyGroup/Version", newVersion.ToString());
        }
    }
    
    void SetVersion(BuildParameters repositryBuildParameters, BuildParameters referenceProjectBuildParameters)
    {
        repositryBuildParameters.Version= referenceProjectBuildParameters.Version;
        var repositryProjectfile = SourceDirectory / repositryBuildParameters.ProjectFolder / $"{repositryBuildParameters.ProjectName}.csproj";
        XmlTasks.XmlPoke(repositryProjectfile, "/Project/PropertyGroup/Version", referenceProjectBuildParameters.Version);
    }
    
    void SetVersionOfPackage(BuildParameters repositryBuildParameters, BuildParameters referenceProjectBuildParameters)
    {
        var repositryProjectfile = SourceDirectory / repositryBuildParameters.ProjectFolder / $"{repositryBuildParameters.ProjectName}.csproj";
        XmlTasks.XmlPoke(repositryProjectfile, $"//PackageReference[@Include='{referenceProjectBuildParameters.PackageName}']/@Version", referenceProjectBuildParameters.Version);
        
    }
    
    void Clean(BuildParameters buildParameters)
    {
        var folder = OutputDirectory / buildParameters.PackageName;
        folder.CreateOrCleanDirectory();
    }
    
    void Restore(BuildParameters buildParameters)
    {
        var cmd = $"restore";
        ProcessTasks.StartProcess(
                "dotnet",
                cmd,
                SourceDirectory/buildParameters.ProjectFolder)
            .AssertZeroExitCode();
    }
    
    void Pack(BuildParameters buildParameters)
    {
        var cmd = $"pack {buildParameters.ProjectName}.csproj --output ./../../packages/{buildParameters.PackageName} -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg";
        ProcessTasks.StartProcess(
                "dotnet",
                cmd,
                SourceDirectory/buildParameters.ProjectFolder)
            .AssertZeroExitCode();
    }

    void Publish(BuildParameters buildParameters)
    {
        var command =$"push nuget linqworks/vvdklibraries {buildParameters.PackageName}\\{buildParameters.PackageName}.{buildParameters.Version}.nupkg"; 
        ProcessTasks.StartProcess(
                "cloudsmith",
                command,
                OutputDirectory)
            .AssertZeroExitCode();
        var publishSymbolsCommand = $"push nuget linqworks/vvdklibraries {buildParameters.PackageName}\\{buildParameters.PackageName}.{buildParameters.Version}.snupkg";
        ProcessTasks.StartProcess(
                "cloudsmith", 
                publishSymbolsCommand,
                OutputDirectory)
            .AssertZeroExitCode();
    }


    
}
