export interface HuespedLite {
  id: string;
  primerNombre: string;
  segundoNombre?: string;
  primerApellido: string;
  segundoApellido?: string;
  fechaNacimiento: string; // ISO yyyy-MM-dd
  documento: string;
  nombreCompleto?: string; // Campo adicional para cuando viene del API
}

export function nombreCompleto(h: HuespedLite): string {
  // Si viene nombreCompleto del API, usarlo
  if (h.nombreCompleto) return h.nombreCompleto;
  
  // Sino, construirlo con los campos individuales
  return [h.primerNombre, h.segundoNombre, h.primerApellido, h.segundoApellido]
    .filter(Boolean)
    .join(' ');
}
