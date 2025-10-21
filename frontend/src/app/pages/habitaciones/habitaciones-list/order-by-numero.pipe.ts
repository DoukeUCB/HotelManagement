import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'orderByNumero', standalone: true })
export class OrderByNumeroPipe implements PipeTransform {
  transform(habitaciones: any[]): any[] {
    if (!habitaciones) return [];
    return habitaciones.slice().sort((a, b) => {
      const numA = Number(a.numero);
      const numB = Number(b.numero);
      return numA - numB;
    });
  }
}
