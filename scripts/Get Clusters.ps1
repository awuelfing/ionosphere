$URI = "https://www.n1mm-lib.hamdocs.com/_getfromclusterratings.php"
$Regex = "(?<Continent>\w{2})\s-\s(?<DisplayName>[\w\.:\d]+)\s\((?<Reports>\d+)\s\/\s(?<Success>\d+)%\),\s(?<Host>[\w\.]+):(?<Port>\d+)"

$Request = Invoke-WebRequest -Uri $URI

$RegMatches = [Regex]::Matches($Request.Content,$Regex)

$Results = foreach($Match in $RegMatches){
    [PSCustomObject]@{
        Continent = $Match.Groups["Continent"].Value;
        DisplayName = $Match.Groups["DisplayName"].Value;
        Reports = $Match.Groups["Reports"].Value;
        Success = $Match.Groups["Success"].Value;
        Host = $Match.Groups["Host"].Value;
        Port = $Match.Groups["Port"].Value;
    }
}
$Results | Export-Csv -NoTypeInformation -Path "Clusters.csv" -Delimiter "," -UseQuotes Never