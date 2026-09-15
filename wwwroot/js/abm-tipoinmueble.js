function inicializarAbmTipoInmueble() {
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => {
            inicializarFormularioTipoInmueble();
            inicializarBotonesEliminarTipoInmueble();
        });
    } else {
        inicializarFormularioTipoInmueble();
        inicializarBotonesEliminarTipoInmueble();
    }
}

function validarFormularioTipoInmueble(datos) {
    limpiarErroresTipoInmueble();
    let esValido = true;
    if (!datos.Tipo || !datos.Tipo.trim()) {
        mostrarErrorTipoInmueble('Tipo', 'Debe ingresar un nombre para el tipo de inmueble.');
        esValido = false;
    }
    return esValido;
}

function mostrarErrorTipoInmueble(campo, mensaje) {
    const span = document.getElementById(`error-${campo}`);
    if (span) span.textContent = mensaje;
}

function limpiarErroresTipoInmueble() {
    document
        .querySelectorAll("[id^='error-']")
        .forEach((span) => (span.textContent = ''));
    const div = document.getElementById('mensajeGeneral');
    if (div) div.classList.add('d-none');
}

function mostrarMensajeGeneralTipoInmueble(mensaje) {
    const div = document.getElementById('mensajeGeneral');
    if (div) {
        div.textContent = mensaje;
        div.classList.remove('d-none');
    }
}

function obtenerTokenAntiForgeryTipoInmueble() {
    const input = document.querySelector('input[name="__RequestVerificationToken"]');
    return input ? input.value : '';
}

function inicializarFormularioTipoInmueble() {
    const form = document.getElementById('formTipoInmueble');
    if (!form) return;

    form.addEventListener('submit', async (event) => {
        event.preventDefault();

        const inputId = document.getElementById('Id');
        const inputTipo = document.getElementById('Tipo');

        const datos = {
            Id: parseInt(inputId ? inputId.value : '0') || 0,
            Tipo: inputTipo ? inputTipo.value.trim() : ''
        };

        if (!validarFormularioTipoInmueble(datos)) {
            return;
        }

        await guardarTipoInmueble(datos);
    });
}

async function guardarTipoInmueble(datos) {
    try {
        const respuesta = await fetch('/TipoInmueble/Guardar', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': obtenerTokenAntiForgeryTipoInmueble(),
            },
            body: JSON.stringify(datos),
        });

        const resultado = await respuesta.json();

        if (respuesta.ok && resultado.success) {
            window.location.href = "/TipoInmueble";
        } else {
            mostrarMensajeGeneralTipoInmueble(
                resultado.message || "Ocurrió un error al guardar."
            );
        }
    } catch (error) {
        mostrarMensajeGeneralTipoInmueble(
            'No se pudo conectar con el servidor. Intente nuevamente más tarde.'
        );
    }
}

let idPendienteEliminarTipoInmueble = null;

function inicializarBotonesEliminarTipoInmueble() {
    const modalElement = document.getElementById('modalConfirmarBaja');
    const modal = modalElement ? new bootstrap.Modal(modalElement) : null;
    const btnConfirmar = document.getElementById('btnConfirmarBaja');

    document.querySelectorAll('.btn-eliminar').forEach((boton) => {
        boton.addEventListener('click', () => {
            idPendienteEliminarTipoInmueble = boton.dataset.id;
            if (modal) modal.show();
        });
    });

    if (btnConfirmar) {
        btnConfirmar.addEventListener('click', async () => {
            if (modal) modal.hide();
            if (idPendienteEliminarTipoInmueble) {
                await eliminarTipoInmueble(idPendienteEliminarTipoInmueble);
                idPendienteEliminarTipoInmueble = null;
            }
        });
    }
}

async function eliminarTipoInmueble(id) {
    try {
        const respuesta = await fetch(`/TipoInmueble/Eliminar/${id}`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': obtenerTokenAntiForgeryTipoInmueble(),
            },
        });

        const resultado = await respuesta.json();

        if (respuesta.ok && resultado.success) {
            const fila = document.getElementById(`fila-${id}`);
            if (fila) fila.remove();
        } else {
            alert(resultado.message || 'No se pudo dar de baja el tipo de inmueble.');
        }
    } catch (error) {
        alert('No se pudo conectar con el servidor. Intente nuevamente más tarde.');
    }
}