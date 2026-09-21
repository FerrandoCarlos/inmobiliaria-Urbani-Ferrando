let idPendienteEliminarUsuario = null;

function inicializarBotonesEliminarUsuario() {
  document.addEventListener('DOMContentLoaded', () => {
    const modalElement = document.getElementById('modalConfirmarBaja');
    const modal = modalElement ? new bootstrap.Modal(modalElement) : null;
    const btnConfirmar = document.getElementById('btnConfirmarBaja');

    document.querySelectorAll('.btn-eliminar').forEach((boton) => {
      boton.addEventListener('click', () => {
        idPendienteEliminarUsuario = boton.dataset.id;
        if (modal) modal.show();
      });
    });

    if (btnConfirmar) {
      btnConfirmar.addEventListener('click', async () => {
        if (modal) modal.hide();
        if (idPendienteEliminarUsuario) {
          await eliminarUsuario(idPendienteEliminarUsuario);
          idPendienteEliminarUsuario = null;
        }
      });
    }
  });
}

async function eliminarUsuario(id) {
  const tokenInput = document.querySelector(
    'input[name="__RequestVerificationToken"]'
  );

  try {
    const respuesta = await fetch(`/Usuarios/Eliminar/${id}`, {
      method: 'POST',
      headers: {
        RequestVerificationToken: tokenInput ? tokenInput.value : '',
      },
    });

    if (respuesta.ok) {
      window.location.reload();
    } else {
      const resultado = await respuesta.json();
      alert(resultado.message || 'No se pudo eliminar el usuario.');
    }
  } catch (error) {
    alert('No se pudo conectar con el servidor. Intente nuevamente.');
  }
}
