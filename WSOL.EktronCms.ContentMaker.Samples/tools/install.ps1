param($installPath, $toolsPath, $package, $project)
$project.Object.References | Where-Object { $_.Name -eq $package.Id } | ForEach-Object { $_.Remove() }