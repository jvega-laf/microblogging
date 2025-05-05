# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY Microblogging.sln .
COPY Microblogging.*/*.csproj ./
RUN for file in Microblogging.*.csproj; do \
    mkdir -p ${file%.csproj} && \
    mv $file ${file%.csproj}/; \
    done

COPY . .
RUN dotnet restore
RUN dotnet publish Microblogging.Api/Microblogging.Api.csproj -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Microblogging.Api.dll"]
