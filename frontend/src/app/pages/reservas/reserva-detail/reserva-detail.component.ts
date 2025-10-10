import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NuevaReservaService } from '../../../core/services/nueva-reserva.service';
import { ReservaDetail } from '../../../shared/models/reserva-detail.model';

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
  reservaData = signal<{ fechaEntrada: string; fechaSalida: string } | null>(null);

  header = computed<ReservaHeader | null>(() => {
    const lista = this.detalles();
    const reserva = this.reservaData();
    if (!lista.length || !reserva) return null;

    const primero = lista[0];
    return {
      reservaId: primero.reservaId,
      nombreHuesped: primero.nombreHuesped,
      numeroHabitacion: primero.numeroHabitacion,
      cantidadHuespedes: lista.reduce((sum, d) => sum + d.cantidadHuespedes, 0),
      fechaEntrada: reserva.fechaEntrada,
      fechaSalida: reserva.fechaSalida
    };
  });

  total = computed(() => this.detalles().reduce((sum, d) => sum + d.precioTotal, 0));

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error.set('ID de reserva no proporcionado');
      this.loading.set(false);
      return;
    }

    this.api.getReservaById(id).subscribe({
      next: (reserva) => {
        this.reservaData.set({
          fechaEntrada: reserva.fecha_Entrada,
          fechaSalida: reserva.fecha_Salida
        });
        this.cargarDetalles(id);
      },
      error: () => {
        this.error.set('No se pudo cargar la reserva');
        this.loading.set(false);
      }
    });
  }

  private cargarDetalles(id: string): void {
    this.api.getDetallesByReservaId(id).subscribe({
      next: (lista) => {
        this.detalles.set(lista);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar los detalles de la reserva');
        this.loading.set(false);
      }
    });
  }
}
