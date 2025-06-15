
public class BuildParameters
{
    public string Version { get; set; }
    public string ProjectName { get; set; }
    public string ProjectFolder { get; set; }
    public string ProjectFilePath => System.IO.Path.Combine(ProjectFolder, ProjectName+ ".csproj");
    public string PackageName { get; set; }
}