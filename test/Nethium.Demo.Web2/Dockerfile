FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-stretch AS build
WORKDIR /src
COPY ["test/Nethium.Demo.Web2/Nethium.Demo.Web2.csproj", "test/Nethium.Demo.Web2/"]
COPY ["test/Nethium.Demo.Abstraction/Nethium.Demo.Abstraction.csproj", "test/Nethium.Demo.Abstraction/"]
COPY ["test/Nethium.Demo.Service.Calc/Nethium.Demo.Service.Calc.csproj", "test/Nethium.Demo.Service.Calc/"]
COPY ["test/Nethium.Demo.Stub/Nethium.Demo.Stub.csproj", "test/Nethium.Demo.Stub/"]
COPY ["src/Nethium.Swagger/src/Nethium.Swagger.csproj", "src/Nethium.Swagger/src/"]
COPY ["src/Nethium.Authentication/src/Nethium.Authentication.csproj", "src/Nethium.Authentication/src/"]
COPY ["src/Nethium.Abstraction/src/Nethium.Abstraction.csproj", "src/Nethium.Abstraction/src/"]
COPY ["src/Nethium.Core/src/Nethium.Core.csproj", "src/Nethium.Core/src/"]
COPY ["src/Nethium.Configuration/src/Nethium.Configuration.csproj", "src/Nethium.Configuration/src/"]
COPY ["src/Nethium.Consul/src/Nethium.Consul.csproj", "src/Nethium.Consul/src/"]
COPY ["src/Nethium.DependencyInjection/src/Nethium.DependencyInjection.csproj", "src/Nethium.DependencyInjection/src/"]
COPY ["src/Nethium.ServiceDiscovery/src/Nethium.ServiceDiscovery.csproj", "src/Nethium.ServiceDiscovery/src/"]
RUN dotnet restore "test/Nethium.Demo.Web2/Nethium.Demo.Web2.csproj"
COPY . .
WORKDIR "/src/test/Nethium.Demo.Web2"
RUN dotnet build "Nethium.Demo.Web2.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Nethium.Demo.Web2.csproj" -c Release -o /app
COPY ["test/Nethium.Demo.Web2/run.sh", "/app"]
RUN chmod +x /app/run.sh

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENV DOCKERIZE_VERSION v0.6.1
COPY test/DockerCompose/dockerize-linux-amd64-$DOCKERIZE_VERSION.tar.gz .
# RUN curl -OL https://github.com/jwilder/dockerize/releases/download/$DOCKERIZE_VERSION/dockerize-linux-amd64-$DOCKERIZE_VERSION.tar.gz \
RUN true \
    && tar -C /usr/local/bin -xzvf dockerize-linux-amd64-$DOCKERIZE_VERSION.tar.gz \
    && rm dockerize-linux-amd64-$DOCKERIZE_VERSION.tar.gz

ENTRYPOINT ["./run.sh"]

CMD ["dotnet", "Nethium.Demo.Web2.dll"]