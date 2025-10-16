import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { HuespedService } from '../../../core/services/huesped.service';
import { HuespedLite, nombreCompleto } from '../../../shared/models/huesped-lite.model';

@Component({
  selector: 'app-huespedes-list',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterLink],
  templateUrl: './huespedes-list.component.html',
  styleUrls: ['./huespedes-list.component.scss']
})
export class HuespedesListComponent implements OnInit {
  huespedes: HuespedLite[] = [];
  loading = true;
  nombreCompleto = nombreCompleto;

  constructor(
    private huespedService: HuespedService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.huespedService.getHuespedes().subscribe({
      next: (hs: HuespedLite[]) => {
        this.huespedes = hs;
        this.loading = false;
      },
      error: () => {
        this.huespedes = [];
        this.loading = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/inicio']);
  }

  eliminarHuesped(id: string): void {
    const confirmacion = window.confirm('¿Eliminar este huésped? Esta acción no se puede deshacer.');
    if (!confirmacion) return;

    this.huespedService.deleteHuesped(id).subscribe({
      next: () => {
        this.huespedes = this.huespedes.filter(h => h.id !== id);
      },
      error: (err) => {
        console.error('Error eliminando huésped:', err);
        window.alert('No se pudo eliminar el huésped. Intenta nuevamente.');
      }
    });
  }

  // Nuevo: navegar a detalle (ver)
  verHuesped(id: string): void {
    // Asume ruta de detalle: /huespedes/:id
    this.router.navigate(['/huespedes', id]);
  }

  // Nuevo: navegar a editar
  editarHuesped(id: string): void {
    // Asume ruta de edición: /huespedes/editar/:id
    this.router.navigate(['/huespedes/editar', id]);
  }
}
