import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
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
        numero: h.numero_Habitacion,
        piso: h.piso,
        tipoNombre: h.tipo_Nombre,
        capacidad: h.capacidad_Maxima,
        tarifaBase: h.precio_Base,
        estado: h.estado_Habitacion
      })))
    );
  }

  updateEstado(id: string, estado: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/Habitacion/${id}/estado`, { estado_Habitacion: estado });
  }
}
