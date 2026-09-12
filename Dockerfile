# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj first and restore (better Docker layer caching)
COPY Shreeyan.csproj ./
RUN dotnet restore "Shreeyan.csproj"

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish "Shreeyan.csproj" -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Cloud Run injects PORT (defaults to 8080) and expects the container
# to listen on 0.0.0.0:$PORT over plain HTTP — Cloud Run itself
# terminates TLS at the edge, so we don't run HTTPS inside the container.
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "Shreeyan.dll"]