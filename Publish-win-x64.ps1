dotnet publish -r win-x64 --no-self-contained --no-dependencies -c Release
$path = ".\Publish\win-x64"
Remove-Item $path\Bin\ -Recurse -ErrorAction Ignore
Copy-Item .\Bin\win-x64\publish -Destination $path\Bin -Recurse -Force
Remove-Item $path\Config -Recurse -ErrorAction Ignore
Copy-Item .\Config -Destination $path\Config  -Recurse -Force