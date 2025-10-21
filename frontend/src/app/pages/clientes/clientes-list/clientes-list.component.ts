import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { ClienteService } from '../../../core/services/cliente.service';
import { OrderByRazonPipe } from './order-by-razon.pipe';

@Component({
  selector: 'app-clientes-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, OrderByRazonPipe],
  templateUrl: './clientes-list.component.html',
  styleUrls: ['./clientes-list.component.scss']
})
export class ClientesListComponent implements OnInit {
  clientes: any[] = [];
  loading = true;
  busqueda: string = '';
  
  // Modal de edición
  modalEditarAbierto = false;
  clienteAEditar: any = null;
  
  // Modal de eliminación
  modalEliminarAbierto = false;
  clienteAEliminar: any = null;

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
      next: (data) => {
        this.clientes = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error al cargar clientes:', err);
        this.loading = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/inicio']);
  }
  
  // Filtrado de clientes por búsqueda
  get clientesFiltrados() {
    const filtro = this.busqueda?.trim().toLowerCase() || '';
    if (!filtro) return this.clientes;
    
    return this.clientes.filter(c => 
      c.razonSocial?.toLowerCase().includes(filtro) ||
      c.documento?.toLowerCase().includes(filtro) ||
      c.email?.toLowerCase().includes(filtro)
    );
  }

  // Métodos para el modal de edición
  abrirModalEditar(cliente: any) {
    console.log('Cliente original a editar:', cliente);
    
    // Clona el objeto para no modificar el original
    this.clienteAEditar = {
      id: cliente.id,
      razonSocial: cliente.razonSocial || cliente.razon_Social || cliente.Razon_Social || '',
      documento: cliente.documento || cliente.Documento || cliente.NIT || cliente.nit || '',
      email: cliente.email || cliente.Email || '',
      telefono: cliente.telefono || cliente.Telefono || '',
      direccion: cliente.direccion || cliente.Direccion || ''
    };
    
    console.log('Datos preparados para edición:', this.clienteAEditar);
    
    this.modalEditarAbierto = true;
  }

  cerrarModalEditar() {
    this.modalEditarAbierto = false;
    this.clienteAEditar = null;
  }

  guardarEdicion() {
    if (!this.clienteAEditar) return;
    
    // Convertir campos a mayúsculas
    const clienteActualizado = {
      ...this.clienteAEditar,
      razonSocial: (this.clienteAEditar.razonSocial || '').toUpperCase(),
      documento: (this.clienteAEditar.documento || '').toUpperCase(),
      direccion: (this.clienteAEditar.direccion || '').toUpperCase()
    };
    
    console.log('Guardando cliente:', clienteActualizado);
    
    this.clienteService.updateCliente(clienteActualizado).subscribe({
      next: (response) => {
        console.log('Cliente actualizado:', response);
        
        // Actualizar la lista local
        this.clientes = this.clientes.map(c => 
          c.id === clienteActualizado.id ? clienteActualizado : c
        );
        
        this.cerrarModalEditar();
      },
      error: (err) => {
        console.error('Error al actualizar cliente:', err);
        alert('No se pudo actualizar el cliente. Intenta nuevamente.');
      }
    });
  }

  // Métodos para el modal de eliminar
  abrirModalEliminar(cliente: any) {
    this.clienteAEliminar = cliente;
    this.modalEliminarAbierto = true;
  }

  cerrarModalEliminar() {
    this.modalEliminarAbierto = false;
    this.clienteAEliminar = null;
  }

  confirmarEliminar() {
    if (!this.clienteAEliminar) return;
    
    this.clienteService.deleteCliente(this.clienteAEliminar.id).subscribe({
      next: () => {
        // Eliminar de la lista local
        this.clientes = this.clientes.filter(c => c.id !== this.clienteAEliminar.id);
        this.cerrarModalEliminar();
      },
      error: (err) => {
        console.error('Error al eliminar cliente:', err);
        alert('No se pudo eliminar el cliente. Intenta nuevamente.');
      }
    });
  }
}
