import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ClienteService } from '../../core/services/cliente.service';

@Component({
  selector: 'app-nuevo-cliente',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './nuevo-cliente.component.html',
  styleUrls: ['./nuevo-cliente.component.scss']
})
export class NuevoClienteComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private api = inject(ClienteService);

  submitting = signal(false);
  mensaje = signal<string | null>(null);
  error = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    razonSocial: ['', [Validators.required, Validators.maxLength(20)]],
    nit: ['', [Validators.required, Validators.maxLength(20)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(30)]]
  });

  guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.mensaje.set(null);
    this.error.set(null);

    const value = this.form.getRawValue();

    this.api.createCliente({
      razon_Social: value.razonSocial,
      nit: value.nit,
      email: value.email
    }).subscribe({
      next: () => {
        this.mensaje.set('Cliente creado correctamente.');
        this.form.reset();
        this.submitting.set(false);
      },
      error: () => {
        this.error.set('No se pudo crear el cliente. Intenta nuevamente.');
        this.submitting.set(false);
      }
    });
  }

  cancelar(): void {
    this.router.navigate(['/clientes']);
  }
}
