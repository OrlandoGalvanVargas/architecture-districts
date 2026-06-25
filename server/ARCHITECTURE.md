# Architecture Decision: Vertical Slice Architecture

## Context

FacilityOS.API is the backend service responsible for exposing REST endpoints consumed by an independent React frontend application.

At the current stage of the project, the system contains a limited number of business domains, primarily:

* Authentication
* District Management

The project is expected to grow incrementally over time by introducing new business capabilities while maintaining code readability, low coupling, and ease of maintenance.

A software architecture was required that:

* Supports feature-based development.
* Reduces unnecessary boilerplate.
* Encourages separation of concerns.
* Remains simple enough for a small-to-medium sized application.
* Scales naturally as new modules are introduced.

---

# Decision

The project adopts a **Vertical Slice Architecture** approach using **MediatR** as the application request dispatcher.

Business functionality is organized by feature rather than by technical layer.

Each use case is implemented as an independent slice containing its own request, handler, validation rules, and related models.

---

# Why Vertical Slice Architecture

Traditional layered architectures commonly organize code into folders such as:

```text
Controllers/
Services/
Repositories/
Entities/
```

While effective, this approach often causes functionality to become scattered across multiple locations.

For example, implementing a single "Create District" operation may require modifications in:

* Controller
* Service
* Repository
* DTOs
* Validators

As the application grows, navigating these layers becomes increasingly difficult.

Vertical Slice Architecture groups everything related to a specific use case into a single location.

This improves:

* Discoverability
* Maintainability
* Feature isolation
* Scalability

---

# Why MediatR

MediatR provides an in-process messaging pattern that allows Controllers to remain thin and focused on HTTP concerns.

Instead of embedding business logic directly inside Controllers, requests are delegated to dedicated handlers.

Benefits include:

* Reduced coupling.
* Clear separation between transport and business logic.
* Easier testing.
* Better organization of application behavior.

---

# Feature Structure

```text
Features/

├── Auth/
│   ├── Login/
│   ├── Logout/
│   ├── RefreshToken/
│   └── Me/
│
├── Districts/
│   ├── GetDistricts/
│   ├── GetDistrictById/
│   ├── CreateDistrict/
│   ├── UpdateDistrict/
│   └── DeleteDistrict/
```

Each operation represents a single responsibility and contains everything required to execute that behavior.

---

# Controller Guidelines

Controllers must remain thin.

Responsibilities:

* Receive HTTP requests.
* Validate request format.
* Forward requests to MediatR.
* Return HTTP responses.

Controllers must not:

* Contain business rules.
* Access persistence directly.
* Implement domain workflows.

Example flow:

```text
HTTP Request
    ↓
Controller
    ↓
MediatR
    ↓
Handler
    ↓
Repository / Data Access
    ↓
Response
```

---

# Current API Scope

## Authentication

| Method | Endpoint      |
| ------ | ------------- |
| POST   | /auth/login   |
| POST   | /auth/logout  |
| POST   | /auth/refresh |
| GET    | /auth/me      |

## Districts

| Method | Endpoint        |
| ------ | --------------- |
| GET    | /districts      |
| GET    | /districts/{id} |
| POST   | /districts      |
| PUT    | /districts/{id} |
| DELETE | /districts/{id} |

These endpoints are derived directly from the frontend requirements and represent the initial contract between both applications.

---

# Architectural Rules

1. One use case equals one Command or Query.
2. One Command or Query equals one Handler.
3. Business logic belongs inside Handlers or Domain Services.
4. Controllers must remain free of business rules.
5. Features must be self-contained whenever possible.
6. Shared abstractions should only be introduced when duplication becomes a real problem.
7. New functionality should be added by creating new slices rather than modifying unrelated modules.

---

# Alternatives Considered

## Clean Architecture

Pros:

* Strong separation of concerns.
* Highly scalable.

Cons:

* Introduces additional layers and boilerplate.
* Increases complexity for the current project size.

Decision:

Not selected at this stage.

---

## Traditional Layered Architecture

Pros:

* Familiar structure.
* Easy onboarding.

Cons:

* Functionality becomes distributed across multiple layers.
* Increased navigation overhead as the project grows.

Decision:

Not selected.

---

# Consequences

Positive:

* Faster feature development.
* Better feature discoverability.
* Reduced boilerplate.
* Easier maintenance.

Trade-offs:

* Requires discipline to prevent cross-feature dependencies.
* Some code duplication may appear before abstractions are introduced.

These trade-offs are considered acceptable for the current scope of the project.

---

# Future Considerations

Potential future additions include:

* JWT Authentication.
* Role-Based Authorization.
* FluentValidation.
* Global Exception Handling.
* Audit Logging.
* Unit Testing.
* Integration Testing.
* Docker Support.
* CI/CD Pipelines.

These enhancements should follow the same architectural principles established in this document.
