// Modelo simple para listar habitaciones en la UI
export type EstadoHabitacion =
  | 'Libre'
  | 'Reservada'
  | 'Ocupada'
  | 'Mantenimiento'
  | 'Fuera de Servicio';

export interface HabitacionLite {
  id: string;          // UUID en string (BIN_TO_UUID en el backend)
  numero: string;      // ej. "101"
  piso: number;        // ej. 1, 2, 3...
  tipoNombre: string;  // ej. "Simple", "Doble", "Suite"
  capacidad: number;   // cantidad de huéspedes
  tarifaBase: number;  // precio base (Bs.)
  estado: EstadoHabitacion;
}
