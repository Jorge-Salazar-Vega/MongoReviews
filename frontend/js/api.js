const API_URL = window.location.hostname === '127.0.0.1' || window.location.hostname === 'localhost'
    ? 'http://127.0.0.1:5173/api'
    : 'https://api-resenas-series.railway.app/api';

async function apiRequest(endpoint, options = {}) {
    const token = localStorage.getItem('token');
    const headers = { 'Content-Type': 'application/json', ...options.headers };
    if (token) headers['Authorization'] = `Bearer ${token}`;

    const url = `${API_URL}${endpoint}`;
    const res = await fetch(url, { ...options, headers });

    const data = await res.json();

    if (!res.ok) {
        throw new Error(data.mensaje || 'Error en la solicitud');
    }

    return data;
}

function get(endpoint) {
    return apiRequest(endpoint);
}

function post(endpoint, body) {
    return apiRequest(endpoint, { method: 'POST', body: JSON.stringify(body) });
}

function put(endpoint, body) {
    return apiRequest(endpoint, { method: 'PUT', body: JSON.stringify(body) });
}

function del(endpoint) {
    return apiRequest(endpoint, { method: 'DELETE' });
}
