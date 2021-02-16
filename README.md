# brockeragefirm
A backend challenge (GBM Software Engineer Challenge)

## Technologies
* ASP.NET Core 5
* Entity Framework Core 5
* MediatR
* AutoMapper
* FluentValidation

## Getting Started
1. Install the latest [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
2. Add SQLite extension in VS Code or SQLite/SQL Server Compact Toolbox in Visual Studio 2019 Community, to explore the database if you want.
3. Navigate to `src/Api` and run `dotnet run` to launch the API (ASP .NET Core Web API)

### Database Configuration

The solution is configured to use Sqlite by default.
Verify the **DefaultConnection** connextion string within **appsettings.json** points to a valid Sqlite instance.

### Database Migration

To use `dotnet-ef` for your migrations add the following flags to your command (values assume you are executing from repository root)

* `--project src/Infrastructure` (optional if in this folder)
* `--startup-project src/API`
* `--output-dir Persistence/Migrations`

For example, to add a new migration from the root folder:

 `dotnet ef migrations add "MyMigration" --project src\Infrastructure --startup-project src\API --output-dir Persistence\Migrations`

Also, when the application is run for the first time it builds an automatic migration populating 2 test users:
* **Email**: jose@test.com, **Password**: Pa$$w0rd1
* **Email**: maria@test.com, **Password**: Pa$$w0rd1

## Overview

### Domain

This contains all business entities to the domain layer. 

### Application

This layer contains the application logic. It's dependent on the domain layer but has no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers. For example, if the application needs to access a notification service, a new interface would be added to the application and implementation would be created within the infrastructure.

### Infrastructure

This layer contains classes for accessing external resources such as persistence and identity services. These classes should be based (in most of the cases) on interfaces defined within the application layer. You can add other external resources as web services, SMTP, file system, etc. It's dependant on the application layer and has no other dependencies on any other layer or project.

### API

This layer is a Web API based on ASP.NET Core 5. This layer depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only *Startup.cs* should reference Infrastructure.

#### Settings
Within **appsettings.json** you can configure:

* Token key and its expiration in hours, although probably is better to use [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=windows) when your application is important and it will be uploaded to Github or another repository.
* Market hours, setting the time open and close.

## Postman

I added a JSON file to import directly a collection of requests in Postman. You can test the API starting with the login of one or both users. Each login request sets a global variable to save the token generated *jose_token* or *maria_token*. Use those tokens to authorize every request. 
