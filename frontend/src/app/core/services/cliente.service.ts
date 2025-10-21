import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';

const API_BASE = 'http://localhost:5000/api';

export interface ClienteLite {
  id: string;
  razonSocial: string;
  nit: string;
  email: string;
}

export interface ClienteCreatePayload {
  razon_Social: string;
  nit: string;
  email: string;
}

export interface ClienteUpdatePayload {
  razon_Social?: string;
  nit?: string;
  email?: string;
}

@Injectable({ providedIn: 'root' })
export class ClienteService {
  private http = inject(HttpClient);

  getClientes(): Observable<any[]> {
    return this.http.get<any[]>(`${API_BASE}/Cliente`).pipe(
      map(items =>
        items.map(item => {
          // Normalizar NIT/documento en la propiedad 'documento'
          const documento =
            item.documento ?? item.Documento ??
            item.NIT ?? item.nit ??
            item.documento_Identidad ?? item.Documento_Identidad ?? '';

          return {
            id: item.id ?? item.ID,
            razonSocial: item.razon_Social ?? item.Razon_Social ?? item.nombre ?? item.Nombre ?? '',
            documento: documento,
            email: item.email ?? item.Email ?? ''
            // ...mapear otros campos si los necesitas...
          };
        })
      )
    );
  }

  getClienteById(id: string): Observable<ClienteLite> {
    return this.http.get<any>(`${API_BASE}/Cliente/${id}`).pipe(
      map(item => ({
        id: item.id ?? item.ID,
        razonSocial: item.razon_Social ?? item.Razon_Social,
        nit: item.nit ?? item.NIT,
        email: item.email ?? item.Email
      }))
    );
  }

  createCliente(payload: ClienteCreatePayload): Observable<any> {
    return this.http.post(`${API_BASE}/Cliente`, payload);
  }

  updateCliente(cliente: any): Observable<any> {
    const payload = {
      ID: cliente.id,
      Razon_Social: cliente.razonSocial.toUpperCase(),
      Documento: cliente.documento.toUpperCase(), // Este es el NIT
      Email: cliente.email
    };

    console.log('Enviando payload al backend:', payload);

    return this.http.put<any>(`${API_BASE}/Cliente/${cliente.id}`, payload).pipe(
      map(response => {
        console.log('Respuesta del backend:', response);
        return {
          id: response.id || response.ID,
          razonSocial: response.razon_Social || response.Razon_Social || '',
          documento: response.documento || response.Documento || '', // Este es el NIT
          email: response.email || response.Email || ''
        };
      })
    );
  }

  deleteCliente(id: string): Observable<void> {
    return this.http.delete<void>(`${API_BASE}/Cliente/${id}`);
  }
}
