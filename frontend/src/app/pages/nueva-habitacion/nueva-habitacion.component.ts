import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { finalize } from 'rxjs';

interface TipoHabitacion {
  id: string;
  nombre: string;
  descripcion: string;
  capacidad_Maxima: number;
  precio_Base: number;
}

@Component({
  selector: 'app-nueva-habitacion',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './nueva-habitacion.component.html',
  styleUrls: ['./nueva-habitacion.component.scss']
})
export class NuevaHabitacionComponent implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private http = inject(HttpClient);

  tiposHabitacion = signal<TipoHabitacion[]>([]);
  submitting = signal(false);
  mensaje = signal('');
  error = signal('');

  form = this.fb.group({
    numero: ['', Validators.required],
    piso: [1, [Validators.required, Validators.min(1)]],
    tipoHabitacionId: ['', Validators.required],
    estado: ['Libre', Validators.required]
  });

  estadosDisponibles = ['Libre', 'Reservada', 'Ocupada', 'Mantenimiento', 'Fuera de Servicio'];

  ngOnInit(): void {
    this.cargarTiposHabitacion();
  }

  private cargarTiposHabitacion(): void {
    this.http.get<TipoHabitacion[]>('http://localhost:5000/api/TipoHabitacion')
      .subscribe({
        next: (tipos) => this.tiposHabitacion.set(tipos),
        error: (err) => {
          console.error('Error al cargar tipos de habitación:', err);
          this.error.set('No se pudieron cargar los tipos de habitación');
        }
      });
  }

  guardar(): void {
    if (this.form.invalid) return;

    this.submitting.set(true);
    this.error.set('');
    this.mensaje.set('');

    const payload = {
      numero_Habitacion: this.form.value.numero!,
      piso: this.form.value.piso!,
      tipo_Habitacion_ID: this.form.value.tipoHabitacionId!,
      estado_Habitacion: this.form.value.estado!
    };

    this.http.post('http://localhost:5000/api/Habitacion', payload)
      .pipe(finalize(() => this.submitting.set(false)))
      .subscribe({
        next: () => {
          this.mensaje.set('Habitación creada exitosamente');
          setTimeout(() => this.router.navigate(['/habitaciones']), 1500);
        },
        error: (err) => {
          console.error('Error al crear habitación:', err);
          this.error.set(err.error?.message || 'Error al crear la habitación');
        }
      });
  }

  cancelar(): void {
    this.router.navigate(['/habitaciones']);
  }
}
