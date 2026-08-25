# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .
COPY src/BankMore.Auth.API/*.csproj ./src/BankMore.Auth.API/
COPY src/BankMore.Auth.Application/*.csproj ./src/BankMore.Auth.Application/
COPY src/BankMore.Auth.Domain/*.csproj ./src/BankMore.Auth.Domain/
COPY src/BankMore.Auth.Infrastructure/*.csproj ./src/BankMore.Auth.Infrastructure/
COPY src/BankMore.Web/*.csproj ./src/BankMore.Web/
COPY tests/BankMore.Auth.Tests/*.csproj ./tests/BankMore.Auth.Tests/

RUN dotnet restore

COPY . .

WORKDIR /src/src/BankMore.Web

RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "BankMore.Web.dll"]
