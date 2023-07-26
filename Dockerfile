FROM mcr.microsoft.com/playwright/dotnet:v1.36.0-jammy AS base
RUN apt-get update \
    && apt-get install -y dotnet-sdk-7.0 \
    && DEBIAN_FRONTEND=noninteractive TZ=Etc/UTC apt-get -y install tzdata 
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["EECEBOT.API/EECEBOT.API.csproj", "EECEBOT.API/"]
COPY ["EECEBOT.Application/EECEBOT.Application.csproj", "EECEBOT.Application/"]
COPY ["EECEBOT.Domain/EECEBOT.Domain.csproj", "EECEBOT.Domain/"]
COPY ["EECEBOT.Contracts/EECEBOT.Contracts.csproj", "EECEBOT.Contracts/"]
COPY ["EECEBOT.Infrastructure/EECEBOT.Infrastructure.csproj", "EECEBOT.Infrastructure/"]
RUN dotnet restore "EECEBOT.API/EECEBOT.API.csproj"
COPY . .
WORKDIR "/src/EECEBOT.API"
RUN dotnet build "EECEBOT.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EECEBOT.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EECEBOT.API.dll"]
