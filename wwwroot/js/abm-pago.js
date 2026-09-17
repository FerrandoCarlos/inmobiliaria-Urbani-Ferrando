function inicializarAbmPago(pago){
    document.addEventListener('DOMContentLoaded', () => {
        inicializarFormularioPago();
        inicializarBotonesEliminarPago();
    })
}

function validarFormularioPago(datos){
    limpiarErroresPago();
    let esValido = true;
    if (!datos.ReservaId) {
        mostrarErrorPago('ReservaId', 'Debe seleccionar una reserva.');
        esValido = false;
    }
    if (!datos.Monto){
        mostrarErrorPago('Monto', 'Debe ingresar un monto.');
        esValido = false;
    }
    if (!datos.Concepto){
        mostrarErrorPago('Concepto', 'Debe ingresar un concepto del pago.');
        esValido = false;
    }
    if (!datos.Estado){
        mostrarErrorPago('Estado', 'Debe ingresar un estado del pago.');
        esValido = false;
    }
    return esValido;
}

function mostrarErrorPago(campo, mensaje) {
    const span = document.getElementById(`error-${campo}`);
    if (span) span.textContent = mensaje;
}

function limpiarErroresPago(){
    document
        .querySelectorAll("[id^='error-']")
        .forEach((span) => (span.textContent = ''));
    const div = document.getElementById('mensajeGeneral');
    if (div) div.classList.add('d-none');
}

function mostrarMensajeGeneralPago(mensaje) {
    const div = document.getElementById('mensajeGeneral');
    if (div) {
        div.textContent = mensaje;
        div.classList.remove('d-none');
    }
}

function obtenerTokenAntiForgeryPago(){
    const input = document.querySelector(
        'input[name="__RequestVerificationToken"]'
    );
    return input ? input.value : null;
}

function inicializarFormularioPago() {
    const form = document.getElementById('formPago');
    if (!form) return;
    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        const datos = {
            Id: parseInt(document.getElementById('Id').value) || 0,
            ReservaId: parseInt(document.getElementById('ReservaId').value) || 0,
            Monto: parseFloat(document.getElementById('Monto').value) || 0,
            Concepto: document.getElementById('Concepto').value || '',
            Estado: document.getElementById('Estado').value || '',
            Fecha: new Date().toISOString()
        };
        if (!validarFormularioPago(datos)){
            return;
        }
        await guardarPago(datos);
        });
}

async function guardarPago(datos){
    try {
        const respuesta = await fetch('/Pagos/Guardar', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                RequestVerificationToken: obtenerTokenAntiForgeryPago(),
            },
            body: JSON.stringify(datos),
        });

        const resultado = await respuesta.json();

        if (respuesta.ok && resultado.success){
            window.location.href = "/Pagos";
        } else {
            mostrarMensajeGeneralPago(
                resultado.message || 'Ocurrió un error al guardar.'
            );
        }
    } catch (error) {
        mostrarMensajeGeneralPago(
            'No se pudo conectar con el servidor. Intente nuevamente más tarde.'
        );
    }
}

let idPendienteEliminarPago = null;

function inicializarBotonesEliminarPago(){
    document.addEventListener('DOMContentLoaded', () => {
        const modalElement = document.getElementById('modalConfirmarBaja');
        const modal = modalElement ? new bootstrap.Modal(modalElement) : null;
        const btnConfirmar = document.getElementById('btnConfirmarBaja');

        document.querySelectorAll('.btn-eliminar').forEach((boton) => {
            boton.addEventListener('click', () => {
                idPendienteEliminarPago = boton.dataset.id;
                if (modal) modal.show();
            });
        });
        if (btnConfirmar) {
            btnConfirmar.addEventListener('click', async () => {
                if (modal) modal.hide();
                if (idPendienteEliminarPago) {
                    await eliminarPago(idPendienteEliminarPago);
                    idPendienteEliminarPago = null;
                }
            });
        }
    });
}

async function eliminarPago(id) {
    const tokenInput = document.querySelector(
        'input[name="__RequestVerificationToken"]'
    );
    try {
        const respuesta = await fetch (`/Pagos/Eliminar/${id}`, {
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
            alert(resultado.message || 'No se pudo dar de baja el pago.');
        }
    } catch (error) {
        alert('No se pudo conectar con el servidor. Intente nuevamente más tarde.');
    }
}

async function cancelarPago(id) {
    try {
        const respuesta = await fetch (`/Pagos/Cancelar/${id}`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken' : obtenerTokenAntiForgeryPago(),
            },
        });
        const resultado = await respuesta.json();
        if (respuesta.ok && resultado.success){
            const fila = document.getElementById(`fila-${id}`);
            if (fila){
                const celdaEstado = fila.querySelector('.estado-pago');
                if (celdaEstado) {
                    celdaEstado.className = 'estado-reserva';
                    celdaEstado.innerHTML = '<span>Cancelado</span>';
                }
                const btnCancelar = fila.querySelector('.btn-cancelar');
                const btnConfirmar = fila.querySelector('.btn-confirmar');
                if (btnCancelar) btnCancelar.remove();
                if (btnConfirmar) btnConfirmar.remove();
            }
        }
    } catch (err) {
        alert('No se pudo concetar con el servidor, Intente nuevamente más tarde.');
    }
}

async function confirmarPago(id){
    try {
        const respuesta = await fetch (`/Pagos/Cancelar/${id}`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken' : obtenerTokenAntiForgeryPago(),
            },
        });
        const resultado = await respuesta.json();
        if (respuesta.ok && resultado.success) {
            const fila = document.getElementById(`fila-${id}`);
            if (fila) {
                const celdaEstado = fila.querySelector('.estado-pago');
                if (celdaEstado.className = 'estado-reserva'){
                    celdaEstado.className = 'estado-reserva';
                    celdaEstado.innerHTML = '<span>Pagado</span>';
                }
                const btnConfirmar = fila.querySelector('.btn-confirmar');
                const btnCancelar = fila.querySelector('.btn-cancelar');
                if (btnConfirmar) btnConfirmar.remove();
                if (btnCancelar) btnCancelar.remove();
            }
        }
    } catch (err) {
        alert ('No se pudo conectar con el servidor, Intente nuevamente más tarde.');
    }
}
