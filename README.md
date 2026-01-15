# 🛍️ E-Commerce REST API

A **production-style E-Commerce REST API** built with **ASP.NET Core (.NET 9)** following **Clean Architecture** principles.
The project focuses on real-world backend concerns such as authentication, order processing, and payment integration.

---

## 🚀 Overview

This project demonstrates how to build a scalable and maintainable backend for an e-commerce system, covering the full flow from authentication and shopping cart management to order creation and payment processing using **Stripe (Test Mode)**.

---

## ✨ Implemented Features

### 🔐 Authentication & Authorization

* JWT-based authentication
* Refresh token mechanism
* Role-based authorization (Admin / User)
* Secure user management with ASP.NET Identity

### 🛒 Shopping Cart

* Add, update, and remove cart items
* Cart persistence per authenticated user
* Automatic cart clearing after order creation
* Product variant support

### 📦 Products & Categories

* Product listing with pagination, filtering, and sorting
* Category management (Admin only)
* Inventory tracking
* Product variants (SKU-based)

### 📋 Orders

* Order creation **directly from the shopping cart**
* Order history per user
* Order status lifecycle:

  * Pending → Processing → Shipped → Delivered
* Admin-only order status updates
* Order search with pagination and filters

### 💳 Payments

* Stripe Checkout integration (Test Mode)
* Dedicated Payments API (separated from Orders)
* Secure checkout session creation
* Payment redirection handled outside the API (frontend-ready design)

### 🧩 Cross-Cutting Concerns

* Clean Architecture (Domain / Application / Infrastructure / API)
* Repository & Unit of Work patterns
* Global exception handling middleware
* Structured logging with Serilog
* Swagger / OpenAPI documentation

---

## 🛠️ Technology Stack

* **ASP.NET Core Web API (.NET 9)**
* **Entity Framework Core**
* **SQL Server**
* **ASP.NET Core Identity**
* **JWT Authentication**
* **Stripe API (Test Mode)**
* **Serilog**
* **AutoMapper**
* **Swagger / OpenAPI**

---

## 🏗️ Architecture

The project follows **Clean Architecture**, ensuring separation of concerns and testability:

```
ECommerce.API
   └── Controllers, Middleware, Filters

ECommerce.Application
   └── Services, DTOs, Interfaces, Business Logic

ECommerce.Infrastructure
   └── EF Core, Repositories, External Services (Stripe)

ECommerce.Domain
   └── Entities, Enums, Core Business Rules
```

---

## 📁 Project Structure

```
ECommerceSolution/
├── ECommerce.API
│   ├── Controllers (Auth, Products, Cart, Orders, Payments)
│   ├── Middleware
│   └── Program.cs
│
├── ECommerce.Application
│   ├── Services
│   ├── DTOs (Requests / Responses)
│   ├── Interfaces
│   └── Mapping
│
├── ECommerce.Infrastructure
│   ├── Repositories
│   ├── Persistence (DbContext, Migrations)
│   ├── Payment (Stripe)
│   └── Identity
│
└── ECommerce.Domain
    ├── Entities
    ├── Enums
    └── Interfaces
```

---

## ▶️ Getting Started

### Prerequisites

* .NET 9 SDK
* SQL Server
* Visual Studio 2022 or VS Code

### Run the Project

```bash
git clone https://github.com/Muhamad-Mahmoud/E-commerce.git
cd ECommerceSolution

dotnet restore
dotnet ef database update --project ECommerce.Infrastructure
dotnet run --project ECommerce.API
```

Swagger UI:

```
https://localhost:7000/swagger
```

---

## 💳 Stripe Test Mode

Use the following **Stripe test card** to simulate payments:

```
Card Number: 4242 4242 4242 4242
Expiry Date: Any future date
CVC: Any 3 digits
```

> ⚠️ Payments are handled in **Test Mode only**.
> The API returns a checkout URL, and redirection is handled by the client (frontend or browser).

---

## 🔐 Authentication

All protected endpoints require a JWT token:

```
Authorization: Bearer <access-token>
```

Tokens are obtained via:

```
POST /api/auth/login
```

---

## 📌 API Highlights

* `POST /api/orders` → Create order from cart
* `POST /api/payments/checkout` → Start Stripe checkout
* `GET /api/orders` → User order history
* `PUT /api/orders/{id}/status` → Admin order updates

Full documentation available via Swagger.

---
