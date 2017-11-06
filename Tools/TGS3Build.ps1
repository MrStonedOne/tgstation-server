$bf = $Env:APPVEYOR_BUILD_FOLDER
$src = "$bf\TGInstallerWrapper\bin\Release"
$version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$src\TG Station Server Installer.exe").FileVersion

$doxdir = "C:\tgsdox"

New-Item -Path $doxdir -ItemType directory

$publish_dox = (-not (Test-Path Env:APPVEYOR_PULL_REQUEST_NUMBER)) -and ("$Env:APPVEYOR_REPO_BRANCH" -eq "master")
$github_url = "github.com/$Env:APPVEYOR_REPO_NAME"

if($publish_dox){
	echo "Cloning https://git@$github_url..."
	git clone -b gh-pages --single-branch "https://git@$github_url" "$doxdir" 2>$null
	rm -r "$doxdir\*"
}

Add-Content "$bf\Tools\Doxyfile" "`nPROJECT_NUMBER = $version`nINPUT = $bf`nOUTPUT_DIRECTORY = $doxdir`nPROJECT_LOGO = $bf/tgs.ico"
doxygen.exe "$bf\Tools\Doxyfile"

if($publish_dox){
	cd $doxdir
	git config --global push.default simple
	git config user.name "Appveyor CI"
	git config user.email "ci@appveyor.com"
	echo '# THIS BRANCH IS AUTO GENERATED BY APPVEYOR CI. SEE Tools/TGS3Build.ps1 ON MASTER' > README.md
	
	# Need to create a .nojekyll file to allow filenames starting with an underscore
	# to be seen on the gh-pages site. Therefore creating an empty .nojekyll file.
	echo "" > .nojekyll
	git add --all
	git commit -m "Deploy code docs to GitHub Pages for Appveyor build $Env:APPVEYOR_BUILD_NUMBER" -m "Commit: $Env:APPVEYOR_REPO_COMMIT"
    git push -f "https://$Env:repo_token@$github_url" 2>&1 | out-null
}

$destination = "$bf\TGS3-Server-v$version.exe"

Move-Item -Path "$src\TG Station Server Installer.exe" -Destination "$destination"

Add-Type -assembly "system.io.compression.filesystem"

$destination_md5sha = "$bf\MD5-SHA1-Server-v$version.txt"

$src2 = "$bf\ClientApps"
[system.io.directory]::CreateDirectory($src2)
Copy-Item "$bf\TGCommandLine\bin\Release\TGCommandLine.exe" "$src2\TGCommandLine.exe"
Copy-Item "$bf\TGControlPanel\bin\Release\TGControlPanel.exe" "$src2\TGControlPanel.exe"
Copy-Item "$bf\TGServiceInterface\bin\Release\TGServiceInterface.dll" "$src2\TGServiceInterface.dll"

$dest2 = "$bf\TGS3-Client-v$version.zip"

[io.compression.zipfile]::CreateFromDirectory($src2, $dest2) 
$destination_md5sha2 = "$bf\MD5-SHA1-Client-v$version.txt"

& fciv -both $destination > $destination_md5sha
& fciv -both $dest2 > $destination_md5sha2
