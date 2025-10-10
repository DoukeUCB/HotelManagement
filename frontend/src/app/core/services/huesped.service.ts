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
          primerNombre: '', // No disponible en la API actual
          segundoNombre: '',
          primerApellido: '',
          segundoApellido: '',
          documento: item.documento_Identidad ?? item.Documento_Identidad,
          fechaNacimiento: item.fecha_Nacimiento ?? item.Fecha_Nacimiento,
          nombreCompleto: item.nombre_Completo ?? item.Nombre_Completo
        }))
      )
    );
  }
}
