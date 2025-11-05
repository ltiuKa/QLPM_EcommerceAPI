 // Config Gateway: ưu tiên window.GATEWAY nếu có; mặc định HTTP 5226 (Ocelot)
 export const GATEWAY = window.GATEWAY || (window.USE_HTTP ? 'http://localhost:5226' : 'https://localhost:7091');

export const tokenStore = {
	get() { return localStorage.getItem('access_token') || ''; },
	set(t) { localStorage.setItem('access_token', t || ''); },
	clear() { localStorage.removeItem('access_token'); }
};

export function decodeJwt(token){
	try{
		const base = token.split('.')[1];
		const json = atob(base.replace(/-/g,'+').replace(/_/g,'/'));
		return JSON.parse(decodeURIComponent(escape(json)));
	}catch{ return null; }
}

export function getUserInfo(){
	const t = tokenStore.get(); if(!t) return null;
	const payload = decodeJwt(t); return payload || null;
}

export function getUserRole(){
	const p = getUserInfo();
	if(!p) return null;
	return p['role'] || p['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
}

export function isLoggedIn(){ return !!tokenStore.get(); }

export async function apiFetch(path, options = {}) {
	const headers = options.headers || {};
	const token = tokenStore.get();
	if (token && !headers['Authorization']) headers['Authorization'] = `Bearer ${token}`;
	if (!headers['Content-Type'] && options.body && !(options.body instanceof FormData)) headers['Content-Type'] = 'application/json';
	try{
		if (window.onApiStart) window.onApiStart();
		const res = await fetch(`${GATEWAY}${path}`, { ...options, headers });
		if (!res.ok) {
			const msg = `${res.status} ${res.statusText}`;
			if (window.onApiError) window.onApiError(msg);
			throw new Error(msg);
		}
		const contentType = res.headers.get('content-type') || '';
		return contentType.includes('application/json') ? res.json() : res.text();
	} finally {
		if (window.onApiEnd) window.onApiEnd();
	}
}

export const AuthAPI = {
	register: (email, password, fullName) => apiFetch('/api/auth/register', { method: 'POST', body: JSON.stringify({ email, password, fullName }) }),
	login: async (email, password) => {
		const data = await apiFetch('/api/auth/login', { method: 'POST', body: JSON.stringify({ email, password }) });
		tokenStore.set(data.access_token);
		return data;
	}
};

export const ProductAPI = {
	list: (q) => q ? apiFetch(`/api/products/search?q=${encodeURIComponent(q)}`) : apiFetch('/api/products'),
	get: (id) => apiFetch(`/api/products/${id}`),
	listCategories: () => apiFetch('/api/categories')
};

export const CartAPI = {
	get: () => apiFetch('/api/cart'),
	add: (item) => apiFetch('/api/cart/items', { method: 'POST', body: JSON.stringify(item) }),
	remove: (itemId) => apiFetch(`/api/cart/items/${itemId}`, { method: 'DELETE' })
};

export const OrderAPI = {
	create: (payload) => apiFetch('/api/orders', { method: 'POST', body: JSON.stringify(payload) }),
	my: () => apiFetch('/api/orders/my'),
	checkoutFromCart: (shippingAddress) => apiFetch('/api/orders/checkout-from-cart', { method: 'POST', body: JSON.stringify(shippingAddress) })
};

export const AdminAPI = {
	createProduct: (p) => apiFetch('/api/products', { method: 'POST', body: JSON.stringify(p) }),
	updateProduct: (id, p) => apiFetch(`/api/products/${id}`, { method: 'PUT', body: JSON.stringify(p) }),
	deleteProduct: (id) => apiFetch(`/api/products/${id}`, { method: 'DELETE' }),
	createCategory: (c) => apiFetch('/api/categories', { method: 'POST', body: JSON.stringify(c) }),
	updateCategory: (id, c) => apiFetch(`/api/categories/${id}`, { method: 'PUT', body: JSON.stringify(c) }),
	deleteCategory: (id) => apiFetch(`/api/categories/${id}`, { method: 'DELETE' }),
	uploadProductImage: (productId, file, isMain=false) => {
		const form = new FormData();
		form.append('file', file);
		return apiFetch(`/api/products/${productId}/images?isMain=${isMain}`, { method: 'POST', body: form });
	}
};


