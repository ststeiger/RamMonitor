
REM https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/sc-create
REM https://docs.oracle.com/cd/E29805_01/server.230/es_admin/src/radm_service_sc_syntax.html
REM installutil <yourproject>.exe
REM installutil /u <yourproject>.exe
REM sc create MyBinary binpath="c:\windows\system32\NewServ.exe" type=share start=auto depend= "+TDI NetBIOS"
REM sc create svnserve 
REM    binpath= "\"C:\Program Files\CollabNet Subversion Server\svnserve.exe\" --service -r \"C:\my repositories\"  "
REM    displayname= "Subversion Server" depend= Tcpip start= auto 


sc create CorRamMonitor DisplayName="COR RamMonitor" binpath="c:\path\to\RamMonitor.exe" start=auto
sc description "CorRamMonitor" "This is my service description"
sc config CorRamMonitor type=interact type=own

REM https://stackoverflow.com/questions/15085856/using-sc-to-install-a-windows-service-and-then-set-recovery-properties
REM https://docs.microsoft.com/en-us/previous-versions/windows/it-pro/windows-server-2012-r2-and-2012/cc742019(v%3Dws.11)
REM https://stackoverflow.com/questions/22872510/how-to-use-sc-to-install-a-service-and-specify-no-action-for-subsequent-failu
sc failure CorRamMonitor reset=86400 actions=restart/5000/restart/5000/restart/5000

sc start CorRamMonitor 



REM sc.exe delete "CorRamMonitor"
