import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common'; // Mantener imports
import { Router } from '@angular/router'; // Mantener imports
import { MockDataService } from '../../../core/services/mock-data.service';
import { ReservaLite } from '../../../shared/models/reserva-lite.model';
import { FormsModule } from '@angular/forms'; // 👈 ¡IMPORTANTE! Necesitas FormsModule para [(ngModel)]

@Component({
  selector: 'app-reservas-list',
  standalone: true,
  // 👈 Añadir FormsModule para el two-way binding del filtro
  imports: [CommonModule, DatePipe, DecimalPipe, FormsModule], 
  templateUrl: './reservas-list.component.html',
  styleUrls: ['./reservas-list.component.scss']
})
export class ReservasListComponent implements OnInit {
  
  // Lista de todas las reservas (original sin filtrar)
  todasLasReservas: ReservaLite[] = []; 
  // Lista que se muestra en la tabla (filtrada)
  reservas: ReservaLite[] = []; 
  loading = true;

  // Variables para el filtro (NUEVAS)
  estadoSeleccionado: string = ''; // Valor por defecto: Sin filtro (TODOS)
  // Definición de estados disponibles (debería coincidir con los datos de tu Mock)
  estadosDisponibles: string[] = ['Pendiente', 'Confirmada', 'Cancelada']; 

  constructor(
    private mock: MockDataService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargarReservas();
  }

  // Método unificado para cargar y manejar las reservas
  cargarReservas(): void {
    this.loading = true;
    this.mock.getReservas().subscribe({
      next: r => { 
        this.todasLasReservas = r; // Guardamos la lista completa
        this.filtrarReservas(); // Filtramos la lista inicial (mostrará todas)
        this.loading = false; 
      },
      error: () => { 
        this.reservas = []; 
        this.todasLasReservas = [];
        this.loading = false; 
      }
    });
  }

  // NUEVO MÉTODO: Aplica el filtro a la lista de reservas
  filtrarReservas(): void {
    if (!this.estadoSeleccionado) {
      // Si el filtro es 'TODOS' o vacío, muestra todas las reservas
      this.reservas = [...this.todasLasReservas];
    } else {
      // Filtra las reservas basándose en el estado seleccionado
      this.reservas = this.todasLasReservas.filter(
        r => r.estado === this.estadoSeleccionado
      );
    }
  }

  // Al hacer click, vamos al detalle
  verReserva(id: string): void {
    this.router.navigate(['/reservas', id]);
  }
  
  // Navegación al inicio
  goBack(): void {
    this.router.navigate(['/inicio']); 
  }
}