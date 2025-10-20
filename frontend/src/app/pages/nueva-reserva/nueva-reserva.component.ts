import { Component, OnInit, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormArray, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, switchMap, tap } from 'rxjs';
import {
  NuevaReservaService,
  ClienteOption,
  HabitacionOption,
  HuespedOption
} from '../../core/services/nueva-reserva.service';

interface HabitacionForm {
  habitacionId: string;
  fechaEntrada: string;
  fechaSalida: string;
  huespedIds: string[];
}

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

  // Sistema de pasos
  pasoActual = signal(1);
  totalPasos = 3;
  habitacionSearchTerm: string[] = [];
  showHabitacionSug: boolean[] = [];

  private norm3 = (v: unknown) =>
  (v ?? '').toString().toLowerCase().normalize('NFD').replace(/\p{Diacritic}/gu, '');


  estadosReserva = ['Pendiente', 'Confirmada', 'Cancelada'];
  
  filteredHabitaciones(i: number): HabitacionOption[] {
  const term = this.norm3(this.habitacionSearchTerm[i] ?? '');
  const disponibles = this.getHabitacionesLibres(i);
  if (!term) return disponibles;

  return disponibles.filter(h => this.norm3(h.numero).includes(term));
}
private ensureHabitacionStateIndex(i: number) {
  while (this.habitacionSearchTerm.length <= i) this.habitacionSearchTerm.push('');
  while (this.showHabitacionSug.length <= i) this.showHabitacionSug.push(false);
}
openHabitacion(i: number) {
  this.ensureHabitacionStateIndex(i);
  this.showHabitacionSug[i] = true;
}

onHabitacionBlur(i: number) {
  setTimeout(() => this.showHabitacionSug[i] = false, 150);
}

onHabitacionInput(i: number, value: string) {
  this.ensureHabitacionStateIndex(i);
  this.habitacionSearchTerm[i] = value;
}

seleccionarHabitacion(i: number, h: HabitacionOption) {
  this.habitacionesFormArray.at(i).patchValue({ habitacionId: h.id });
  this.habitacionSearchTerm[i] = '';
  this.showHabitacionSug[i] = false;
}

