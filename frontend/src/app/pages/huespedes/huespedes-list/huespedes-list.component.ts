import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MockDataService } from '../../../core/services/mock-data.service';
import { HuespedLite, nombreCompleto } from '../../../shared/models/huesped-lite.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-huespedes-list',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './huespedes-list.component.html',
  styleUrls: ['./huespedes-list.component.scss']
})
export class HuespedesListComponent implements OnInit {
  huespedes: HuespedLite[] = [];
  loading = true;
  nombreCompleto = nombreCompleto;

  constructor(private mock: MockDataService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.mock.getHuespedes().subscribe({
      next: (hs: HuespedLite[]) => {          // 👈 tipado explícito
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
}
