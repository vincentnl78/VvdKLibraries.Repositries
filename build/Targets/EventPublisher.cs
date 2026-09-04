
// ReSharper disable InconsistentNaming



using Nuke.Common;

// ReSharper disable once CheckNamespace
partial class Build
{
    readonly BuildParameters EventPublisherBuildParameters = new BuildParameters
    {
        ProjectFolder = "EventPublisher.Service",
        ProjectName = "EventPublisher.Service"
    };
    
    Target SetBuildParameters_EventPublisher => x => x
        .DependsOn(LoadSettings)
        .Executes(() =>
        {
            SetParameters(EventPublisherBuildParameters);
        });
    
    Target SetVersionOfPackages_EventPublisher => x => x
        .DependsOn(SetBuildParameters_EventPublisher)
        .Executes(() =>
        {
            SetParameters(EventPublisherInterfacesBuildParameters);
            MatchVersionToOtherProject(EventPublisherBuildParameters,EventPublisherInterfacesBuildParameters);
            SetVersionOfPackage(
                EventPublisherBuildParameters, 
                EventPublisherInterfacesBuildParameters
                );
        });
    
    Target Clean_EventPublisher => x => x
        .DependsOn(SetVersionOfPackages_EventPublisher)
        .Executes(() =>
        {
            Clean(EventPublisherBuildParameters);
        });
    
    Target Restore_EventPublisher => x => x
        .DependsOn(Clean_EventPublisher)
        .After(Publish_EventPublisherInterfaces) // Ensure contracts are published before restoring repositries
        .Executes(() =>
        {
            Restore(EventPublisherBuildParameters,false);
        });
    
    Target Compile_EventPublisher => x => x
        .DependsOn(Restore_EventPublisher)
        .Executes(() =>
        {
            Compile(EventPublisherBuildParameters);
        });
    
    Target Test_EventPublisher => x => x
        .DependsOn(Compile_EventPublisher)
        .Executes(() =>
        {
           
        });
    
    Target Pack_EventPublisher => x => x
        .DependsOn(Test_EventPublisher)
        .Executes(() =>
        {
            Pack(EventPublisherBuildParameters);
        });
    
    Target Publish_EventPublisher => x => x
        .DependsOn(Pack_EventPublisher)
        .Executes(() =>
        {
            Publish(EventPublisherBuildParameters);
        });
}