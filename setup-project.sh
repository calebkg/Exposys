#!/bin/bash

echo "🚀 Setting up Expense Management System..."

# Create main project directory
mkdir -p expense-management-system
cd expense-management-system

echo "📁 Creating directory structure..."

# Frontend structure
mkdir -p frontend/src/app/core/{guards,services,models,interceptors}
mkdir -p frontend/src/app/features/auth/{login,register,forgot-password}
mkdir -p frontend/src/app/features/{dashboard,expenses/{expense-list,expense-form},budget,profile,activity-logs}
mkdir -p frontend/src/app/features/admin/admin-panel
mkdir -p frontend/src/app/shared/components/{layout,navbar,sidebar,loading-spinner,confirm-dialog}
mkdir -p frontend/src/environments

# Backend structure
mkdir -p backend/ExpenseManagement.API/Controllers
mkdir -p backend/ExpenseManagement.Core/{Entities,Interfaces}
mkdir -p backend/ExpenseManagement.Infrastructure/{Data,Repositories,Services}
mkdir -p backend/ExpenseManagement.Application

echo "✅ Directory structure created!"
echo "📝 Next steps:"
echo "1. Copy all the file contents I provided into the respective directories"
echo "2. Run 'git init' to initialize the repository"
echo "3. Add and commit your files"
echo "4. Create a GitHub repository and push"

echo ""
echo "📋 Key files to create first:"
echo "Backend:"
echo "  - backend/ExpenseManagement.sln"
echo "  - backend/ExpenseManagement.API/Program.cs"
echo "  - backend/ExpenseManagement.API/appsettings.json"
echo ""
echo "Frontend:"
echo "  - frontend/package.json"
echo "  - frontend/angular.json"
echo "  - frontend/src/app/app.module.ts"
echo ""
echo "🎯 After creating files, run:"
echo "  git init"
echo "  git add ."
echo "  git commit -m 'Initial commit: Expense Management System'"
echo "  git remote add origin YOUR_GITHUB_REPO_URL"
echo "  git push -u origin main"
