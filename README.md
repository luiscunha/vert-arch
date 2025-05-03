# Demo application using Vertical Slice Architecture

This demo adopts a feature-based approach, keeping related logic within the same file. The application includes the following features:

📦 Entity Framework Core (EF Core) with SQL Database

- The project uses EF Core as the Object-Relational Mapper (ORM) for interacting with a relational SQL database.
- It abstracts the data access logic and allows for strong-typed LINQ queries, change tracking, and lazy/eager loading.

  
📂 Database Migrations

- EF Core's migration system is used to version and evolve the database schema over time.
- Ensures that the database remains in sync with the domain models and can be managed consistently across different environments.

🧱 CQRS Pattern (Command and Query Responsibility Segregation)

- Implements CQRS to separate read (query) and write (command) operations for improved scalability and clarity.
- Each use case is represented by specific command or query handlers, promoting a clear single-responsibility approach.

✅ FluentValidation

- Uses [FluentValidation](https://docs.fluentvalidation.net/en/latest/) for validating input models (commands and queries).
- Validation logic is kept declarative and centralized, making it easy to maintain and test.


🎯 Result Pattern for Operation Outcomes
- Applies the Result<T> pattern to consistently return success or failure states.
- Helps eliminate ambiguity and encourages explicit handling of business rules and errors.
  
🚏 Carter Library for Endpoint Routing 
- Uses the [Carter](https://github.com/CarterCommunity/Carter) library to define and register endpoints cleanly and modularly.
- Carter acts as a thin layer over ASP.NET Core Minimal APIs, enabling better organization of route definitions by feature or domain.

🧱 Minimal APIs Approach

- Leverages Minimal APIs introduced in .NET 6+ to build fast, lightweight, and readable HTTP endpoint definitions.

🔍 Filtering, Sorting, and Paging

- Exposes generic API endpoints that support filtering, sorting, and paging of data collections.

🔐 API Key Authentication

- The project implements a lightweight API Key authentication mechanism to restrict access to protected endpoints (POST & PUT).

- Clients must provide a valid API key (via a custom header X-Api-Key) to authenticate requests.

⚠️ API Error Handling with ProblemDetails

- Implements standardized error responses using the [RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807) format (application/problem+json).
- Helps API consumers understand errors better and integrates smoothly with tools like Swagger or Postman.


🧯 Global Error Handling Middleware

- A centralized exception handling middleware is used to catch and manage unhandled exceptions across the entire API pipeline.

- Ensures consistent error formatting using the ProblemDetails standard (application/problem+json), improving client-side error parsing.

