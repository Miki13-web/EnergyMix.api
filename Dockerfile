FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY EnergyMix.api/EnergyMix.api.csproj EnergyMix.api/
RUN dotnet restore EnergyMix.api/EnergyMix.api.csproj

COPY . .
WORKDIR /src/EnergyMix.api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "EnergyMix.api.dll"]
