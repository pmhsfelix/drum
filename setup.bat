git submodule init
git submodule update
nuget restore src\Drum.sln
msbuild vendor\Strathweb.TypedRouting\Strathweb.TypedRouting.sln