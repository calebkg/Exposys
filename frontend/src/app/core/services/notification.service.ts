import { Injectable } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ToastrService } from "ngx-toastr";

@Injectable({
  providedIn: "root",
})
export class NotificationService {
  constructor(
    private snackBar: MatSnackBar,
    private toastr: ToastrService,
  ) {}

  showSuccess(message: string, title: string = "Success"): void {
    this.toastr.success(message, title);
  }

  showError(message: string, title: string = "Error"): void {
    this.toastr.error(message, title);
  }

  showWarning(message: string, title: string = "Warning"): void {
    this.toastr.warning(message, title);
  }

  showInfo(message: string, title: string = "Info"): void {
    this.toastr.info(message, title);
  }

  showSnackBar(
    message: string,
    action: string = "Close",
    duration: number = 3000,
  ): void {
    this.snackBar.open(message, action, {
      duration,
      horizontalPosition: "end",
      verticalPosition: "top",
    });
  }
}
