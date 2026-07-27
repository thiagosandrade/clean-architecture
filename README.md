# Todo Management Backend API

A scalable backend API built with **.NET**, following **Clean Architecture** and modern backend development principles.

The system provides APIs for authentication, todo management, advanced search capabilities, semantic search using AI embeddings, background processing, and integration with distributed infrastructure components.

---

# 🚀 Features

## Todo Management

Complete todo management capabilities:

- Create todos
- Update todos
- Delete todos
- Complete/uncomplete todos
- Bulk todo creation
- Pagination
- Dynamic sorting
- Filtering
- Search support


## Authentication & Authorization

Implemented using:

- JWT authentication
- ASP.NET Core Identity
- User context abstraction
- Protected API endpoints


# 🔎 Search Capabilities

The application supports multiple search strategies.

## Standard Search

Traditional filtering:

- Title matching
- Description matching
- Status filtering
- Priority filtering


## Elasticsearch Search

Todo data is indexed into Elasticsearch to provide:

- Fast text search
- Filtering
- Sorting
- Pagination
- Scalable querying


## Semantic Search

The application supports AI-powered semantic search.


# 🧩 Application Layer

The application layer contains business use cases.

Implemented patterns:

- CQRS
- Command handlers
- Query handlers
- Validation pipeline
- Domain abstractions



| Service       | Purpose            |
| ------------- | ------------------ |
| PostgreSQL    | Primary database   |
| Elasticsearch | Search engine      |
| Kibana        | Elasticsearch UI   |
| RabbitMQ      | Messaging          |
| Seq           | Structured logging |








