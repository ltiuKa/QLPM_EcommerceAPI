import { isLoggedIn, getUserRole, tokenStore } from '/Frontend/assets/js/api.js';

export function applyNavbarAuth()
{
	const isAuth = isLoggedIn();
	const role = getUserRole();
	const adminLink = document.querySelector('a[href="/Frontend/admin/index.html"]');
	const btnLogin = document.querySelector('a[href="/Frontend/login.html"]');
	const btnRegister = document.querySelector('a[href="/Frontend/register.html"]');

	if (adminLink) adminLink.style.display = role === 'Admin' ? '' : 'none';
	if (isAuth)
	{
		if (btnLogin) btnLogin.outerHTML = '<a class="btn btn-outline-light btn-sm me-2" href="/Frontend/profile.html">Tài khoản</a>';
		if (btnRegister) btnRegister.outerHTML = '<button id="btnLogout" class="btn btn-warning btn-sm">Đăng xuất</button>';
		const btnLogout = document.getElementById('btnLogout');
		if (btnLogout) btnLogout.onclick = () => { tokenStore.clear(); location.href='/Frontend/index.html'; };
	}
}

export function ensureToastContainer(){
	let c = document.getElementById('toast-container');
	if (!c){
		c = document.createElement('div');
		c.id = 'toast-container';
		c.style.position = 'fixed';
		c.style.top = '10px';
		c.style.right = '10px';
		c.style.zIndex = '1080';
		document.body.appendChild(c);
	}
	return c;
}

export function showToast(message, variant = 'danger'){
	const c = ensureToastContainer();
	const el = document.createElement('div');
	el.className = `alert alert-${variant}`;
	el.textContent = message;
	c.appendChild(el);
	setTimeout(()=> el.remove(), 3000);
}

export function enableGlobalSpinner(){
	let s = document.getElementById('global-spinner');
	if (!s){
		s = document.createElement('div');
		s.id = 'global-spinner';
		s.style.position = 'fixed';
		s.style.inset = '0';
		s.style.display = 'none';
		s.style.alignItems = 'center';
		s.style.justifyContent = 'center';
		s.style.background = 'rgba(255,255,255,0.5)';
		s.innerHTML = '<div class="spinner-border text-primary" role="status"></div>';
		document.body.appendChild(s);
	}
	window.onApiStart = () => { s.style.display = 'flex'; };
	window.onApiEnd = () => { s.style.display = 'none'; };
	window.onApiError = (msg) => { showToast(msg, 'danger'); };
}


