param($installPath, $toolsPath, $package, $project)

Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1
$msbuild.Xml.Imports | Where-Object {$_.Project.ToLowerInvariant().EndsWith("pegasus.targets") } | Foreach {
    write-host "Removing Pegasus Targets File:" $_.Project
    $_.Parent.RemoveChild($_)
}