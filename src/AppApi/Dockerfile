FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/AppApi/AppApi.csproj", "src/AppApi/"]
RUN dotnet restore "src/AppApi/AppApi.csproj"
COPY . .
WORKDIR /src/src/AppApi
RUN dotnet publish "AppApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet tool install --global dotnet-ef \
    && PATH="${PATH}:/root/.dotnet/tools" dotnet ef migrations bundle --self-contained -r linux-x64 -o /app/efbundle

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=build /app/publish .
COPY --from=build /app/efbundle ./efbundle
COPY src/AppApi/script.sh ./script.sh
RUN chmod +x ./efbundle ./script.sh
ENTRYPOINT ["./script.sh"]
