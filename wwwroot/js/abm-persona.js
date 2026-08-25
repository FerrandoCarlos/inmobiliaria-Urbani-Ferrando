// Módulo genérico de ABM para entidades con forma "persona"
// (Dni, Nombre, Apellido, Telefono, Email). Reutilizado por
// Propietario e Inquilino para evitar duplicar la misma lógica.

function inicializarAbmPersona(nombreEntidad) {
  document.addEventListener('DOMContentLoaded', () => {
    inicializarFormulario(nombreEntidad);
    inicializarBotonesEliminar(nombreEntidad);
  });
}

function validarFormulario(datos) {
  limpiarErrores();
  let esValido = true;

  if (!datos.Dni || !/^\d{7,9}$/.test(datos.Dni)) {
    mostrarError('Dni', 'El DNI debe contener entre 7 y 9 dígitos numéricos.');
    esValido = false;
  }

  if (!datos.Nombre || datos.Nombre.trim().length === 0) {
    mostrarError('Nombre', 'El nombre es obligatorio.');
    esValido = false;
  } else if (!/^[a-zA-ZÀ-ÿñÑ\s]+$/.test(datos.Nombre)) {
    mostrarError('Nombre', 'El nombre no puede contener números ni símbolos.');
    esValido = false;
  }

  if (!datos.Apellido || datos.Apellido.trim().length === 0) {
    mostrarError('Apellido', 'El apellido es obligatorio.');
    esValido = false;
  } else if (!/^[a-zA-ZÀ-ÿñÑ\s]+$/.test(datos.Apellido)) {
    mostrarError(
      'Apellido',
      'El apellido no puede contener números ni símbolos.'
    );
    esValido = false;
  }

  if (!datos.Email || datos.Email.trim().length === 0) {
    mostrarError('Email', 'El email es obligatorio.');
    esValido = false;
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(datos.Email)) {
    mostrarError('Email', 'El formato del email no es válido.');
    esValido = false;
  }

  return esValido;
}

function mostrarError(campo, mensaje) {
  const span = document.getElementById(`error-${campo}`);
  if (span) span.textContent = mensaje;
}

function limpiarErrores() {
  document
    .querySelectorAll("[id^='error-']")
    .forEach((span) => (span.textContent = ''));
  ocultarMensajeGeneral();
}

function mostrarMensajeGeneral(mensaje) {
  const div = document.getElementById('mensajeGeneral');
  if (div) {
    div.textContent = mensaje;
    div.classList.remove('d-none');
  }
}

function ocultarMensajeGeneral() {
  const div = document.getElementById('mensajeGeneral');
  if (div) div.classList.add('d-none');
}

function obtenerTokenAntiForgery() {
  const input = document.querySelector(
    'input[name="__RequestVerificationToken"]'
  );
  return input ? input.value : null;
}

function inicializarFormulario(nombreEntidad) {
  const form = document.getElementById('formPersona');
  if (!form) return;

  form.addEventListener('submit', async (event) => {
    event.preventDefault();

    const datos = {
      Id: parseInt(document.getElementById('Id').value) || 0,
      Dni: document.getElementById('Dni').value.trim(),
      Nombre: document.getElementById('Nombre').value.trim(),
      Apellido: document.getElementById('Apellido').value.trim(),
      Telefono: document.getElementById('Telefono').value.trim(),
      Email: document.getElementById('Email').value.trim(),
    };

    if (!validarFormulario(datos)) {
      return;
    }

    await guardarPersona(nombreEntidad, datos);
  });
}

async function guardarPersona(nombreEntidad, datos) {
  try {
    const respuesta = await fetch(`/${nombreEntidad}/Guardar`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        RequestVerificationToken: obtenerTokenAntiForgery(),
      },
      body: JSON.stringify(datos),
    });

    const resultado = await respuesta.json();

    if (respuesta.ok && resultado.success) {
      window.location.href = `/${nombreEntidad}`;
    } else {
      mostrarMensajeGeneral(
        resultado.message || 'Ocurrió un error al guardar.'
      );
    }
  } catch (error) {
    mostrarMensajeGeneral(
      'No se pudo conectar con el servidor. Intente nuevamente.'
    );
  }
}

let idPendienteEliminar = null;
let nombreEntidadActual = null;
function inicializarBotonesEliminar(nombreEntidad) {
  nombreEntidadActual = nombreEntidad;
  const modalElement = document.getElementById('modalConfirmarBaja');
  const modal = modalElement ? new bootstrap.Modal(modalElement) : null;
  const btnConfirmar = document.getElementById('btnConfirmarBaja');

  document.querySelectorAll('.btn-eliminar').forEach((boton) => {
    boton.addEventListener('click', () => {
      idPendienteEliminar = boton.dataset.id;
      if (modal) modal.show();
    });
  });

  if (btnConfirmar) {
    btnConfirmar.addEventListener('click', async () => {
      if (modal) modal.hide();
      if (idPendienteEliminar) {
        await eliminarPersona(nombreEntidadActual, idPendienteEliminar);
        idPendienteEliminar = null;
      }
    });
  }
}

async function eliminarPersona(nombreEntidad, id) {
  const tokenInput = document.querySelector(
    'input[name="__RequestVerificationToken"]'
  );

  try {
    const respuesta = await fetch(`/${nombreEntidad}/Eliminar/${id}`, {
      method: 'POST',
      headers: {
        RequestVerificationToken: tokenInput ? tokenInput.value : '',
      },
    });

    const resultado = await respuesta.json();

    if (respuesta.ok && resultado.success) {
      const fila = document.getElementById(`fila-${id}`);
      if (fila) fila.remove();
    } else {
      alert(resultado.message || 'No se pudo eliminar el registro.');
    }
  } catch (error) {
    alert('No se pudo conectar con el servidor. Intente nuevamente.');
  }
}
