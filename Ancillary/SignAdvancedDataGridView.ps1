#Signs AdvancedDataGridView.dll to give it a strong name
$key = Import-StrongNameKeyPair -KeyFile AdventureWorks.snk;
#write-host $key.PublicKey;
dir -rec ..\packages\ADGV.0.1.0.10\lib\net40\*.dll | where { -not (test-strongname $_) } | set-strongname -keypair $key -verbose;
dir -rec ..\Lib\TabStrip.dll | where { -not (test-strongname $_) } | set-strongname -keypair $key -verbose;
