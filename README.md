Technology Used:
- .NET (ASP.NET Core)	Backend microservices	Excellent performance, mature ecosystem, first-class support for REST & GraphQL, great for enterprise apps
- Angular	Frontend (web store, admin dashboard)	Strong typing (TypeScript), good for large teams, supports modular architecture
- GraphQL	API layer between frontend and backend	Ideal for complex data like products, categories, reviews — avoids overfetching/underfetching
- Docker	Containerization	Package microservices consistently, makes local development and CI/CD easier
- Kubernetes	Orchestration	Scale microservices independently (e.g., more replicas for Product API on Black Friday)
- Message Broker (RabbitMQ / Kafka)	Async communication	Handle order events, inventory updates, email notifications reliably
- Database (SQL or NoSQL)	Persistent data store	Use SQL (e.g., Postgres, SQL Server) for transactions, NoSQL (MongoDB) for catalog or search if needed