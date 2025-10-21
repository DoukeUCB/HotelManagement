import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { HabitacionLite } from '../../shared/models/habitacion-lite.model';

@Injectable({
  providedIn: 'root'
})
export class HabitacionService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5000/api';

  getHabitaciones(): Observable<HabitacionLite[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Habitacion`).pipe(
      map(response => response.map(h => ({
        id: h.id,
        numero: h.numero_Habitacion ?? h.numero ?? h.numeroHabitacion,
        piso: h.piso,
        tipoNombre: h.tipo_Nombre ?? h.tipoNombre ?? h.tipo_Nombre,
        tipoId: h.tipo_Id ?? h.tipoId ?? h.TipoId ?? null,
        capacidad: h.capacidad_Maxima ?? h.capacidadMaxima ?? h.capacidad,
        tarifaBase: h.precio_Base ?? h.tarifaBase ?? null,
        estado: h.estado_Habitacion ?? h.estadoHabitacion ?? h.estado
      })))
    );
  }

  // Nuevo: obtener tipos de habitación
  getTiposHabitacion(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/TipoHabitacion`);
  }

  updateEstado(id: string, estado: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/Habitacion/${id}/estado`, { estado_Habitacion: estado });
  }

  // Enviar payload arbitrario al endpoint de estado (devuelve HttpResponse)
  updateEstadoKey(id: string, payload: any) {
    return this.http.put<any>(`${this.apiUrl}/Habitacion/${id}/estado`, payload, { observe: 'response' });
  }

  updateHabitacion(habitacion: any): Observable<HttpResponse<any>> {
    const id = habitacion.id ?? habitacion.ID;
    const payload = {
      ID: id,
      numero_Habitacion: habitacion.numero_Habitacion ?? habitacion.numero ?? null,
      piso: habitacion.piso ?? null,
      tipo_Id: habitacion.tipo_Id ?? habitacion.tipoId ?? null,
      Tipo_Habitacion_ID: habitacion.Tipo_Habitacion_ID ?? habitacion.tipo_Id ?? habitacion.tipoId ?? null,
      tipo_Nombre: habitacion.tipo_Nombre ?? habitacion.tipoNombre ?? '',
      capacidad_Maxima: habitacion.capacidad_Maxima ?? habitacion.capacidad ?? null,
      estado_Habitacion: habitacion.estado_Habitacion ?? habitacion.estado ?? 'Libre'
    };
    // Retornamos la respuesta completa para depuración
    return this.http.put<any>(`${this.apiUrl}/Habitacion/${id}`, payload, { observe: 'response' });
  }

  deleteHabitacion(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Habitacion/${id}`);
  }

  // Nuevo: crear habitación con el formato que espera el backend
  createHabitacion(habitacion: any): Observable<any> {
    const payload = {
      numero_Habitacion: habitacion.numero ?? null,
      piso: habitacion.piso ?? null,
      Tipo_Habitacion_ID: habitacion.tipoHabitacionId ?? habitacion.tipo_Id ?? null,
      capacidad_Maxima: habitacion.capacidad ?? null,
      estado_Habitacion: habitacion.estado_Habitacion ?? habitacion.estado ?? 'Libre'
    };
    return this.http.post<any>(`${this.apiUrl}/Habitacion`, payload);
  }
}
