import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, switchMap, tap } from 'rxjs';
import {
  NuevaReservaService,
  ClienteOption,
  HabitacionOption,
  HuespedOption
} from '../../core/services/nueva-reserva.service';

@Component({
  selector: 'app-nueva-reserva',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './nueva-reserva.component.html',
  styleUrls: ['./nueva-reserva.component.scss']
})
export class NuevaReservaComponent implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private api = inject(NuevaReservaService);

  clientes = signal<ClienteOption[]>([]);
  habitaciones = signal<HabitacionOption[]>([]);
  huespedes = signal<HuespedOption[]>([]);
  catalogosCargando = signal(false);

  submitting = signal(false);
  mensaje = signal<string | null>(null);
  error = signal<string | null>(null);

  estadosReserva = ['Pendiente', 'Confirmada', 'Cancelada', 'Completada', 'No-Show'];

  form = this.fb.nonNullable.group({
    clienteId: ['', Validators.required],
    habitacionId: ['', Validators.required],
    huespedId: ['', Validators.required],
    fechaEntrada: ['', Validators.required],
    fechaSalida: ['', Validators.required],
    estadoReserva: ['Pendiente', Validators.required],
    cantidadHuespedes: [1, [Validators.required, Validators.min(1)]],
    montoTotal: [null as number | null, [Validators.required, Validators.min(0)]],
    precioDetalle: [null as number | null, [Validators.required, Validators.min(0)]]
  });

  ngOnInit(): void {
    this.cargarCatalogos();
  }

  private cargarCatalogos(): void {
    this.catalogosCargando.set(true);
    this.error.set(null);
    this.api
      .getClientes()
      .pipe(
        tap(list => this.clientes.set(list)),
        switchMap(() => this.api.getHabitaciones()),
        tap(list => this.habitaciones.set(list)),
        switchMap(() => this.api.getHuespedes()),
        tap(list => this.huespedes.set(list)),
        finalize(() => this.catalogosCargando.set(false))
      )
      .subscribe({
        next: list => this.huespedes.set(list),
        error: () => this.error.set('No se pudieron cargar los catálogos. Intenta nuevamente.')
      });
  }

  crearReserva(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.error.set(null);
    this.mensaje.set(null);
    this.submitting.set(true);

    const data = this.form.getRawValue();
    const reservaPayload = {
      cliente_ID: data.clienteId,
      fecha_Reserva: new Date().toISOString(),
      fecha_Entrada: data.fechaEntrada,
      fecha_Salida: data.fechaSalida,
      estado_Reserva: data.estadoReserva,
      monto_Total: data.montoTotal ?? data.precioDetalle ?? 0
    };

    this.api
      .createReserva(reservaPayload)
      .pipe(
        switchMap(reservaCreada => {
          const reservaId = reservaCreada?.id ?? reservaCreada?.ID;
          if (!reservaId) {
            throw new Error('La API no devolvió el ID de la reserva creada.');
          }

          return this.api.createDetalleReserva({
            reserva_ID: reservaId,
            habitacion_ID: data.habitacionId,
            huesped_ID: data.huespedId,
            precio_Total: data.precioDetalle ?? data.montoTotal ?? 0,
            cantidad_Huespedes: data.cantidadHuespedes
          });
        }),
        finalize(() => this.submitting.set(false))
      )
      .subscribe({
        next: () => {
          this.mensaje.set('Reserva creada exitosamente.');
          this.form.reset({
            clienteId: '',
            habitacionId: '',
            huespedId: '',
            fechaEntrada: '',
            fechaSalida: '',
            estadoReserva: 'Pendiente',
            cantidadHuespedes: 1,
            montoTotal: null,
            precioDetalle: null
          });
        },
        error: (err: unknown) => {
          console.error('Error al crear la reserva', err);
          this.error.set(
            'No se pudo crear la reserva. Verifica los datos o intenta nuevamente más tarde.'
          );
        }
      });
  }

  volverAlListado(): void {
    this.router.navigate(['/reservas']);
  }
}
