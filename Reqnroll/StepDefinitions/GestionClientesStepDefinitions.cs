

using Reqnroll;

using FluentAssertions;

using HotelManagement.DTOs;

using HotelManagement.Services;

using HotelManagement.Repositories;

using HotelManagement.Application.Services;

using Microsoft.EntityFrameworkCore;

using Reqnroll.Support;


namespace Reqnroll.StepDefinitions

{

[Binding]

public class GestionClientesStepDefinitions

{

private readonly ReservaTestContext _context;

private IClienteService? _clienteService;


// Variables para almacenar el resultado de la prueba

private ClienteCreateDTO? _dtoParaCrear;

private ClienteDTO? _clienteResultado;

private bool _esExitoso;

private string? _mensajeError;


public GestionClientesStepDefinitions(ReservaTestContext context)

{

_context = context;

}


[BeforeScenario("@gestion-clientes")]

public async Task BeforeScenario()

{

// 1. Configuración idéntica a CrearReservaCompletaStepDefinitions

_context.DbContext = TestDatabaseHelper.CreateTestDbContext();

await TestDatabaseHelper.LimpiarDatos(_context.DbContext);


// 2. Inicializar dependencias

var clienteRepo = new ClienteRepository(_context.DbContext);

var clienteValidator = new HotelManagement.Aplicacion.Validators.ClienteValidator(_context.DbContext);


_clienteService = new ClienteService(clienteRepo, clienteValidator);


// 3. Resetear estado

_esExitoso = false;

_mensajeError = null;

_clienteResultado = null;

}


[AfterScenario("@gestion-clientes")]

public void AfterScenario()

{

_context.DbContext?.Dispose();

}


#region Given (Antecedentes)


[Given(@"que el sistema de clientes está limpio")]

public void DadoQueElSistemaDeClientesEstaLimpio()

{

// Ya se limpió en BeforeScenario

}


[Given(@"que tengo los datos del nuevo cliente:")]

public void DadoQueTengoLosDatosDelNuevoCliente(Table table)

{

var row = table.Rows[0];

_dtoParaCrear = new ClienteCreateDTO

{

Razon_Social = row["Razon_Social"],

NIT = row["NIT"],

Email = row["Email"]

};

}


[Given(@"que ya existe un cliente en el sistema con:")]

public async Task DadoQueYaExisteUnClienteEnElSistemaCon(Table table)

{

var row = table.Rows[0];

var clientePrevio = new ClienteCreateDTO

{

Razon_Social = row["Razon_Social"],

NIT = row["NIT"],

Email = row["Email"]

};


// AQUÍ ESTÁ LA CLAVE: Guardamos el cliente realmente en la BD

// para que cuando el test intente crear el segundo, falle.

if (_clienteService != null)

{

await _clienteService.CreateAsync(clientePrevio);

}

}


#endregion


#region When (Acciones)

[When(@"ejecuto el registro del cliente")]
public async Task CuandoEjecutoElRegistroDelCliente()
{
    try
    {
        if (_dtoParaCrear == null) throw new Exception("No hay datos preparados");
        
        _clienteResultado = await _clienteService!.CreateAsync(_dtoParaCrear);
        _esExitoso = true;
    }
    catch (Exception ex)
    {
        _esExitoso = false;
        
        // 1. Capturamos el mensaje principal
        _mensajeError = ex.Message; 

        // 2. INTROSPECCIÓN DE ERRORES DE VALIDACIÓN
        // Si usas FluentValidation o similar, los detalles no suelen estar en Message,
        // sino en una propiedad 'Errors' o en el ToString() completo de la excepción.
        
        // Añadimos el StackTrace o el ToString para ver los detalles en la consola de pruebas
        _mensajeError += $"\n DETALLES TÉCNICOS: {ex.ToString()}";
        
        if (ex.InnerException != null)
            _mensajeError += $" | Inner: {ex.InnerException.Message}";
            
        // Opción: Imprimir en consola para verlo en el output del test runner inmediatamente
        Console.WriteLine($"ERROR EN TEST: {_mensajeError}");
    }
}

[When(@"intento registrar otro cliente con:")]

public async Task CuandoIntentoRegistrarOtroClienteCon(Table table)

{

// Reutilizamos la lógica de preparación

DadoQueTengoLosDatosDelNuevoCliente(table);

// Reutilizamos la lógica de ejecución

await CuandoEjecutoElRegistroDelCliente();

}


#endregion


#region Then (Verificaciones)


[Then(@"el cliente debe crearse correctamente en el sistema")]

public void EntoncesElClienteDebeCrearseCorrectamente()

{

_esExitoso.Should().BeTrue($"Se esperaba éxito pero falló con: {_mensajeError}");

_clienteResultado.Should().NotBeNull();

_clienteResultado!.ID.Should().NotBeNullOrEmpty("El ID del cliente debe haber sido generado");

}


[Then(@"al consultar el NIT ""(.*)"" debe existir el cliente")]

public async Task EntoncesAlConsultarElNITDebeExistirElCliente(string nit)

{

// Verificación directa en BD para asegurar persistencia

var clienteDb = await _context.DbContext!.Clientes

.FirstOrDefaultAsync(c => c.NIT == nit);


clienteDb.Should().NotBeNull($"No se encontró cliente con NIT {nit} en la BD");

clienteDb!.Email.Should().Be(_dtoParaCrear!.Email);

}


[Then(@"el registro debe ser rechazado")]

public void EntoncesElRegistroDebeSerRechazado()

{

_esExitoso.Should().BeFalse("La operación debería haber fallado por validación");

_clienteResultado.Should().BeNull();

}



[Then(@"el mensaje de error debe mencionar ""(.*)""")]

public void EntoncesElMensajeDeErrorDebeMencionar(string texto)

{

_mensajeError.Should().NotBeNull();

_mensajeError!.ToLower().Should().Contain(texto.ToLower());

}


#endregion

#region When (Acciones - Nuevas para Delete)

[When(@"elimino el cliente con NIT ""(.*)""")]
public async Task CuandoEliminoElClienteConNIT(string nit)
{
    try
    {
        // 1. Buscamos el cliente en la BD para obtener su ID
        var clienteExistente = await _context.DbContext!.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.NIT == nit);

        if (clienteExistente == null)
        {
            throw new Exception($"No se puede eliminar: No se encontró un cliente con NIT {nit} en la preparación de la prueba.");
        }

        // 2. CORRECCIÓN: Convertimos el byte[] a Guid y luego a String
        // El error ocurría porque clienteExistente.ID es byte[], pero el servicio espera string.
        string idParaBorrar;
        
        // Verificamos si es un array de bytes (común en MySQL/SQLite con GUIDs)
        if (clienteExistente.ID is byte[] idBytes)
        {
            idParaBorrar = new Guid(idBytes).ToString();
        }
        else
        {
            // Fallback por si acaso en el futuro cambia el tipo
            idParaBorrar = clienteExistente.ID.ToString()!;
        }

        // 3. Ejecutamos la eliminación
        await _clienteService!.DeleteAsync(idParaBorrar);

        _esExitoso = true;
    }
    catch (Exception ex)
    {
        _esExitoso = false;
        _mensajeError = ex.Message;
        Console.WriteLine($"ERROR EN DELETE: {_mensajeError}");
    }
}
#endregion

#region Then (Verificaciones - Nuevas para Delete)

[Then(@"la eliminación debe ser exitosa")]
public void EntoncesLaEliminacionDebeSerExitosa()
{
    _esExitoso.Should().BeTrue($"Se esperaba que la eliminación fuera exitosa, pero falló: {_mensajeError}");
}

[Then(@"el cliente con NIT ""(.*)"" ya no debe existir en el sistema")]
public async Task EntoncesElClienteYaNoDebeExistirEnElSistema(string nit)
{
    // Verificamos directamente en la base de datos que el registro ya no esté
    var clienteDb = await _context.DbContext!.Clientes
        .FirstOrDefaultAsync(c => c.NIT == nit);

    clienteDb.Should().BeNull($"El cliente con NIT {nit} debería haber sido eliminado de la BD, pero aún existe.");
}

#endregion

#region When (Acciones - Update)

[When(@"actualizo el cliente con NIT ""(.*)"" con los nuevos datos:")]
public async Task CuandoActualizoElClienteConLosNuevosDatos(string nit, Table table)
{
    try
    {
        // 1. Buscar el cliente existente para obtener su ID
        var clienteExistente = await _context.DbContext!.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.NIT == nit);

        if (clienteExistente == null)
            throw new Exception($"No se encontró cliente con NIT {nit} para actualizar.");

        // 2. Conversión de ID (byte[] -> string)
        string idParaActualizar;
        if (clienteExistente.ID is byte[] idBytes)
        {
            idParaActualizar = new Guid(idBytes).ToString();
        }
        else
        {
            idParaActualizar = clienteExistente.ID.ToString()!;
        }

        // 3. Preparar el DTO correcto: ClienteUpdateDTO
        var row = table.Rows[0];
        
        // CORRECCIÓN AQUÍ: Usamos ClienteUpdateDTO
        var dtoActualizacion = new ClienteUpdateDTO 
        {
            Razon_Social = row["Razon_Social"],
            Email = row["Email"],
            NIT = nit // Asignamos el NIT si tu UpdateDTO lo tiene. Si no, borra esta línea.
        };

        // 4. Llamar al servicio de Update
        await _clienteService!.UpdateAsync(idParaActualizar, dtoActualizacion);

        _esExitoso = true;
    }
    catch (Exception ex)
    {
        _esExitoso = false;
        
        // 1. Empezamos con el mensaje básico ("Errores de validación")
        _mensajeError = ex.Message;

        // 2. INTROSPECCIÓN (Reflection): 
        // Buscamos si la excepción tiene una propiedad llamada "Errors" (común en FluentValidation)
        // Esto funciona aunque no tengamos acceso directo al tipo de dato.
        var propiedadErrors = ex.GetType().GetProperty("Errors");

        if (propiedadErrors != null)
        {
            // Obtenemos el valor de la propiedad "Errors"
            var valorErrors = propiedadErrors.GetValue(ex);

            // Si es una lista (IEnumerable), recorremos los errores y los agregamos al texto
            if (valorErrors is System.Collections.IEnumerable listaErrores)
            {
                var detalles = new List<string>();
                foreach (var error in listaErrores)
                {
                    // Convertimos cada error a string (esto suele dar el mensaje detallado como "Email inválido")
                    detalles.Add(error.ToString()!); 
                }
                
                // Concatenamos los detalles encontrados al mensaje que verificará el Test
                if (detalles.Any())
                {
                    _mensajeError += " | DETALLES: " + string.Join(", ", detalles);
                }
            }
        }
        else
        {
            // Si no encontramos la propiedad "Errors", guardamos todo el ToString por si acaso
            _mensajeError += " " + ex.ToString();
        }

        Console.WriteLine($"ERROR EN UPDATE (Extendido): {_mensajeError}");
    }
}
#endregion

