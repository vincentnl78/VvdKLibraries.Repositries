
using Nuke.Common;
// ReSharper disable InconsistentNaming

partial class Build
{
    readonly BuildParameters RepositryContractsBuildParameters = new BuildParameters
    {
        ProjectFolder = "VvdKRepositry.Repositries.Contracts",
        ProjectName = "VvdKRepositry.Repositries.Contracts"
    };
    
    Target SetBuildParameters_RepositriesContracts => x => x
        .Executes(() =>
        {
            SetBuildParameters(RepositryContractsBuildParameters);
        });
    
    Target IncreaseVersion_RepositriesContracts => x => x
        .DependsOn(SetBuildParameters_RepositriesContracts)
        .Executes(() =>
        {
            IncreaseVersion(RepositryContractsBuildParameters);
        });
    
    Target Clean_RepositriesContracts => x => x
        .DependsOn(IncreaseVersion_RepositriesContracts)
        .Executes(() =>
        {
            Clean(RepositryContractsBuildParameters);
        });
    
    Target Pack_RepositriesContracts => x => x
        .DependsOn(Clean_RepositriesContracts)
        .Executes(() =>
        {
            Pack(RepositryContractsBuildParameters);
        });
    
    Target Publish_RepositriesContracts => x => x
        .DependsOn(Pack_RepositriesContracts)
        .Executes(() =>
        {
            Publish(RepositryContractsBuildParameters);
        });
}