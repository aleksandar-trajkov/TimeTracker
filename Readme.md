# Time Tracking Application

A comprehensive time tracking solution built with **.NET 9** backend and **React** frontend, featuring user management, time entry tracking, reporting, and real-time collaboration capabilities.

## ✨ Features

### Core Functionality
- **Time Tracking**: Track time spent on various tasks with start/stop functionality
- **Category Management**: Organize time entries with customizable categories
- **User Management**: Multi-user support with organization-based access control
- **Reports & Analytics**: Generate detailed reports on time usage with export capabilities
- **Real-time Updates**: Live collaboration with SignalR integration

### User Types
- **Normal Users**: Track personal time, manage own entries, view personal reports
- **Admin Users**: Manage organization users, categories, and access all organizational data

### Views & Interface
- **Multiple Time Views**: Day, week, month, and table views for time entries
- **User-friendly Interface**: Modern React-based UI with Bootstrap styling
- **Responsive Design**: Works across desktop and mobile devices

## 🛠️ Technologies Used

### Backend (.NET 9)
- **Framework**: ASP.NET Core 9.0 with Minimal APIs
- **Architecture**: Clean Architecture with CQRS pattern
- **Patterns**: 
  - **MediatR**: Command/Query handling and pipeline behaviors
  - **FluentValidation**: Request validation middleware
  - **Repository Pattern**: Data access abstraction
- **Database**: SQL Server with Entity Framework Core 9.0
- **Authentication**: JWT Bearer tokens with custom implementation
- **API Documentation**: Swagger/OpenAPI with API versioning
- **Object Mapping**: Mapster for DTO mapping
- **Health Checks**: Built-in health monitoring

### Infrastructure
- **Data Access**: Entity Framework Core with SQL Server
- **Messaging**: Azure Service Bus integration
- **Email**: SMTP email service integration
- **Containerization**: Docker support with multi-stage builds
- **Dependency Injection**: Built-in .NET DI container

### Frontend (React)
- **Framework**: React 18 with TypeScript
- **State Management**: Zustand for cross-page state management
- **Data Fetching**: React Query (TanStack Query) for caching and synchronization
- **Routing**: React Router with lazy loading
- **UI Framework**: Bootstrap 5 with custom SCSS styling
- **Build Tool**: Vite for fast development and optimized builds
- **Performance**: Suspense for loading states and code splitting

### Testing
- **Unit Testing**: xUnit with FluentAssertions
- **Test Containers**: Testcontainers for integration testing with SQL Server
- **Mocking**: NSubstitute for dependency mocking
- **Test Coverage**: Coverlet for code coverage reporting
- **Frontend Testing**: Vitest with Happy DOM environment

### DevOps & Quality
- **Code Quality**: EditorConfig for consistent formatting
- **Version Control**: Git with comprehensive .gitignore
- **API Versioning**: URL-based versioning (v1, v2)
- **Environment Configuration**: appsettings.json with environment overrides

## 🚀 Getting Started

### Prerequisites

