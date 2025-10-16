import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { ReservaLite } from '../../shared/models/reserva-lite.model';
import { ReservaDetail } from '../../shared/models/reserva-detail.model';
import { mapApiReservaDetail } from '../adapters/reservas.adapter';

const API_BASE = 'http://localhost:5000/api';

export interface ClienteOption {
  id: string;
  label: string;
  email: string;
}

export interface HabitacionOption {
  id: string;
  numero: string;
  estado: string;
  piso?: number;
  tipoNombre?: string;
  capacidad?: number;
  tarifaBase?: number;
}

export interface HuespedOption {
  id: string;
  nombre: string;
}

export interface CreateReservaPayload {
  cliente_ID: string;
  estado_Reserva: string;
  monto_Total: number;
}

export interface CreateDetallePayload {
  reserva_ID: string;
  habitacion_ID: string;
  huesped_ID: string;
  precio_Total: number;
  cantidad_Huespedes: number;
}

export interface CreateDetallesMultiplesPayload {
  reserva_ID: string;
  habitaciones: {
    habitacion_ID: string;
    fecha_Entrada: string;
    fecha_Salida: string;
    huesped_IDs: string[];
  }[];
}

export interface UpdateReservaPayload {
  cliente_ID?: string;
  fecha_Entrada?: string;
  fecha_Salida?: string;
  estado_Reserva?: string;
  monto_Total?: number;
}

export interface UpdateDetallePayload {
  habitacion_ID?: string;
  huesped_ID?: string;
  precio_Total?: number;
  cantidad_Huespedes?: number;
}

export interface ReservaData {
  id: string;
  cliente_ID: string;
  fecha_Entrada: string;
  fecha_Salida: string;
  estado_Reserva: string;
  monto_Total: number;
}

export interface ApiReservaListItem {
  id: string;
  ID?: string;
  cliente_ID: string;
  cliente_Nombre?: string;
  fecha_Creacion?: string;
  fecha_Entrada?: string; // Viene de DetalleReserva
  fecha_Salida?: string;  // Viene de DetalleReserva
  estado_Reserva: string;
  monto_Total: number;
}

@Injectable({ providedIn: 'root' })
export class NuevaReservaService {
  private http = inject(HttpClient);

  getClientes(): Observable<ClienteOption[]> {
    return this.http.get<any[]>(`${API_BASE}/Cliente`).pipe(
      map(items =>
        items.map(item => ({
          id: item.id ?? item.ID,
          label: item.razon_Social ?? item.razonSocial ?? item.Razon_Social ?? 'Cliente sin nombre',
          email: item.email ?? item.Email ?? ''
        }))
      )
    );
  }

  getHabitaciones(): Observable<HabitacionOption[]> {
    return this.http.get<any[]>(`${API_BASE}/Habitacion`).pipe(
      map(items =>
        items.map(item => ({
          id: item.id ?? item.ID,
          numero: item.numero_Habitacion ?? item.numeroHabitacion ?? item.Numero_Habitacion ?? 'N/D',
          estado: item.estado_Habitacion ?? item.estado ?? item.Estado_Habitacion ?? 'Sin estado',
          piso: item.piso ?? item.Piso ?? 0,
          tipoNombre: item.tipo_Habitacion?.nombre ?? item.tipoNombre ?? item.Tipo_Habitacion?.Nombre ?? '',
          capacidad: item.tipo_Habitacion?.capacidad_Maxima ?? item.capacidad ?? item.Capacidad_Maxima ?? 0,
          tarifaBase: item.tipo_Habitacion?.precio_Base ?? item.tarifaBase ?? item.Precio_Base ?? 0
        }))
      )
    );
  }

  getHuespedes(): Observable<HuespedOption[]> {
    return this.http.get<any[]>(`${API_BASE}/Huesped`).pipe(
      map(items =>
        items.map(item => {
          // Usar Nombre_Completo del backend si existe
          const nombreCompleto = item.nombre_Completo ?? item.Nombre_Completo;
          
          // Si no existe, construirlo manualmente
          const nombreManual = [
            item.nombre ?? item.Nombre,
            item.apellido ?? item.Apellido,
            item.segundo_Apellido ?? item.Segundo_Apellido
          ]
            .filter(Boolean)
            .join(' ') || 'Huésped sin nombre';

          return {
            id: item.id ?? item.ID,
            nombre: nombreCompleto || nombreManual
          };
        })
      )
    );
  }

  getReservasList(): Observable<ReservaLite[]> {
    return this.http.get<ApiReservaListItem[]>(`${API_BASE}/Reserva`).pipe(
      map(items =>
        items.map(item => ({
          id: item.id ?? item.ID ?? '',
          cliente: item.cliente_Nombre ?? 'Cliente no disponible',
          fechaEntrada: item.fecha_Entrada ?? item.fecha_Creacion ?? '', // Fallback a fecha_Creacion
          fechaSalida: item.fecha_Salida ?? item.fecha_Creacion ?? '',
          estado: item.estado_Reserva,
          montoTotal: item.monto_Total
        }))
      )
    );
  }

  createReserva(payload: CreateReservaPayload): Observable<any> {
    return this.http.post(`${API_BASE}/Reserva`, payload);
  }

  createDetalleReserva(payload: CreateDetallePayload): Observable<any> {
    return this.http.post(`${API_BASE}/DetalleReserva`, payload);
  }

  createDetallesMultiples(reservaId: string, habitaciones: any[]): Observable<any> {
    const payload = {
      reserva_ID: reservaId,
      habitaciones: habitaciones
    };
    return this.http.post(`${API_BASE}/DetalleReserva/multiple`, payload);
  }

  updateReserva(id: string, payload: UpdateReservaPayload): Observable<any> {
    return this.http.put(`${API_BASE}/Reserva/${id}`, payload);
  }

  deleteReserva(id: string): Observable<void> {
    console.log('🗑️ Intentando eliminar reserva con ID:', id);
    console.log('🔗 URL:', `${API_BASE}/Reserva/${id}`);
    return this.http.delete<void>(`${API_BASE}/Reserva/${id}`);
  }

  getReservaById(id: string): Observable<ReservaData> {
    return this.http.get<any>(`${API_BASE}/Reserva/${id}`).pipe(
      map(item => ({
        id: item.id ?? item.ID,
        cliente_ID: item.cliente_ID ?? item.Cliente_ID,
        fecha_Entrada: item.fecha_Entrada ?? item.Fecha_Entrada,
        fecha_Salida: item.fecha_Salida ?? item.Fecha_Salida,
        estado_Reserva: item.estado_Reserva ?? item.Estado_Reserva,
        monto_Total: item.monto_Total ?? item.Monto_Total
      }))
    );
  }

  getDetallesByReservaId(reservaId: string): Observable<ReservaDetail[]> {
    return this.http.get<any>(`${API_BASE}/DetalleReserva/reserva/${reservaId}`).pipe(
      map(res => Array.isArray(res) ? res : [res]),
      map(list => list.map(mapApiReservaDetail))
    );
  }

  updateDetalleReserva(id: string, payload: UpdateDetallePayload): Observable<any> {
    return this.http.put(`${API_BASE}/DetalleReserva/${id}`, payload);
  }
}