habitacionesLibres = computed<HabitacionOption[]>(() => {
    return (this.habitaciones() ?? []).filter(h => {
      const estado = (h.estado ?? '').toString().toLowerCase();
      return estado === 'libre';
    });
  });
  getHabitacionesLibres(indexActual: number): HabitacionOption[] {
  const todas = this.habitaciones() ?? [];

  // Obtener IDs seleccionados en otros grupos
  const seleccionadas = new Set(
    this.habitacionesFormArray.controls
      .map((fg, i) => i !== indexActual ? fg.get('habitacionId')?.value : null)
      .filter(id => !!id)
  );

  return todas.filter(h => {
    const estado = (h.estado ?? '').toString().toLowerCase();
    return estado === 'libre' && !seleccionadas.has(h.id);
  });
}


  /** Si una selección deja de estar libre (o desaparece), la limpiamos. */
  private _syncSeleccionConDisponibilidad = effect(() => {
    // dispara cuando cambie el catálogo filtrado
    const libres = this.habitacionesLibres();
    const idsLibres = new Set(libres.map(h => h.id));
    for (const fg of this.habitacionesFormArray.controls) {
      const sel = fg.get('habitacionId')?.value;
      if (sel && !idsLibres.has(sel)) {
        fg.get('habitacionId')?.reset('');
      }
    }
  });

  form = this.fb.nonNullable.group({
    clienteId: ['', Validators.required],
    estadoReserva: ['Pendiente', Validators.required],
    montoTotal: [null as number | null, [Validators.required, Validators.min(0)]],
    habitaciones: this.fb.array<FormGroup<{
      habitacionId: any;
      fechaEntrada: any;
      fechaSalida: any;
      huespedIds: any;
    }>>([], Validators.minLength(1))
  });

  // Computed para validar cada paso (se mantienen pero no se usan en template)
  paso1Valido = computed(() => {
    const clienteId = this.form.get('clienteId')?.value;
    return !!(clienteId && clienteId !== '');
  });

  paso2Valido = computed(() => {
    const habitaciones = this.habitacionesFormArray;
    return habitaciones.length > 0 && habitaciones.controls.every(h => {
      const huespedIds = h.get('huespedIds')?.value as string[] || [];
      return h.get('habitacionId')?.valid && 
             h.get('fechaEntrada')?.valid && 
             h.get('fechaSalida')?.valid &&
             huespedIds.length > 0;
    });
  });

  paso3Valido = computed(() => {
    return this.form.controls.montoTotal.valid;
  });

  get habitacionesFormArray(): FormArray {
    return this.form.get('habitaciones') as FormArray;
  }

  // Computed para obtener el nombre del cliente seleccionado
  clienteSeleccionado = computed(() => {
    const clienteId = this.form.controls.clienteId.value;
    const cliente = this.clientes().find(c => c.id === clienteId);
    return cliente?.label || 'No seleccionado';
  });

  ngOnInit(): void {
    this.cargarCatalogos();
    this.agregarHabitacion();
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

  // Navegación de pasos
  irAPaso(paso: number): void {
    if (paso >= 1 && paso <= this.totalPasos) {
      this.pasoActual.set(paso);
    }
  }

  siguientePaso(): void {
    if (this.pasoActual() < this.totalPasos) {
      this.pasoActual.set(this.pasoActual() + 1);
    }
  }

  pasoAnterior(): void {
    if (this.pasoActual() > 1) {
      this.pasoActual.set(this.pasoActual() - 1);
    }
  }

  agregarHabitacion(): void {
    const habitacionGroup = this.fb.nonNullable.group({
      habitacionId: ['', Validators.required],
      fechaEntrada: ['', Validators.required],
      fechaSalida: ['', Validators.required],
      huespedIds: [[] as string[], Validators.minLength(1)]
    });

    this.habitacionesFormArray.push(habitacionGroup);
  }

  eliminarHabitacion(index: number): void {
    if (this.habitacionesFormArray.length > 1) {
      this.habitacionesFormArray.removeAt(index);
    }
  }

  agregarHuesped(habitacionIndex: number, huespedId: string): void {
    const habitacionGroup = this.habitacionesFormArray.at(habitacionIndex);
    const huespedIds = habitacionGroup.get('huespedIds')?.value as string[] || [];
    
    if (huespedId && !huespedIds.includes(huespedId)) {
      habitacionGroup.patchValue({
        huespedIds: [...huespedIds, huespedId]
      });
    }
  }

  eliminarHuesped(habitacionIndex: number, huespedId: string): void {
    const habitacionGroup = this.habitacionesFormArray.at(habitacionIndex);
    const huespedIds = (habitacionGroup.get('huespedIds')?.value as string[] || [])
      .filter(id => id !== huespedId);
    
    habitacionGroup.patchValue({ huespedIds });
  }

  obtenerNombreHuesped(id: string): string {
    return this.huespedes().find(h => h.id === id)?.nombre || 'Desconocido';
  }

  calcularDias(fechaEntrada: string, fechaSalida: string): number {
    if (!fechaEntrada || !fechaSalida) return 0;
    const entrada = new Date(fechaEntrada);
    const salida = new Date(fechaSalida);
    const diferencia = salida.getTime() - entrada.getTime();
    return Math.max(0, Math.ceil(diferencia / (1000 * 60 * 60 * 24)));
  }

  calcularTotalDias(): number {
    let total = 0;
    this.habitacionesFormArray.controls.forEach(h => {
      const entrada = h.get('fechaEntrada')?.value;
      const salida = h.get('fechaSalida')?.value;
      if (entrada && salida) {
        total += this.calcularDias(entrada, salida);
      }
    });
    return total;
  }

  crearReserva(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.error.set('Por favor, completa todos los campos requeridos correctamente.');
      return;
    }

    const habitaciones = this.habitacionesFormArray.controls;
    for (let i = 0; i < habitaciones.length; i++) {
      const huespedIds = habitaciones[i].get('huespedIds')?.value as string[] || [];
      if (huespedIds.length === 0) {
        this.error.set(`La habitación ${i + 1} debe tener al menos un huésped asignado.`);
        this.irAPaso(2);
        return;
      }
    }

    this.error.set(null);
    this.mensaje.set(null);
    this.submitting.set(true);

    const data = this.form.getRawValue();

    const reservaPayload = {
      cliente_ID: data.clienteId,
      estado_Reserva: data.estadoReserva,
      monto_Total: data.montoTotal || 0
    };

    this.api
      .createReserva(reservaPayload)
      .pipe(
        switchMap(reservaCreada => {
          const reservaId = reservaCreada?.id ?? reservaCreada?.ID;
          
          if (!reservaId) {
            throw new Error('La API no devolvió el ID de la reserva creada.');
          }

          const habitacionesPayload = (data.habitaciones as HabitacionForm[]).map((h: HabitacionForm) => ({
            habitacion_ID: h.habitacionId,
            fecha_Entrada: h.fechaEntrada,
            fecha_Salida: h.fechaSalida,
            huesped_IDs: h.huespedIds
          }));

          return this.api.createDetallesMultiples(reservaId, habitacionesPayload);
        }),
        finalize(() => this.submitting.set(false))
      )
      .subscribe({
        next: () => {
          this.mensaje.set('✅ Reserva creada exitosamente.');
          
          setTimeout(() => {
            this.router.navigate(['/reservas']);
          }, 2000);
        },
        error: (err: any) => {
          let mensajeError = '❌ No se pudo crear la reserva.';
          
          if (err.error?.errors) {
            mensajeError += '\n' + err.error.errors.join('\n');
          } else if (err.error?.message) {
            mensajeError += ' ' + err.error.message;
          } else if (err.message) {
            mensajeError += ' ' + err.message;
          }
          
          this.error.set(mensajeError);
        }
      });
  }

  volverAlListado(): void {
    this.router.navigate(['/reservas']);
  }

  // Nuevas funciones que se evalúan en tiempo real desde el template
  isPaso1Valid(): boolean {
    const clienteId = this.form.get('clienteId')?.value;
    return !!(clienteId && clienteId !== '');
  }

  isPaso2Valid(): boolean {
    const habitaciones = this.habitacionesFormArray;
    if (habitaciones.length === 0) return false;
    for (const h of habitaciones.controls) {
      const habitacionId = h.get('habitacionId')?.value;
      const fechaEntrada = h.get('fechaEntrada')?.value;
      const fechaSalida = h.get('fechaSalida')?.value;
      const huespedIds = (h.get('huespedIds')?.value as string[]) || [];
      if (!habitacionId || habitacionId === '') return false;
      if (!fechaEntrada || !fechaSalida) return false;
      // validar que fechaSalida sea posterior a fechaEntrada (opcional)
      try {
        if (new Date(fechaSalida) <= new Date(fechaEntrada)) return false;
      } catch {
        return false;
      }
      if (huespedIds.length === 0) return false;
    }
    return true;
  }

  isPaso3Valid(): boolean {
    const montoCtrl = this.form.get('montoTotal');
    return !!montoCtrl && montoCtrl.valid;
  }
  // Control de autocompletado
