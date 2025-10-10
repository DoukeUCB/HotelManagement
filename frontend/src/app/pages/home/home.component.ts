import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, FooterComponent], // 👈 habilita routerLink
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
