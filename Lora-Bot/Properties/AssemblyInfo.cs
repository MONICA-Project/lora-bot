﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Allgemeine Informationen über eine Assembly werden über die folgenden
// Attribute gesteuert. Ändern Sie diese Attributwerte, um die Informationen zu ändern,
// die einer Assembly zugeordnet sind.
[assembly: AssemblyTitle("LoraDisplay")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("LoraDisplay")]
[assembly: AssemblyCopyright("Copyright ©  2018 - 10.10.2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Durch Festlegen von ComVisible auf FALSE werden die Typen in dieser Assembly
// für COM-Komponenten unsichtbar.  Wenn Sie auf einen Typ in dieser Assembly von
// COM aus zugreifen müssen, sollten Sie das ComVisible-Attribut für diesen Typ auf "True" festlegen.
[assembly: ComVisible(false)]

// Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM verfügbar gemacht wird
[assembly: Guid("fb11b997-af4d-427f-96fd-4c84143057cf")]

// Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion
//      Buildnummer
//      Revision
//
// Sie können alle Werte angeben oder Standardwerte für die Build- und Revisionsnummern verwenden,
// übernehmen, indem Sie "*" eingeben:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.6.2")]
[assembly: AssemblyFileVersion("1.6.2")]
/*
 * 1.1.0 Update Scral addresses
 * 1.2.0 Run Module Events in threads so that one Module can not block others, TXTOut now appends to the logfile
 * 1.3.0 Scral now get its config from configfile, lora now want to get battery as [0-9].[0-9]{2} value
 * 1.4.0 Adding Debugmode for finetuning Lora-Trackers
 * 1.4.1 Remove old Wirelesscode and Rename some Classes
 * 1.5.0 Send over Mqtt the new status items and refactoring
 * 1.5.1 Dependencies in debian Packet cleaned
 * 1.6.0 Implement Height in LoraBot
 * 1.6.1 Fixing parsing bug with linebreaks in Lora
 * 1.6.2 Adding a Test for Lora Binary
 */
