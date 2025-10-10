import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ClienteService, ClienteLite } from '../../../core/services/cliente.service';

@Component({
  selector: 'app-clientes-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './clientes-list.component.html',
  styleUrls: ['./clientes-list.component.scss']
})
export class ClientesListComponent implements OnInit {
  clientes: ClienteLite[] = [];
  loading = true;

  constructor(
    private clienteService: ClienteService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargarClientes();
  }

  cargarClientes(): void {
    this.loading = true;
    this.clienteService.getClientes().subscribe({
      next: (lista) => {
        this.clientes = lista;
        this.loading = false;
      },
      error: () => {
        this.clientes = [];
        this.loading = false;
      }
    });
  }

  editarCliente(id: string): void {
    this.router.navigate(['/editar-cliente'], { queryParams: { id } });
  }

  eliminarCliente(id: string): void {
    if (!confirm('¿Eliminar este cliente?')) return;
    this.clienteService.deleteCliente(id).subscribe({
      next: () => this.cargarClientes(),
      error: () => alert('No se pudo eliminar el cliente.')
    });
  }

  goBack(): void {
    this.router.navigate(['/inicio']);
  }
}
