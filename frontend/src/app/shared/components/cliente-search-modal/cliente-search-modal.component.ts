import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClienteOption } from '../../../core/services/nueva-reserva.service';

@Component({
  selector: 'app-cliente-search-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="modal-overlay" (click)="close()">
      <div class="modal-content" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h3>Buscar Cliente</h3>
          <button class="close-btn" (click)="close()">&times;</button>
        </div>
        <div class="modal-body">
          <input
            type="text"
            [(ngModel)]="searchTerm"
            (ngModelChange)="search()"
            placeholder="Buscar por razón social o NIT..."
            class="search-input"
          />
          <div class="results-list">
            <div
              *ngFor="let cliente of filteredClientes"
              class="result-item"
              (click)="selectCliente(cliente)"
            >
              <strong>{{ cliente.razon_Social }}</strong>
              <small>NIT: {{ cliente.nit || 'No disponible' }}</small>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0,0,0,0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
    }
    .modal-content {
      background: white;
      border-radius: 8px;
      width: 90%;
      max-width: 500px;
      max-height: 90vh;
      display: flex;
      flex-direction: column;
    }
    .modal-header {
      padding: 16px;
      border-bottom: 1px solid #e2e8f0;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    .modal-body {
      padding: 16px;
      overflow-y: auto;
    }
    .search-input {
      width: 100%;
      padding: 8px;
      border: 1px solid #e2e8f0;
      border-radius: 4px;
      margin-bottom: 16px;
    }
    .result-item {
      padding: 12px;
      border: 1px solid #e2e8f0;
      border-radius: 4px;
      margin-bottom: 8px;
      cursor: pointer;
      display: flex;
      flex-direction: column;
    }
    .result-item:hover {
      background: #f8fafc;
    }
  `]
})
export class ClienteSearchModalComponent {
  @Input() clientes: ClienteOption[] = [];
  @Input() visible = false;
  @Output() clienteSelected = new EventEmitter<ClienteOption>();
  @Output() visibleChange = new EventEmitter<boolean>();

  searchTerm = '';
  filteredClientes: ClienteOption[] = [];

  search() {
    const term = this.searchTerm.toLowerCase();
    this.filteredClientes = this.clientes.filter(c => 
      c.razon_Social.toLowerCase().includes(term) || 
      (c.nit && c.nit.toLowerCase().includes(term))
    );
  }

  selectCliente(cliente: ClienteOption) {
    this.clienteSelected.emit(cliente);
    this.close();
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(false);
  }
}
