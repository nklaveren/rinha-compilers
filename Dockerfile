FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /s

COPY . .
RUN dotnet restore && dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
COPY --from=build-env s/out .


ENTRYPOINT ["dotnet", "RinhaDeCompiler.dll"]