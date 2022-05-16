#!/usr/bin/env bash

# remove the UWP project in AppCenter builds to prevent an error when trying
# to restore NuGet packages for it.
rm ../UWP/TaglierinaPanoramica.UWP.csproj

# fix AppCenter build by downgrading NuGet to 5.6
LATEST_NUGET_PATH='/Library/Frameworks/Mono.framework/Versions/Latest/lib/mono/nuget'
NUGET_URL='https://dist.nuget.org/win-x86-commandline/v5.6.0/nuget.exe'

cd $LATEST_NUGET_PATH
sudo mv nuget.exe nuget_old.exe

sudo curl $NUGET_URL -4 -sL -o './nuget.exe'
sudo chmod a+x nuget.exe
