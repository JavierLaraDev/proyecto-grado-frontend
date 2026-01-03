// ===== GRÁFICO GANANCIAS =====
window.crearGananciasChart = (labels, data) => {
    const ctx = document.getElementById('gananciasChart');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Ganancias (Bs)',
                data: data,
                borderColor: '#10b981',
                backgroundColor: 'rgba(16,185,129,0.15)',
                tension: 0.4,
                fill: true
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false
        }
    });
};

// ===== GRÁFICO ESTADO PEDIDOS =====
window.crearEstadoPedidos = (
    pendientes,
    proceso,
    enviados,
    entregados,
    cancelados
) => {
    const ctx = document.getElementById('estadoPedidosChart');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: [
                'Pendientes',
                'En Proceso',
                'Enviados',
                'Entregados',
                'Cancelados'
            ],
            datasets: [{
                data: [
                    pendientes,
                    proceso,
                    enviados,
                    entregados,
                    cancelados
                ],
                backgroundColor: [
                    '#f59e0b',
                    '#3b82f6',
                    '#06b6d4',
                    '#10b981',
                    '#ef4444'
                ]
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom'
                }
            }
        }
    });
};