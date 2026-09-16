let fechasOcupadas = [];
function inicializarAbmReserva() {
  document.addEventListener('DOMContentLoaded', () => {
    configurarRestriccionesFechas();
    configurarEscuchaInmueble();
    inicializarFormularioReserva();
  });
}

function validarFormularioReserva(datos) {
  limpiarErroresReserva();
  let esValido = true;

  if (!datos.InquilinoId) {
    mostrarErrorReserva('InquilinoId', 'Debe seleccionar un inquilino.');
    esValido = false;
  }

  if (!datos.InmuebleId) {
    mostrarErrorReserva('InmuebleId', 'Debe seleccionar un inmueble.');
    esValido = false;
  }

  if (!datos.FechaDesde) {
    mostrarErrorReserva('FechaDesde', 'La fecha desde es obligatoria.');
    esValido = false;
  }

  if (!datos.FechaHasta) {
    mostrarErrorReserva('FechaHasta', 'La fecha hasta es obligatoria.');
    esValido = false;
  } else if (datos.FechaDesde && datos.FechaHasta <= datos.FechaDesde) {
    mostrarErrorReserva(
      'FechaHasta',
      'La fecha hasta debe ser posterior a la fecha desde.'
    );
    esValido = false;
  }
  if (datos.FechaDesde && datos.FechaHasta) {
    if (!validarFechasLibres(datos.FechaDesde, datos.FechaHasta)) {
      mostrarErrorReserva('FechaDesde', 'El inmueble ya está reservado en las fechas seleccionadas.');
      esValido = false;
    }
  }

  return esValido;
}

function mostrarErrorReserva(campo, mensaje) {
  const span = document.getElementById(`error-${campo}`);
  if (span) span.textContent = mensaje;
}

function limpiarErroresReserva() {
  document
    .querySelectorAll("[id^='error-']")
    .forEach((span) => (span.textContent = ''));
  const div = document.getElementById('mensajeGeneral');
  if (div) div.classList.add('d-none');
}

function mostrarMensajeGeneralReserva(mensaje) {
  const div = document.getElementById('mensajeGeneral');
  if (div) {
    div.textContent = mensaje;
    div.classList.remove('d-none');
  }
}

function obtenerTokenAntiForgeryReserva() {
  const input = document.querySelector(
    'input[name="__RequestVerificationToken"]'
  );
  return input ? input.value : null;
}

function inicializarFormularioReserva() {
  const form = document.getElementById('formReserva');
  if (!form) return;

  form.addEventListener('submit', async (event) => {
    event.preventDefault();

    const datos = {
      Id: parseInt(document.getElementById('Id').value) || 0,
      InquilinoId: parseInt(document.getElementById('InquilinoId').value) || 0,
      InmuebleId: parseInt(document.getElementById('InmuebleId').value) || 0,
      FechaDesde: document.getElementById('FechaDesde').value,
      FechaHasta: document.getElementById('FechaHasta').value,
    };

    if (!validarFormularioReserva(datos)) {
      return;
    }

    await guardarReserva(datos);
  });
}

async function guardarReserva(datos) {
  try {
    const respuesta = await fetch('/Reservas/Guardar', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        RequestVerificationToken: obtenerTokenAntiForgeryReserva(),
      },
      body: JSON.stringify(datos),
    });

    const resultado = await respuesta.json();

    if (respuesta.ok && resultado.success) {
      window.location.href = '/Reservas';
    } else {
      mostrarMensajeGeneralReserva(
        resultado.message || 'Ocurrió un error al guardar.'
      );
    }
  } catch (error) {
    mostrarMensajeGeneralReserva(
      'No se pudo conectar con el servidor. Intente nuevamente.'
    );
  }
}

let idPendienteEliminarReserva = null;

