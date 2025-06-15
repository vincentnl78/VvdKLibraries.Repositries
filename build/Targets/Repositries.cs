
// ReSharper disable InconsistentNaming



using Nuke.Common;

// ReSharper disable once CheckNamespace
partial class Build
{
    readonly BuildParameters RepositryBuildParameters = new BuildParameters
    {
        ProjectFolder = "VvdKRepositry.Repositries",
        ProjectName = "VvdKRepositry.Repositries"
    };
    
    
    
    Target SetBuildParameters_Repositries => x => x
        .DependsOn(LoadSettings)
        .Executes(() =>
        {
            SetParameters(RepositryBuildParameters);
        });
    
    Target SetVersionOfPackages_Repositries => x => x
        .DependsOn(SetBuildParameters_Repositries)
        .Executes(() =>
        {
            SetParameters(RepositryContractsBuildParameters);
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
        .After(Publish_RepositriesContracts) // Ensure contracts are published before restoring repositries
        .Executes(() =>
        {
            Restore(RepositryBuildParameters,false);
        });
    
    Target Compile_Repositries => x => x
        .DependsOn(Restore_Repositries)
        .Executes(() =>
        {
            Compile(RepositryBuildParameters);
        });
    
    Target Test_Repositries => x => x
        .DependsOn(Compile_Repositries)
        .Executes(() =>
        {
           
        });
    
    Target Pack_Repositries => x => x
        .DependsOn(Test_Repositries)
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