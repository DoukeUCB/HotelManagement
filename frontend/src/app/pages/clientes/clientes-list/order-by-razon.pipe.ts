import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'orderByRazon', standalone: true })
export class OrderByRazonPipe implements PipeTransform {
  transform(items: any[]): any[] {
    if (!items) return [];
    return items.slice().sort((a, b) => {
      const aName = (a?.razonSocial ?? a?.Razon_Social ?? a?.nombre ?? '').toString().toLowerCase();
      const bName = (b?.razonSocial ?? b?.Razon_Social ?? b?.nombre ?? '').toString().toLowerCase();
      return aName.localeCompare(bName);
    });
  }
}
