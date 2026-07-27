// Reemplazá 5xxx por el puerto real que te muestre "dotnet run" en la terminal.
const API_URL = "http://localhost:5000/api/productos";

const form = document.getElementById("form-producto");
const tabla = document.getElementById("tabla-productos");
const btnGuardar = document.getElementById("btn-guardar");

// ---------- LISTAR (GET) ----------
// fetch() sin segundo parámetro hace un GET por defecto.
// Es una función asíncrona: "await" espera a que la respuesta llegue
// antes de seguir, sin trabar el resto de la página mientras tanto.
async function cargarProductos() {
  const res = await fetch(API_URL);      // 1. pedimos los datos
  const productos = await res.json();    // 2. convertimos la respuesta (JSON) a un array de objetos JS

  tabla.innerHTML = ""; // limpiamos la tabla antes de redibujarla

  // Por cada producto que vino de la API, armamos una fila de tabla.
  // Los botones "Editar" y "Borrar" llevan el id (y datos) del
  // producto pegados en el onclick, para saber sobre cuál actuar.
  productos.forEach(p => {
    tabla.innerHTML += `
      <tr>
        <td>${p.nombre}</td>
        <td>${p.descripcion}</td>
        <td>$${p.precio}</td>
        <td>${p.stock}</td>
        <td>
          <button class="editar" onclick="editarProducto(${p.id}, '${p.nombre}', '${p.descripcion}', ${p.precio}, ${p.stock})">Editar</button>
          <button class="borrar" onclick="borrarProducto(${p.id})">Borrar</button>
        </td>
      </tr>`;
  });
}

// ---------- CREAR o EDITAR (POST o PUT) ----------
// "submit" es el evento que dispara un <form> al tocar el botón de tipo submit.
form.addEventListener("submit", async (e) => {
  e.preventDefault(); // evita que la página se recargue (comportamiento por defecto del form)

  const id = document.getElementById("producto-id").value;

  // Armamos un objeto JS con exactamente los mismos nombres de
  // propiedad que espera el modelo Producto.cs del lado del servidor.
  const producto = {
    nombre: document.getElementById("nombre").value,
    descripcion: document.getElementById("descripcion").value,
    precio: parseFloat(document.getElementById("precio").value),
    stock: parseInt(document.getElementById("stock").value)
  };

  if (id) {
    // Si "producto-id" tiene valor, estamos EDITANDO: PUT
    producto.id = parseInt(id);
    await fetch(`${API_URL}/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" }, // le avisamos a la API que el body es JSON
      body: JSON.stringify(producto)                    // convertimos el objeto JS a texto JSON
    });
  } else {
    // Si está vacío, estamos CREANDO: POST
    await fetch(API_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(producto)
    });
  }

  // Reseteamos el formulario y volvemos a pedir la lista actualizada,
  // así la tabla siempre refleja lo que realmente hay en la base.
  form.reset();
  document.getElementById("producto-id").value = "";
  btnGuardar.textContent = "Agregar";
  cargarProductos();
});

// Se llama al tocar "Editar": carga los datos de ese producto en el
// formulario para que el usuario los modifique y confirme.
function editarProducto(id, nombre, descripcion, precio, stock) {
  document.getElementById("producto-id").value = id;
  document.getElementById("nombre").value = nombre;
  document.getElementById("descripcion").value = descripcion;
  document.getElementById("precio").value = precio;
  document.getElementById("stock").value = stock;
  btnGuardar.textContent = "Guardar cambios";
}

// ---------- BORRAR (DELETE) ----------
async function borrarProducto(id) {
  if (!confirm("¿Borrar este producto?")) return; // confirmación simple antes de borrar
  await fetch(`${API_URL}/${id}`, { method: "DELETE" });
  cargarProductos(); // refrescamos la tabla
}

// Llamada inicial: apenas carga la página, ya mostramos los productos
// que existan en la base de datos.
cargarProductos();
