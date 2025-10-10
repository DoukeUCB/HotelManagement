import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ClienteService } from '../../core/services/cliente.service';

@Component({
  selector: 'app-editar-cliente',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './editar-cliente.component.html',
  styleUrls: ['./editar-cliente.component.scss']
})
export class EditarClienteComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private api = inject(ClienteService);

  submitting = signal(false);
  mensaje = signal<string | null>(null);
  error = signal<string | null>(null);
  cargando = signal(false);

  clienteId = '';

  form = this.fb.nonNullable.group({
    razonSocial: ['', [Validators.required, Validators.maxLength(20)]],
    nit: ['', [Validators.required, Validators.maxLength(20)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(30)]]
  });

  ngOnInit(): void {
    this.clienteId = this.route.snapshot.queryParamMap.get('id') ?? '';
    if (!this.clienteId) {
      this.error.set('No se proporcionó ID de cliente.');
      return;
    }
    this.cargarCliente();
  }

  private cargarCliente(): void {
    this.cargando.set(true);
    this.api.getClienteById(this.clienteId).subscribe({
      next: (cliente) => {
        this.form.patchValue({
          razonSocial: cliente.razonSocial,
          nit: cliente.nit,
          email: cliente.email
        });
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudo cargar el cliente.');
        this.cargando.set(false);
      }
    });
  }

  guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.mensaje.set(null);
    this.error.set(null);

    const value = this.form.getRawValue();

    this.api.updateCliente(this.clienteId, {
      razon_Social: value.razonSocial,
      nit: value.nit,
      email: value.email
    }).subscribe({
      next: () => {
        this.mensaje.set('Cliente actualizado correctamente.');
        setTimeout(() => this.router.navigate(['/clientes']), 1500);
        this.submitting.set(false);
      },
      error: () => {
        this.error.set('No se pudo actualizar el cliente.');
        this.submitting.set(false);
      }
    });
  }

  cancelar(): void {
    this.router.navigate(['/clientes']);
  }
}
