let currentPage = 1;
let currentTotalPages = 1;

async function loadSeries(page = 1) {
    currentPage = page;
    const genero = document.getElementById('genreFilter')?.value || '';
    const ordenarPor = document.getElementById('sortFilter')?.value || 'rating';
    const orden = ordenarPor === 'rating' ? 'desc' : 'asc';

    let url = `/series?pagina=${page}&limite=12&ordenarPor=${ordenarPor}&orden=${orden}`;
    if (genero) url += `&genero=${encodeURIComponent(genero)}`;

    try {
        const res = await get(url);
        const data = res.datos;
        currentTotalPages = data.totalPaginas;
        renderSeries(data.datos);
        renderPagination('pagination', data.pagina, data.totalPaginas, loadSeries);
    } catch (err) {
        document.getElementById('seriesGrid').innerHTML = `<p>Error al cargar series: ${err.message}</p>`;
    }
}

function renderSeries(series) {
    const grid = document.getElementById('seriesGrid');
    if (!series || series.length === 0) {
        grid.innerHTML = '<p style="color:var(--text-muted)">No se encontraron series.</p>';
        return;
    }

    grid.innerHTML = series.map(s => `
        <div class="card serie-card" onclick="window.location='serie.html?id=${s.id}'">
            <div class="poster">${s.poster ? `<img src="${s.poster}" alt="${s.titulo}" style="width:100%;height:100%;object-fit:cover;border-radius:8px">` : '📺'}</div>
            <div class="info">
                <h3>${s.titulo}</h3>
                <div class="meta">
                    <span>${s.anioEstreno}</span>
                    <span>${s.temporadas} ${s.temporadas === 1 ? 'temp' : 'temps'}</span>
                    <span>${s.generos.slice(0, 2).join(', ')}</span>
                </div>
                <div class="desc">${s.descripcion}</div>
                <div class="rating">★ ${s.ratingPromedio.toFixed(1)} <span style="color:var(--text-muted);font-weight:400;font-size:0.85rem">(${s.totalResenas})</span></div>
            </div>
        </div>
    `).join('');
}

async function searchSeries() {
    const q = document.getElementById('searchInput')?.value?.trim();
    if (!q) { loadSeries(); return; }

    try {
        const res = await get(`/series/buscar?q=${encodeURIComponent(q)}&pagina=1&limite=12`);
        const data = res.datos;
        renderSeries(data.datos);
        renderPagination('pagination', data.pagina, data.totalPaginas, (p) => {
            get(`/series/buscar?q=${encodeURIComponent(q)}&pagina=${p}&limite=12`).then(r => {
                renderSeries(r.datos.datos);
                renderPagination('pagination', r.datos.pagina, r.datos.totalPaginas, arguments.callee);
            });
        });
        document.getElementById('genreFilter').value = '';
    } catch (err) {
        showToast(err.message, 'error');
    }
}

document.addEventListener('DOMContentLoaded', () => {
    if (document.getElementById('seriesGrid')) loadSeries();
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('keypress', (e) => { if (e.key === 'Enter') searchSeries(); });
    }
});
