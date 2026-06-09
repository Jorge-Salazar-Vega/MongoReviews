let serieId = null;
let editingReviewId = null;
let currentReviewPage = 1;

document.addEventListener('DOMContentLoaded', () => {
    const params = new URLSearchParams(window.location.search);
    serieId = params.get('id');
    if (!serieId) {
        document.getElementById('serieDetail').innerHTML = '<p>Serie no encontrada</p>';
        return;
    }
    loadSerieDetail();
    loadReviews();

    const reviewForm = document.getElementById('reviewFormElement');
    if (reviewForm) reviewForm.addEventListener('submit', handleReviewSubmit);

    document.getElementById('cancelReviewBtn')?.addEventListener('click', cancelReviewEdit);
});

async function loadSerieDetail() {
    try {
        const res = await get(`/series/${serieId}`);
        const s = res.datos;
        const container = document.getElementById('serieDetail');

        container.innerHTML = `
            <div class="poster-large">${s.poster ? `<img src="${s.poster}" alt="${s.titulo}" style="width:100%;height:100%;object-fit:cover;border-radius:12px">` : '📺'}</div>
            <div class="info">
                <h1>${s.titulo}</h1>
                <div class="meta">
                    <span>${s.anioEstreno}</span>
                    <span>${s.temporadas} temporadas</span>
                </div>
                <div class="genres">${s.generos.map(g => `<span class="genre-tag">${g}</span>`).join('')}</div>
                <div class="rating-big">★ ${s.ratingPromedio.toFixed(1)} <span>(${s.totalResenas} reseñas)</span></div>
                <p style="margin-top:1rem;color:var(--text-muted)">${s.descripcion}</p>
            </div>
        `;

        document.title = `${s.titulo} - MongoReviews`;
        checkUserReview();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function checkUserReview() {
    const form = document.getElementById('reviewForm');
    if (!form) return;
    if (!AppState.autenticado) { form.classList.add('hidden'); return; }

    try {
        const res = await get(`/resenas/serie/${serieId}?pagina=1&limite=50`);
        const reviews = res.datos.datos;
        const userReview = reviews.find(r => r.idUsuario === AppState.usuario?.id);
        if (userReview) {
            form.classList.add('hidden');
        } else {
            form.classList.remove('hidden');
        }
    } catch {
        form.classList.remove('hidden');
    }
}

async function loadReviews(page = 1) {
    currentReviewPage = page;
    try {
        const res = await get(`/resenas/serie/${serieId}?pagina=${page}&limite=10`);
        const data = res.datos;
        renderReviews(data.datos);
        renderPagination('reviewsPagination', data.pagina, data.totalPaginas, loadReviews);
    } catch (err) {
        document.getElementById('reviewsList').innerHTML = `<p>Error: ${err.message}</p>`;
    }
}

function renderReviews(reviews) {
    const container = document.getElementById('reviewsList');
    if (!reviews || reviews.length === 0) {
        container.innerHTML = '<p style="color:var(--text-muted)">No hay reseñas aún. ¡Sé el primero!</p>';
        return;
    }

    container.innerHTML = reviews.map(r => `
        <div class="review-card">
            <div class="header">
                <div class="user-info">
                    <div class="user-avatar">👤</div>
                    <span>${r.nombreUsuario}</span>
                    <span class="date">${formatDate(r.createdAt)}</span>
                </div>
                <div class="rating">★ ${r.puntuacion}/10</div>
            </div>
            ${r.comentario ? `<p>${r.comentario}</p>` : ''}
            ${AppState.usuario?.id === r.idUsuario ? `
                <div class="actions" style="margin-top:0.5rem">
                    <button class="btn btn-secondary btn-sm" onclick="editReview('${r.id}', ${r.puntuacion}, '${r.comentario?.replace(/'/g, "\\'") || ''}')">Editar</button>
                    <button class="btn btn-danger btn-sm" onclick="deleteReview('${r.id}')">Eliminar</button>
                </div>
            ` : ''}
        </div>
    `).join('');
}

function editReview(id, puntuacion, comentario) {
    editingReviewId = id;
    document.getElementById('reviewForm').classList.remove('hidden');
    document.getElementById('reviewFormTitle').textContent = 'Editar reseña';
    document.getElementById('comentario').value = comentario;
    const starInput = document.querySelector(`input[name="puntuacion"][value="${puntuacion}"]`);
    if (starInput) starInput.checked = true;
}

function cancelReviewEdit() {
    editingReviewId = null;
    document.getElementById('reviewFormTitle').textContent = 'Deja tu reseña';
    document.getElementById('reviewFormElement').reset();
}

async function handleReviewSubmit(e) {
    e.preventDefault();
    document.getElementById('puntuacionError').textContent = '';

    const puntuacion = document.querySelector('input[name="puntuacion"]:checked')?.value;
    const comentario = document.getElementById('comentario').value.trim();

    if (!puntuacion) {
        document.getElementById('puntuacionError').textContent = 'Selecciona una puntuación';
        return;
    }

    const payload = { idSerie: serieId, puntuacion: parseInt(puntuacion), comentario };

    try {
        if (editingReviewId) {
            await put(`/resenas/${editingReviewId}`, payload);
            showToast('Reseña actualizada');
        } else {
            await post('/resenas', payload);
            showToast('Reseña publicada');
        }
        cancelReviewEdit();
        document.getElementById('reviewForm').classList.add('hidden');
        loadReviews();
        loadSerieDetail();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function deleteReview(id) {
    if (!confirm('¿Eliminar esta reseña?')) return;
    try {
        await del(`/resenas/${id}`);
        showToast('Reseña eliminada');
        loadReviews();
        loadSerieDetail();
        document.getElementById('reviewForm').classList.remove('hidden');
    } catch (err) {
        showToast(err.message, 'error');
    }
}