#region Then (Verificaciones - Update)

[Then(@"la actualización debe ser exitosa")]
public void EntoncesLaActualizacionDebeSerExitosa()
{
    _esExitoso.Should().BeTrue($"La actualización falló: {_mensajeError}");
}

[Then(@"al consultar el NIT ""(.*)"" el cliente debe tener la Razon Social ""(.*)""")]
public async Task EntoncesElClienteDebeTenerLaRazonSocial(string nit, string nuevaRazonSocial)
{
    // Verificamos directo en BD que el cambio persista
    var clienteDb = await _context.DbContext!.Clientes
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.NIT == nit);

    clienteDb.Should().NotBeNull();
    clienteDb!.Razon_Social.Should().Be(nuevaRazonSocial);
}

[Then(@"el email debe ser ""(.*)""")]
public async Task EntoncesElEmailDebeSer(string nuevoEmail)
{
    // NOTA: Para verificar esto necesitamos el cliente actual.
    // Como este paso suele ir encadenado al anterior, podemos buscar por el NIT 
    // que usamos en el paso anterior, o (más robusto) buscar por el email directamente si es único.
    
    // Opción A: Buscar si existe algún cliente con ese email nuevo
    var clienteDb = await _context.DbContext!.Clientes
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.Email == nuevoEmail);

    clienteDb.Should().NotBeNull($"El email {nuevoEmail} no se guardó en la base de datos.");
}

