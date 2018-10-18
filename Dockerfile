FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY Cierge/*.csproj ./cierge/
WORKDIR /app/cierge
RUN dotnet restore

# copy and build app and libraries
WORKDIR /app/
COPY Cierge/. ./cierge/
WORKDIR /app/cierge

RUN dotnet publish -c Release --output /out/

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /out .
COPY rsa_signing_key.json /run/secrets/
COPY cierge.appsettings.json appsettings.json

ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS "http://*:5000"
EXPOSE 5000

ENTRYPOINT ["dotnet", "Cierge.dll"]