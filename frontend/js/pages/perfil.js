document.addEventListener('DOMContentLoaded', () => {
    if (!AppState.autenticado) {
        window.location.href = 'login.html';
        return;
    }
    loadProfile();
    loadMyReviews();
});

async function loadProfile() {
    try {
        const res = await get('/usuarios/perfil');
        const user = res.datos;
        document.getElementById('profileName').textContent = user.nombre || 'Usuario';
        document.getElementById('profileEmail').textContent = user.email;
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function loadMyReviews() {
    if (!AppState.usuario?.id) return;

    try {
        const res = await get(`/usuarios/${AppState.usuario.id}/resenas`);
        const reviews = res.datos;
        const container = document.getElementById('myReviews');

        if (!reviews || reviews.length === 0) {
            container.innerHTML = '<p style="color:var(--text-muted)">No has escrito reseñas aún.</p>';
            return;
        }

        container.innerHTML = reviews.map(r => `
            <div class="review-card">
                <div class="header">
                    <div class="user-info">
                        <span style="font-weight:600">${r.nombreUsuario}</span>
                        <span class="date">${formatDate(r.createdAt)}</span>
                    </div>
                    <div class="rating">★ ${r.puntuacion}/10</div>
                </div>
                ${r.comentario ? `<p>${r.comentario}</p>` : ''}
                <a href="serie.html?id=${r.idSerie}" style="font-size:0.85rem">Ver serie →</a>
            </div>
        `).join('');
    } catch (err) {
        showToast(err.message, 'error');
    }
}
