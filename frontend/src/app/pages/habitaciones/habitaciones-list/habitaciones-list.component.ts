import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { NuevaReservaService } from '../../../core/services/nueva-reserva.service';
import { Subscription, timer } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { HabitacionService } from '../../../core/services/habitacion.service';
import { OrderByNumeroPipe } from './order-by-numero.pipe';

@Component({
  selector: 'app-habitaciones-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, OrderByNumeroPipe],
  templateUrl: './habitaciones-list.component.html',
  styleUrls: ['./habitaciones-list.component.scss']
})
export class HabitacionesListComponent implements OnInit, OnDestroy {
  private api = inject(NuevaReservaService);

  habitaciones: any[] = [];
  loading = true;

  // Lista de tipos para el select
  tipos: any[] = [];

  // Modales
  modalEditarAbierto = false;
  habitacionAEditar: any = null;
  // nuevo: guardar estado original para detectar cambios
  originalEstado: string | null = null;

  modalEliminarAbierto = false;
  habitacionAEliminar: any = null;

  private pollSub: Subscription | null = null;

  // Para búsqueda por número de habitación
  busquedaNumero: string = '';

  constructor(
    private habitacionService: HabitacionService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargarHabitaciones();

    // Cargar tipos de habitación para el select
    this.habitacionService.getTiposHabitacion().subscribe({
      next: (t: any[]) => {
        this.tipos = t || [];
      },
      error: (err: any) => {
        console.error('Error cargando tipos de habitación:', err);
      }
    });

    // Polling cada 15s para reflejar cambios en BD (puedes ajustar intervalo)
    this.pollSub = timer(15000, 15000).subscribe(() => {
      this.cargarHabitaciones();
    });
  }

  ngOnDestroy(): void {
    this.pollSub?.unsubscribe();
  }

  goBack(): void {
    this.router.navigate(['/inicio']);
  }

  cargarHabitaciones() {
    this.loading = true;
    this.habitacionService.getHabitaciones().subscribe({
      next: (data) => {
        this.habitaciones = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error cargando habitaciones:', err);
        this.loading = false;
      }
    });
  }

  // Editar
  abrirModalEditar(h: any) {
    // clonar
    this.habitacionAEditar = { ...h };
    this.originalEstado = (h.estado ?? 'Libre'); // guardar estado original

    // intentar asignar tipoId si el objeto tiene tipoId o tipoNombre
    if (!this.habitacionAEditar.tipoId && this.habitacionAEditar.tipoNombre) {
      const match = this.tipos.find(t => {
        const nombreTipo = (t.tipo_Nombre ?? t.nombre ?? t.tipoNombre ?? '').toString().trim();
        return nombreTipo === this.habitacionAEditar.tipoNombre;
      });
      if (match) {
        this.habitacionAEditar.tipoId = match.id ?? match.ID ?? match.tipo_Id;
        this.habitacionAEditar.tipoNombre = this.habitacionAEditar.tipoNombre || (match.tipo_Nombre ?? match.nombre ?? match.tipoNombre);
      }
    } else if (this.habitacionAEditar.tipoId && !this.habitacionAEditar.tipoNombre) {
      // si tenemos tipoId pero no tipoNombre, buscar y asignar nombre
      const match2 = this.tipos.find(t => (t.id ?? t.ID ?? t.tipo_Id) === this.habitacionAEditar.tipoId);
      if (match2) {
        this.habitacionAEditar.tipoNombre = match2.tipo_Nombre ?? match2.nombre ?? match2.tipoNombre;
      }
    }

    this.modalEditarAbierto = true;
  }

  cerrarModalEditar() {
    this.modalEditarAbierto = false;
    this.habitacionAEditar = null;
  }

