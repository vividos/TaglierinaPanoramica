#!/usr/bin/env bash

# remove the UWP project in AppCenter builds to prevent an error when trying
# to restore NuGet packages for it.
rm ../UWP/TaglierinaPanoramica.UWP.csproj