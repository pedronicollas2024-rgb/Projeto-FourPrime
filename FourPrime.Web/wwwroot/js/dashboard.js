// Inicializa os gráficos do dashboard
document.addEventListener('DOMContentLoaded', function () {
    // Gráfico de carros por marca
    const ctxMarca = document.getElementById('chartMarcas');
    if (ctxMarca) {
        const labels = JSON.parse(ctxMarca.getAttribute('data-labels'));
        const data = JSON.parse(ctxMarca.getAttribute('data-values'));

        new Chart(ctxMarca, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Carros por Marca',
                    data: data,
                    backgroundColor: 'rgba(54, 162, 235, 0.5)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

    // Gráfico de carros por categoria
    const ctxCategoria = document.getElementById('chartCategorias');
    if (ctxCategoria) {
        const labels = JSON.parse(ctxCategoria.getAttribute('data-labels'));
        const data = JSON.parse(ctxCategoria.getAttribute('data-values'));

        new Chart(ctxCategoria, {
            type: 'pie',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Carros por Categoria',
                    data: data,
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.5)',
                        'rgba(54, 162, 235, 0.5)',
                        'rgba(255, 205, 86, 0.5)',
                        'rgba(75, 192, 192, 0.5)',
                        'rgba(153, 102, 255, 0.5)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true
            }
        });
    }
});