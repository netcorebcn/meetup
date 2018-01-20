#!/bin/bash
# shopt -s globstar

rm *.sln
dotnet new sln 
dotnet sln add **/*.csproj
