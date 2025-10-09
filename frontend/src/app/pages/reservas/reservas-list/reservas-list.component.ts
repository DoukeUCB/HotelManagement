import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DatePipe, DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import { MockDataService } from '../../../core/services/mock-data.service';
import { ReservaLite } from '../../../shared/models/reserva-lite.model';

@Component({
  selector: 'app-reservas-list',
  standalone: true,
  imports: [CommonModule, DatePipe, DecimalPipe],
  templateUrl: './reservas-list.component.html',
  styleUrls: ['./reservas-list.component.scss']
})
export class ReservasListComponent implements OnInit {
  reservas: ReservaLite[] = [];
  loading = true;

  constructor(
    private mock: MockDataService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.mock.getReservas().subscribe({
      next: r => { this.reservas = r; this.loading = false; },
      error: () => { this.reservas = []; this.loading = false; }
    });
  }

  // Al hacer click, vamos al detalle -> ahí se consulta en Swagger
  verReserva(id: string): void {
    this.router.navigate(['/reservas', id]);
  }
}
