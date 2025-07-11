
# Defini variáveis do projeto
$PATH = (Get-Location).ProviderPath
$PROJECT_CONFIGURATION_NAME = "MaCamp.csproj"
$PROJECT_CONFIGURATION_PATH = Join-Path $PATH $PROJECT_CONFIGURATION_NAME
$FRAMEWORK = "net9.0-android"
$CONFIGURATION = "Release"
$ANDROID_PACKAGE_FORMAT = "apk"
$API_KEY="ca-app-pub-8959365990645001~9967771040"
$ANDROID_MANIFEST="Platforms\Android\AndroidManifest.xml"
$PACKAGE_NAME = "br.com.macamp.app"
$ADB = "$env:ANDROID_HOME\platform-tools\adb.exe"

Write-Host "[01/19] Variáveis definidas"

# Verifica se o arquivo CSPROJ existe
if (Test-Path $PROJECT_CONFIGURATION_PATH) {
    Write-Host "[02/19] arquivo csproj encontrado"
} else {
    Write-Host "[02/19] Arquivo csproj não encontrado em $PROJECT_CONFIGURATION_PATH"
	Write-Host "Pressione ESC ou Enter para sair..."
	do {
		$key = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	} while ($key.VirtualKeyCode -ne 13 -and $key.VirtualKeyCode -ne 27)
    exit
}

[xml]$projectConfigurationXml = Get-Content -Path $PROJECT_CONFIGURATION_PATH

$RAW = $projectConfigurationXml.Project.PropertyGroup.RootNamespace
$PROJECT_ID = "$RAW".trim()
$KEYINFO_PATH = Join-Path $PATH "$PROJECT_ID.keyinfo"
$KEYSTORE_PATH = Join-Path $PATH "$PROJECT_ID.keystore"

Write-Host "[03/19] Id do projeto: $PROJECT_ID"

# Verifica se o arquivo KEYINFO existe
if (Test-Path $KEYINFO_PATH) {
    Write-Host "[04/19] arquivo Keyinfo encontrado"
} else {
    Write-Host "[04/19] Arquivo Keyinfo não encontrado em $KEYINFO_PATH"
	Write-Host "Pressione ESC ou Enter para sair..."
	do {
		$key = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	} while ($key.VirtualKeyCode -ne 13 -and $key.VirtualKeyCode -ne 27)
    exit
}

# Verifica se o arquivo KEYSTORE existe
if (Test-Path $KEYSTORE_PATH) {
    Write-Host "[05/19] arquivo Keystore encontrado"
} else {
    Write-Host "[05/19] Arquivo Keystore não encontrado em $KEYSTORE_PATH"
	Write-Host "Pressione ESC ou Enter para sair..."
	do {
		$key = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	} while ($key.VirtualKeyCode -ne 13 -and $key.VirtualKeyCode -ne 27)
    exit
}

# Carrega e lê os dados do XML
[xml]$keyinfoXml = Get-Content -Path $KEYINFO_PATH

$KEY_ALIAS = $keyinfoXml.KeyStore.Alias
$PASSWORD = $keyinfoXml.KeyStore.Password
$RAW = $projectConfigurationXml.Project.PropertyGroup.ApplicationId
$PACKAGE_NAME = "$RAW".trim()

Write-Host "[06/19] Alias = $KEY_ALIAS"
Write-Host "[07/19] Password = <escondido>"
Write-Host "[08/19] Formato = $ANDROID_PACKAGE_FORMAT"
Write-Host "[09/19] Package = $PACKAGE_NAME"

# Verifica se o arquivo AndroidManifest.xml existe
if (Test-Path $ANDROID_MANIFEST) { 
    Write-Host "[10/19] AndroidManifest.xml encontrado" 
} else { 
    Write-Host "[10/19] Arquivo AndroidManifest.xml não encontrado em $ANDROID_MANIFEST"
    Write-Host "Pressione ESC ou Enter para sair..."
	do {
		$key = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	} while ($key.VirtualKeyCode -ne 13 -and $key.VirtualKeyCode -ne 27)
    exit
}

# 2. Carrega o XML e pegar o atributo 'package'
[xml]$xml = Get-Content $ANDROID_MANIFEST

# Captura o valor original do APPLICATION_ID
$ORIGINAL=Select-String -Path $ANDROID_MANIFEST -Pattern 'com\.google\.android\.gms\.ads\.APPLICATION_ID' | ForEach-Object { $_.Line -replace '.*android:value="([^"]+)".*','$1' }
if ($ORIGINAL) {
    Write-Host "[11/19] Valor original = $ORIGINAL" 
} else { 
    Write-Host "[11/19] Não foi possível capturar o valor original"
    Write-Host "Pressione ESC ou Enter para sair..."
	do {
		$key = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	} while ($key.VirtualKeyCode -ne 13 -and $key.VirtualKeyCode -ne 27)
    exit
}

