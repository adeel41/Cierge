FROM microsoft/dotnet:2.1-sdk-stretch AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY Cierge/*.csproj ./cierge/
WORKDIR /app/cierge
RUN dotnet restore

# copy and build app and libraries
WORKDIR /app/
COPY cierge/. ./cierge/
WORKDIR /app/cierge
# add IL Linker package
RUN dotnet add package ILLink.Tasks -v 0.1.5-preview-1841731 -s https://dotnet.myget.org/F/dotnet-core/api/v3/index.json
RUN dotnet publish -c Release -r linux-x64 -o out /p:ShowLinkerSizeComparison=true

# test application -- see: dotnet-docker-unit-testing.md
#FROM build AS testrunner
#WORKDIR /app/tests
#COPY tests/. .
#ENTRYPOINT ["dotnet", "test", "--logger:trx"]

FROM microsoft/dotnet:2.1-runtime-deps-stretch-slim AS runtime
WORKDIR /app
COPY --from=build /app/cierge/out ./
COPY rsa_signing_key.json /run/secrets/
ENV ASPNETCORE_ENVIRONMENT Production
EXPOSE 5000

ENTRYPOINT ["./cierge"]