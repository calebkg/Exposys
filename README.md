# Expense Management System

A complete full-stack expense management application built with Angular frontend and C# .NET 8 Web API backend.

## Features

### User Management

- User registration and authentication with JWT
- Role-based access control (User/Admin)
- Profile management with image upload
- Password reset functionalityyy

### Expense Managementt

- CRUD operations for expenses
- Category and tag management
- Receipt file upload
- Advanced filtering and search
- Export to Excel/PDF

### Budget Management

- Create budgets with different periods (Daily, Weekly, Monthly, Yearly, Custom)
- Budget alerts and notifications
- Real-time budget tracking
- Category-specific budgets

### Dashboard & Analytics

- Expense summaries and charts
- Category breakdown
- Monthly trends
- Recent activity

### Admin Features

- User management
- System-wide expense reports
- Activity logs monitoring
- Data export capabilities

### Additional Features

- Dark/Light mode toggle
- Email notifications
- Monthly reports
- Activity logging
- Responsive design

## Technology Stack

### Frontend

- **Angular 17** with TypeScript
- **Angular Material** for UI components
- **Tailwind CSS** for styling
- **Chart.js** for data visualization
- **RxJS** for reactive programming

### Backend

- **C# .NET 8** Web API
- **Entity Framework Core** with MySQL
- **ASP.NET Core Identity** for authentication
- **JWT** for token-based authentication
- **AutoMapper** for object mapping
- **Serilog** for logging
- **Swagger** for API documentation

### Database

- **MySQL** with Entity Framework Core
- Clean database design with proper relationships
- Automated migrations

## Getting Started

### Prerequisites

- Node.js 18+ and npm
- .NET 8 SDK
- MySQL Server 8.0+
- Angular CLI (`npm install -g @angular/cli`)

### Backend Setup

1. **Navigate to the backend directory:**

   ```bash
   cd backend
   ```

2. **Restore NuGet packages:**

   ```bash
   dotnet restore
   ```

3. **Update database connection string in `appsettings.json`:**

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "server=localhost;database=ExpenseManagement;user=root;password=your_password;"
     }
   }
   ```

4. **Update email settings in `appsettings.json`:**

   ```json
   {
     "Email": {
       "SmtpHost": "smtp.gmail.com",
       "SmtpPort": "587",
       "UseSsl": "true",
       "Username": "your_email@gmail.com",
       "Password": "your_app_password",
       "FromName": "Expense Management System",
       "FromAddress": "noreply@expensemanagement.com"
     }
   }
   ```

5. **Create and seed the database:**

   ```bash
   cd ExpenseManagement.API
   dotnet ef database update
   ```

6. **Run the API:**

   ```bash
   dotnet run
   ```

   The API will be available at `https://localhost:7001` and Swagger UI at `https://localhost:7001`

### Frontend Setup

1. **Navigate to the frontend directory:**

   ```bash
   cd frontend
   ```

2. **Install dependencies:**

   ```bash
   npm install
   ```

3. **Update API URL in environment files if needed:**

   ```typescript
   // src/environments/environment.ts
   export const environment = {
     production: false,
     apiUrl: "https://localhost:7001/api",
   };
   ```

4. **Start the development server:**

   ```bash
   ng serve
   ```

   The application will be available at `http://localhost:4200`

## Default Credentials

- **Admin User:**
  - Email: `admin@expensemanagement.com`
  - Password: `Admin@123!`

## API Documentation

Once the backend is running, you can access the Swagger documentation at:

- Development: `https://localhost:7001`

The API includes the following endpoints:

### Authentication

- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/forgot-password` - Forgot password
- `POST /api/auth/reset-password` - Reset password

### Expenses

- `GET /api/expenses` - Get paginated expenses
- `GET /api/expenses/{id}` - Get expense by ID
- `POST /api/expenses` - Create expense
- `PUT /api/expenses/{id}` - Update expense
- `DELETE /api/expenses/{id}` - Delete expense
- `GET /api/expenses/summary` - Get expense summary
- `GET /api/expenses/export` - Export expenses

### Categories & Tags

- `GET /api/categories` - Get categories
- `POST /api/categories` - Create category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

### Budgets

- `GET /api/budgets` - Get budgets
- `POST /api/budgets` - Create budget
- `PUT /api/budgets/{id}` - Update budget
- `DELETE /api/budgets/{id}` - Delete budget
- `GET /api/budgets/alerts` - Get budget alerts

### Users (Admin only)

- `GET /api/users` - Get all users
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

## Project Structure

```
expense-management/
├── backend/
│   ├── ExpenseManagement.API/          # Web API project
│   ├── ExpenseManagement.Core/         # Domain entities and interfaces
│   ├── ExpenseManagement.Infrastructure/ # Data access and services
│   ├── ExpenseManagement.Application/   # Business logic and DTOs
│   └── ExpenseManagement.sln           # Solution file
├── frontend/
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/                   # Core services and guards
│   │   │   ├── features/               # Feature modules
│   │   │   ├── shared/                 # Shared components
│   │   │   └── ...
│   │   ├── environments/               # Environment configurations
│   │   └── ...
│   ├── angular.json                    # Angular configuration
│   ├── package.json                    # NPM dependencies
│   └── tailwind.config.js             # Tailwind configuration
└── README.md
```

## Development Notes

### Database Migrations

To create a new migration:

```bash
cd backend/ExpenseManagement.API
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Adding New Features

1. **Backend:**

   - Add entity to `ExpenseManagement.Core/Entities/`
   - Create repository interface in `ExpenseManagement.Core/Interfaces/`
   - Implement repository in `ExpenseManagement.Infrastructure/Repositories/`
   - Create controller in `ExpenseManagement.API/Controllers/`

2. **Frontend:**
   - Create model in `src/app/core/models/`
   - Create service in `src/app/core/services/`
   - Create components in `src/app/features/`

### Security Considerations

- JWT tokens expire after 24 hours
- Passwords must meet complexity requirements
- File uploads are validated and stored securely
- All API endpoints require authentication except auth endpoints
- Admin endpoints require admin role
- CORS is configured for specific origins

## Deployment

### Backend Deployment

1. Update connection strings for production
2. Set environment to Production
3. Update CORS settings for production URLs
4. Deploy to your preferred hosting service

### Frontend Deployment

1. Build for production: `ng build --prod`
2. Update environment files with production API URLs
3. Deploy the `dist/` folder to your web server

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License.
