#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ncov.api/ncov.api.csproj", "ncov.api/"]
COPY ["NCovid.Service/NCovid.Service.csproj", "NCovid.Service/"]
RUN dotnet restore "ncov.api/ncov.api.csproj"
COPY . .
WORKDIR "/src/ncov.api"
RUN dotnet build "ncov.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ncov.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ncov.api.dll"]