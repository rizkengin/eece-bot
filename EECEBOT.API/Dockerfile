FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
RUN apt-get update -yq \
    && apt-get install wget -yq \
    && wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update -yq \
    && apt-get install powershell -yq
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
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
COPY --from=build /app/build/playwright.ps1 /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN pwsh playwright.ps1 install
RUN pwsh playwright.ps1 install-deps
RUN rm -rf playwright.ps1
RUN apt remove wget powershell -yq
ENTRYPOINT ["dotnet", "EECEBOT.API.dll"]