const GATEWAY = "https://localhost:7091"; // đổi sang http nếu cần
let ACCESS_TOKEN = "";

async function register() {
  const email = document.getElementById('email').value;
  const password = document.getElementById('password').value;
  const res = await fetch(`${GATEWAY}/api/auth/register`, {
    method: 'POST', headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password, fullName: 'Demo User' })
  });
  const data = await res.json().catch(()=>({}));
  alert('Register: ' + res.status + ' ' + JSON.stringify(data));
}

async function login() {
  const email = document.getElementById('email').value;
  const password = document.getElementById('password').value;
  const res = await fetch(`${GATEWAY}/api/auth/login`, {
    method: 'POST', headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  });
  const data = await res.json();
  ACCESS_TOKEN = data.access_token || '';
  document.getElementById('token').innerText = ACCESS_TOKEN;
}

async function loadProducts() {
  const q = document.getElementById('search').value;
  const url = q ? `${GATEWAY}/api/products/search?q=${encodeURIComponent(q)}` : `${GATEWAY}/api/products`;
  const res = await fetch(url);
  const items = await res.json();
  const host = document.getElementById('products');
  host.innerHTML = '';
  items.forEach(p => {
    const div = document.createElement('div');
    div.className = 'card';
    const variantId = (p.variants && p.variants.length > 0) ? p.variants[0].id : null;
    const unitPrice = (p.variants && p.variants.length > 0) ? (p.variants[0].basePrice || 0) : 0;
    div.innerHTML = `
      <div><b>${p.name}</b></div>
      <div>${p.shortDescription || ''}</div>
      ${p.mainImageUrl ? `<img src="${p.mainImageUrl}" style="max-width:200px"/>` : ''}
      <div>
        <button onclick='addToCart("${p.id}", ${variantId ? `"${variantId}"` : null}, ${unitPrice})'>Thêm vào giỏ (biến thể đầu)</button>
      </div>
    `;
    host.appendChild(div);
  });
}

async function addToCart(productId, variantId, unitPrice) {
  if (!ACCESS_TOKEN) { alert('Hãy đăng nhập'); return; }
  const res = await fetch(`${GATEWAY}/api/cart/items`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${ACCESS_TOKEN}` },
    body: JSON.stringify({ productId, productVariantId: variantId, quantity: 1, unitPrice })
  });
  const data = await res.json().catch(()=>({}));
  alert('AddToCart: ' + res.status);
}

async function viewCart() {
  if (!ACCESS_TOKEN) { alert('Hãy đăng nhập'); return; }
  const res = await fetch(`${GATEWAY}/api/cart`, { headers: { 'Authorization': `Bearer ${ACCESS_TOKEN}` } });
  const cart = await res.json();
  const host = document.getElementById('orders');
  host.innerHTML = '';
  const div = document.createElement('div');
  div.className = 'card';
  const lines = (cart.items || []).map(i => `- ${i.productId} x ${i.quantity} = ${i.quantity * i.unitPrice}`);
  div.innerHTML = `Giỏ ${cart.id || ''}<br/>${lines.join('<br/>') || 'Trống'}`;
  host.appendChild(div);
}

async function checkoutFromCart() {
  if (!ACCESS_TOKEN) { alert('Hãy đăng nhập'); return; }
  const res = await fetch(`${GATEWAY}/api/orders/checkout-from-cart`, {
    method: 'POST',
    headers: { 'Authorization': `Bearer ${ACCESS_TOKEN}`, 'Content-Type': 'application/json' },
    body: JSON.stringify('Địa chỉ giao hàng demo')
  });
  const data = await res.json().catch(()=>({}));
  alert('Checkout: ' + res.status);
}

async function viewMyOrders() {
  if (!ACCESS_TOKEN) { alert('Hãy đăng nhập'); return; }
  const res = await fetch(`${GATEWAY}/api/orders/my`, { headers: { 'Authorization': `Bearer ${ACCESS_TOKEN}` } });
  const items = await res.json();
  const host = document.getElementById('orders');
  host.innerHTML = '';
  items.forEach(o => {
    const div = document.createElement('div');
    div.className = 'card';
    div.innerHTML = `Đơn ${o.id}<br/>Trạng thái: ${o.status}<br/>Tổng tiền: ${o.totalAmount}`;
    host.appendChild(div);
  });
}


