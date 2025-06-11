export interface Budget {
  id: number;
  name: string;
  amount: number;
  period: BudgetPeriod;
  categoryId?: number;
  category?: Category;
  startDate: Date;
  endDate: Date;
  currentSpent: number;
  remainingAmount: number;
  percentageUsed: number;
  isExceeded: boolean;
  alertThreshold: number;
  isAlertSent: boolean;
  userId: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface BudgetCreate {
  name: string;
  amount: number;
  period: BudgetPeriod;
  categoryId?: number;
  alertThreshold: number;
  startDate: Date;
  endDate: Date;
}

export interface BudgetUpdate {
  id: number;
  name: string;
  amount: number;
  period: BudgetPeriod;
  categoryId?: number;
  alertThreshold: number;
  startDate: Date;
  endDate: Date;
}

export enum BudgetPeriod {
  Daily = "Daily",
  Weekly = "Weekly",
  Monthly = "Monthly",
  Yearly = "Yearly",
  Custom = "Custom",
}

export interface BudgetAlert {
  id: number;
  budgetId: number;
  budget: Budget;
  message: string;
  alertType: AlertType;
  isRead: boolean;
  createdAt: Date;
}

export enum AlertType {
  Warning = "Warning",
  Exceeded = "Exceeded",
  NearLimit = "NearLimit",
}

import { Category } from "./expense.model";
