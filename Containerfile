# Use Red Hat UBI 8 for runtime
FROM registry.access.redhat.com/ubi8/dotnet-70-runtime AS base

# Use Red Hat UBI 8 SDK for build
FROM registry.access.redhat.com/ubi8/dotnet-70 AS build

USER root
WORKDIR /src
COPY . .

# Build the application
FROM build AS restore
RUN dotnet restore "RefKafkaApp.ConsumerService.sln"

RUN dotnet test "RefKafkaApp.ConsumerService.sln" -c Release -o /app/build

# Publish the application
FROM restore AS publish
RUN dotnet publish "RefKafkaApp.ConsumerService.sln" -c Release -o /app/publish

# Create the final image
FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consumer.Api.dll"]