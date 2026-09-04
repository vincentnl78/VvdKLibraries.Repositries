// ReSharper disable InconsistentNaming
using Nuke.Common;

// ReSharper disable once CheckNamespace
partial class Build
{
    readonly BuildParameters EventPublisherInterfacesBuildParameters = new BuildParameters
    {
        ProjectFolder = "EventPublisher.Interfaces",
        ProjectName = "EventPublisher.Interfaces"
    };
    
    Target SetParameters_EventPublisherInterfaces => x => x
        .DependsOn(LoadSettings)
        .Executes(() =>
        {
            SetParameters(EventPublisherInterfacesBuildParameters);
        });
    
    Target IncreaseVersion_EventPublisherInterfaces => x => x
        .DependsOn(SetParameters_EventPublisherInterfaces)
        .Executes(() =>
        {
            BumpVersion(EventPublisherInterfacesBuildParameters);
        });
    
    Target Clean_EventPublisherInterfaces => x => x
        .DependsOn(IncreaseVersion_EventPublisherInterfaces)
        .Executes(() =>
        {
            Clean(EventPublisherInterfacesBuildParameters);
        });

    Target Restore_EventPublisherInterfaces => x => x
        .DependsOn(Clean_EventPublisherInterfaces)
        .Executes(() =>
        {
            Restore(EventPublisherInterfacesBuildParameters,true);
        });
    
    Target Compile_EventPublisherInterfaces => x => x
        .DependsOn(Restore_EventPublisherInterfaces)
        .Executes(() =>
        {
            Compile(EventPublisherInterfacesBuildParameters);
        });
    
    Target Test_EventPublisherInterfaces => x => x
        .DependsOn(Compile_EventPublisherInterfaces)
        .Executes(() =>
        {
           
        });
    
    Target Pack_EventPublisherInterfaces => x => x
        .DependsOn(Test_EventPublisherInterfaces)
        .Executes(() =>
        {
            Pack(EventPublisherInterfacesBuildParameters);
        });
    
    Target Publish_EventPublisherInterfaces => x => x
        .DependsOn(Pack_EventPublisherInterfaces)
        .Executes(() =>
        {
            Publish(EventPublisherInterfacesBuildParameters);
        });
}