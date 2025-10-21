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

  private _submitting = false;
  private _mensaje = '';
  private _error = '';

  // Signals existentes (no tocar)
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
    this.error.set(null);
    this.mensaje.set(null);

    // Payload con las claves que espera ClienteCreatePayload
    const payload = {
      razon_Social: (this.form.value.razonSocial || '').toUpperCase(),
      nit: (this.form.value.nit || '').toUpperCase(),
      email: this.form.value.email || ''
    };

    this.api.createCliente(payload).subscribe({
      next: () => {
        this.submitting.set(false);
        this.mensaje.set('Cliente creado correctamente. Redirigiendo...');

        // Espera 1.5s para que el usuario vea el mensaje, luego redirige a /clientes
        setTimeout(() => {
          this.router.navigate(['/clientes']);
        }, 1500);
      },
      error: (err: any) => {
        this.submitting.set(false);
        console.error('Error creando cliente:', err);
        this.error.set('No se pudo crear el cliente. Intenta nuevamente.');
      }
    });
  }

  cancelar(): void {
    this.router.navigate(['/clientes']);
  }
}
