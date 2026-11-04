param([string] $UnityEditorRoot = 'C:/Program Files/Unity/Hub/Editor/6000.3.10f1/Editor')
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$mono = Join-Path $UnityEditorRoot 'Data/MonoBleedingEdge/bin/mono.exe'
$compiler = Join-Path $UnityEditorRoot 'Data/MonoBleedingEdge/lib/mono/4.5/mcs.exe'
$outputDir = Join-Path $projectRoot 'Temp/VoiceCorrectionTests'
New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
$testProgram = Join-Path $outputDir 'VoiceCorrections.Tests.exe'
$sourceFiles = @(
    'Tests/Support/VoiceCorrectionStubs.cs',
    'Tests/VoiceCorrections.Tests.cs',
    'Assets/Scripts/VoiceRecognizer/VoiceKeywordMatcher.cs',
    'Assets/Scripts/VoiceRecognizer/VoiceCommandParser.cs',
    'Assets/Scripts/VoiceRecognizer/VoiceInput.cs',
    'Assets/Scripts/MenuScripts/Pause/SimplePause.cs',
    'Assets/Scripts/MenuListener.cs'
) | ForEach-Object { Join-Path $projectRoot $_ }
& $mono $compiler '-langversion:latest' "-out:$testProgram" $sourceFiles
if ($LASTEXITCODE -ne 0) { throw 'Voice correction test compilation failed' }
& $mono $testProgram
if ($LASTEXITCODE -ne 0) { throw 'Voice correction regression tests failed' }
