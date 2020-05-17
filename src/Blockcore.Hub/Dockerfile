FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /usr/local/app/

RUN apt-get update \
    && apt-get install -y curl \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

COPY . ./
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 9912
ENTRYPOINT ["dotnet", "Blockcore.Hub.dll"]
