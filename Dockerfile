FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CatalogoRopaMVC.csproj ./
RUN dotnet restore CatalogoRopaMVC.csproj

COPY . .
RUN dotnet publish CatalogoRopaMVC.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0

EXPOSE 8080

COPY --from=build --chown=app:app /app/publish .

USER app
ENTRYPOINT ["dotnet", "CatalogoRopaMVC.dll"]
