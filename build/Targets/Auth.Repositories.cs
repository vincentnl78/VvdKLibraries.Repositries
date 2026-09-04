
// ReSharper disable InconsistentNaming



using Nuke.Common;

// ReSharper disable once CheckNamespace
partial class Build
{
    readonly BuildParameters AuthRepositriesBuildParameters = new()
    {
        ProjectFolder = "Auth.Repositories",
        ProjectName = "Auth.Repositories"
    };
    
    Target SetBuildParameters_AuthRepositries => x => x
        .DependsOn(LoadSettings)
        .Executes(() =>
        {
            SetParameters(AuthRepositriesBuildParameters);
        });
    
    Target SetVersionOfPackages_AuthRepositries => x => x
        .DependsOn(SetBuildParameters_AuthRepositries)
        .Executes(() =>
        {
            SetParameters(AuthInterfacesBuildParameters);
            MatchVersionToOtherProject(AuthRepositriesBuildParameters,AuthInterfacesBuildParameters);
            SetVersionOfPackage(
                AuthRepositriesBuildParameters,
                AuthInterfacesBuildParameters
                );
        });
    
    Target Clean_AuthRepositries => x => x
        .DependsOn(SetVersionOfPackages_AuthRepositries)
        .Executes(() =>
        {
            Clean(AuthRepositriesBuildParameters);
        });
    
    Target Restore_AuthRepositries => x => x
        .DependsOn(Clean_AuthRepositries)
        .After(Publish_AuthObjects, Publish_AuthInterfaces) // Ensure contracts are published before restoring repositries
        .Executes(() =>
        {
            Restore(AuthRepositriesBuildParameters,false);
        });
    
    Target Compile_AuthRepositries => x => x
        .DependsOn(Restore_AuthRepositries)
        .Executes(() =>
        {
            Compile(AuthRepositriesBuildParameters);
        });
    
    Target Test_AuthRepositries => x => x
        .DependsOn(Compile_AuthRepositries)
        .Executes(() =>
        {
           
        });
    
    Target Pack_AuthRepositries => x => x
        .DependsOn(Test_AuthRepositries)
        .Executes(() =>
        {
            Pack(AuthRepositriesBuildParameters);
        });
    
    Target Publish_AuthRepositries => x => x
        .DependsOn(Pack_AuthRepositries)
        .Executes(() =>
        {
            Publish(AuthRepositriesBuildParameters);
        });
}