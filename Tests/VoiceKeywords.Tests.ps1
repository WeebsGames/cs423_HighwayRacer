$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$voiceSource = Get-Content -LiteralPath (Join-Path $projectRoot 'Assets/Scripts/VoiceRecognizer/VoiceInput.cs') -Raw
$sceneSource = Get-Content -LiteralPath (Join-Path $projectRoot 'Assets/Scenes/Track1.unity') -Raw
Add-Type -Path (Join-Path $projectRoot 'Assets/Scripts/VoiceRecognizer/VoiceKeywordMatcher.cs')

$script:assertions = 0
function Assert-True([bool] $condition, [string] $message) {
    if (-not $condition) { throw $message }
    $script:assertions++
}

$commandGroups = @{}
$keywordCount = 0
foreach ($field in @('boostKeyword', 'shieldKeyword', 'sirenKeyword', 'pauseKeyword')) {
    $declaration = [regex]::Match($voiceSource, ('public string\[\] ' + $field + ' = \{([^}]+)\}'))
    Assert-True $declaration.Success "Missing C# keyword list: $field"
    [string[]] $defaults = @([regex]::Matches($declaration.Groups[1].Value, '"([^"]+)"') | ForEach-Object { $_.Groups[1].Value })
    $savedList = [regex]::Match($sceneSource, ('(?m)^  ' + $field + ':\r?\n((?:  - [^\r\n]+\r?\n)+)'))
    Assert-True $savedList.Success "Missing serialized keyword list: $field"
    [string[]] $saved = @([regex]::Matches($savedList.Groups[1].Value, '(?m)^  - ([^\r\n]+)') | ForEach-Object { $_.Groups[1].Value })
    Assert-True (($defaults -join '|') -ceq ($saved -join '|')) "Track1 overrides the C# defaults for $field"
    $commandGroups[$field] = $saved

    foreach ($keyword in $defaults) {
        $match = [VoiceKeywordMatcher]::FindMatch($saved, @($keyword))
        Assert-True ($null -ne $match) "Saved $field does not recognize '$keyword'"
        $keywordCount++
    }
}

# These are the commands highlighted in the teammate's bug report.
$reported = @{
    boostKeyword = @('faster', 'go fast', 'nitro')
    shieldKeyword = @('protection')
    sirenKeyword = @('three', 'alarm', 'emergency lights', 'move other cars away', 'out of the way')
    pauseKeyword = @('freeze')
}
foreach ($field in $reported.Keys) {
    foreach ($phrase in $reported[$field]) {
        $match = [VoiceKeywordMatcher]::FindMatch($commandGroups[$field], @('unrelated', 'nothing here', 'no command', $phrase))
        Assert-True ($null -ne $match) "Missed reported command '$phrase' in the fourth alternative"
    }
}

foreach ($phrase in @('please GO   FAST!', "go`nfast", 'go-fast')) {
    Assert-True ($null -ne [VoiceKeywordMatcher]::FindMatch($commandGroups.boostKeyword, @($phrase))) "Missed spacing/punctuation variant '$phrase'"
}
foreach ($phrase in @('MOVE, OTHER CARS AWAY!', "move`nother  cars away")) {
    Assert-True ($null -ne [VoiceKeywordMatcher]::FindMatch($commandGroups.sirenKeyword, @($phrase))) "Missed siren phrase '$phrase'"
}
Assert-True ($null -ne [VoiceKeywordMatcher]::FindMatch($commandGroups.shieldKeyword, @('prepare the power up that protects my car'))) 'Missed unhyphenated power-up phrase'
Assert-True ($null -ne [VoiceKeywordMatcher]::FindMatch($commandGroups.boostKeyword, @('1'))) 'Missed numeric boost command'
Assert-True ($null -ne [VoiceKeywordMatcher]::FindMatch($commandGroups.shieldKeyword, @('2'))) 'Missed numeric shield command'
Assert-True ($null -ne [VoiceKeywordMatcher]::FindMatch($commandGroups.sirenKeyword, @('3'))) 'Missed numeric siren command'
foreach ($phrase in @('two', '2')) {
    Assert-True ($null -eq [VoiceKeywordMatcher]::FindMatch($commandGroups.sirenKeyword, @($phrase))) "Shield command '$phrase' incorrectly activates the siren"
}
foreach ($phrase in @('speedometer', 'someone', 'stone', 'booster')) {
    Assert-True ($null -eq [VoiceKeywordMatcher]::FindMatch($commandGroups.boostKeyword, @($phrase))) "Substring '$phrase' incorrectly activates boost"
}
Assert-True ($null -eq [VoiceKeywordMatcher]::FindMatch($commandGroups.pauseKeyword, @('freezer'))) 'Substring incorrectly pauses the game'
Assert-True ($null -eq [VoiceKeywordMatcher]::FindMatch(@($null, '', ' '), @('boost'))) 'Empty keyword matched'
Assert-True ($null -eq [VoiceKeywordMatcher]::FindMatch($null, @('boost'))) 'Null keyword list matched'
Assert-True ($null -eq [VoiceKeywordMatcher]::FindMatch($commandGroups.boostKeyword, $null)) 'Null transcript list matched'
Assert-True ($null -eq [VoiceKeywordMatcher]::FindMatch($commandGroups.boostKeyword, @($null, '', ' '))) 'Empty transcript matched'
Assert-True ($null -ne [VoiceKeywordMatcher]::FindMatch(@('start'), @('please START!'))) 'Menu start command failed'
Assert-True ($null -eq [VoiceKeywordMatcher]::FindMatch(@('race'), @('embrace'))) 'Menu command matched inside another word'

Write-Output "PASS: $script:assertions assertions, including all $keywordCount saved gameplay keywords and the 10 reported commands."
