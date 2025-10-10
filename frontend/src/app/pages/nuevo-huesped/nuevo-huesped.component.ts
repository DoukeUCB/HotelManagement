import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NuevoHuespedService } from '../../core/services/nuevo-huesped.service';

@Component({
  selector: 'app-nuevo-huesped',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './nuevo-huesped.component.html',
  styleUrls: ['./nuevo-huesped.component.scss']
})
export class NuevoHuespedComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private api = inject(NuevoHuespedService);

  submitting = signal(false);
  mensaje = signal<string | null>(null);
  error = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    nombreCompleto: ['', [Validators.required, Validators.maxLength(100)]],
    documento: ['', [Validators.required, Validators.maxLength(20)]],
    telefono: ['', Validators.maxLength(20)],
    email: ['', [Validators.email, Validators.maxLength(100)]],
    fechaNacimiento: ['']
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
    const payload = {
      nombre_Completo: value.nombreCompleto,
      documento_Identidad: value.documento,
      telefono: value.telefono || null,
      email: value.email || null,
      fecha_Nacimiento: value.fechaNacimiento ? new Date(value.fechaNacimiento).toISOString() : null
    };

    console.log('Enviando payload:', payload);

    this.api.createHuesped(payload).subscribe({
      next: (res) => {
        console.log('Respuesta del servidor:', res);
        this.mensaje.set('Huésped registrado correctamente.');
        this.form.reset();
        this.submitting.set(false);
      },
      error: (err) => {
        console.error('Error del servidor:', err);
        this.error.set('No se pudo registrar el huésped. Intenta nuevamente.');
        this.submitting.set(false);
      }
    });
  }

  cancelar(): void {
    this.router.navigate(['/huespedes']);
  }
}
