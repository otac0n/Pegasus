param($installPath, $toolsPath, $package, $project)

$targetsFile = [System.IO.Path]::Combine($toolsPath, 'Pegasus.targets')
$targetsUri = new-object Uri('file://' + $targetsFile)
$projectUri = new-object Uri('file://' + $project.FullName)
$targetsPath = $projectUri.MakeRelativeUri($targetsUri).ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar)

write-host "Importing Pegasus Targets File:" $targetsPath
Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1
$msbuild.Xml.AddImport($targetsPath) | Out-Null