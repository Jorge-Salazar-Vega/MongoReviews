const AppState = {
    usuario: null,
    token: localStorage.getItem('token'),
    autenticado: false,
};

function initApp() {
    const token = localStorage.getItem('token');
    if (token) {
        AppState.token = token;
        AppState.autenticado = true;
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            AppState.usuario = {
                id: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
                email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
                rol: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
                nombre: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || 'Usuario'
            };
        } catch {
            logout();
        }
    }
    renderNavbar();
}

function renderNavbar() {
    const nav = document.getElementById('navbar');
    if (!nav) return;

    let links = `
        <a href="index.html" class="logo">Mongo<span>Reviews</span></a>
        <div class="nav-links">
            <a href="index.html">Inicio</a>
    `;

    if (AppState.autenticado) {
        links += `<a href="perfil.html">Perfil</a>`;
        if (AppState.usuario?.rol === 'admin') {
            links += `<a href="admin.html">Admin</a>`;
        }
        links += `<button onclick="logout()">Cerrar sesión</button>`;
    } else {
        links += `
            <a href="login.html">Iniciar sesión</a>
            <a href="register.html" class="btn-primary" style="color:white">Registrarse</a>
        `;
    }

    links += `</div>`;
    nav.innerHTML = links;
}

function logout() {
    localStorage.removeItem('token');
    AppState.token = null;
    AppState.usuario = null;
    AppState.autenticado = false;
    window.location.href = 'index.html';
}

function showToast(mensaje, tipo = 'success') {
    const container = document.getElementById('toastContainer');
    if (!container) return;
    const toast = document.createElement('div');
    toast.className = `toast ${tipo}`;
    toast.textContent = mensaje;
    container.appendChild(toast);
    setTimeout(() => toast.remove(), 3000);
}

function formatDate(dateStr) {
    return new Date(dateStr).toLocaleDateString('es-MX', {
        year: 'numeric', month: 'short', day: 'numeric'
    });
}

document.addEventListener('DOMContentLoaded', initApp);