# Substitui o valor do APPLICATION_ID pelo API_KEY
(Get-Content $ANDROID_MANIFEST) -replace 'android:name="com.google.android.gms.ads.APPLICATION_ID" android:value="[^"]+"', "android:name=`"com.google.android.gms.ads.APPLICATION_ID`" android:value=`"$API_KEY`"" | Set-Content $ANDROID_MANIFEST
if ($?) { 
    Write-Host "[12/19] Valor substituído para: $API_KEY" 
} else { 
    Write-Host "[12/19] Falha na substituição do valor"
    Write-Host "Pressione ESC ou Enter para sair..."
	do {
		$key = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	} while ($key.VirtualKeyCode -ne 13 -and $key.VirtualKeyCode -ne 27)
    exit
}

Write-Host "[13/19] Limpando projeto"
dotnet clean -f $FRAMEWORK

Write-Host "[14/19] Restaurando dependências"
dotnet restore

# Mensagem pré-compilação
Write-Host "[15/19] Compilando arquivo $ANDROID_PACKAGE_FORMAT"

# Compila e assina o arquivo
dotnet publish $PATH -f $FRAMEWORK -c $CONFIGURATION -p:AndroidPackageFormat=$ANDROID_PACKAGE_FORMAT -p:AndroidKeyStore=true -p:AndroidSigningKeyAlias=$KEY_ALIAS -p:AndroidSigningKeyPass=$PASSWORD -p:AndroidSigningStorePass=$PASSWORD -p:AndroidSigningKeyStore=$KEYSTORE_PATH
if ($?) {
	Write-Host "[16/19] Build concluído com sucesso"
} else {
    Write-Host "[16/19] Falha no build"
    Write-Host "Pressione ESC ou Enter para sair..."
	do {
		$key = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	} while ($key.VirtualKeyCode -ne 13 -and $key.VirtualKeyCode -ne 27)
    exit
}

# Restaura o valor original no AndroidManifest.xml
(Get-Content $ANDROID_MANIFEST) -replace 'android:name="com.google.android.gms.ads.APPLICATION_ID" android:value="[^"]+"', "android:name=`"com.google.android.gms.ads.APPLICATION_ID`" android:value=`"$ORIGINAL`"" | Set-Content $ANDROID_MANIFEST
if ($?) { 
    Write-Host "[17/19] Valor restaurado para: $ORIGINAL" 
} else {
    Write-Host "[17/19] Falha ao restaurar valor original"
    Write-Host "Pressione ESC ou Enter para sair..."
	do {
		$key = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	} while ($key.VirtualKeyCode -ne 13 -and $key.VirtualKeyCode -ne 27)
    exit
}

# Lista dispositivos/emuladores conectados via ADB
$DEVICES = (& $ADB devices) | Select-String -Pattern "device$"
Write-Host "[18/19] Dispositivos/emuladores conectados: $($DEVICES.Count)"

# Verifica se há algum emulador/dispositivo em execução
if ($DEVICES.Count -gt 0) {
    # Pergunta ao usuário se deseja desinstalar o APK existente e instalar o novo
    $RESP = Read-Host "Deseja desinstalar o APK existente e instalar a nova versão? (S/N)"

    if ($RESP.ToUpper() -eq "S") {
        # Desinstala APK atual do dispositivo/emulador
        & $ADB uninstall $PACKAGE_NAME
        if ($?) {
			Write-Host "APK desinstalado com sucesso."
		} else {
			Write-Host "Falha ao desinstalar APK."
		}

        # Instalar nova versão do APK
        & $ADB install -r "$PATH\bin\$CONFIGURATION\$FRAMEWORK\publish\$PACKAGE_NAME-Signed.apk"
        if ($?) {
			Write-Host "APK instalado com sucesso."
		} else {
			Write-Host "Falha ao instalar APK."
		}
    } else {
        Write-Host "Instalação cancelada pelo usuário."
    }
}

if ($devices.Count -eq 0 -or $RESP.ToUpper() -ne "S") {
	# Informa onde o arquivo foi gerado
	$FILE_PATH = Join-Path -Path "$PATH\bin\$CONFIGURATION\$FRAMEWORK" -ChildPath "publish"
	Write-Host "[19/19] Arquivo gerado em: $FILE_PATH"

	# Aguarda ESC ou Enter para sair
	Write-Host "Pressione ESC ou Enter para abrir o diretório e sair..."
	do {
		$key = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	} while ($key.VirtualKeyCode -ne 13 -and $key.VirtualKeyCode -ne 27)

	# Abre o diretório no Explorer
	ii $FILE_PATH
}

# Encerra o terminal
exit
