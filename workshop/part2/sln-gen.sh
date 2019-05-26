rm -f *.sln
dotnet new sln -n $1 
dotnet sln add **/*.csproj  