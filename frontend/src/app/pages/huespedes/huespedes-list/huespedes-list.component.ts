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
}
