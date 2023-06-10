FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Web/Web.csproj", "Web/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Contracts/Contracts/Contracts.csproj", "Contracts/Contracts/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Presentation/Presentation.csproj", "Presentation/"]
RUN dotnet restore "Web/Web.csproj"
COPY . .
COPY Web/appsettings.json /app/appsettings.json
WORKDIR "/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /app/appsettings.json appsettings.json


# Set the default connection string as an environment variable
ARG DEFAULT_CONNECTION_STRING
ENV DefaultConnectionString=$DEFAULT_CONNECTION_STRING

# Run the script to replace the placeholder with the environment variable value
COPY env-config.sh /app/env-config.sh
RUN chmod +x /app/env-config.sh
RUN /app/env-config.sh

ENTRYPOINT ["dotnet", "Web.dll"]
