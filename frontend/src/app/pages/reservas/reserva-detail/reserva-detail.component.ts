import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NuevaReservaService } from '../../../core/services/nueva-reserva.service';
import { ReservaDetail } from '../../../shared/models/reserva-detail.model';
import { forkJoin } from 'rxjs';

interface ReservaHeader {
  reservaId: string;
  nombreHuesped: string;
  numeroHabitacion: string;
  cantidadHuespedes: number;
  fechaEntrada: string;
  fechaSalida: string;
}

@Component({
  selector: 'app-reserva-detail',
  standalone: true,
  imports: [CommonModule, DatePipe, DecimalPipe, RouterLink],
  templateUrl: './reserva-detail.component.html',
  styleUrls: ['./reserva-detail.component.scss']
})
export default class ReservaDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private api = inject(NuevaReservaService);

  loading = signal(true);
  error = signal<string | null>(null);
  detalles = signal<ReservaDetail[]>([]);
  montoTotal = signal<number>(0);

  header = computed<ReservaHeader | null>(() => {
    const lista = this.detalles();
    if (!lista.length) return null;

    const primero = lista[0];
    return {
      reservaId: primero.reservaId,
      nombreHuesped: primero.nombreHuesped,
      numeroHabitacion: primero.numeroHabitacion,
      cantidadHuespedes: lista.length,
      fechaEntrada: primero.fechaEntrada,
      fechaSalida: primero.fechaSalida
    };
  });

  total = computed(() => this.montoTotal());

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error.set('ID de reserva no proporcionado');
      this.loading.set(false);
      return;
    }

    console.log('🔍 Cargando reserva con ID:', id);

    forkJoin({
      reserva: this.api.getReservaById(id),
      detalles: this.api.getDetallesByReservaId(id)
    }).subscribe({
      next: ({ reserva, detalles }) => {
        console.log('✅ Reserva cargada:', reserva);
        console.log('✅ Detalles cargados:', detalles);
        
        this.montoTotal.set(reserva.monto_Total);
        this.detalles.set(detalles);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('❌ Error cargando reserva:', err);
        this.error.set('No se pudo cargar la información de la reserva');
        this.loading.set(false);
      }
    });
  }
}
