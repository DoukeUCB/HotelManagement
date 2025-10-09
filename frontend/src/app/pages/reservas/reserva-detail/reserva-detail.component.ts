import { Component, inject, signal, computed } from '@angular/core';
import { NgIf, NgFor, DatePipe, DecimalPipe, CurrencyPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { catchError, finalize, of, tap } from 'rxjs';
import { ReservasService } from '../../../core/services/reserva.service'; // <-- ojo al nombre/ubicación
import { ReservaDetail } from '../../../shared/models/reserva-detail.model';

@Component({
  selector: 'app-reserva-detail',
  standalone: true,
  imports: [NgIf, NgFor, RouterLink, DatePipe, DecimalPipe, CurrencyPipe],
  templateUrl: './reserva-detail.component.html',
  styleUrls: ['./reserva-detail.component.scss']
})
export default class ReservaDetailComponent {
  private route = inject(ActivatedRoute);
  private api = inject(ReservasService);

  loading = signal(true);
  error = signal<string | null>(null);
  detalles = signal<ReservaDetail[]>([]); // lista de detalles

  // Primer ítem para cabecera
  header = computed(() => this.detalles()[0] ?? null);
  // Total sumando los precios de cada item (por si la API devuelve varios)
  total = computed(() => this.detalles().reduce((acc, d) => acc + (d.precioTotal ?? 0), 0));

  constructor() {
    const reservaId = this.route.snapshot.paramMap.get('id') ?? '';
    if (!reservaId) {
      this.error.set('ID de reserva no proporcionado');
      this.loading.set(false);
      return;
    }

    this.api.getDetallesByReservaId(reservaId)
      .pipe(
        tap(() => console.log('[Detalle] llamando API con id:', reservaId)),
        catchError(err => {
          console.error('[Detalle] error API:', err);
          this.error.set('No se pudo cargar el detalle de la reserva.');
          this.detalles.set([]);
          return of([] as ReservaDetail[]);
        }),
        finalize(() => this.loading.set(false))
      )
      .subscribe(list => {
        console.log('[Detalle] respuesta normalizada:', list);
        this.detalles.set(list);
      });
  }
}
