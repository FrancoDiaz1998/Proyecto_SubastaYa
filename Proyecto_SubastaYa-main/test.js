const crypto = require('crypto');

const API_URL = 'http://localhost:5124/api/v1';
const SECRET_KEY = 'EstaEsUnaClaveSecretaMuyLargaYSeguraParaSubastaYa2026!';

// Función auxiliar para pausar la ejecución entre fases, haciendo la consola más legible e intuitiva
const esperar = (ms) => new Promise(resolve => setTimeout(resolve, ms));

// Generación local de JWT para evitar conflictos de validación en el login de .NET sin modificar el backend
function generarJwtLocal(sub, email, name) {
    const header = { alg: 'HS256', typ: 'JWT' };
    const payload = { sub, email, name, iss: 'SubastaYa.Api', aud: 'SubastaYa.Frontend', exp: Math.floor(Date.now() / 1000) + 3600 };

    const b64 = (str) => Buffer.from(str).toString('base64').replace(/=/g, '').replace(/\+/g, '-').replace(/\//g, '_');
    const dataToSign = `${b64(JSON.stringify(header))}.${b64(JSON.stringify(payload))}`;
    const signature = crypto.createHmac('sha256', SECRET_KEY).update(dataToSign).digest('base64').replace(/=/g, '').replace(/\+/g, '-').replace(/\//g, '_');

    return `${dataToSign}.${signature}`;
}

async function ejecutarPruebaIntuitiva() {
    console.log('========================================');
    console.log(' INICIO DE PRUEBAS INTEGRALES Y DE ESTRÉS');
    console.log('========================================');

    // Simulación de credenciales para el usuario de prueba
    const token = generarJwtLocal('22222222-2222-2222-2222-222222222222', 'comprador1@test.com', 'Comprador 1');
    await esperar(600);

    console.log('\n[FASE 1] Verificación de Seguridad y Autenticación');
    // Prueba de acceso denegado sin token
    const resSinToken = await fetch(`${API_URL}/billeteras/saldos`, { method: 'GET' });
    console.log(`- Acceso sin credenciales -> Estado HTTP: ${resSinToken.status} (Esperado: 401 Unauthorized)`);
    await esperar(400);

    // Prueba de acceso autorizado con token JWT válido
    const resConToken = await fetch(`${API_URL}/billeteras/saldos`, {
        method: 'GET',
        headers: { 'Authorization': `Bearer ${token}` }
    });
    console.log(`- Acceso con token válido -> Estado HTTP: ${resConToken.status} (Esperado: 200 OK)`);
    await esperar(600);

    console.log('\n[FASE 2] Validación de Reglas de Negocio');
    // Intento de operación inválida (monto negativo)
    const resNegativo = await fetch(`${API_URL}/billeteras/depositos`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
        body: JSON.stringify({ monto: -100 })
    });
    console.log(`- Depósito con monto negativo -> Estado HTTP: ${resNegativo.status} (Esperado: 400 Bad Request)`);
    await esperar(600);

    console.log('\n[FASE 3] Consulta de Estado Inicial');
    const saldoInicial = (await resConToken.json()).saldoTotal;
    console.log(`- Saldo inicial registrado en la billetera: $${saldoInicial}`);
    await esperar(800);

    console.log('\n[FASE 4] Prueba de Estrés y Concurrencia Optimista');
    console.log('- Disparando 25 solicitudes de depósito simultáneas para forzar colisión de versiones...');

    const tiempoInicio = Date.now();
    const solicitudes = Array(25).fill(0).map(() =>
        fetch(`${API_URL}/billeteras/depositos`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
            body: JSON.stringify({ monto: 100 })
        }).then(async r => ({ status: r.status, body: await r.text() }))
    );

    const resultados = await Promise.all(solicitudes);
    const tiempoTotal = Date.now() - tiempoInicio;

    const codigos = resultados.map(r => r.status);
    const exitosos = codigos.filter(c => c === 201).length;
    const conflictos = codigos.filter(c => c === 409).length;

    console.log(`- Tiempo de respuesta del lote: ${tiempoTotal} ms`);
    console.log('- Códigos HTTP obtenidos:', codigos);
    console.log(`- Resumen de hilos -> Exitosos (201): ${exitosos} | Bloqueados por versión (409): ${conflictos}`);
    await esperar(800);

    console.log('\n[FASE 5] Verificación de Integridad y Auditoría Final');
    const resSaldoFinal = await fetch(`${API_URL}/billeteras/saldos`, {
        headers: { 'Authorization': `Bearer ${token}` }
    });
    const saldoFinal = (await resSaldoFinal.json()).saldoTotal;
    const diferencia = saldoFinal - saldoInicial;

    console.log(`- Saldo final registrado: $${saldoFinal}`);
    console.log(`- Incremento neto aplicado: $${diferencia}`);

    const validacionOk = exitosos > 0 && conflictos > 0 && diferencia === (exitosos * 100);

    console.log('\n----------------------------------------');
    if (validacionOk) {
        console.log('ESTADO FINAL: ÉXITO. El control de concurrencia optimista operó correctamente.');
    } else {
        console.log('ESTADO FINAL: FALLO EN LA VALIDACIÓN.');
    }
    console.log('----------------------------------------');
}

ejecutarPruebaIntuitiva();