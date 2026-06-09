let editingSerieId = null;

document.addEventListener('DOMContentLoaded', () => {
    if (!AppState.autenticado || AppState.usuario?.rol !== 'admin') {
        window.location.href = 'index.html';
        return;
    }
    loadAdminSeries();

    document.getElementById('serieForm').addEventListener('submit', handleSerieSave);
});

async function loadAdminSeries(page = 1) {
    try {
        const res = await get(`/series?pagina=${page}&limite=20&ordenarPor=titulo&orden=asc`);
        const data = res.datos;
        renderAdminTable(data.datos);
        renderPagination('adminPagination', data.pagina, data.totalPaginas, loadAdminSeries);
    } catch (err) {
        showToast(err.message, 'error');
    }
}

function renderAdminTable(series) {
    const tbody = document.getElementById('adminTableBody');
    if (!series || series.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5">No hay series registradas.</td></tr>';
        return;
    }

    tbody.innerHTML = series.map(s => `
        <tr>
            <td><strong>${s.titulo}</strong></td>
            <td>${s.anioEstreno}</td>
            <td>${s.generos.join(', ')}</td>
            <td>★ ${s.ratingPromedio.toFixed(1)}</td>
            <td>
                <button class="btn btn-secondary btn-sm" onclick="openEditModal('${s.id}')">Editar</button>
                <button class="btn btn-danger btn-sm" onclick="deleteSerie('${s.id}')">Eliminar</button>
            </td>
        </tr>
    `).join('');
}

function openCreateModal() {
    editingSerieId = null;
    document.getElementById('modalTitle').textContent = 'Nueva Serie';
    document.getElementById('serieForm').reset();
    document.getElementById('serieModal').classList.remove('hidden');
}

async function openEditModal(id) {
    editingSerieId = id;
    document.getElementById('modalTitle').textContent = 'Editar Serie';

    try {
        const res = await get(`/series/${id}`);
        const s = res.datos;
        document.getElementById('editTitulo').value = s.titulo;
        document.getElementById('editDescripcion').value = s.descripcion;
        document.getElementById('editGeneros').value = s.generos.join(', ');
        document.getElementById('editAnio').value = s.anioEstreno;
        document.getElementById('editTemporadas').value = s.temporadas;
        document.getElementById('editPoster').value = s.poster || '';
        document.getElementById('serieModal').classList.remove('hidden');
    } catch (err) {
        showToast(err.message, 'error');
    }
}

function closeModal() {
    document.getElementById('serieModal').classList.add('hidden');
    editingSerieId = null;
}

async function handleSerieSave(e) {
    e.preventDefault();

    const payload = {
        titulo: document.getElementById('editTitulo').value.trim(),
        descripcion: document.getElementById('editDescripcion').value.trim(),
        generos: document.getElementById('editGeneros').value.split(',').map(g => g.trim()).filter(Boolean),
        anioEstreno: parseInt(document.getElementById('editAnio').value),
        temporadas: parseInt(document.getElementById('editTemporadas').value),
        poster: document.getElementById('editPoster').value.trim() || null
    };

    try {
        if (editingSerieId) {
            await put(`/series/${editingSerieId}`, payload);
            showToast('Serie actualizada');
        } else {
            await post('/series', payload);
            showToast('Serie creada');
        }
        closeModal();
        loadAdminSeries();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function deleteSerie(id) {
    if (!confirm('¿Estás seguro de eliminar esta serie? Se eliminarán también todas sus reseñas.')) return;

    try {
        await del(`/series/${id}`);
        showToast('Serie eliminada');
        loadAdminSeries();
    } catch (err) {
        showToast(err.message, 'error');
    }
}
