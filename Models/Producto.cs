using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogoAPI.Models;

// Esta clase representa UNA fila de la tabla "Productos" en la base de datos.
// Entity Framework va a leer esta clase y, gracias a las migraciones,
// va a crear una tabla con una columna por cada propiedad que ves acá abajo.
//
// A esto se lo llama "clase de dominio" o "entidad": es un molde que
// describe qué información tiene un Producto, sin saber nada de bases
// de datos, ni de la web. Solo describe el dato.
public class Producto
{
    // Cada Producto tiene un identificador único (clave primaria).
    // Entity Framework, por convención, detecta que una propiedad
    // llamada "Id" es la clave primaria y la autoincrementa sola.
    public int Id { get; set; }

    // string.Empty en vez de dejarlo en null evita que la propiedad
    // arranque en null y nos tire errores de "referencia nula" antes
    // de que el usuario cargue el dato.
    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    // decimal (no float/double) es el tipo correcto para dinero,
    // porque no tiene errores de redondeo como los tipos de punto flotante.
    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }

    public int Stock { get; set; }
}