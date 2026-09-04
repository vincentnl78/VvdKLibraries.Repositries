
// ReSharper disable InconsistentNaming



using Nuke.Common;

// ReSharper disable once CheckNamespace
partial class Build
{
    readonly BuildParameters AuthInterfacesBuildParameters = new()
    {
        ProjectFolder = "Auth.Interfaces",
        ProjectName = "Auth.Interfaces"
    };
    
    Target SetBuildParameters_AuthInterfaces => x => x
        .DependsOn(LoadSettings)
        .Executes(() =>
        {
            SetParameters(AuthInterfacesBuildParameters);
        });
    
    Target SetVersionOfPackages_AuthInterfaces => x => x
        .DependsOn(SetBuildParameters_AuthInterfaces)
        .Executes(() =>
        {
            SetParameters(AuthObjectsBuildParameters);
            SetParameters(RepositryContractsBuildParameters);
            MatchVersionToOtherProject(AuthInterfacesBuildParameters,AuthObjectsBuildParameters);
            SetVersionOfPackage(
                AuthInterfacesBuildParameters,
                AuthObjectsBuildParameters
                );
            SetVersionOfPackage(
                AuthInterfacesBuildParameters,
                RepositryContractsBuildParameters
            );
        });
    
    Target Clean_AuthInterfaces => x => x
        .DependsOn(SetVersionOfPackages_AuthInterfaces)
        .Executes(() =>
        {
            Clean(AuthInterfacesBuildParameters);
        });
    
    Target Restore_AuthInterfaces => x => x
        .DependsOn(Clean_AuthInterfaces)
        .After(Publish_AuthObjects, Publish_RepositriesContracts) // Ensure contracts are published before restoring repositries
        .Executes(() =>
        {
            Restore(AuthInterfacesBuildParameters,false);
        });
    
    Target Compile_AuthInterfaces => x => x
        .DependsOn(Restore_AuthInterfaces)
        .Executes(() =>
        {
            Compile(AuthInterfacesBuildParameters);
        });
    
    Target Test_AuthInterfaces => x => x
        .DependsOn(Compile_AuthInterfaces)
        .Executes(() =>
        {
           
        });
    
    Target Pack_AuthInterfaces => x => x
        .DependsOn(Test_AuthInterfaces)
        .Executes(() =>
        {
            Pack(AuthInterfacesBuildParameters);
        });
    
    Target Publish_AuthInterfaces => x => x
        .DependsOn(Pack_AuthInterfaces)
        .Executes(() =>
        {
            Publish(AuthInterfacesBuildParameters);
        });
}