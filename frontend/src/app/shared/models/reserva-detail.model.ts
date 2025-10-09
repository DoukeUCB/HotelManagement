// Lo que devuelve tu API (tal cual)
export interface ApiReservaDetail {
  id: string;
  reserva_ID: string;
  habitacion_ID: string;
  huesped_ID: string;
  precio_Total: number;
  cantidad_Huespedes: number;
  numero_Habitacion: string;
  nombre_Huesped: string;      // viene mal codificado en tu ejemplo (ver nota)
  fecha_Entrada: string;       // "2025-12-15T00:00:00"
  fecha_Salida: string;        // "2025-12-20T00:00:00"
}

// Lo que usaremos en la UI (camelCase + tipos)
export interface ReservaDetail {
  id: string;
  reservaId: string;
  habitacionId: string;
  huespedId: string;
  precioTotal: number;
  cantidadHuespedes: number;
  numeroHabitacion: string;
  nombreHuesped: string;
  fechaEntrada: string; // ISO string
  fechaSalida: string;  // ISO string
}
