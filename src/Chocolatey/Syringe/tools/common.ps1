function Test-IisInstalled()
{
    $service = Get-WmiObject -Class Win32_Service -Filter "Name='w3svc'";
    if ($service)
    {
        write-host $service.Status
        if ($service.Status -eq "OK")
        {
            return $True;
        }
    }

    return $False;
}


function Parse-Parameters($arguments)
{
    $packageParameters = $env:chocolateyPackageParameters
    Write-Host "Package parameters: $packageParameters"

    if ($packageParameters)
    {
          $match_pattern = "(?:\s*)(?<=[-|/])(?<name>\w*)[:|=]('((?<value>.*?)(?<!\\)')|(?<value>[\w]*))"

          if ($packageParameters -match $match_pattern )
          {
              $results = $packageParameters | Select-String $match_pattern -AllMatches
              $results.matches | % {

                $key = $_.Groups["name"].Value.Trim();
                $value = $_.Groups["value"].Value.Trim();

                write-host "$key : $value";

                if ($arguments.ContainsKey($key))
                {
                    $arguments[$key] = $value;
                }
            }
          }
    }
}