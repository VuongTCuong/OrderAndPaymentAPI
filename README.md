# OrderAndPaymentAPI

An **ASP.NET Core backend learning project** for product, shopping cart, and order management, with a simple Razor Pages interface. It demonstrates RESTful APIs, authentication, authorization, database persistence, and order-processing business logic.

> **Payment note:** Checkout is **simulated**: submitting checkout creates an order. This project does **not** process real payments or integrate a payment gateway.

## Tech stack

- **Backend:** C#, .NET 10, ASP.NET Core Web API
- **Database:** MySQL, Entity Framework Core (EF Core), EF Core migrations
- **Authentication:** JWT bearer tokens, refresh tokens, and cookie authentication for web pages
- **UI:** ASP.NET Core Razor Pages, CSS, JavaScript
- **Architecture:** Controllers, Services, Repositories, DTOs, dependency injection
- **Error handling:** Centralized exception handlers and Problem Details

## Features

- User registration and login
- JWT authentication, token refresh, and logout
- Role-based access for `user` and `admin`
- Product listing and product details; admin product management
- Shopping cart: view, add, update quantity, remove items, and clear cart
- Simulated checkout that creates an order from the cart
- Users can view their orders and request cancellation
- Admins can view orders and update order status
- Basic Razor Pages for account, product, cart, and order workflows

## API endpoints

The routes below reflect the current controllers in the repository.

| Method | Endpoint | Purpose | Access |
|---|---|---|---|
| POST | `/api/auth/register` | Register | Public |
| POST | `/api/auth/login` | Log in | Public |
| POST | `/api/auth/refresh` | Refresh token | Authenticated |
| POST | `/api/auth/logout` | Log out | Authenticated |
| GET | `/api/product` | List products | Public |
| GET | `/api/product/{id}` | Get product | Public |
| POST | `/api/product/add` | Add product | Admin |
| PUT | `/api/product` | Update product | Admin |
| DELETE | `/api/product/delete/{id}` | Delete product | Admin |
| GET | `/api/cart` | View cart | User |
| POST | `/api/cart/add` | Add to cart | User |
| PUT | `/api/cart/update` | Update cart | User |
| DELETE | `/api/cart/delete/{productId}` | Remove cart item | User |
| DELETE | `/api/cart/clear` | Clear cart | User |
| GET | `/api/order` | View all orders | Admin |
| GET | `/api/order/user` | View own orders | User |
| GET | `/api/order/add` | Create order from cart | Authenticated |
| PUT | `/api/order/update` | Update order status | Admin |
| PUT | `/api/order/cancel` | Cancel order | Authenticated |

**Implementation note:** `/api/order/add` currently uses `GET` even though it creates an order. A future improvement is to change this to `POST` to follow HTTP semantics.

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- MySQL server
- Git
- Optional: `dotnet-ef` CLI tool for applying migrations

### 1. Clone the repository

```bash
git clone https://github.com/VuongTCuong/OrderAndPaymentAPI.git
cd OrderAndPaymentAPI
```

### 2. Configure the database and JWT

Configure the following keys using **.NET user secrets** or local configuration (never commit real credentials):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=orderandpayment;user=YOUR_USER;password=YOUR_PASSWORD"
  },
  "Jwt": {
    "Key": "REPLACE_WITH_A_LONG_RANDOM_SECRET_OF_AT_LEAST_32_BYTES",
    "Issuer": "YOUR_ISSUER",
    "Audience": "YOUR_AUDIENCE"
  }
}
```

For local development, you can initialize user secrets and set the values with:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=orderandpayment;user=YOUR_USER;password=YOUR_PASSWORD"
dotnet user-secrets set "Jwt:Key" "REPLACE_WITH_A_LONG_RANDOM_SECRET_OF_AT_LEAST_32_BYTES"
dotnet user-secrets set "Jwt:Issuer" "YOUR_ISSUER"
dotnet user-secrets set "Jwt:Audience" "YOUR_AUDIENCE"
```

Use a securely generated random signing key in real deployments. Ensure MySQL is running and the configured account has appropriate database permissions.

### 3. Restore packages and apply migrations

```bash
dotnet restore
# Install dotnet-ef if it is not already installed:
dotnet tool install --global dotnet-ef
# If already installed, skip the installation command.
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

Open the localhost URL shown in the terminal to use the Razor Pages interface. API routes are available under `/api/`. You can also use Postman to send requests to the endpoints above.

> This README documents the configuration and commands expected from the source code; a clean-machine installation has not yet been verified.

## Project structure

```text
Controllers/      HTTP API endpoints
DTO/              Request and response models
Data/             EF Core DbContext
Entities/         Domain/database entities
Repositories/     Database access
Services/         Application and business logic
Middleware/       Exception handling
Exceptions/       Application exceptions
Migrations/       Database migrations
Pages/            Razor Pages interface
wwwroot/          Static CSS and JavaScript
Program.cs        Service registration and application pipeline
```

## Current limitations and possible improvements

- Checkout is simulated; no real payment provider is connected.
- Add an automated test project for service and controller behavior.
- Add API examples, sample request bodies, and API documentation.
- Review transaction boundaries and order status transitions for edge cases.

## Author

**VuongTCuong** — [GitHub](https://github.com/VuongTCuong)

This repository is a personal project built to practice ASP.NET Core backend development.
