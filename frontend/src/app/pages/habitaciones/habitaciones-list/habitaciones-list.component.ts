import { Component, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { MockDataService } from '../../../core/services/mock-data.service';
import { HabitacionLite } from '../../../shared/models/habitacion-lite.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-habitaciones-list',
  standalone: true,
  imports: [CommonModule, DecimalPipe],
  templateUrl: './habitaciones-list.component.html',
  styleUrls: ['./habitaciones-list.component.scss']
})
export class HabitacionesListComponent implements OnInit {
  habitaciones: HabitacionLite[] = [];
  loading = true;

  constructor(private mock: MockDataService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.mock.getHabitaciones().subscribe({
      next: h => { this.habitaciones = h; this.loading = false; },
      error: () => { this.habitaciones = []; this.loading = false; }
    });
  }
  goBack(): void {
    this.router.navigate(['/inicio']); 
  }
}
