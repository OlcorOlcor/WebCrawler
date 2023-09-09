# frontend build stage
FROM node:14 AS frontend
WORKDIR /build
COPY WebCrawler/WebCrawler/package.json .             
COPY WebCrawler/WebCrawler/package-lock.json .
RUN npm install
COPY WebCrawler/WebCrawler/rollup.config.mjs .
COPY WebCrawler/WebCrawler/wwwroot ./wwwroot
RUN npm run build

# backend build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS backend
WORKDIR /build
COPY WebCrawler/WebCrawler/WebCrawler.csproj .
RUN dotnet restore WebCrawler.csproj
COPY WebCrawler/WebCrawler .
RUN dotnet publish -c Release -o /publish

# combine and run
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /app
COPY --from=frontend /build/wwwroot ./wwwroot
COPY --from=backend /publish .
ENTRYPOINT /app/WebCrawler