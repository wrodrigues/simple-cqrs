FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/AppWorker/AppWorker.csproj", "src/AppWorker/"]
RUN dotnet restore "src/AppWorker/AppWorker.csproj"
COPY . .
WORKDIR /src/src/AppWorker
RUN dotnet publish "AppWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AppWorker.dll"]
