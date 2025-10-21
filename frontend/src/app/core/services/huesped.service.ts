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
          segundoNombre: item.segundo_Nombre ?? item.Segundo_Nombre ?? '',
          primerApellido: item.apellido ?? item.Apellido ?? '',
          segundoApellido: item.segundo_Apellido ?? item.Segundo_Apellido ?? '',
          documento: item.documento_Identidad ?? item.Documento_Identidad ?? '',
          fechaNacimiento: item.fecha_Nacimiento ?? item.Fecha_Nacimiento ?? '',
          telefono: item.telefono ?? item.Telefono ?? '',
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
    // Convierte campos a mayúsculas
    const uppercasePayload = {
      ...payload,
      Nombre: (payload.Nombre || '').toUpperCase(),
      Apellido: (payload.Apellido || '').toUpperCase(),
      Segundo_Apellido: payload.Segundo_Apellido ? payload.Segundo_Apellido.toUpperCase() : null,
      Documento_Identidad: (payload.Documento_Identidad || '').toUpperCase()
    };
    
    return this.http.post(`${API_BASE}/Huesped`, uppercasePayload);
  }

  // Nuevo: elimina un huésped por ID
  deleteHuesped(id: string): Observable<void> {
    return this.http.delete<void>(`${API_BASE}/Huesped/${id}`);
  }

  updateHuesped(huesped: any): Observable<HuespedLite> {
    // Combinar primer nombre y segundo nombre si ambos existen
    const nombreCompleto = huesped.segundoNombre 
      ? `${huesped.primerNombre} ${huesped.segundoNombre}` 
      : huesped.primerNombre;
    
    // Adapta los datos al formato que espera el backend y convierte a mayúsculas
    const payload = {
      ID: huesped.id,
      Nombre: nombreCompleto.toUpperCase(),
      Apellido: (huesped.primerApellido || '').toUpperCase(),
      Segundo_Apellido: (huesped.segundoApellido || '').toUpperCase(),
      Documento_Identidad: (huesped.documento || '').toUpperCase(),
      Telefono: huesped.telefono || '',
      Fecha_Nacimiento: huesped.fechaNacimiento || null
    };

    console.log('Enviando payload al backend:', payload);

    return this.http.put<any>(`${API_BASE}/Huesped/${huesped.id}`, payload).pipe(
      map(response => {
        console.log('Respuesta del backend:', response);
        
        // Al recibir la respuesta, separamos el nombre y segundo nombre
        let primerNombre = response.nombre ?? response.Nombre ?? '';
        let segundoNombre = '';
        
        // Si el nombre contiene espacios, intentamos separar primer y segundo nombre
        if (primerNombre && primerNombre.includes(' ')) {
          const partes = primerNombre.split(' ');
          primerNombre = partes[0];
          segundoNombre = partes.slice(1).join(' ');
        }
        
        return {
          id: response.id ?? response.ID,
          primerNombre: primerNombre,
          segundoNombre: segundoNombre,
          primerApellido: response.apellido ?? response.Apellido ?? '',
          segundoApellido: response.segundo_Apellido ?? response.Segundo_Apellido ?? '',
          documento: response.documento_Identidad ?? response.Documento_Identidad ?? '',
          fechaNacimiento: response.fecha_Nacimiento ?? response.Fecha_Nacimiento ?? '',
          telefono: response.telefono ?? response.Telefono ?? '',
          nombreCompleto: response.nombre_Completo ?? response.Nombre_Completo ?? ''
        };
      })
    );
  }
}
