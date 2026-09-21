function inicializarBuscadorSelect(config) {
  const inputTexto = document.getElementById(config.inputTextoId);
  const inputHidden = document.getElementById(config.inputHiddenId);
  const listaResultados = document.getElementById(config.resultadosId);

  if (!inputTexto || !inputHidden || !listaResultados) return;

  if (config.valorInicialTexto) {
    inputTexto.value = config.valorInicialTexto;
  }

  let timeoutId = null;

  inputTexto.addEventListener('input', () => {
    inputHidden.value = '';
    clearTimeout(timeoutId);

    const query = inputTexto.value.trim();
    if (query.length < 2) {
      listaResultados.classList.add('d-none');
      listaResultados.innerHTML = '';
      return;
    }

    timeoutId = setTimeout(async () => {
      try {
        const respuesta = await fetch(
          `${config.endpoint}?q=${encodeURIComponent(query)}`
        );
        const resultados = await respuesta.json();
        mostrarResultados(resultados);
      } catch (error) {
        listaResultados.classList.add('d-none');
      }
    }, 300);
  });

  function mostrarResultados(resultados) {
    listaResultados.innerHTML = '';

    if (resultados.length === 0) {
      listaResultados.innerHTML =
        '<div class="buscador-item text-muted">Sin resultados</div>';
      listaResultados.classList.remove('d-none');
      return;
    }

    resultados.forEach((item) => {
      const div = document.createElement('div');
      div.className = 'buscador-item';
      div.textContent = item.texto;
      div.addEventListener('click', () => {
        inputTexto.value = item.texto;
        inputHidden.value = item.id;
        listaResultados.classList.add('d-none');
      });
      listaResultados.appendChild(div);
    });

    listaResultados.classList.remove('d-none');
  }
  // Cierra la lista si se hace click afuera
  document.addEventListener('click', (event) => {
    if (
      !inputTexto.contains(event.target) &&
      !listaResultados.contains(event.target)
    ) {
      listaResultados.classList.add('d-none');
    }
  });
}
