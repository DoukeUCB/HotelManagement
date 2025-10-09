import { ApiReservaDetail, ReservaDetail } from '../../shared/models/reserva-detail.model';

/**
 * Corrige mojibake común cuando UTF-8 se interpretó como CP1252/Latin1 y viceversa.
 * Casos observados: "Mar├¡a", "P├®rez", "Garc├¡a", etc.
 */
export function decodeMojibake(input: string | null | undefined): string {
  if (!input) return '';

  // Reemplazos más típicos que aparecen en estas situaciones
  const replacements: Array<[RegExp, string]> = [
    // Secuencias con "├" y "┬"
    [/├¡/g, 'í'],
    [/├á/g, 'á'],
    [/├ó/g, 'ó'],
    [/├ú/g, 'ú'],
    [/├®/g, 'é'],
    [/├▒/g, 'ñ'],
    [/├ü/g, 'ü'],

    [/┬í/g, 'í'],
    [/┬á/g, 'á'],
    [/┬ó/g, 'ó'],
    [/┬ú/g, 'ú'],
    [/┬®/g, 'é'],
    [/┬ñ/g, 'ñ'],
    [/┬ü/g, 'ü'],

    // Variante clásica "Ã" cuando UTF-8 se leyó como Latin1
    [/Ã¡/g, 'á'],
    [/Ã©/g, 'é'],
    [/Ãí/g, 'í'],
    [/Ã³/g, 'ó'],
    [/Ãº/g, 'ú'],
    [/Ã±/g, 'ñ'],
    [/Ã¼/g, 'ü'],
    [/Ã€/g, '€'],

    // Otras combinaciones vistas a veces
    [/Â¿/g, '¿'],
    [/Â¡/g, '¡'],
    [/Â°/g, '°'],
    [/Âº/g, 'º'],
    [/Âª/g, 'ª'],
  ];

  let out = input;
  for (const [pattern, repl] of replacements) {
    out = out.replace(pattern, repl);
  }
  return out;
}

export function mapApiReservaDetail(x: ApiReservaDetail): ReservaDetail {
  return {
    id: x.id,
    reservaId: x.reserva_ID,
    habitacionId: x.habitacion_ID,
    huespedId: x.huesped_ID,
    precioTotal: x.precio_Total,
    cantidadHuespedes: x.cantidad_Huespedes,
    numeroHabitacion: x.numero_Habitacion,

    // 🔧 Arreglo de mojibake siempre (no solo cuando contiene '├'),
    // porque puede venir en otras variantes como 'PÃ©rez'
    nombreHuesped: decodeMojibake(x.nombre_Huesped),

    // Estas cadenas ISO ("2025-12-15T00:00:00") las toma bien el DatePipe
    // Si en tu zona te da problemas de TZ, podrías añadir 'Z' al final.
    fechaEntrada: x.fecha_Entrada,
    fechaSalida:  x.fecha_Salida,
  };
}
