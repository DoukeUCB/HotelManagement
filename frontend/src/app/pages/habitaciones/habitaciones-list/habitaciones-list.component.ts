import { Component, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import { HabitacionService } from '../../../core/services/habitacion.service';
import { HabitacionLite, EstadoHabitacion } from '../../../shared/models/habitacion-lite.model';
import { FormsModule } from '@angular/forms';
import { MockDataService } from '../../../core/services/mock-data.service';

@Component({
  selector: 'app-habitaciones-list',
  templateUrl: './habitaciones-list.component.html',
  styleUrls: ['./habitaciones-list.component.scss'],
  standalone: true,
  imports: [CommonModule, DecimalPipe, FormsModule]
})
export class HabitacionesListComponent implements OnInit {
  loading = true;
  habitaciones: HabitacionLite[] = [];

  estadosDisponibles: EstadoHabitacion[] = [
    'Libre',
    'Reservada',
    'Ocupada',
    'Mantenimiento',
    'Fuera de Servicio'
  ];

  constructor(
    private mockService: MockDataService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargarHabitaciones();
  }

  cargarHabitaciones(): void {
    this.loading = true;
    this.mockService.getHabitaciones()
      .subscribe({
        next: (data) => {
          this.habitaciones = data;
          this.loading = false;
        },
        error: (error) => {
          console.error('Error cargando habitaciones:', error);
          this.loading = false;
        }
      });
  }

  goBack(): void {
    this.router.navigate(['/']);
  }

  cambiarEstado(id: string, nuevoEstado: EstadoHabitacion): void {
    const habitacion = this.habitaciones.find(h => h.id === id);
    if (habitacion) {
      habitacion.estado = nuevoEstado;
    }
  }
}
