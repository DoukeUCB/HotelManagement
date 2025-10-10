import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ReservaLite } from '../../../shared/models/reserva-lite.model';
import { NuevaReservaService } from '../../../core/services/nueva-reserva.service';

@Component({
  selector: 'app-reservas-list',
  standalone: true,
  imports: [CommonModule, DatePipe, DecimalPipe, FormsModule, RouterLink],
  templateUrl: './reservas-list.component.html',
  styleUrls: ['./reservas-list.component.scss']
})
export class ReservasListComponent implements OnInit {
  todasLasReservas: ReservaLite[] = [];
  reservas: ReservaLite[] = [];
  loading = true;

  estadoSeleccionado = '';
  estadosDisponibles: string[] = [];

  constructor(
    private api: NuevaReservaService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargarReservas();
  }

  cargarReservas(): void {
    this.loading = true;
    this.api.getReservasList().subscribe({
      next: r => {
        this.todasLasReservas = r;
        this.actualizarEstadosDisponibles();
        this.filtrarReservas();
        this.loading = false;
      },
      error: () => {
        this.todasLasReservas = [];
        this.reservas = [];
        this.estadosDisponibles = [];
        this.estadoSeleccionado = '';
        this.loading = false;
      }
    });
  }

  private actualizarEstadosDisponibles(): void {
    this.estadosDisponibles = Array.from(
      new Set(this.todasLasReservas.map(r => r.estado))
    );
    if (!this.estadosDisponibles.includes(this.estadoSeleccionado)) {
      this.estadoSeleccionado = '';
    }
  }

  filtrarReservas(): void {
    if (!this.estadoSeleccionado) {
      this.reservas = [...this.todasLasReservas];
    } else {
      this.reservas = this.todasLasReservas.filter(
        r => r.estado === this.estadoSeleccionado
      );
    }
  }

  editarReserva(id: string): void {
    this.router.navigate(['/editar-reserva'], { queryParams: { id } });
  }

  eliminarReserva(id: string): void {
    if (!confirm('¿Eliminar esta reserva?')) return;
    this.api.deleteReserva(id).subscribe({
      next: () => this.cargarReservas(),
      error: () => alert('No se pudo eliminar la reserva. Intenta nuevamente.')
    });
  }

  verReserva(id: string): void {
    this.router.navigate(['/reservas', id]);
  }
  
  goBack(): void {
    this.router.navigate(['/inicio']); 
  }
}