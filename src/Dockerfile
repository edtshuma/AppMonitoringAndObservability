FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app
EXPOSE 80
COPY ./publish .

ENTRYPOINT [ "dotnet", "PlatformService.dll" ]
