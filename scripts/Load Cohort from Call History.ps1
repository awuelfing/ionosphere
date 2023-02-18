$file = "C:\Users\user\source\repos\Ionosphere\ignore\callhistoryfiles\YCCC call history file.txt"
$bearerfile = "C:\Users\user\source\repos\Ionosphere\ignore\bearer\bearer.txt"
$base_uri = "https://localhost:7100/api/cohort/appendone"
$user = "Admin"

$token = [System.IO.File]::ReadAllText($bearerfile)

$headers = @{Authorization="Bearer $token"}

$callsigns = [System.IO.File]::ReadAllLines($file) | Where-Object {$PSItem -notlike "#*" -and $PSItem -notlike "!*"}
foreach($call in $callsigns)
{
    $first = ($call -split ",")[0]
    $fulluri = "$($base_uri)?Username=$($user)&Callsign=$($first)"
    Write-Host $fulluri
    Invoke-WebRequest -Uri $fulluri -Headers $headers
}