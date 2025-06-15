
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
    
    Target SetBuildParameters_Repositries => x => x
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
            SetVersion(RepositryBuildParameters,RepositryContractsBuildParameters);
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