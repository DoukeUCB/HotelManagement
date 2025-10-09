import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { ApiReservaDetail, ReservaDetail } from '../../shared/models/reserva-detail.model';
import { mapApiReservaDetail } from '../../core/adapters/reservas.adapter';

// ⚠️ Cambia por tu base real de Swagger
const API_BASE = 'http://localhost:5000';
// Ruta que mostraste en Swagger:
const DETALLE_BY_RESERVA = '/api/DetalleReserva';

@Injectable({ providedIn: 'root' })
export class ReservasService {
  private http = inject(HttpClient);

  /**
   * Devuelve los detalles de una reserva por reservaId.
   * Normaliza respuesta: si la API devuelve objeto -> [obj]; si array -> array.
   */
  getDetallesByReservaId(reservaId: string): Observable<ReservaDetail[]> {
    const url = `${API_BASE}${DETALLE_BY_RESERVA}/${reservaId}`;
    return this.http
      .get<ApiReservaDetail | ApiReservaDetail[]>(url)
      .pipe(
        map(res => Array.isArray(res) ? res : [res]),
        map(list => list.map(mapApiReservaDetail))
      );
  }
}