[Then(@"la actualización debe ser rechazada")]
public void EntoncesLaActualizacionDebeSerRechazada()
{
    // Verificamos que la bandera de éxito sea falsa
    _esExitoso.Should().BeFalse("La actualización debería haber fallado por validación de datos, pero fue exitosa.");
    
    // Opcional: Verificar que _mensajeError no sea nulo
    _mensajeError.Should().NotBeNullOrEmpty("Se esperaba un mensaje de error explicando el rechazo.");
}

#endregion

#region When (Acciones - Select/Consultar)

[When(@"consulto los datos del cliente con NIT ""(.*)""")]
public async Task CuandoConsultoLosDatosDelClienteConNIT(string nit)
{
    try
    {
        // 1. PASO PREVIO: Necesitamos el ID.
        // Como el servicio solo acepta ID, buscamos en la BD cuál es el ID de ese NIT.
        var clienteEnBd = await _context.DbContext!.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.NIT == nit);

        if (clienteEnBd == null)
        {
            throw new Exception($"Error de prueba: No se encontró un cliente con NIT {nit} en la base de datos simulada.");
        }

        // 2. Convertir el ID a string (Misma lógica que usaste en Delete/Update)
        string idParaConsultar;
        if (clienteEnBd.ID is byte[] idBytes)
        {
            idParaConsultar = new Guid(idBytes).ToString();
        }
        else
        {
            idParaConsultar = clienteEnBd.ID.ToString()!;
        }

        // 3. AHORA SÍ: Llamamos al servicio oficial GetByIdAsync
        _clienteResultado = await _clienteService!.GetByIdAsync(idParaConsultar);

        _esExitoso = true;
    }
    catch (Exception ex)
    {
        _esExitoso = false;
        _mensajeError = ex.Message;
        Console.WriteLine($"ERROR EN CONSULTA: {_mensajeError}");
    }
}

#endregion

#region Then (Verificaciones - Select/Consultar)

[Then(@"la consulta debe ser exitosa")]
public void EntoncesLaConsultaDebeSerExitosa()
{
    _esExitoso.Should().BeTrue($"Se esperaba encontrar al cliente, pero falló: {_mensajeError}");
    _clienteResultado.Should().NotBeNull("El servicio devolvió null, pero se esperaba un objeto ClienteDTO.");
}

[Then(@"los datos del cliente obtenido deben coincidir:")]
public void EntoncesLosDatosDelClienteObtenidoDebenCoincidir(Table table)
{
    var row = table.Rows[0];
    string razonEsperada = row["Razon_Social"];
    string emailEsperado = row["Email"];

    _clienteResultado.Should().NotBeNull();
    
    // Verificamos que los datos que trajo el servicio GetById sean los correctos
    _clienteResultado!.Razon_Social.Should().Be(razonEsperada);
    _clienteResultado!.Email.Should().Be(emailEsperado);
}

#endregion
}


} 