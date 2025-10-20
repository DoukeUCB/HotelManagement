import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { NuevaReservaService } from '../../../core/services/nueva-reserva.service';
import { Subscription, timer } from 'rxjs';

@Component({
  selector: 'app-habitaciones-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './habitaciones-list.component.html',
  styleUrls: ['./habitaciones-list.component.scss']
})
export class HabitacionesListComponent implements OnInit, OnDestroy {
  private api = inject(NuevaReservaService);
  private router = inject(Router);

  habitaciones: any[] = [];
  loading = true;

  private pollSub: Subscription | null = null;

  ngOnInit(): void {
    this.cargarHabitaciones();

    // Polling cada 15s para reflejar cambios en BD (puedes ajustar intervalo)
    this.pollSub = timer(15000, 15000).subscribe(() => {
      this.cargarHabitaciones();
    });
  }

  ngOnDestroy(): void {
    this.pollSub?.unsubscribe();
  }

  goBack(): void {
    this.router.navigate(['/inicio']);
  }

  cargarHabitaciones(): void {
    this.loading = true;
    this.api.getHabitaciones().subscribe({
      next: (list) => {
        // El servicio ya mapea todo correctamente, solo asignamos
        this.habitaciones = list.map(h => ({
          id: h.id,
          numero: h.numero,
          piso: h.piso ?? 0,
          tipoNombre: h.tipoNombre ?? 'Sin tipo',
          capacidad: h.capacidad ?? 0,
          tarifaBase: h.tarifaBase ?? 0,
          estado: h.estado ?? 'Libre'
        }));
        this.loading = false;
      },
      error: (err) => {
        console.error('Error cargando habitaciones:', err);
        this.habitaciones = [];
        this.loading = false;
      }
    });
  }
}
