properties {
	$base_directory = Resolve-Path .
	$src_directory = "$base_directory\src"
	$output_directory = "$base_directory\build"
	$sln_file = "$src_directory\drum.sln"
	$target_config = "Release"
	$framework_version = "v4.5"
	$xunit_path = "$src_directory\packages\xunit.runners.1.9.2\tools\xunit.console.clr4.exe"
	$nuget_path = "$src_directory\.nuget\nuget.exe"

	$buildNumber = 0;
	$version = "1.0.0.0"
	$preRelease = $null
}

task default -depends Clean, RunTests

task Clean {
	rmdir $output_directory -ea SilentlyContinue -recurse	
	exec { msbuild /nologo /verbosity:quiet $sln_file /p:Configuration=$target_config /t:Clean }
}

task Compile -depends Clean {
	exec { msbuild /nologo /verbosity:q $sln_file /p:Configuration=$target_config /p:TargetFrameworkVersion=v4.5 }
}

task RunTests -depends Compile {
	$project = "Drum.Tests"
	mkdir $output_directory\xunit\$project -ea SilentlyContinue
	.$xunit_path "$src_directory\$project\bin\Release\$project.dll" /html "$output_directory\xunit\$project\index.html"
}

