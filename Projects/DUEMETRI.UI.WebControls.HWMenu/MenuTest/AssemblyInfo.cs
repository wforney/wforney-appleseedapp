using System.Reflection;
using System.Runtime.CompilerServices;

//
// Le informazioni generali relative a un assembly sono controllate dal seguente 
// insieme di attributi. Per modificare le informazioni associate a un assembly
// occorre quindi modificare i valori di questi attributi.
//
[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// Le informazioni sulla versione di un assembly sono costituite dai seguenti valori:
//
//      Numero di versione principale
//      Numero di versione secondario 
//      Numero revisione
//      Numero build
//
// � possibile specificare tutti i valori oppure impostare valori predefiniti per i numeri relativi alla revisione e alla build 
// utilizzando il carattere "*" come mostrato di seguito:

[assembly: AssemblyVersion("1.0.*")]

//
// Per firmare l'assembly � necessario specificare una chiave da utilizzare.
// Fare riferimento alla documentazione di Microsoft .NET Framework per ulteriori informazioni sulla firma degli assembly.
//
// Utilizzare gli attributi elencati di seguito per verificare la chiave utilizzata per la firma. 
//
// Note: 
//   (*) Se non � specificata alcuna chiave, non sar� possibile firmare l'assembly.
//   (*) KeyName fa riferimento a una chiave installata nel provider di servizi di
//       crittografia (CSP) sul computer in uso. KeyFile fa riferimento a un file che contiene
//       una chiave.
//   (*) Se entrambi i valori KeyFile e KeyName sono specificati, si 
//       verificher� il seguente processo:
//       (1) Se KeyName � presente in CSP, verr� utilizzata tale chiave.
//       (2) Se KeyName non esiste e KeyFile esiste, la chiave 
//           di KeyFile verr� installata nel CSP e utilizzata.
//   (*) Per creare un KeyFile, � possibile utilizzare l'utilit� sn.exe (Strong Name).
//       Quando si specifica il KeyFile, il percorso dovr� essere
//        relativo alla "directory di output del progetto". Il percorso della directory di output
//        del progetto varia a seconda che il progetto in uso sia un progetto locale o Web.
//        Per i progetti locali, la directory di output del progetto � definita come
//       <Project Directory>\obj\<Configuration>. Se ad esempio il KeyFile si
//       trova nella directory del progetto, occorre specificare l'attributo AssemblyKeyFile 
//       come [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//        Per i progetti Web, la directory di output del progetto � definita come
//       %HOMEPATH%\VSWebCache\<Machine Name>\<Project Directory>\obj\<Configuration>.
//   (*) La firma ritardata � un'opzione avanzata. Vedere la documentazione di Microsoft
//       .NET Framework per ulteriori informazioni.
//
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]
