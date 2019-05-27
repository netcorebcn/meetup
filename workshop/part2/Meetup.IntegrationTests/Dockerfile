FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS builder
ARG main

COPY ${main}*.csproj ${main}
RUN dotnet restore ${main}
COPY ${main} ${main}

WORKDIR ${main}
RUN dotnet build

ENTRYPOINT [ "dotnet", "test", "--no-build" ]