  guardarEdicion() {
    if (!this.habitacionAEditar) return;

    const tipoSeleccionado = this.tipos.find(t => (t.id ?? t.ID ?? t.tipo_Id) === this.habitacionAEditar.tipoId);
    const tipoNombre = tipoSeleccionado ? (tipoSeleccionado.tipo_Nombre ?? tipoSeleccionado.nombre ?? tipoSeleccionado.tipoNombre) : (this.habitacionAEditar.tipoNombre ?? '');

    // normalizar estado para backend (ya no hay tratamiento especial para "Mantenimiento")
    const estadoBackend = this.normalizeEstadoForBackend(this.habitacionAEditar.estado);

    // Si solo cambió el estado, intentar el endpoint específico con variantes de payload (genérico)
    if (this.originalEstado !== null && this.habitacionAEditar.estado !== this.originalEstado) {
      const variantesPayloads = [
        { estado_Habitacion: estadoBackend },
        { Estado_Habitacion: estadoBackend },
        { estado: estadoBackend },
        { Estado: estadoBackend },
        { EstadoHabitacion: estadoBackend },
        { estadoHabitacion: estadoBackend }
      ];

      const tryVariant = (i: number) => {
        if (i >= variantesPayloads.length) {
          alert('No se pudo actualizar el estado. Revisa el servidor o los logs (probé varias variantes de payload).');
          return;
        }

        const payload = variantesPayloads[i];
        console.log(`Probando variante ${i + 1}:`, payload);

        this.habitacionService.updateEstadoKey(this.habitacionAEditar.id, payload).subscribe({
          next: (resp) => {
            console.log('updateEstado OK, respuesta:', resp);
            this.habitaciones = this.habitaciones.map(h => h.id === this.habitacionAEditar.id ? { ...h, estado: estadoBackend } : h);
            this.cerrarModalEditar();
          },
          error: (err: any) => {
            console.warn(`Variante ${i + 1} falló:`, err);
            if (i < variantesPayloads.length - 1) {
              tryVariant(i + 1);
            } else {
              console.error('Cuerpo de error final del servidor:', err?.error ?? err);
              const serverMsg = err?.error ? (typeof err.error === 'string' ? err.error : JSON.stringify(err.error)) : (err?.message || `HTTP ${err?.status}` );
              alert(`No se pudo actualizar el estado. Respuesta servidor: ${serverMsg}`);
            }
          }
        });
      };

      tryVariant(0);
      return;
    }

    // Si no es solo cambio de estado, usar la actualización completa
    this._guardarEdicionCompleta(tipoNombre, estadoBackend);
  }

