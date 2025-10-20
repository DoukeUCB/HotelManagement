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

  getClientes(): Observable<ClienteLite[]> {
    return this.http.get<any[]>(`${API_BASE}/Cliente`).pipe(
      map(items =>
        items.map(item => ({
          id: item.id ?? item.ID,
          razonSocial: item.razon_Social ?? item.Razon_Social ?? '',
          nit: item.nit ?? item.NIT ?? '',
          email: item.email ?? item.Email ?? ''
        }))
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

  updateCliente(id: string, payload: ClienteUpdatePayload): Observable<any> {
    return this.http.put(`${API_BASE}/Cliente/${id}`, payload);
  }

  deleteCliente(id: string): Observable<void> {
    return this.http.delete<void>(`${API_BASE}/Cliente/${id}`);
  }
}
