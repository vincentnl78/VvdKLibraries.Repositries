// ReSharper disable InconsistentNaming
using Nuke.Common;

// ReSharper disable once CheckNamespace
partial class Build
{
    readonly BuildParameters RepositryContractsBuildParameters = new BuildParameters
    {
        ProjectFolder = "VvdKRepositry.Repositries.Contracts",
        ProjectName = "VvdKRepositry.Repositries.Contracts"
    };
    
    Target SetParameters_RepositriesContracts => x => x
        .DependsOn(LoadSettings)
        .Executes(() =>
        {
            SetParameters(RepositryContractsBuildParameters);
        });
    
    Target IncreaseVersion_RepositriesContracts => x => x
        .DependsOn(SetParameters_RepositriesContracts)
        .Executes(() =>
        {
            BumpVersion(RepositryContractsBuildParameters);
        });
    
    Target Clean_RepositriesContracts => x => x
        .DependsOn(IncreaseVersion_RepositriesContracts)
        .Executes(() =>
        {
            Clean(RepositryContractsBuildParameters);
            //CleanPackageFiles(RepositryContractsBuildParameters);
        });

    Target Restore_RepositriesContracts => x => x
        .DependsOn(Clean_RepositriesContracts)
        .Executes(() =>
        {
            Restore(RepositryContractsBuildParameters,true);
        });
    
    Target Compile_RepositriesContracts => x => x
        .DependsOn(Restore_RepositriesContracts)
        .Executes(() =>
        {
            Compile(RepositryContractsBuildParameters);
        });
    
    Target Test_RepositriesContracts => x => x
        .DependsOn(Compile_RepositriesContracts)
        .Executes(() =>
        {
           
        });
    
    Target Pack_RepositriesContracts => x => x
        .DependsOn(Test_RepositriesContracts)
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