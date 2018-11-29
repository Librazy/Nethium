# Nethium

A modular microservice framework for ASP.NET Core

## Usage

1. Run `.UseNethium()` on your `IWebHostBuilder`

for 2.x

```csharp
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseNethium(args, CreateLoggerFactory())
        .UseStartup<Startup>();
```

for 3.0

```csharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder
                .UseNethium()
                .UseStartup<Startup>();
        });
```

2. Register microservice interfaces in `ConfigureServices`

```csharp
services.RegisterInterface<IStoreService>("store");
services.RegisterInterface<IAggregateService>("aggregate");
```

3. Register microservice class in `ConfigureServices`

```csharp
services.RegisterService<IStoreService, StoreController>("/api/store", "/api/store/health");
```

4. Register microservice stub assembly in `ConfigureServices`

```csharp
services.AddStub(Assembly.GetAssembly(typeof(SwaggerException)));
```

5. Run `.AddNethiumControllers()` on your `IMvcBuilder`

```csharp
services
    .AddMvc()
    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
    .AddNethiumControllers();
```

6. Run `.AddNethiumServices()` on your IServiceCollection

```csharp
services.AddNethiumServices();
```

7. Enable swagger and authorization if needed in `Configure`

```csharp
app.UseAuthentication();

app.UseMvc();

app.UseSwagger();
app.UseSwaggerUi3();
```

## Configure

You can bootstrap Nethium configuration in three ways:

* Write your configuration in nethium.json
* Configure them in `.UseNethium()`
* Use environment variables

The config entries are:

| Config Entry                       | Description                               |
|------------------------------------|-------------------------------------------|
| Nethium:Consul:Address             | Address of Consul Agent					 |
| Nethium:Consul:Datacenter          | Datacenter of Consul						 |
| Nethium:Consul:Token               | Token of Consul							 |
| Nethium:Consul:WaitTime            | Wait time of Consul						 |
| Nethium:Consul:Prefix              | Key prefix								 |
| Nethium:Consul:ConfigurationPrefix | Key prefix to be removed					 |
| Nethium:Consul:Watch               | Key to watch for refreshing configuration |

After framework bootstrap, you can also store your configuration in Consul Key/Value

| Config Entry            | Description            |
|-------------------------|------------------------|
| Nethium:ServerId        | Server ID			   |
| Nethium:ServerSecretKey | Server JWT signing key |
| Nethium:BaseUrl         | Server base url		   |
| Nethium:Port            | Server port			   |

## Example

You can run the example docker-compose project in test/demo/DockerCompose by `docker-compose up --build`.

This command should build three ASP.NET Core image hosting five microservice, and bootstrap the cluster with three consul server and three consul client.

You may need to add a `settings/version` key to the Consul Key/Value storage so the apps will startup.