  // Extrae la lógica de actualización completa para reusar desde el fallback
  private _guardarEdicionCompleta(tipoNombre: string, estadoBackend: string) {
    // preparar payload base
    const basePayload = {
      id: this.habitacionAEditar.id,
      ID: this.habitacionAEditar.id,
      numero_Habitacion: this.habitacionAEditar.numero,
      piso: this.habitacionAEditar.piso,
      tipo_Id: this.habitacionAEditar.tipoId ?? null,
      Tipo_Habitacion_ID: this.habitacionAEditar.tipoId ?? null,
      tipo_Nombre: tipoNombre,
      capacidad_Maxima: this.habitacionAEditar.capacidad,
      // estado_Habitacion será asignado dinámicamente en los intentos
    };

    // variantes a probar (genérico, sin rama por 'mantenimiento')
    const estadoBase = estadoBackend;
    const variantesEstado = [estadoBase];
    const up = estadoBase.toUpperCase();
    if (up !== estadoBase) variantesEstado.push(up);
    const estadosUnicos = variantesEstado.filter((v, i, a) => v && a.indexOf(v) === i);

    const tryUpdate = (index: number) => {
      if (index >= estadosUnicos.length) {
        alert('No se pudo guardar la habitación. Intenta nuevamente o revisa el servidor.');
        return;
      }
      const payload = { ...basePayload, estado_Habitacion: estadosUnicos[index] };
      console.log(`Intento ${index + 1}/${estadosUnicos.length} - payload:`, payload);
      this.habitacionService.updateHabitacion(payload).subscribe({
        next: (response) => {
          console.log(`Respuesta HTTP status=${response.status}`, response);
          const actualizado = response.body;
          const updated = actualizado || {
            id: payload.ID,
            numero: payload.numero_Habitacion,
            piso: payload.piso,
            tipoNombre: payload.tipo_Nombre,
            capacidad: payload.capacidad_Maxima,
            estado: payload.estado_Habitacion
          };
          this.habitaciones = this.habitaciones.map(h => h.id === updated.id ? updated : h);
          this.cerrarModalEditar();
        },
        error: (err: any) => {
          console.error(`Error intento ${index + 1}:`, err);
          // Mostrar cuerpo de error si lo devuelve el servidor
          if (err?.error) {
            console.error('Cuerpo de error del servidor:', err.error);
            // Si es 5xx, reintentar con otra variante
            if (err.status >= 500 && err.status < 600 && index < estadosUnicos.length - 1) {
              console.warn('Error 5xx, reintentando con siguiente variante de estado...');
              tryUpdate(index + 1);
              return;
            }
            // Mostrar mensaje detallado al usuario
            const serverMsg = typeof err.error === 'string' ? err.error : JSON.stringify(err.error);
            alert(`No se pudo guardar la habitación. Respuesta servidor: ${serverMsg}`);
            return;
          }

          // si no hay cuerpo, manejo estándar
          if (err && err.status && err.status >= 500 && index < estadosUnicos.length - 1) {
            tryUpdate(index + 1);
            return;
          }

          const serverMsg =
            err?.message ||
            (err?.status ? `HTTP ${err.status} ${err.statusText || ''}` : null);
          alert(`No se pudo guardar la habitación. ${serverMsg ? 'Motivo: ' + serverMsg : 'Intenta nuevamente.'}`);
        }
      });
    };

    tryUpdate(0);
  }

  // Eliminar
  abrirModalEliminar(h: any) {
    this.habitacionAEliminar = h;
    this.modalEliminarAbierto = true;
  }

  cerrarModalEliminar() {
    this.modalEliminarAbierto = false;
    this.habitacionAEliminar = null;
  }

  confirmarEliminar() {
    if (!this.habitacionAEliminar) return;
    this.habitacionService.deleteHabitacion(this.habitacionAEliminar.id).subscribe({
      next: () => {
        this.habitaciones = this.habitaciones.filter(h => h.id !== this.habitacionAEliminar.id);
        this.cerrarModalEliminar();
      },
      error: (err: any) => {
        console.error('Error eliminando habitación:', err);
        alert('No se pudo eliminar la habitación. Intenta nuevamente.');
      }
    });
  }

  // Genera la clase CSS para el badge a partir del estado (reemplaza espacios por guiones)
  getStatusClass(status: string | null | undefined): string {
    const s = (status || 'Libre').toString();
    const safe = s.replace(/\s+/g, '-').replace(/[^A-Za-z0-9\-]/g, '');
    return 'status-' + safe;
  }

  // Normaliza el estado para lo que espera el backend (convierte a texto con espacios)
  normalizeEstadoForBackend(status: string | null | undefined): string {
    if (!status) return 'Libre';
    const s = status.toString().trim();

    // Mapear variantes comunes (sin 'Mantenimiento')
    if (/^fuera[\s\-_]?de[\s\-_]?servicio$/i.test(s)) return 'Fuera de Servicio';
    if (/^reservad/i.test(s)) return 'Reservada';
    if (/^ocupad/i.test(s)) return 'Ocupada';
    // Por defecto convertir guiones/underscores a espacios
    return s.replace(/[-_]+/g, ' ').trim();
  }

  get habitacionesFiltradas() {
    let filtradas = this.habitaciones;
    if (this.busquedaNumero.trim()) {
      filtradas = filtradas.filter(h =>
        h.numero?.toString().includes(this.busquedaNumero.trim())
      );
    }
    // El pipe orderByNumero se encargará de ordenar en el HTML
    return filtradas;
  }
}
