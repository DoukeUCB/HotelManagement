import { Component, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import { HabitacionService } from '../../../core/services/habitacion.service';
import { HabitacionLite } from '../../../shared/models/habitacion-lite.model';

@Component({
  selector: 'app-habitaciones-list',
  templateUrl: './habitaciones-list.component.html',
  styleUrls: ['./habitaciones-list.component.scss'],
  standalone: true,
  imports: [CommonModule, DecimalPipe]
})
export class HabitacionesListComponent implements OnInit {
  loading = true;
  habitaciones: HabitacionLite[] = [];

  constructor(
    private habitacionService: HabitacionService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargarHabitaciones();
  }

  cargarHabitaciones(): void {
    this.loading = true;
    this.habitacionService.getHabitaciones()
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
}
