function inicializarAbmInmueble(inmueble) {
    document.addEventListener('DOMContentLoaded', () => {
        inicializarFormulario(inmueble);
        inicializarBotonesEliminar(inmueble);
    })
}
function validarFormulario(datos) {
    limpiarErrores();
    let esValido = true;

    if (!datos.PropietarioId || !/^\d+$/.test(datos.PropietarioId)) {
        mostrarError('PropietarioId', 'Debe seleccionar un propietario válido.');
        esValido = false;
    }

    if (!datos.Cupo || !/^\d{1,2}$/.test(datos.Cupo)) {
        mostrarError('Cupo', 'El cupo debe tener entre 1 y 2 dígitos.');
        esValido = false;
    }

    if (!datos.Direccion || datos.Direccion.trim().length === 0) {
        mostrarError('Direccion', 'La dirección es obligatoria.');
        esValido = false;
    }

    if (!datos.PrecioXDia || !/^\d+([.,]\d{1,2})?$/.test(datos.PrecioXDia)) {
        mostrarError('PrecioXDia', 'Ingrese un precio válido con hasta 2 decimales.');
        esValido = false;
    }

    if (!datos.Estado || datos.Estado.trim().length === 0) {
        mostrarError('Estado', 'El estado es obligatorio.');
        esValido = false;
    }

    if (!datos.PorcentajeReserva || !/^(100(\.0{1,2})?|\d{1,2}([.,]\d{1,2})?)$/.test(datos.PorcentajeReserva)) {
        mostrarError('PorcentajeReserva', 'El porcentaje debe estar entre 0 y 100 con hasta 2 decimales.');
        esValido = false;
    }

    if (!datos.Latitud || !/^-?\d+([.,]\d+)?$/.test(datos.Latitud)) {
        mostrarError('Latitud', 'Latitud no válida (ej: -32.8908).');
        esValido = false;
    }

    if (!datos.Longitud || !/^-?\d+([.,]\d+)?$/.test(datos.Longitud)) {
        mostrarError('Longitud', 'Longitud no válida (ej: -68.8271).');
        esValido = false;
    }

    return esValido;
}

function mostrarError(campo, mensaje) {
    const span = document.getElementById(`error-${campo}`);
    if (span) span.textContent = mensaje;
}

function limpiarErrores() {
    document.querySelectorAll("[id^='error-']").forEach((span) => (span.textContent = ''));
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
    const input = document.querySelector('input[name="__RequestVerificationToken"]');
    return input ? input.value : '';
}

function inicializarFormulario(entidad) {
    const form = document.getElementById('formInmueble');
    if (!form) return;

    form.addEventListener('submit', async (event) => {
        event.preventDefault();

        const datosCampos = {
            Id: parseInt(document.getElementById('Id')?.value) || 0,
            PropietarioId: document.getElementById('PropietarioId')?.value.trim() || '',
            Cupo: document.getElementById('Cupo')?.value.trim() || '',
            Direccion: document.getElementById('Direccion')?.value.trim() || '',
            Tipo: document.getElementById('Tipo')?.value.trim() || '',
            PrecioXDia: document.getElementById('PrecioXDia')?.value.trim() || '',
            Estado: document.getElementById('Estado')?.value.trim() || '',
            PorcentajeReserva: document.getElementById('PorcentajeReserva')?.value.trim() || '',
            Latitud: document.getElementById('Latitud')?.value.trim() || '',
            Longitud: document.getElementById('Longitud')?.value.trim() || '',
            Activo: true
        };

        if (!validarFormulario(datosCampos)) return;

        const datosParaEnviar = {
            ...datosCampos,
            PropietarioId: parseInt(datosCampos.PropietarioId),
            Cupo: parseInt(datosCampos.Cupo),
            PrecioXDia: parseFloat(datosCampos.PrecioXDia),
            PorcentajeReserva: parseFloat(datosCampos.PorcentajeReserva),
            Latitud: parseFloat(datosCampos.Latitud),
            Longitud: parseFloat(datosCampos.Longitud)
        };

        await guardarInmueble(entidad, datosParaEnviar);
    });
}

async function guardarInmueble(entidad, datos) {
    try {
        const respuesta = await fetch(`/${entidad}/Guardar`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                RequestVerificationToken: obtenerTokenAntiForgery()
            },
            body: JSON.stringify(datos)
        });

        const resultado = await respuesta.json();

        if (respuesta.ok && resultado.success) {
            window.location.href = `/${entidad}`;
        } else {
            mostrarMensajeGeneral(resultado.message || 'Ocurrió un error al guardar.');
        }
    } catch (error) {
        mostrarMensajeGeneral('No se pudo conectar con el servidor. Intente nuevamente.');
    }
}

let idPendienteEliminar = null;
let entidadActual = '';

function inicializarBotonesEliminar(entidad) {
    entidadActual = entidad;
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
                await eliminarInmueble(entidadActual, idPendienteEliminar);
                idPendienteEliminar = null;
            }
        });
    }
}

async function eliminarInmueble(entidad, id) {
    try {
        const respuesta = await fetch(`/${entidad}/Eliminar/${id}`, {
            method: 'POST',
            headers: {
                RequestVerificationToken: obtenerTokenAntiForgery()
            }
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