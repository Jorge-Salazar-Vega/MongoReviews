document.addEventListener('DOMContentLoaded', () => {
    const registerForm = document.getElementById('registerForm');
    if (registerForm) registerForm.addEventListener('submit', handleRegister);

    const loginForm = document.getElementById('loginForm');
    if (loginForm) loginForm.addEventListener('submit', handleLogin);
});

async function handleRegister(e) {
    e.preventDefault();
    clearErrors();

    const nombre = document.getElementById('nombre').value.trim();
    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirmPassword').value;

    let valid = true;

    if (nombre.length < 3) {
        showFieldError('nombreError', 'El nombre debe tener al menos 3 caracteres');
        valid = false;
    }
    if (!email.includes('@')) {
        showFieldError('emailError', 'Ingresa un email válido');
        valid = false;
    }
    if (password.length < 8) {
        showFieldError('passwordError', 'La contraseña debe tener al menos 8 caracteres');
        valid = false;
    }
    if (password !== confirmPassword) {
        showFieldError('confirmError', 'Las contraseñas no coinciden');
        valid = false;
    }
    if (!valid) return;

    try {
        const res = await post('/auth/register', { nombre, email, password });
        localStorage.setItem('token', res.datos.token);
        showToast('Registro exitoso');
        setTimeout(() => window.location.href = 'index.html', 500);
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function handleLogin(e) {
    e.preventDefault();
    clearErrors();

    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value;

    if (!email || !password) {
        showFieldError('emailError', 'Todos los campos son obligatorios');
        return;
    }

    try {
        const res = await post('/auth/login', { email, password });
        localStorage.setItem('token', res.datos.token);
        showToast('Sesión iniciada');
        setTimeout(() => window.location.href = 'index.html', 500);
    } catch (err) {
        showToast(err.message, 'error');
    }
}

function showFieldError(id, message) {
    const el = document.getElementById(id);
    if (el) el.textContent = message;
}

function clearErrors() {
    document.querySelectorAll('.form-error').forEach(el => el.textContent = '');
}
