import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import {
  Budget,
  BudgetCreate,
  BudgetUpdate,
  BudgetAlert,
} from "../models/budget.model";

@Injectable({
  providedIn: "root",
})
export class BudgetService {
  private apiUrl = `${environment.apiUrl}/budgets`;

  constructor(private http: HttpClient) {}

  getBudgets(): Observable<Budget[]> {
    return this.http.get<Budget[]>(this.apiUrl);
  }

  getBudget(id: number): Observable<Budget> {
    return this.http.get<Budget>(`${this.apiUrl}/${id}`);
  }

  createBudget(budget: BudgetCreate): Observable<Budget> {
    return this.http.post<Budget>(this.apiUrl, budget);
  }

  updateBudget(budget: BudgetUpdate): Observable<Budget> {
    return this.http.put<Budget>(`${this.apiUrl}/${budget.id}`, budget);
  }

  deleteBudget(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getBudgetAlerts(): Observable<BudgetAlert[]> {
    return this.http.get<BudgetAlert[]>(`${this.apiUrl}/alerts`);
  }

  markAlertAsRead(alertId: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/alerts/${alertId}/read`, {});
  }

  checkBudgetStatus(budgetId: number): Observable<{
    isExceeded: boolean;
    percentageUsed: number;
    remainingAmount: number;
  }> {
    return this.http.get<{
      isExceeded: boolean;
      percentageUsed: number;
      remainingAmount: number;
    }>(`${this.apiUrl}/${budgetId}/status`);
  }
}
