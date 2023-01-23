

$y = Invoke-WebRequest -Uri "https://www.reversebeacon.net/cont_includes/status.php?t=skt"
#better to anchor to tags instead of linefeed but here we are
$Regex = ".*\n\s+<td><a.*>\s*(?<Station>[\w-]+).*\n\n.*\n.*\n\s*<td>(?<QTH>\w+).*\n.*\n.*\n\s*(?<PrimaryPrefix>\w+)"
$RegMatches = [Regex]::Matches($y.Content,$Regex)


$Results = foreach($Match in $RegMatches){
    #"Station:$($match.Groups["Station"].Value) - QTH $($match.Groups["QTH"].Value)"
    [PSCustomObject]@{
        Station = $Match.Groups["Station"].Value;
        QTH = $Match.Groups["QTH"].Value;
        PrimaryPrefix = $Match.Groups["PrimaryPrefix"].Value;
    }
}

$Results | Export-Csv -NoTypeInformation -Path "RBN.csv" -Delimiter ","