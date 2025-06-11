import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, Observable, tap } from "rxjs";
import { Router } from "@angular/router";
import { jwtDecode } from "jwt-decode";
import { environment } from "../../../environments/environment";
import {
  User,
  UserLogin,
  UserRegistration,
  AuthResponse,
} from "../models/user.model";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router,
  ) {
    this.loadUserFromToken();
  }

  login(credentials: UserLogin): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap((response) => {
          this.setSession(response);
        }),
      );
  }

  register(userData: UserRegistration): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/register`, userData)
      .pipe(
        tap((response) => {
          this.setSession(response);
        }),
      );
  }

  forgotPassword(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/forgot-password`, { email });
  }

  resetPassword(token: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/reset-password`, { token, password });
  }

  refreshToken(): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/refresh-token`, {})
      .pipe(
        tap((response) => {
          this.setSession(response);
        }),
      );
  }

  logout(): void {
    localStorage.removeItem("token");
    localStorage.removeItem("tokenExpiry");
    this.currentUserSubject.next(null);
    this.router.navigate(["/auth/login"]);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;

    const expiry = localStorage.getItem("tokenExpiry");
    if (!expiry) return false;

    return new Date().getTime() < parseInt(expiry);
  }

  isAdmin(): boolean {
    const user = this.currentUserSubject.value;
    return user?.role === "Admin";
  }

  getToken(): string | null {
    return localStorage.getItem("token");
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  private setSession(authResponse: AuthResponse): void {
    const expiryTime = new Date().getTime() + authResponse.expiresIn * 1000;
    localStorage.setItem("token", authResponse.token);
    localStorage.setItem("tokenExpiry", expiryTime.toString());
    this.currentUserSubject.next(authResponse.user);
  }

  private loadUserFromToken(): void {
    const token = this.getToken();
    if (token && this.isAuthenticated()) {
      try {
        const decodedToken: any = jwtDecode(token);
        const user: User = {
          id: decodedToken.nameid,
          email: decodedToken.email,
          firstName: decodedToken.given_name,
          lastName: decodedToken.family_name,
          role: decodedToken.role,
          profilePicture: decodedToken.picture,
          isActive: true,
          createdAt: new Date(),
          updatedAt: new Date(),
        };
        this.currentUserSubject.next(user);
      } catch (error) {
        console.error("Error decoding token:", error);
        this.logout();
      }
    }
  }
}
