using System;
using System.Linq;
using System.Xml.Linq;
using Nuke.Common.IO;

public partial class Build
{
    // Minimal patch version increment helper
    string IncrementPatchVersion(string version)
    {
        var parts = version.Split('.');
        if (parts.Length < 3) throw new Exception($"Version '{version}' is not semantic.");

        var patch = int.Parse(parts[2]);
        patch++;

        return $"{parts[0]}.{parts[1]}.{patch}";
    }

    // Read current version from csproj
    string ReadVersion(BuildParameters project)
    {
        var doc = XDocument.Load(SourceDirectory/project.ProjectFilePath);
        var verElement = doc.Descendants("Version").FirstOrDefault();
        if (verElement == null)
            throw new Exception($"No <Version> element found in {project.ProjectName}");
        return verElement.Value;
    }

    // Update version in csproj
    void UpdateVersion(BuildParameters project, string newVersion)
    {
        var doc = XDocument.Load(SourceDirectory/project.ProjectFilePath);
        var verElement = doc.Descendants("Version").FirstOrDefault();
        if (verElement == null)
            throw new Exception($"No <Version> element found in {project.ProjectName}");

        verElement.Value = newVersion;
        doc.Save(SourceDirectory/ project.ProjectFilePath);
    }

    void BumpVersion(BuildParameters parameters)
    {
        // CoreLib bump
        var coreVersion = ReadVersion(parameters);
        parameters.Version = IncrementPatchVersion(coreVersion);
        UpdateVersion(parameters, parameters.Version);


        // if(Version.TryParse(parameters.Version, out var version))
        // {
        //     //var newVersion = new Version(version.Major, version.Minor, version.Build + 1);
        //     parameters.Version = newVersion.ToString();
        //     var projectfile = SourceDirectory / parameters.ProjectFolder / $"{parameters.ProjectName}.csproj";
        //     XmlTasks.XmlPoke(projectfile, "/Project/PropertyGroup/Version", newVersion.ToString());
        // }
    }

    void MatchVersionToOtherProject(BuildParameters repositryBuildParameters, BuildParameters referenceProjectBuildParameters)
    {
        repositryBuildParameters.Version = referenceProjectBuildParameters.Version;
        var repositryProjectfile = SourceDirectory / repositryBuildParameters.ProjectFolder /
                                   $"{repositryBuildParameters.ProjectName}.csproj";
        XmlTasks.XmlPoke(repositryProjectfile, "/Project/PropertyGroup/Version",
            referenceProjectBuildParameters.Version);
    }

    void SetVersionOfPackage(BuildParameters repositryBuildParameters, BuildParameters referenceProjectBuildParameters)
    {
        // Update ConsumerApp package reference to CoreLib with new version
        var doc = XDocument.Load(SourceDirectory/ repositryBuildParameters.ProjectFilePath);
        var pkgRef = doc.Descendants("PackageReference")
            .FirstOrDefault(x => x.Attribute("Include")?.Value == referenceProjectBuildParameters.PackageName);

        if (pkgRef == null)
            throw new Exception(
                $"No PackageReference to {referenceProjectBuildParameters.PackageName} found in {repositryBuildParameters.ProjectName}");

        pkgRef.SetAttributeValue("Version", referenceProjectBuildParameters.Version);
        doc.Save(SourceDirectory/ repositryBuildParameters.ProjectFilePath);

        //var repositryProjectfile = SourceDirectory / repositryBuildParameters.ProjectFolder / $"{repositryBuildParameters.ProjectName}.csproj";
        //XmlTasks.XmlPoke(repositryProjectfile, $"//PackageReference[@Include='{referenceProjectBuildParameters.PackageName}']/@Version", referenceProjectBuildParameters.Version);
    }
}