document.addEventListener("DOMContentLoaded", () => {

    //--------------------------------------
    // 🔗 URL BASE DA API
    //--------------------------------------
    const api = "https://localhost:5203/api";  // ajuste se necessário

    //--------------------------------------
    // 🔗 ELEMENTOS DO DOM
    //--------------------------------------
    const marcaSelect = document.getElementById("marcaSelect");
    const modeloSelect = document.getElementById("modeloSelect"); // categoria
    const precoSelect = document.getElementById("precoSelect");
    const precoMin = document.getElementById("precoMin");
    const precoMax = document.getElementById("precoMax");
    const customPrice = document.getElementById("customPrice");

    const searchBtn = document.querySelector(".search-btn");
    const carList = document.getElementById("carList");

    //--------------------------------------
    // 🟦 MOSTRAR / ESCONDER CAMPOS PERSONALIZADOS
    //--------------------------------------
    precoSelect.addEventListener("change", () => {
        customPrice.style.display = precoSelect.value === "custom" ? "flex" : "none";
    });

    //--------------------------------------
    // 🟧 CARREGAR MARCAS DA API
    //--------------------------------------
    async function loadMarcas() {
        const res = await fetch(`${api}/marcas`);
        const data = await res.json();

        data.forEach(m => {
            marcaSelect.innerHTML += `<option value="${m.nome.toLowerCase()}">${m.nome}</option>`;
        });
    }

    //--------------------------------------
    // 🟧 CARREGAR CATEGORIAS (modelos)
    //--------------------------------------
    async function loadCategorias() {
        const res = await fetch(`${api}/categorias`);
        const data = await res.json();

        data.forEach(c => {
            modeloSelect.innerHTML += `<option value="${c.nome.toLowerCase()}">${c.nome}</option>`;
        });
    }

    //--------------------------------------
    // 🔍 BUSCAR CARROS COM FILTROS
    //--------------------------------------
    async function loadCarros() {

        const params = new URLSearchParams();

        // Marca
        if (marcaSelect.value)
            params.append("marca", marcaSelect.value);

        // Categoria (modelo)
        if (modeloSelect.value)
            params.append("categoria", modeloSelect.value);

        // Preço
        if (precoSelect.value && precoSelect.value !== "custom") {
            params.append("precoMax", precoSelect.value);
        }

        // Preço customizado
        if (precoSelect.value === "custom") {
            if (precoMin.value) params.append("precoMin", precoMin.value);
            if (precoMax.value) params.append("precoMax", precoMax.value);
        }

        const url = `${api}/carros?${params.toString()}`;
        console.log("📡 Enviando para API:", url);

        const res = await fetch(url);
        const data = await res.json();

        renderCarros(data);
    }

    //--------------------------------------
    // 🖼️ RENDERIZA OS CARDS
    //--------------------------------------
    function renderCarros(lista) {

        carList.innerHTML = "";

        if (!lista.length) {
            carList.innerHTML = `<p class="no-results">Nenhum veículo encontrado.</p>`;
            return;
        }

        lista.forEach(carro => {
            carList.innerHTML += `
                <div class="car-card">
                    <img src="${carro.capaUrl || '/img/default-car.png'}" class="car-img" />

                    <div class="car-info">
                        <h3>${carro.modelo}</h3>
                        <p><strong>Marca:</strong> ${carro.marca}</p>
                        <p><strong>Categoria:</strong> ${carro.categoria}</p>
                        <p><strong>Preço:</strong> R$ ${carro.preco.toLocaleString()}</p>
                    </div>
                </div>
            `;
        });
    }

    //--------------------------------------
    // 🟦 EVENTOS
    //--------------------------------------
    searchBtn.addEventListener("click", loadCarros);

    //--------------------------------------
    // 🚀 INICIALIZAÇÃO
    //--------------------------------------
    loadMarcas();
    loadCategorias();
    loadCarros(); // opcional: já carrega tudo ao abrir
});
