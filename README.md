# vert-arch
Vertical Architecture 

Demo application using Vertical Slice Architecture

This demo adopts a feature-based approach, keeping related logic within the same file. The application includes the following features:

- Entity Framework (EF) with a SQL database approach;
- Database migrations;
- CQRS pattern for handling use case scenarios;
- FluentValidation (via NuGet) for validating commands and queries;
- Result pattern for consistent operation outcomes;
- Carter library (via NuGet) for register all endpoints;
- Minimal APIs approach;
- Filtering, Sorting and Paging endpoints;
