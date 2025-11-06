# syntax=docker/dockerfile:1

# Create a stage for building the application.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# For Dockerfile parametrization
ARG MODULENAME

COPY deps /source/deps

# Copy only csproj for restoring
COPY Modules/${MODULENAME}/API/${MODULENAME}Module.API.csproj                       /source/Modules/${MODULENAME}/API/
COPY Modules/${MODULENAME}/Application/${MODULENAME}Module.Application.csproj       /source/Modules/${MODULENAME}/Application/
COPY Modules/${MODULENAME}/Domain/${MODULENAME}Module.Domain.csproj                 /source/Modules/${MODULENAME}/Domain/
COPY Modules/${MODULENAME}/Infrastructure/${MODULENAME}Module.Infrastructure.csproj /source/Modules/${MODULENAME}/Infrastructure/

COPY MyMiniGrading.sln /source/

# Restoring dependencies (separately from all other files for caching)
WORKDIR /source/Modules/${MODULENAME}/API/
RUN dotnet restore "${MODULENAME}Module.API.csproj" --use-current-runtime

# Copy only necessary folders for the project
COPY Modules/${MODULENAME} /source/Modules/${MODULENAME}

# Test
RUN dotnet test --no-restore

# This is the architecture you’re building for, which is passed in by the builder.
# Placing it here allows the previous steps to be cached across architectures.
ARG TARGETARCH

# Build the application.
# If TARGETARCH is "amd64", replace it with "x64" - "x64" is .NET's canonical name for this and "amd64" doesn't
#   work in .NET 6.0.
RUN dotnet publish "${MODULENAME}Module.API.csproj" -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app --no-restore

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
EXPOSE 80

# For Dockerfile parametrization
ARG MODULENAME

WORKDIR /app

# Copy everything needed to run the app from the "build" stage.
COPY --chown=$APP_UID:$APP_UID --from=build /app .

# Switch to a non-privileged user (defined in the base image) that the app will run under.
USER $APP_UID

# Define entrypoint
ENV DLL="${MODULENAME}Module.API.dll"
ENTRYPOINT dotnet $DLL
