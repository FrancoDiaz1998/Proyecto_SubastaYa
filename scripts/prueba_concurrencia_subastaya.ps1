param(
    [string]$BaseUrl = "http://localhost:5124"
)

$ErrorActionPreference = "Stop"

# Windows PowerShell 5.1 no siempre carga este ensamblado automaticamente.
Add-Type -AssemblyName System.Net.Http

function Login([string]$email) {
    $body = @{
        email = $email
        password = "Demo123!"
    } | ConvertTo-Json

    return Invoke-RestMethod `
        -Method Post `
        -Uri "$BaseUrl/api/v1/auth/login" `
        -ContentType "application/json" `
        -Body $body
}

Write-Host "=== SubastaYa - Prueba de concurrencia optimista ===" -ForegroundColor Cyan
Write-Host "API: $BaseUrl"

Write-Host "`n1) Obteniendo tokens..."
$vendedor = Login "vendedor@test.com"
$comprador1 = Login "comprador1@test.com"
$comprador2 = Login "comprador2@test.com"

Write-Host "Tokens obtenidos." -ForegroundColor Green

Write-Host "`n2) Creando una subasta nueva para que la prueba sea repetible..."

$ahora = [DateTimeOffset]::UtcNow
$crearBody = @{
    titulo = "Prueba concurrencia $(Get-Date -Format 'HHmmss')"
    descripcion = "Subasta temporal creada automaticamente para probar optimistic locking."
    urlImagen = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8"
    categoriaId = -1
    precioBase = 10000
    incrementoMinimo = 1000
    fechaInicio = $ahora.AddSeconds(-10).ToString("o")
    fechaFin = $ahora.AddMinutes(10).ToString("o")
} | ConvertTo-Json

$crearHeaders = @{
    Authorization = "Bearer $($vendedor.token)"
}

$subasta = Invoke-RestMethod `
    -Method Post `
    -Uri "$BaseUrl/api/v1/subastas" `
    -Headers $crearHeaders `
    -ContentType "application/json" `
    -Body $crearBody

$subastaId = $subasta.id
$monto = 11000

Write-Host "Subasta creada: ID $subastaId" -ForegroundColor Green
Write-Host "Las dos solicitudes ofertaran exactamente: $monto"

Write-Host "`n3) Disparando DOS POST sin esperar la respuesta del primero..."

$client = New-Object System.Net.Http.HttpClient

function CrearRequestPuja([string]$token, [int]$id, [decimal]$montoPuja) {
    $request = New-Object System.Net.Http.HttpRequestMessage(
        [System.Net.Http.HttpMethod]::Post,
        "$BaseUrl/api/v1/subastas/$id/pujas"
    )

    $request.Headers.Authorization =
        New-Object System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $token)

    $json = @{ monto = $montoPuja } | ConvertTo-Json -Compress
    $request.Content = New-Object System.Net.Http.StringContent(
        $json,
        [System.Text.Encoding]::UTF8,
        "application/json"
    )

    return $request
}

$request1 = CrearRequestPuja $comprador1.token $subastaId $monto
$request2 = CrearRequestPuja $comprador2.token $subastaId $monto

$inicioMs = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()

$task1 = $client.SendAsync($request1)
$task2 = $client.SendAsync($request2)

[System.Threading.Tasks.Task]::WaitAll([System.Threading.Tasks.Task[]]@($task1, $task2))

$finMs = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()

$response1 = $task1.Result
$response2 = $task2.Result

$body1 = $response1.Content.ReadAsStringAsync().GetAwaiter().GetResult()
$body2 = $response2.Content.ReadAsStringAsync().GetAwaiter().GetResult()

Write-Host "`n=== RESULTADOS ===" -ForegroundColor Cyan
Write-Host "Ventana total de la prueba: $($finMs - $inicioMs) ms"

Write-Host "`nComprador 1 -> HTTP $([int]$response1.StatusCode) $($response1.StatusCode)"
Write-Host $body1

Write-Host "`nComprador 2 -> HTTP $([int]$response2.StatusCode) $($response2.StatusCode)"
Write-Host $body2

Write-Host "`n4) Verificando el estado final de la subasta..."
$detalle = Invoke-RestMethod -Method Get -Uri "$BaseUrl/api/v1/subastas/$subastaId"

$pujasIguales = @(
    $detalle.historialPujas |
    Where-Object { [decimal]$_.monto -eq $monto }
)

Write-Host "Cantidad de pujas de $monto persistidas: $($pujasIguales.Count)"
Write-Host "Puja actual: $($detalle.pujaActual)"
Write-Host "Cantidad total de pujas: $($detalle.cantidadPujas)"

$codigos = @([int]$response1.StatusCode, [int]$response2.StatusCode)

if (
    (($codigos -contains 201) -or ($codigos -contains 200)) -and
    ($codigos -contains 409) -and
    ($pujasIguales.Count -eq 1)
) {
    Write-Host "`nPRUEBA OK: una solicitud fue aceptada, la otra fue rechazada y solo una puja quedo persistida." -ForegroundColor Green
}
else {
    Write-Host "`nLa ejecucion no mostro exactamente una aceptada + una 409 + una sola puja." -ForegroundColor Yellow
    Write-Host "Si el segundo request llego despues del commit del primero, puede haber sido rechazado por regla de monto en vez del token de concurrencia."
}

$client.Dispose()
