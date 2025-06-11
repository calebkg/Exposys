import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import {
  Expense,
  ExpenseCreate,
  ExpenseUpdate,
  ExpenseFilter,
  ExpenseSummary,
  Category,
  Tag,
} from "../models/expense.model";

@Injectable({
  providedIn: "root",
})
export class ExpenseService {
  private apiUrl = `${environment.apiUrl}/expenses`;

  constructor(private http: HttpClient) {}

  getExpenses(
    page: number = 1,
    pageSize: number = 10,
    filter?: ExpenseFilter,
  ): Observable<{
    expenses: Expense[];
    totalCount: number;
    totalPages: number;
  }> {
    let params = new HttpParams()
      .set("page", page.toString())
      .set("pageSize", pageSize.toString());

    if (filter) {
      if (filter.startDate) {
        params = params.set("startDate", filter.startDate.toISOString());
      }
      if (filter.endDate) {
        params = params.set("endDate", filter.endDate.toISOString());
      }
      if (filter.categoryIds?.length) {
        params = params.set("categoryIds", filter.categoryIds.join(","));
      }
      if (filter.tagIds?.length) {
        params = params.set("tagIds", filter.tagIds.join(","));
      }
      if (filter.minAmount) {
        params = params.set("minAmount", filter.minAmount.toString());
      }
      if (filter.maxAmount) {
        params = params.set("maxAmount", filter.maxAmount.toString());
      }
      if (filter.searchTerm) {
        params = params.set("searchTerm", filter.searchTerm);
      }
    }

    return this.http.get<{
      expenses: Expense[];
      totalCount: number;
      totalPages: number;
    }>(this.apiUrl, { params });
  }

  getExpense(id: number): Observable<Expense> {
    return this.http.get<Expense>(`${this.apiUrl}/${id}`);
  }

  createExpense(expense: ExpenseCreate): Observable<Expense> {
    const formData = new FormData();
    formData.append("title", expense.title);
    if (expense.description) {
      formData.append("description", expense.description);
    }
    formData.append("amount", expense.amount.toString());
    formData.append("date", expense.date.toISOString());
    formData.append("categoryId", expense.categoryId.toString());
    formData.append("tagIds", JSON.stringify(expense.tagIds));
    if (expense.receipt) {
      formData.append("receipt", expense.receipt);
    }

    return this.http.post<Expense>(this.apiUrl, formData);
  }

  updateExpense(expense: ExpenseUpdate): Observable<Expense> {
    const formData = new FormData();
    formData.append("title", expense.title);
    if (expense.description) {
      formData.append("description", expense.description);
    }
    formData.append("amount", expense.amount.toString());
    formData.append("date", expense.date.toISOString());
    formData.append("categoryId", expense.categoryId.toString());
    formData.append("tagIds", JSON.stringify(expense.tagIds));
    if (expense.receipt) {
      formData.append("receipt", expense.receipt);
    }

    return this.http.put<Expense>(`${this.apiUrl}/${expense.id}`, formData);
  }

  deleteExpense(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getExpenseSummary(
    startDate?: Date,
    endDate?: Date,
  ): Observable<ExpenseSummary> {
    let params = new HttpParams();
    if (startDate) {
      params = params.set("startDate", startDate.toISOString());
    }
    if (endDate) {
      params = params.set("endDate", endDate.toISOString());
    }

    return this.http.get<ExpenseSummary>(`${this.apiUrl}/summary`, { params });
  }

  exportExpenses(
    format: "excel" | "pdf",
    filter?: ExpenseFilter,
  ): Observable<Blob> {
    let params = new HttpParams().set("format", format);

    if (filter) {
      if (filter.startDate) {
        params = params.set("startDate", filter.startDate.toISOString());
      }
      if (filter.endDate) {
        params = params.set("endDate", filter.endDate.toISOString());
      }
      if (filter.categoryIds?.length) {
        params = params.set("categoryIds", filter.categoryIds.join(","));
      }
    }

    return this.http.get(`${this.apiUrl}/export`, {
      params,
      responseType: "blob",
    });
  }

  // Categories
  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${environment.apiUrl}/categories`);
  }

  createCategory(
    category: Omit<Category, "id" | "userId" | "createdAt" | "updatedAt">,
  ): Observable<Category> {
    return this.http.post<Category>(
      `${environment.apiUrl}/categories`,
      category,
    );
  }

  updateCategory(category: Category): Observable<Category> {
    return this.http.put<Category>(
      `${environment.apiUrl}/categories/${category.id}`,
      category,
    );
  }

  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/categories/${id}`);
  }

  // Tags
  getTags(): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${environment.apiUrl}/tags`);
  }

  createTag(
    tag: Omit<Tag, "id" | "userId" | "createdAt" | "updatedAt">,
  ): Observable<Tag> {
    return this.http.post<Tag>(`${environment.apiUrl}/tags`, tag);
  }

  updateTag(tag: Tag): Observable<Tag> {
    return this.http.put<Tag>(`${environment.apiUrl}/tags/${tag.id}`, tag);
  }

  deleteTag(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/tags/${id}`);
  }
}
