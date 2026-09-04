
// ReSharper disable InconsistentNaming



using Nuke.Common;

// ReSharper disable once CheckNamespace
partial class Build
{
    readonly BuildParameters AuthServicesBuildParameters = new()
    {
        ProjectFolder = "Auth.Service",
        ProjectName = "Auth.Service"
    };
    
    Target SetBuildParameters_AuthServices => x => x
        .DependsOn(LoadSettings)
        .Executes(() =>
        {
            SetParameters(AuthServicesBuildParameters);
        });
    
    Target SetVersionOfPackages_AuthServices => x => x
        .DependsOn(SetBuildParameters_AuthServices)
        .Executes(() =>
        {
            SetParameters(AuthInterfacesBuildParameters);
            MatchVersionToOtherProject(AuthServicesBuildParameters,AuthInterfacesBuildParameters);
            SetVersionOfPackage(
                AuthServicesBuildParameters,
                AuthInterfacesBuildParameters
                );
        });
    
    Target Clean_AuthServices => x => x
        .DependsOn(SetVersionOfPackages_AuthServices)
        .Executes(() =>
        {
            Clean(AuthServicesBuildParameters);
        });
    
    Target Restore_AuthServices => x => x
        .DependsOn(Clean_AuthServices)
        .After(Publish_AuthObjects, Publish_AuthInterfaces, Publish_RepositriesContracts) // Ensure contracts are published before restoring repositries
        .Executes(() =>
        {
            Restore(AuthServicesBuildParameters,false);
        });
    
    Target Compile_AuthServices => x => x
        .DependsOn(Restore_AuthServices)
        .Executes(() =>
        {
            Compile(AuthServicesBuildParameters);
        });
    
    Target Test_AuthServices => x => x
        .DependsOn(Compile_AuthServices)
        .Executes(() =>
        {
           
        });
    
    Target Pack_AuthServices => x => x
        .DependsOn(Test_AuthServices)
        .Executes(() =>
        {
            Pack(AuthServicesBuildParameters);
        });
    
    Target Publish_AuthServices => x => x
        .DependsOn(Pack_AuthServices)
        .Executes(() =>
        {
            Publish(AuthServicesBuildParameters);
        });
}