showSuggestions = false;

seleccionarCliente(c: any) {
  this.form.controls.clienteId.setValue(c.id);
  this.searchTerm.set(c.label);
  this.showSuggestions = false;
}

// Esconde el menú con pequeño delay para permitir el click
onBlur() {
  setTimeout(() => this.showSuggestions = false, 200);
}

  // === BUSCADOR (cliente por Razón Social o NIT) ===
searchTerm = signal<string>('');

// Normaliza a dígitos para comparar NIT sin puntos/guiones/espacios
private onlyDigits = (s: unknown) => (s ?? '').toString().replace(/\D+/g, '');

/// Normaliza texto: minúsculas + sin tildes
private norm = (v: unknown) =>
  (v ?? '')
    .toString()
    .toLowerCase()
    .normalize('NFD')
    .replace(/\p{Diacritic}/gu, ''); // requiere TS target ES2021+ (Angular actual lo soporta)

// Solo dígitos (para NIT)
private digits = (v: unknown) => (v ?? '').toString().replace(/\D+/g, '');

filteredClientes = computed<ClienteOption[]>(() => {
  const termRaw = this.searchTerm().trim();
  if (!termRaw) return this.clientes(); // sin término -> devuelve todo

  const term = this.norm(termRaw);
  const termDigits = this.digits(termRaw);

  return this.clientes().filter((c: any) => {
    // Candidatos de texto (agrega aquí cualquier alias que use tu backend)
    const textParts = [
      c.label,
      c.nombre,
      c.nombreCompleto,
      c.razonSocial,
      c.razon_social,
      c.razon_Social,
      c.nombreComercial,
      c.comercialName,
      c.email,
    ];

    // Candidatos numéricos (NIT, doc, etc.)
    const numParts = [
      c.nit,
      c.NIT,
      c.numeroDocumento,
      c.documento,
      c.ci,
      c.ciNit,
    ];

    // Concatenamos y normalizamos
    const textBlob = this.norm(textParts.filter(Boolean).join(' '));
    const digitsBlob = this.digits(numParts.filter(Boolean).join(' '));

    // Coincide por texto o por dígitos
    const byText = textBlob.includes(term);
    const byDigits = !!termDigits && digitsBlob.includes(termDigits);

    // Además: por si el NIT está embebido en el label (ej: "Farmacia — NIT 123456")
    const labelDigits = this.digits(c.label);
    const byLabelDigits = !!termDigits && labelDigits.includes(termDigits);

    return byText || byDigits || byLabelDigits;
  });
});
// ====== BÚSQUEDA DE HUÉSPEDES (por habitación) ======
huespedSearchTerm: string[] = [];   // término por índice de habitación
showHuespedSug: boolean[] = [];     // visibilidad del panel por índice