function inicializarBotonesEliminarReserva() {
  document.addEventListener('DOMContentLoaded', () => {
    const modalElement = document.getElementById('modalConfirmarBaja');
    const modal = modalElement ? new bootstrap.Modal(modalElement) : null;
    const btnConfirmar = document.getElementById('btnConfirmarBaja');

    document.querySelectorAll('.btn-eliminar').forEach((boton) => {
      boton.addEventListener('click', () => {
        idPendienteEliminarReserva = boton.dataset.id;
        if (modal) modal.show();
      });
    });

    if (btnConfirmar) {
      btnConfirmar.addEventListener('click', async () => {
        if (modal) modal.hide();
        if (idPendienteEliminarReserva) {
          await eliminarReserva(idPendienteEliminarReserva);
          idPendienteEliminarReserva = null;
        }
      });
    }
  });
}

async function eliminarReserva(id) {
  const tokenInput = document.querySelector(
    'input[name="__RequestVerificationToken"]'
  );

  try {
    const respuesta = await fetch(`/Reservas/Eliminar/${id}`, {
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
      alert(resultado.message || 'No se pudo finalizar la reserva.');
    }
  } catch (error) {
    alert('No se pudo conectar con el servidor. Intente nuevamente.');
  }
}

function configurarRestriccionesFechas() {
  const inputDesde = document.getElementById('FechaDesde');
  const inputHasta = document.getElementById('FechaHasta');

  if (inputDesde) {
    const hoy = new Date().toISOString().split('T')[0];
    inputDesde.setAttribute('min', hoy);
    inputDesde.addEventListener('change', () => {
      if (inputHasta) {
        inputHasta.setAttribute('min', inputDesde.value);
        if (inputHasta.value && inputHasta.value <= inputDesde.value) {
          inputHasta.value = '';
        }
      }
    })
  }
}

function configurarEscuchaInmueble() {
  const selectInmueble = document.getElementById('InmuebleId');
  if (selectInmueble) {
    selectInmueble.addEventListener('change', (e) => cargarFechasOcupadas(e.target.value));
    if (selectInmueble.value) {
      cargarFechasOcupadas(selectInmueble.value);
    }
  }
}

async function cargarFechasOcupadas(inmuebleId) {
  if (!inmuebleId || inmuebleId === "0") {
    fechasOcupadas = [];
    return;
  }
  try {
    const res = await fetch(`/Reservas/ObtenerFechasOcupadas?inmuebleId=${inmuebleId}`);
    if (res.ok) {
      fechasOcupadas = await res.json();
    }
  } catch (err) {
    console.error('Error al cargar fechas ocupadas: ', err);
  }
}

function validarFechasLibres(fechaDesdeStr, fechaHastaStr) {
  const desde = new Date(fechaDesdeStr.replace(/-/g, '/'));
  const hasta = new Date(fechaHastaStr.replace(/-/g, '/'));

  for (const rango of fechasOcupadas) {
    const rDesde = new Date(rango.desde.replace(/-/g, '/'));
    const rHasta = new Date(rango.hasta.replace(/-/g, '/'));

    if (desde < rHasta && hasta > rDesde) {
      return false;
    }
  }
  return true;
}

async function finalizarReserva(id) {
  try {
    const respuesta = await fetch(`/Reservas/Finalizar/${id}`, {
      method: 'POST',
      headers: {
        'RequestVerificationToken': obtenerTokenAntiForgeryReserva(),
      },
    });
    const resultado = await respuesta.json();
    if (respuesta.ok && resultado.success) {
      const fila = document.getElementById(`fila-${id}`);
      if (fila) {
        const celdaEstado = fila.querySelector('.estado-reserva');
        if (celdaEstado) {
          celdaEstado.className='estado-reserva';
          celdaEstado.innerHTML= '<span class="badge bg-secondary">Finalizado</span>';
        }
        const btnFinalizar = fila.querySelector('.btn-finalizar');
        const btnExtender = fila.querySelector('.btn-extender');
        if (btnFinalizar) btnFinalizar.remove();
        if (btnExtender) btnExtender.remove();
      }
    } else {
      alert(resultado.message || 'No se pudo finalizar la reserva.');
    }
  } catch (err) {
    alert('No se pudo finalizar la reserva.');
  }
}
