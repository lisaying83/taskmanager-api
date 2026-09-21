FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/TaskManager.Api/TaskManager.Api.csproj", "src/TaskManager.Api/"]
COPY ["src/TaskManager.Application/TaskManager.Application.csproj", "src/TaskManager.Application/"]
COPY ["src/TaskManager.Infrastructure/TaskManager.Infrastructure.csproj", "src/TaskManager.Infrastructure/"]
COPY ["src/TaskManager.Domain/TaskManager.Domain.csproj", "src/TaskManager.Domain/"]

RUN dotnet restore "src/TaskManager.Api/TaskManager.Api.csproj"

COPY . .

RUN dotnet build "src/TaskManager.Api/TaskManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/TaskManager.Api/TaskManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install New Relic .NET Agent
RUN apt-get update \
    && apt-get install -y curl gnupg ca-certificates \
    && curl --fail --silent --show-error \
       --location \
       --proto '=https' \
       --proto-redir '=https' \
       https://download.newrelic.com/NEWRELIC_APT_2DAD550E.public \
       | gpg --import --batch --no-default-keyring \
       --keyring /usr/share/keyrings/download.newrelic.com-newrelic.gpg \
    && echo 'deb [signed-by=/usr/share/keyrings/download.newrelic.com-newrelic.gpg] https://apt.newrelic.com/debian/ newrelic non-free' \
       > /etc/apt/sources.list.d/newrelic.list \
    && apt-get update \
    && apt-get install -y newrelic-dotnet-agent \
    && rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "TaskManager.Api.dll"]
