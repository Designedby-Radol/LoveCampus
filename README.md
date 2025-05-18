# Campus Love - Dating App for Campus Students

A C# console application that simulates a dating/matching system for campus students. The application follows clean architecture principles and implements the Factory Method pattern for database connections.

## Features

- User registration with profile details (name, age, gender, interests, career, profile phrase)
- Profile browsing with like/dislike functionality
- Matching system when two users like each other
- Credit system limiting daily likes
- Store for purchasing additional credits with Capcoins
- Statistics dashboard showing most popular users

## Project Structure

```
CampusLove/
├── Domain/                # Domain layer
│   ├── Entities/          # Domain entities
│   └── Interfaces/        # Domain interfaces
├── Infrastructure/        # Infrastructure layer
│   ├── Config/            # Configuration
│   ├── Factories/         # Factory pattern implementation
│   └── Repositories/      # Repository implementations
├── App/                   # Application layer
│   ├── Services/          # Application services
│   └── UI/                # User interface components
└── Program.cs             # Entry point
```

## Prerequisites

- .NET 8.0 SDK
- MySQL Server

## Setup

1. Create the database by running the SQL script: `database.sql`
2. Update the database connection string in `Infrastructure/Config/DatabaseConfig.cs` if needed
3. Build and run the application:

```
dotnet build
dotnet run
```

## Technologies

- C# / .NET 8.0
- MySQL
- Dapper ORM
- Spectre.Console (for enhanced console UI)

## Design Patterns

- Factory Method: Used for creating repositories
- Repository Pattern: For database access
- Clean Architecture: Separation of concerns with domain, application, and infrastructure layers