
using DefaultNamespace;
using Microsoft.Build.Utilities;
using Nuke.Common;
// ReSharper disable InconsistentNaming

partial class Build
{
    readonly BuildParameters RepositryBuildParameters = new BuildParameters
    {
        ProjectFolder = "VvdKRepositry.Repositries",
        ProjectName = "VvdKRepositry.Repositries"
    };
    
    Target LoadSettings => x => x
        .Executes(() =>
        {
            string json = System.IO.File.ReadAllText(RootDirectory/"build"/ "secrets.json");
            var secrets = System.Text.Json.JsonSerializer.Deserialize<Secrets>(json);
            GitHubOwner=secrets.GitHubOwner;
            GitHubToken=secrets.GitHubToken;
        });
    
    Target SetBuildParameters_Repositries => x => x
        .DependsOn(LoadSettings)
        .Executes(() =>
        {
            SetBuildParameters(RepositryBuildParameters);
        });
    
    Target SetVersionOfPackages_Repositries => x => x
        .DependsOn(SetBuildParameters_Repositries)
        .Executes(() =>
        {
            SetBuildParameters(RepositryContractsBuildParameters);
            System.Threading.Thread.Sleep(15000); // 15,000 ms = 15 sec
            MatchVersionToOtherProject(RepositryBuildParameters,RepositryContractsBuildParameters);
            SetVersionOfPackage(
                RepositryBuildParameters, 
                RepositryContractsBuildParameters
                );
        });
    
    Target Clean_Repositries => x => x
        .DependsOn(SetVersionOfPackages_Repositries)
        .Executes(() =>
        {
            Clean(RepositryBuildParameters);
        });
    
    Target Restore_Repositries => x => x
        .DependsOn(Clean_Repositries)
        .After(Publish_RepositriesContracts)
        .Executes(() =>
        {
            Restore(RepositryBuildParameters);
        });
    
    Target Pack_Repositries => x => x
        .DependsOn(Restore_Repositries)
        .Executes(() =>
        {
            Pack(RepositryBuildParameters);
        });
    
    Target Publish_Repositries => x => x
        .DependsOn(Pack_Repositries)
        .Executes(() =>
        {
            Publish(RepositryBuildParameters);
        });
}