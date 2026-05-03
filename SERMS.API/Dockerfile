# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution + restore
COPY ["SERMS.API/SERMS.API.csproj", "SERMS.API/"]
COPY ["SERMS.Domain/SERMS.Domain.csproj", "SERMS.Domain/"]
COPY ["SERMS.Infrastructure/SERMS.Infrastructure.csproj", "SERMS.Infrastructure/"]

RUN dotnet restore "SERMS.API/SERMS.API.csproj"

# Copy everything else
COPY . .

WORKDIR "/src/SERMS.API"
RUN dotnet publish "SERMS.API.csproj" -c Release -o /app/publish

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "SERMS.API.dll"]