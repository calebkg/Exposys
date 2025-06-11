import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { AuthService } from "../../../core/services/auth.service";
import { NotificationService } from "../../../core/services/notification.service";

@Component({
  selector: "app-register",
  templateUrl: "./register.component.html",
  styleUrls: ["./register.component.scss"],
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  isLoading = false;
  hidePassword = true;
  hideConfirmPassword = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private notificationService: NotificationService,
  ) {
    this.registerForm = this.fb.group(
      {
        firstName: ["", [Validators.required, Validators.minLength(2)]],
        lastName: ["", [Validators.required, Validators.minLength(2)]],
        email: ["", [Validators.required, Validators.email]],
        password: [
          "",
          [
            Validators.required,
            Validators.minLength(8),
            Validators.pattern(
              /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/,
            ),
          ],
        ],
        confirmPassword: ["", [Validators.required]],
      },
      { validators: this.passwordMatchValidator },
    );
  }

  ngOnInit(): void {}

  passwordMatchValidator(form: FormGroup) {
    const password = form.get("password");
    const confirmPassword = form.get("confirmPassword");
    if (
      password &&
      confirmPassword &&
      password.value !== confirmPassword.value
    ) {
      return { passwordMismatch: true };
    }
    return null;
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.isLoading = true;
      this.authService.register(this.registerForm.value).subscribe({
        next: (response) => {
          this.notificationService.showSuccess("Registration successful!");
          this.router.navigate(["/dashboard"]);
        },
        error: (error) => {
          this.notificationService.showError(
            error.error?.message || "Registration failed. Please try again.",
          );
          this.isLoading = false;
        },
        complete: () => {
          this.isLoading = false;
        },
      });
    }
  }

  getErrorMessage(field: string): string {
    const control = this.registerForm.get(field);
    if (control?.hasError("required")) {
      return `${this.getFieldDisplayName(field)} is required`;
    }
    if (control?.hasError("email")) {
      return "Enter a valid email";
    }
    if (control?.hasError("minlength")) {
      const minLength = control?.errors?.["minlength"].requiredLength;
      return `${this.getFieldDisplayName(field)} must be at least ${minLength} characters`;
    }
    if (control?.hasError("pattern") && field === "password") {
      return "Password must contain uppercase, lowercase, number and special character";
    }
    if (
      this.registerForm.hasError("passwordMismatch") &&
      field === "confirmPassword"
    ) {
      return "Passwords do not match";
    }
    return "";
  }

  private getFieldDisplayName(field: string): string {
    const fieldNames: { [key: string]: string } = {
      firstName: "First name",
      lastName: "Last name",
      email: "Email",
      password: "Password",
      confirmPassword: "Confirm password",
    };
    return fieldNames[field] || field;
  }
}
