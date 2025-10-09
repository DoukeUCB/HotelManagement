import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule], // 👈 Directivas de reactive forms
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  // 👇 este símbolo debe existir si el template lo referencia
  searchCtrl = new FormControl<string>('');

  tiles = [
    { title: 'Reservas', link: '/reservas', desc: 'Crear/editar/listar' },
    { title: 'Habitaciones', link: '/habitaciones', desc: 'Estados y tarifas' },
    { title: 'Huéspedes', link: '/huespedes', desc: 'Perfiles y estancias' },
  ];

  onSearch() {
    console.log('Buscar:', this.searchCtrl.value);
  }
}
