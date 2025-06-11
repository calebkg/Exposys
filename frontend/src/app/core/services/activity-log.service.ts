import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import { ActivityLog, ActivityFilter } from "../models/activity-log.model";

@Injectable({
  providedIn: "root",
})
export class ActivityLogService {
  private apiUrl = `${environment.apiUrl}/activity-logs`;

  constructor(private http: HttpClient) {}

  getActivityLogs(
    page: number = 1,
    pageSize: number = 10,
    filter?: ActivityFilter,
  ): Observable<{
    logs: ActivityLog[];
    totalCount: number;
    totalPages: number;
  }> {
    let params = new HttpParams()
      .set("page", page.toString())
      .set("pageSize", pageSize.toString());

    if (filter) {
      if (filter.userId) {
        params = params.set("userId", filter.userId.toString());
      }
      if (filter.startDate) {
        params = params.set("startDate", filter.startDate.toISOString());
      }
      if (filter.endDate) {
        params = params.set("endDate", filter.endDate.toISOString());
      }
      if (filter.action) {
        params = params.set("action", filter.action);
      }
      if (filter.entity) {
        params = params.set("entity", filter.entity);
      }
    }

    return this.http.get<{
      logs: ActivityLog[];
      totalCount: number;
      totalPages: number;
    }>(this.apiUrl, { params });
  }

  getUserActivityLogs(userId: number): Observable<ActivityLog[]> {
    return this.http.get<ActivityLog[]>(`${this.apiUrl}/user/${userId}`);
  }
}
