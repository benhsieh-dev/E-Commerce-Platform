# Technology Implementation in Progress:
- .NET (ASP.NET Core)
- Backend microservices	
- Angular	Frontend (web store, admin dashboard)
- GraphQL	API layer between frontend and backend	
- Docker	Containerization	Package microservices consistently, makes local development and CI/CD easier
- Kubernetes	Orchestration	Scale microservices independently 
- Message Broker (RabbitMQ / Kafka)	Async communication	Handle order events, inventory updates, email notifications reliably
- Database (SQL or NoSQL)	Persistent data store	Use SQL (e.g., Postgres, SQL Server) for transactions, NoSQL (MongoDB) for catalog or search if needed

## Starting Instructions
To start the Angular app locally, here are the step-by-step instructions:

  1. Prerequisites

  Make sure you have these installed:
  - Node.js (version 18 or higher)
  - npm (comes with Node.js)
  - Angular CLI: npm install -g @angular/cli

  2. Start Backend Services First

  Navigate to each service directory and start them:

  ### Terminal 1 - API Gateway (Port 5000)
  cd /Volumes/Samsung/E-Commerce-Platform/src/ApiGateway
  dotnet run

  ### Terminal 2 - User Service (Port 5001)
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/UserService/UserService
  dotnet run

  ### Terminal 3 - Property Service (Port 5002)
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/PropertyService/PropertyService
  dotnet run

  ### Terminal 4 - Booking Service (Port 5003)
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/BookingService/BookingService
  dotnet run

  ### Terminal 5 - Payment Service (Port 5004)
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/PaymentService/PaymentService
  dotnet run

  ### Terminal 6 - Review Service (Port 5005)
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/ReviewService/ReviewService
  dotnet run

  ### Terminal 7 - Notification Service (Port 5006)
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/NotificationService/NotificationService
  dotnet run

  3. Start Angular Frontend

  ### Terminal 8 - Angular App (Port 4200)
  cd /Volumes/Samsung/E-Commerce-Platform/src/Frontend

  ### Install dependencies first
  npm install

  ### Start the development server
  npm start
  ### or
  ng serve

  🌐 Access the Application

  Once all services are running:

  - Frontend: http://localhost:4200
  - GraphQL Playground: http://localhost:5000/ui/playground
  - API Gateway Health: http://localhost:5000/health
  - Service Status: http://localhost:5000/status

  📱 What You'll See

  Homepage

  - Clean header with navigation and user menu
  - Property search interface (once implemented)
  - Footer with links

  Authentication

  - Login Page: http://localhost:4200/auth/login
  - Register Page: http://localhost:4200/auth/register
  - Demo accounts available in development mode

  GraphQL Playground

  - Interactive API explorer at http://localhost:5000/ui/playground
  - Test all queries and mutations
  - View schema documentation

  🔧 Quick Development Setup

  For faster development, create a startup script:

  start-all.sh
  #!/bin/bash

  # Start all backend services in background
  cd /Volumes/Samsung/E-Commerce-Platform/src/ApiGateway && dotnet run &
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/UserService/UserService && dotnet run &
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/PropertyService/PropertyService && dotnet run &
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/BookingService/BookingService && dotnet run &
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/PaymentService/PaymentService && dotnet run &
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/ReviewService/ReviewService && dotnet run &
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/NotificationService/NotificationService && dotnet run &

  # Wait a moment for services to start
  sleep 10

  # Start Angular frontend
  cd /Volumes/Samsung/E-Commerce-Platform/src/Frontend && npm start

  Make executable and run:
  chmod +x start-all.sh
  ./start-all.sh

  🛠 Troubleshooting

  Common Issues:

  1. Port Conflicts: Make sure ports 5000-5006 and 4200 are available
  2. Dependencies: Run npm install in the Frontend directory
  3. CORS Issues: The API Gateway is configured for CORS, but ensure all services are running
  4. SSL Certificates: You might see warnings about HTTPS certificates in development

  Check Service Health:

  Visit http://localhost:5000/status to see if all microservices are running properly.

  Development Mode Features:

  - Hot reload for Angular changes
  - Demo login accounts available
  - Detailed error messages
  - GraphQL introspection enabled

  The app should now be running with a clean Material Design interface, working authentication, and full integration with the GraphQL API Gateway

  Here are the commands to start the AirBnB Platform application locally:

  # Startup Commands

  1. Install Angular Dependencies

  cd /Volumes/Samsung/E-Commerce-Platform/src/Frontend
  npm install

  2. Start Backend Services (in separate terminals)

  Terminal 1 - API Gateway (Port 5000):
  cd /Volumes/Samsung/E-Commerce-Platform/src/ApiGateway
  dotnet run

  Terminal 2 - User Service (Port 5001):
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/UserService/UserService
  dotnet run

  Terminal 3 - Property Service (Port 5002):
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/PropertyService/PropertyService
  dotnet run

  Terminal 4 - Booking Service (Port 5003):
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/BookingService/BookingService
  dotnet run

  Terminal 5 - Payment Service (Port 5004):
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/PaymentService/PaymentService
  dotnet run

  Terminal 6 - Review Service (Port 5005):
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/ReviewService/ReviewService
  dotnet run

  Terminal 7 - Notification Service (Port 5006):
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/NotificationService/NotificationService
  dotnet run

  3. Start Angular Frontend

  Terminal 8 - Angular App (Port 4200):
  cd /Volumes/Samsung/E-Commerce-Platform/src/Frontend
  ng serve
  # or
  npm start

  # Access URLs

  - Frontend App: http://localhost:4200
  - GraphQL Playground: http://localhost:5000/ui/playground
  - API Gateway Health: http://localhost:5000/health
  - All Services Status: http://localhost:5000/status

  ⚡ Alternative: Single Command Startup

  Create a script to start all services:

  Option 1 - Background processes:
  # Start all backend services in background
  cd /Volumes/Samsung/E-Commerce-Platform/src/ApiGateway && dotnet run & \
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/UserService/UserService && dotnet run & \
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/PropertyService/PropertyService && dotnet run & \
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/BookingService/BookingService && dotnet run & \
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/PaymentService/PaymentService && dotnet run & \
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/ReviewService/ReviewService && dotnet run & \
  cd /Volumes/Samsung/E-Commerce-Platform/src/Services/NotificationService/NotificationService && dotnet run &

  # Wait for services to start, then start frontend
  sleep 10 && cd /Volumes/Samsung/E-Commerce-Platform/src/Frontend && ng serve

  Option 2 - Using tmux (if available):
  # Create new tmux session
  tmux new-session -d -s airbnb

  # Split into multiple panes and run services