import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "./core/guards/auth.guard";
import { AdminGuard } from "./core/guards/admin.guard";
import { GuestGuard } from "./core/guards/guest.guard";

import { LayoutComponent } from "./shared/components/layout/layout.component";
import { LoginComponent } from "./features/auth/login/login.component";
import { RegisterComponent } from "./features/auth/register/register.component";
import { ForgotPasswordComponent } from "./features/auth/forgot-password/forgot-password.component";
import { DashboardComponent } from "./features/dashboard/dashboard.component";
import { ExpenseListComponent } from "./features/expenses/expense-list/expense-list.component";
import { BudgetComponent } from "./features/budget/budget.component";
import { ProfileComponent } from "./features/profile/profile.component";
import { AdminPanelComponent } from "./features/admin/admin-panel/admin-panel.component";
import { ActivityLogsComponent } from "./features/activity-logs/activity-logs.component";

const routes: Routes = [
  // Auth routes (guest only)
  {
    path: "auth",
    canActivate: [GuestGuard],
    children: [
      { path: "login", component: LoginComponent },
      { path: "register", component: RegisterComponent },
      { path: "forgot-password", component: ForgotPasswordComponent },
      { path: "", redirectTo: "login", pathMatch: "full" },
    ],
  },

  // Protected routes
  {
    path: "",
    component: LayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: "dashboard", component: DashboardComponent },
      { path: "expenses", component: ExpenseListComponent },
      { path: "budget", component: BudgetComponent },
      { path: "profile", component: ProfileComponent },
      { path: "activity-logs", component: ActivityLogsComponent },
      {
        path: "admin",
        component: AdminPanelComponent,
        canActivate: [AdminGuard],
      },
      { path: "", redirectTo: "dashboard", pathMatch: "full" },
    ],
  },

  // Fallback route
  { path: "**", redirectTo: "auth/login" },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
