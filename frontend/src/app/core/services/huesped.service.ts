import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { HuespedLite } from '../../shared/models/huesped-lite.model';

const API_BASE = 'http://localhost:5000/api';

@Injectable({ providedIn: 'root' })
export class HuespedService {
  private http = inject(HttpClient);

  getHuespedes(): Observable<HuespedLite[]> {
    return this.http.get<any[]>(`${API_BASE}/Huesped`).pipe(
      map(items =>
        items.map(item => ({
          id: item.id ?? item.ID,
          primerNombre: item.nombre ?? item.Nombre ?? '',
          segundoNombre: '',
          primerApellido: item.apellido ?? item.Apellido ?? '',
          segundoApellido: item.segundo_Apellido ?? item.Segundo_Apellido ?? '',
          documento: item.documento_Identidad ?? item.Documento_Identidad ?? '',
          fechaNacimiento: item.fecha_Nacimiento ?? item.Fecha_Nacimiento ?? '',
          nombreCompleto: item.nombre_Completo ?? item.Nombre_Completo ?? ''
        }))
      )
    );
  }

  // Agregar: crea un nuevo huésped en el backend
  createHuesped(payload: {
    Nombre: string;
    Apellido: string;
    Segundo_Apellido?: string | null;
    Documento_Identidad: string;
    Telefono?: string | null;
    Fecha_Nacimiento?: string | null;
  }): Observable<any> {
    return this.http.post(`${API_BASE}/Huesped`, payload);
  }

  // Nuevo: elimina un huésped por ID
  deleteHuesped(id: string): Observable<void> {
    return this.http.delete<void>(`${API_BASE}/Huesped/${id}`);
  }
}
