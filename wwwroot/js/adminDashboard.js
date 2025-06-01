let ordiniChart;

function renderOrdiniChart(months, data) {
    const ctx = document.getElementById('ordiniMensiliChart').getContext('2d');

    const arancioSemi = 'rgba(253, 126, 20, 0.4)';  
    const arancioPieno = 'rgba(253, 126, 20, 1)';  

    if (ordiniChart) {
        ordiniChart.data.labels = months;
        ordiniChart.data.datasets[0].data = data;
        ordiniChart.update();
    } else {
        ordiniChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: months,
                datasets: [{
                    label: 'Ordini di lavoro',
                    data: data,
                    backgroundColor: arancioSemi,
                    borderColor: arancioPieno,
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 10
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                        labels: {
                            color: '#FD7E14'
                        }
                    }
                }
            }
        });
    }
}
