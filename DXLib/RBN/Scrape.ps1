

$y = Invoke-WebRequest -Uri "https://www.reversebeacon.net/cont_includes/status.php?t=skt"
#better to anchor to tags instead of linefeed but here we are
$Regex = ".*\n\s+<td><a.*>\s*(?<Station>[\w-]+).*\n\n.*\n.*\n\s*<td>(?<QTH>\w+).*\n.*\n.*\n\s*(?<PrimaryPrefix>\w+).*\n.*\n.*\s+(?<Continent>\w+).*\n.*\n.*\n\s+(?<ITUZone>\w+).*\n.*\n.*\n\s+(?<CQZone>\w+)"
$RegMatches = [Regex]::Matches($y.Content,$Regex)


$Results = foreach($Match in $RegMatches){
    #"Station:$($match.Groups["Station"].Value) - QTH $($match.Groups["QTH"].Value)"
    [PSCustomObject]@{
        Station = $Match.Groups["Station"].Value;
        QTH = $Match.Groups["QTH"].Value;
        PrimaryPrefix = $Match.Groups["PrimaryPrefix"].Value;
        Continent = $Match.Groups["Continent"].Value;
        CQZone = $Match.Groups["CQZone"].Value;
        ITUZone = $Match.Groups["ITUZone"].Value;
    }
}

$Results | Export-Csv -NoTypeInformation -Path "RBN.csv" -Delimiter "," -UseQuotes Never