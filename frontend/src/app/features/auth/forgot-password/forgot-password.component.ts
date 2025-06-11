import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { AuthService } from "../../../core/services/auth.service";
import { NotificationService } from "../../../core/services/notification.service";

@Component({
  selector: "app-forgot-password",
  templateUrl: "./forgot-password.component.html",
  styleUrls: ["./forgot-password.component.scss"],
})
export class ForgotPasswordComponent implements OnInit {
  forgotPasswordForm: FormGroup;
  isLoading = false;
  isEmailSent = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private notificationService: NotificationService,
  ) {
    this.forgotPasswordForm = this.fb.group({
      email: ["", [Validators.required, Validators.email]],
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.forgotPasswordForm.valid) {
      this.isLoading = true;
      const email = this.forgotPasswordForm.get("email")?.value;

      this.authService.forgotPassword(email).subscribe({
        next: () => {
          this.isEmailSent = true;
          this.notificationService.showSuccess(
            "Password reset email sent successfully!",
          );
        },
        error: (error) => {
          this.notificationService.showError(
            error.error?.message ||
              "Failed to send reset email. Please try again.",
          );
          this.isLoading = false;
        },
        complete: () => {
          this.isLoading = false;
        },
      });
    }
  }

  getErrorMessage(): string {
    const emailControl = this.forgotPasswordForm.get("email");
    if (emailControl?.hasError("required")) {
      return "Email is required";
    }
    if (emailControl?.hasError("email")) {
      return "Enter a valid email";
    }
    return "";
  }
}
