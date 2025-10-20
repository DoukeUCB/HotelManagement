export interface ReservaLite {
  id: string;
  cliente: string;
  fechaEntrada: string; // ISO yyyy-MM-dd
  fechaSalida: string;
  estado: string;
  montoTotal: number;
}
