FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY EcommerceApi.slnx .
COPY src/EcommerceApi.Api/EcommerceApi.Api.csproj                           src/EcommerceApi.Api/
COPY src/EcommerceApi.Application/EcommerceApi.Application.csproj           src/EcommerceApi.Application/
COPY src/EcommerceApi.Domain/EcommerceApi.Domain.csproj                     src/EcommerceApi.Domain/
COPY src/EcommerceApi.Infrastructure/EcommerceApi.Infrastructure.csproj     src/EcommerceApi.Infrastructure/

RUN dotnet restore

COPY . .

RUN dotnet publish src/EcommerceApi.Api/EcommerceApi.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN groupadd --system appgroup && useradd --system --gid appgroup appuser

COPY --from=build /app/publish .

USER appuser

EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "EcommerceApi.Api.dll"]