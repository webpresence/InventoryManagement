# Inventory Management System

## Introduction

The Inventory Management System streamlines inventory tracking, providing secure and efficient management of stock levels, locations, and other critical information. It features robust security measures to ensure secure system access, data handling, and storage practices.

## System Architecture

The system is organized into four main projects:

- **Domain**: Contains core business logic and entities.
- **Infrastructure**: Implements the persistence layer, database interactions, and migrations.
- **API**: Serves as the entry point for the frontend, exposing RESTful APIs for inventory management and integrating security measures.
- **Security**: Enhances security with middleware and services, including XSS prevention.

## Setup Instructions

### Prerequisites

- Microsoft SQL Server
- .NET Core 8 SDK or later
- An IDE (Visual Studio or VS Code recommended)

### Database Configuration

1. **Connection String**: Update the `AppSettings.json` file in the API project with your SQL Server connection string:

    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;"
    }
    ```

2. **Database Migrations**: Apply the database schema by running:

    ```bash
    dotnet ef database update --startup-project InventoryManagement.Infrastructure/InventoryManagement.Infrastructure.csproj
    ```

    Migrations are also automatically applied at project launch if the connection string is correct.

### Modifying the Database Schema

- To change the table structure or add new tables, modify the Domain project.
- Add a new migration after changes with:

    ```bash
    dotnet ef migrations add <Migration_Name> --startup-project InventoryManagement.Infrastructure/InventoryManagement.Infrastructure.csproj
    ```

## Security Features

### Authentication and Authorization

- Utilizes JWT for secure authentication and authorization.

### Secure Data Handling and Storage

- **HTTPS**: Ensures encrypted data communication, enhancing security.
- **CORS**: Allows requests only from trusted origins.
- **Rate Limiting**: Restricts the number of API requests within a set timeframe.

### Preventing SQL Injection and XSS

- **Entity Framework Core**: Protects against SQL injection via parameterized queries.
- **AntiXssMiddleware**: Sanitizes incoming requests to prevent XSS attacks.

## Swagger Documentation

- **Swagger UI**: Accessible at [https://localhost:44334/swagger](https://localhost:44334/swagger) for interactive API documentation and testing.
- **Swagger JSON Schema**: Available at [https://localhost:44334/swagger/v1/swagger.json](https://localhost:44334/swagger/v1/swagger.json) for integration and development purposes.

## API Descriptions

For detailed information on the API, refer to the following documentation available at the repository root:
- [InventoryManagement.API.xml](./InventoryManagement.API.xml) for XML documentation of the API.
- [InventoryManagement_API_Documentation_V1_0.pdf](./InventoryManagement_API_Documentation_V1_0.pdf) for PDF documentation of the API.

## Conclusion

The Inventory Management System is designed for security and efficiency, facilitating easy extension and maintenance while safeguarding against common web vulnerabilities. The system's architecture and security measures ensure a robust solution for inventory management.
