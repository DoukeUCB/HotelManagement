import { Pipe, PipeTransform } from '@angular/core';
import { HuespedLite, nombreCompleto } from '../../../shared/models/huesped-lite.model';

@Pipe({ name: 'orderByNombre', standalone: true })
export class OrderByNombrePipe implements PipeTransform {
  transform(huespedes: HuespedLite[]): HuespedLite[] {
    if (!huespedes) return [];
    return huespedes.slice().sort((a, b) => {
      const nombreA = nombreCompleto(a).toLowerCase();
      const nombreB = nombreCompleto(b).toLowerCase();
      return nombreA.localeCompare(nombreB);
    });
  }
}
