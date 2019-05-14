#!/usr/bin/env bash
rm -rf *.sln && dotnet new sln && dotnet sln add **/**/*.csproj