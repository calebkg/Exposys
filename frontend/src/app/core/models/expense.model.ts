export interface Expense {
  id: number;
  title: string;
  description?: string;
  amount: number;
  date: Date;
  categoryId: number;
  category: Category;
  tags: Tag[];
  receiptUrl?: string;
  userId: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface ExpenseCreate {
  title: string;
  description?: string;
  amount: number;
  date: Date;
  categoryId: number;
  tagIds: number[];
  receipt?: File;
}

export interface ExpenseUpdate {
  id: number;
  title: string;
  description?: string;
  amount: number;
  date: Date;
  categoryId: number;
  tagIds: number[];
  receipt?: File;
}

export interface Category {
  id: number;
  name: string;
  color: string;
  icon: string;
  userId: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface Tag {
  id: number;
  name: string;
  color: string;
  userId: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface ExpenseFilter {
  startDate?: Date;
  endDate?: Date;
  categoryIds?: number[];
  tagIds?: number[];
  minAmount?: number;
  maxAmount?: number;
  searchTerm?: string;
}

export interface ExpenseSummary {
  totalExpenses: number;
  totalAmount: number;
  categoryBreakdown: CategoryExpense[];
  monthlyTrend: MonthlyExpense[];
}

export interface CategoryExpense {
  categoryId: number;
  categoryName: string;
  amount: number;
  count: number;
  percentage: number;
}

export interface MonthlyExpense {
  month: string;
  year: number;
  amount: number;
  count: number;
}
