import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HuespedService } from '../../core/services/huesped.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-nuevo-huesped',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './nuevo-huesped.component.html',
  styleUrls: ['./nuevo-huesped.component.scss']
})
export class NuevoHuespedComponent implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private service = inject(HuespedService);

  submitting = signal(false);
  mensaje = signal<string | null>(null);
  error = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    primerNombre: ['', [Validators.required, Validators.minLength(2)]],
    segundoNombre: [''],
    primerApellido: ['', [Validators.required, Validators.minLength(2)]],
    segundoApellido: [''],
    documento: ['', [Validators.required, Validators.minLength(4)]],
    telefono: [''],
    fechaNacimiento: ['']
  });

  ngOnInit(): void {
    // ...si se necesita inicializar algo...
  }

  cancelar(): void {
    this.router.navigate(['/huespedes']);
  }

  guardar(): void {
    this.mensaje.set(null);
    this.error.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.error.set('Por favor completa los campos obligatorios correctamente.');
      return;
    }

    this.submitting.set(true);

    // Extraer valores con non-null assertion para evitar undefined
    const primerNombre: string = this.form.get('primerNombre')!.value.trim();
    const segundoNombre: string = (this.form.get('segundoNombre')!.value || '').trim();
    const primerApellido: string = this.form.get('primerApellido')!.value.trim();
    const segundoApellido: string | null = (this.form.get('segundoApellido')!.value || '').trim() || null;
    const documento: string = this.form.get('documento')!.value;
    const telefono: string | null = this.form.get('telefono')!.value || null;
    const fechaNacimiento: string | null = this.form.get('fechaNacimiento')!.value || null;

    // Construir Nombre completo para el backend: incluir segundoNombre si existe
    const Nombre = segundoNombre ? `${primerNombre} ${segundoNombre}` : primerNombre;
    const Apellido = primerApellido;
    const Segundo_Apellido = segundoApellido;

    const payload = {
      Nombre,
      Apellido,
      Segundo_Apellido,
      Documento_Identidad: documento,
      Telefono: telefono,
      Fecha_Nacimiento: fechaNacimiento
    };

    this.service.createHuesped(payload).pipe(
      finalize(() => this.submitting.set(false))
    ).subscribe({
      next: () => {
        this.mensaje.set('✅ Huésped creado correctamente.');
        setTimeout(() => this.router.navigate(['/huespedes']), 900);
      },
      error: (err: any) => {
        const msg = err?.error?.message ?? err?.message ?? 'Error al crear huésped';
        this.error.set(`❌ ${msg}`);
      }
    });
  }
}
