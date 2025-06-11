import { Component, OnInit } from "@angular/core";
import { Router, NavigationEnd } from "@angular/router";
import { AuthService } from "../../../core/services/auth.service";
import { Observable } from "rxjs";
import { filter } from "rxjs/operators";

interface MenuItem {
  label: string;
  icon: string;
  route: string;
  adminOnly?: boolean;
}

@Component({
  selector: "app-sidebar",
  templateUrl: "./sidebar.component.html",
  styleUrls: ["./sidebar.component.scss"],
})
export class SidebarComponent implements OnInit {
  currentRoute = "";
  isAdmin = false;

  menuItems: MenuItem[] = [
    {
      label: "Dashboard",
      icon: "dashboard",
      route: "/dashboard",
    },
    {
      label: "Expenses",
      icon: "receipt_long",
      route: "/expenses",
    },
    {
      label: "Budget",
      icon: "savings",
      route: "/budget",
    },
    {
      label: "Activity Logs",
      icon: "history",
      route: "/activity-logs",
    },
    {
      label: "Profile",
      icon: "person",
      route: "/profile",
    },
    {
      label: "Admin Panel",
      icon: "admin_panel_settings",
      route: "/admin",
      adminOnly: true,
    },
  ];

  constructor(
    private router: Router,
    private authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.currentRoute = event.url;
      });

    this.authService.currentUser$.subscribe((user) => {
      this.isAdmin = user?.role === "Admin";
    });

    this.currentRoute = this.router.url;
  }

  isActiveRoute(route: string): boolean {
    return this.currentRoute === route || this.currentRoute.startsWith(route);
  }

  shouldShowMenuItem(item: MenuItem): boolean {
    if (item.adminOnly) {
      return this.isAdmin;
    }
    return true;
  }

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }
}
