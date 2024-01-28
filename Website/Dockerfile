FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /Website

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-jammy-chiseled
WORKDIR /Website
COPY --from=build-env /Website/out .
ENTRYPOINT ["dotnet", "Website.dll"]