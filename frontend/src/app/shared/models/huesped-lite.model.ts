export interface HuespedLite {
  id: string;
  primerNombre: string;
  segundoNombre?: string;
  primerApellido: string;
  segundoApellido?: string;
  fechaNacimiento: string; // ISO yyyy-MM-dd
  documento: string;
}

export function nombreCompleto(h: HuespedLite): string {
  return [h.primerNombre, h.segundoNombre, h.primerApellido, h.segundoApellido]
    .filter(Boolean)
    .join(' ');
}