// Normalizador simple (si ya tienes uno, puedes reutilizarlo)
private norm2 = (v: unknown) =>
  (v ?? '').toString().toLowerCase().normalize('NFD').replace(/\p{Diacritic}/gu, '');

// Abrir/cerrar panel de sugerencias
openHuesped(i: number) {
  this.ensureHuespedStateIndex(i);
  this.showHuespedSug[i] = true;
}

onHuespedBlur(i: number) {
  // pequeño delay para permitir el mousedown de la opción
  setTimeout(() => this.showHuespedSug[i] = false, 150);
}

onHuespedInput(i: number, value: string) {
  this.ensureHuespedStateIndex(i);
  this.huespedSearchTerm[i] = value;
  // si no hay texto pero quieres mostrar todo al enfocar:
  // this.showHuespedSug[i] = true;
}

seleccionarHuesped(i: number, h: HuespedOption, inputEl?: HTMLInputElement) {
  // reutiliza tu lógica existente para agregar al FormArray
  this.agregarHuesped(i, h.id);
  // limpia el input y oculta sugerencias
  this.huespedSearchTerm[i] = '';
  if (inputEl) inputEl.value = '';
  this.showHuespedSug[i] = false;
}

// Lista filtrada por nombre para la habitación i
filteredHuespedes(i: number): HuespedOption[] {
  const term = this.norm2(this.huespedSearchTerm[i] ?? '');
  const all = this.huespedes();
  if (!term) return all;

  return all.filter(h => this.norm2(h.nombre).includes(term));
}

// Asegura índices en arrays auxiliares (al agregar/eliminar hab.)
private ensureHuespedStateIndex(i: number) {
  while (this.huespedSearchTerm.length <= i) this.huespedSearchTerm.push('');
  while (this.showHuespedSug.length <= i) this.showHuespedSug.push(false);
}

}
