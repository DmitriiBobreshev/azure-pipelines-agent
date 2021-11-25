function Get-Signtool() {
  <#
    .SYNOPSIS
    Function used to get signtool from windows SDK
  #>

  $systemBit = "x64"
  $programFiles = ${Env:ProgramFiles(x86)}

  if((Get-WmiObject Win32_Processor).AddressWidth -eq 64) {
    $systemBit = "x86"
    $programFiles = ${Env:ProgramFiles}
  }

  $signtoolPath = ""
  try {
    $windowsSdkPath=Get-ChildItem "$programFiles\Windows Kits\10\bin\1*" | Select-Object FullName | Sort-Object -Descending { [version](Split-Path $_.FullName -leaf)} | Select-Object -first 1

    $signtoolPath = "$($windowsSdkPath.FullName)\$systemBit\signtool.exe"
    return $signtoolPath
  } catch {
    Write-Host "##vso[task.logissue type=error]Unbable to get signtool in $signtoolPath"
    exit 1
  }
}
