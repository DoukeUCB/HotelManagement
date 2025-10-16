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
  reservasPaginadas: ReservaLite[] = [];
  loading = true;

  estadoSeleccionado = '';
  estadosDisponibles: string[] = [];
  
  // Búsqueda
  terminoBusqueda = '';
  
  // Paginación
  paginaActual = 1;
  itemsPorPagina = 10;
  totalPaginas = 1;

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
    let resultado = [...this.todasLasReservas];

    // Filtro por estado
    if (this.estadoSeleccionado) {
      resultado = resultado.filter(r => r.estado === this.estadoSeleccionado);
    }

    // Filtro por búsqueda
    if (this.terminoBusqueda.trim()) {
      const termino = this.terminoBusqueda.toLowerCase();
      resultado = resultado.filter(r => 
        r.cliente.toLowerCase().includes(termino) ||
        r.estado.toLowerCase().includes(termino)
      );
    }

    this.reservas = resultado;
    this.paginaActual = 1;
    this.actualizarPaginacion();
  }

  actualizarPaginacion(): void {
    this.totalPaginas = Math.ceil(this.reservas.length / this.itemsPorPagina);
    const inicio = (this.paginaActual - 1) * this.itemsPorPagina;
    const fin = inicio + this.itemsPorPagina;
    this.reservasPaginadas = this.reservas.slice(inicio, fin);
  }

  cambiarPagina(pagina: number): void {
    if (pagina >= 1 && pagina <= this.totalPaginas) {
      this.paginaActual = pagina;
      this.actualizarPaginacion();
    }
  }

  get paginasArray(): number[] {
    return Array.from({ length: this.totalPaginas }, (_, i) => i + 1);
  }

  calcularDias(fechaEntrada: string, fechaSalida: string): number {
    const entrada = new Date(fechaEntrada);
    const salida = new Date(fechaSalida);
    const diferencia = salida.getTime() - entrada.getTime();
    return Math.ceil(diferencia / (1000 * 60 * 60 * 24));
  }

  editarReserva(id: string): void {
    this.router.navigate(['/editar-reserva'], { queryParams: { id } });
  }

  eliminarReserva(id: string): void {
    if (!confirm('¿Eliminar esta reserva?')) return;
    
    this.api.deleteReserva(id).subscribe({
      next: () => {
        this.cargarReservas();
      },
      error: (err) => {
        alert(`No se pudo eliminar la reserva. Error: ${err.status} - ${err.message}`);
      }
    });
  }

  verReserva(id: string): void {
    this.router.navigate(['/reservas', id]);
  }
  
  goBack(): void {
    this.router.navigate(['/inicio']); 
  }
}