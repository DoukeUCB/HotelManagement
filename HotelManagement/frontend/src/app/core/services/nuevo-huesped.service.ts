import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

const API_BASE = 'http://localhost:5000/api';

export interface NuevoHuespedPayload {
  nombre_Completo: string;
  documento_Identidad: string;
  telefono?: string | null;
  email?: string | null;
  fecha_Nacimiento?: string | null;
}

@Injectable({ providedIn: 'root' })
export class NuevoHuespedService {
  private http = inject(HttpClient);

  createHuesped(payload: NuevoHuespedPayload): Observable<any> {
    return this.http.post(`${API_BASE}/Huesped`, payload);
  }
}
