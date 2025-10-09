import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { ReservaLite } from '../../shared/models/reserva-lite.model';

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
}

export interface HuespedOption {
  id: string;
  nombre: string;
}

export interface CreateReservaPayload {
  cliente_ID: string;
  fecha_Reserva: string;
  fecha_Entrada: string;
  fecha_Salida: string;
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

export interface ApiReservaListItem {
  id: string;
  ID?: string;
  cliente_ID: string;
  cliente_Nombre?: string;
  fecha_Reserva?: string;
  fecha_Entrada: string;
  fecha_Salida: string;
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
          estado: item.estado_Habitacion ?? item.estado ?? item.Estado_Habitacion ?? 'Sin estado'
        }))
      )
    );
  }

  getHuespedes(): Observable<HuespedOption[]> {
    return this.http.get<any[]>(`${API_BASE}/Huesped`).pipe(
      map(items =>
        items.map(item => {
          const nombreCompuesto =
            [item.primerNombre, item.segundoNombre, item.primerApellido, item.segundoApellido]
              .filter(Boolean)
              .join(' ') || 'Huésped sin nombre';

          return {
            id: item.id ?? item.ID,
            nombre: item.nombre_Completo ?? item.nombreCompleto ?? nombreCompuesto
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
          fechaEntrada: item.fecha_Entrada,
          fechaSalida: item.fecha_Salida,
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
}
