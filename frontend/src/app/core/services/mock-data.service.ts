import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ReservaLite } from '../../shared/models/reserva-lite.model';
import { HabitacionLite } from '../../shared/models/habitacion-lite.model';
import { HuespedLite } from '../../shared/models/huesped-lite.model';

@Injectable({ providedIn: 'root' })
export class MockDataService {
  constructor(private http: HttpClient) {}
  getReservas(): Observable<ReservaLite[]> {
    return this.http.get<ReservaLite[]>('assets/data/reservas.json');
  }

    getHabitaciones(): Observable<HabitacionLite[]> {
    return this.http.get<HabitacionLite[]>('assets/data/habitaciones.json');
  }

  getHuespedes() {
  return this.http.get<HuespedLite[]>('assets/data/huespedes.json');
  }
}
