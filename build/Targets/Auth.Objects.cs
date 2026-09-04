
// ReSharper disable InconsistentNaming



using Nuke.Common;

// ReSharper disable once CheckNamespace
partial class Build
{
    readonly BuildParameters AuthObjectsBuildParameters = new BuildParameters
    {
        ProjectFolder = "Auth.Objects",
        ProjectName = "Auth.Objects"
    };
    
    Target SetParameters_AuthObjects => x => x
        .DependsOn(LoadSettings)
        .Executes(() =>
        {
            SetParameters(AuthObjectsBuildParameters);
        });
    
    Target IncreaseVersion_AuthObjects => x => x
        .DependsOn(SetParameters_AuthObjects)
        .Executes(() =>
        {
            BumpVersion(AuthObjectsBuildParameters);
        });
    
    Target Clean_AuthObjects => x => x
        .DependsOn(IncreaseVersion_AuthObjects)
        .Executes(() =>
        {
            Clean(AuthObjectsBuildParameters);
        });

    Target Restore_AuthObjects => x => x
        .DependsOn(Clean_AuthObjects)
        .Executes(() =>
        {
            Restore(AuthObjectsBuildParameters,true);
        });
    
    Target Compile_AuthObjects => x => x
        .DependsOn(Restore_AuthObjects)
        .Executes(() =>
        {
            Compile(AuthObjectsBuildParameters);
        });
    
    Target Test_AuthObjects => x => x
        .DependsOn(Compile_AuthObjects)
        .Executes(() =>
        {
           
        });
    
    Target Pack_AuthObjects => x => x
        .DependsOn(Test_AuthObjects)
        .Executes(() =>
        {
            Pack(AuthObjectsBuildParameters);
        });
    
    Target Publish_AuthObjects => x => x
        .DependsOn(Pack_AuthObjects)
        .Executes(() =>
        {
            Publish(AuthObjectsBuildParameters);
        });
}