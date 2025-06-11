export interface ActivityLog {
  id: number;
  userId: number;
  user: User;
  action: string;
  entity: string;
  entityId: number;
  oldValues?: string;
  newValues?: string;
  ipAddress: string;
  userAgent: string;
  timestamp: Date;
}

export interface ActivityFilter {
  userId?: number;
  startDate?: Date;
  endDate?: Date;
  action?: string;
  entity?: string;
}

export enum ActivityAction {
  Create = "Create",
  Update = "Update",
  Delete = "Delete",
  Login = "Login",
  Logout = "Logout",
  Export = "Export",
  Import = "Import",
}

export enum ActivityEntity {
  User = "User",
  Expense = "Expense",
  Category = "Category",
  Tag = "Tag",
  Budget = "Budget",
}

import { User } from "./user.model";