- **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **SQL Server** (LocalDB, Express, or Full) - [Download here](https://www.microsoft.com/sql-server/sql-server-downloads)
- **Node.js 20+** (for frontend) - [Download here](https://nodejs.org/)
- **Docker** (optional) - [Download here](https://www.docker.com/products/docker-desktop)

### Backend Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/aleksandar-trajkov/TimeTracker.git
   cd TimeTracker
   ```

2. **Configure Database Connection**
   
   Update `TimeTracker.WebApi/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "Database": "Server=(localdb)\\mssqllocaldb;Database=TimeTracker;Trusted_Connection=True;TrustServerCertificate=True;"
     },
     "Database": {
       "AutoMigrate": true
     }
   }
   ```

3. **Run Database Migrations**
   ```bash
   cd TimeTracker.WebApi
   dotnet ef database update
   ```

4. **Start the Backend API**
   ```bash
   dotnet run --project TimeTracker.WebApi
   ```
   
   The API will be available at:
   - **HTTPS**: `https://localhost:9622`
   - **Swagger UI**: `https://localhost:9622/swagger/index.html`

### Frontend Setup (if applicable)

1. **Navigate to frontend directory**
   ```bash
   cd TimeTracker.Frontend.React
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Configure environment**
   
   Create `.env.local`:
   ```env
   VITE_API_BASE_URL=https://localhost:9622/api
   VITE_QUERY_STALE_TIME=300000
   VITE_QUERY_GC_TIME=600000
   ```

4. **Start the development server**
   ```bash
   npm run dev
   ```
   
   The frontend will be available at `http://localhost:9621`

### Docker Setup (Alternative)

1. **Build and run with Docker**
   ```bash
   docker build -t timetracker-api .
   docker run -p 8080:8080 -p 8081:8081 timetracker-api
   ```

### Running Tests

**Backend Tests**
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Run specific test project
dotnet test TimeTracker.UnitTests
```

**Frontend Tests**
```bash
cd TimeTracker.Frontend.React
npm run test
npm run test:coverage
```

## 📁 Project Structure

```
TimeTracker/
├── TimeTracker.WebApi/                 # Main API application
├── TimeTracker.Application/            # Business logic and use cases
├── TimeTracker.Application.Interfaces/ # Application contracts
├── TimeTracker.Domain/                 # Domain models and entities
├── TimeTracker.Infrastructure.*/       # Infrastructure implementations
├── TimeTracker.WebApi.Contracts/       # API request/response models
├── TimeTracker.Common/                 # Shared utilities
├── TimeTracker.IoC/                    # Dependency injection configuration
├── TimeTracker.UnitTests/              # Unit tests
├── TimeTracker.UnitTests.Common/       # Test utilities and builders
└── TimeTracker.Frontend.React/         # React frontend
```

## 🔧 Configuration

### Database Configuration
Configure your database connection in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Database": "Server=localhost;Database=TimeTracker;Trusted_Connection=True;"
  },
  "Database": {
    "AutoMigrate": true
  }
}
```

### Authentication Configuration
JWT authentication settings:
```json
{
  "Auth": {
    "SecurityKey": "your-256-bit-secret-key",
    "Authority": "https://yourdomain.com",
    "Audience": "timetracker-api",
    "ExpirationHours": 2
  }
}
```

### Health Check Configuration
Health monitoring endpoint:
```json
{
  "HealthCheck": {
    "Endpoint": "/health-check"
  }
}
```

## 🏗️ Architecture

### Clean Architecture Layers
- **WebApi**: REST API controllers and minimal API endpoints
- **Application**: Use cases, commands, queries, and business logic
- **Domain**: Core business entities and domain logic
- **Infrastructure**: External concerns (database, email, messaging)

### Design Patterns
- **CQRS**: Command Query Responsibility Segregation with MediatR
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Loose coupling and testability
- **Request/Response Pattern**: Structured API communication

## 🧪 Testing Strategy

- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test database and external service integration
- **Test Containers**: Real database testing with containerized SQL Server
- **Builder Pattern**: Fluent test data creation
- **Mock Doubles**: Simplified mocking for dependencies

## 📈 Development

### Available Scripts

**Backend**
```bash
dotnet build                     # Build the solution
dotnet test                      # Run tests
dotnet run                       # Start the application
dotnet ef migrations add <name>  # Add new migration
dotnet ef database update        # Apply migrations
```

**Frontend**
```bash
npm run dev          # Start development server
npm run build        # Build for production
npm run preview      # Preview production build
npm run test         # Run tests
npm run lint         # Run linting
```

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🔗 Useful Links

- **API Documentation**: Available at `/swagger` when running locally
- **Health Check**: Available at `/health-check`
- **Repository**: [https://github.com/aleksandar-trajkov/TimeTracker](https://github.com/aleksandar-trajkov/TimeTracker)