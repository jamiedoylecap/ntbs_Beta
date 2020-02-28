FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# restore audit lib (in a separate layer to speed up the build)
COPY EFAuditer/EFAuditer.csproj ./EFAuditer/
RUN dotnet restore EFAuditer/EFAuditer.csproj

# restore app (in a separate layer to speed up the build)
COPY ./NuGet.Config ./NuGet.Config
COPY ./frontend-dotnetcore/dist ./frontend-dotnetcore/dist
COPY ntbs-service/ntbs-service.csproj ./ntbs-service/
RUN dotnet restore ntbs-service/ntbs-service.csproj

# copy and build app

COPY ntbs-service/ ./ntbs-service/
COPY ./EFAuditer ./EFAuditer
RUN dotnet publish ntbs-service/*.csproj -c Release -o out

FROM node AS build-frontend
WORKDIR /app

# copy package.json and restore as distinct layers
COPY ntbs-service/package.json .
COPY ntbs-service/package-lock.json .
RUN npm install

# copy everything else and build frontend app
COPY ./ntbs-service/wwwroot ./wwwroot
COPY ./ntbs-service/tsconfig.json ./
COPY ./ntbs-service/webpack* ./
RUN npm run build:prod


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime

RUN apt-get update \
    && apt-get install -y krb5-user \
    && apt-get install -y procps

# Satisfying Openshift requirements:
# - this tells it that the app is OK to run under random user id
USER 1001
# - we don't have the permissions to run on default 80 port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

WORKDIR /app
COPY --from=build /app/ntbs-service/out ./
COPY --from=build-frontend /app/wwwroot/dist ./wwwroot/dist/

ENTRYPOINT ["dotnet", "ntbs-service.dll"]