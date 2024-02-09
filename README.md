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

- **Swagger UI**: Accessible at [https://localhost:44334/swagger](https://localhost:44334/swagger) for interactive API documentation and testing. (When project is run in Debug mode).
- **Swagger JSON Schema**: Available at [https://localhost:44334/swagger/v1/swagger.json](https://localhost:44334/swagger/v1/swagger.json) for integration and development purposes. (When project is run in Debug mode).

## API Descriptions

For detailed information on the API, refer to the following documentation available at the repository root:
- [InventoryManagement.API.xml](./InventoryManagement.API.xml) for XML documentation of the API.
- [InventoryManagement_API_Documentation_V1_0.pdf](./InventoryManagement_API_Documentation_V1_0.pdf) for PDF documentation of the API.

  ## Obtaining a Token

### Register a New User (Optional)
If you don't already have a user account, you need to register one. Send a `POST` request to the `api/Auth/register` endpoint with the required user information.

**Example Request:**

```json
POST /api/Auth/register
{
    "email": "user@example.com",
    "password": "YourSecurePassword"
}
```
### Login to Obtain a Token
Send a POST request to the api/Auth/login endpoint using the registered user's credentials.

```json
POST /api/Auth/login
{
    "email": "user@example.com",
    "password": "YourSecurePassword"
}
```

**Example Response:**

```json
{
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ..."
}

```

### Using the Token to Access Secured Endpoints
Once you have the token, include it in the Authorization header as a Bearer token when making requests to secured endpoints.

**Using Postman:**
In Postman, make a new request to GET **/api/InventoryItems**.
Under the "Authorization" tab, select "Bearer Token" and paste your token into the token field.
**Using curl:**

```bash
curl -X GET "https://localhost:44334/api/InventoryItems" -H "accept: application/json" -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ..."

```

This command sends a GET request to the api/InventoryItems endpoint with the authorization token included in the header.


> **Note:**
>
> - Replace "https://localhost:44334/api/InventoryItems" with the actual URL of your API if different.
> - Ensure the token is valid and has not expired, as expired tokens will result in unauthorized access errors.


## Conclusion

The Inventory Management System is designed for security and efficiency, facilitating easy extension and maintenance while safeguarding against common web vulnerabilities. The system's architecture and security measures ensure a robust solution for inventory management.

## License

Copyright (c) [2024] [Olivers Vilks]

This project is available for non-commercial use. Individuals and educational institutions are free to use, modify, and distribute the software provided in this project for educational and research purposes. Commercial use by companies and for-profit organizations is strictly prohibited without explicit permission from the copyright owner.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


