FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 8000

FROM node:14-buster-slim AS node_base
COPY . .
FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
COPY --from=node_base . .

WORKDIR /src
COPY . .
RUN dotnet restore "Foundation.csproj" --configfile ../Nuget.config

RUN npm ci
RUN npm run dev

RUN dotnet build "Foundation.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "Foundation.csproj" -c Debug -o /app/publish
COPY ./docker/build-script/wait_sqlserver_start_and_attachdb.sh /app/publish/wait_sqlserver_start_and_attachdb.sh
RUN chmod -R 777 /app/publish/wait_sqlserver_start_and_attachdb.sh
COPY ./App_Data/* /app/publish/App_Data/

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

#wait sql server container start and attach alloy database then start web
ENTRYPOINT ./wait_sqlserver_start_and_attachdb.sh

