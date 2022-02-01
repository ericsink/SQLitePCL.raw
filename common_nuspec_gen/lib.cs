/*
   Copyright 2014-2019 SourceGear, LLC

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

public enum TFM
{
    NONE,
    IOS,
    TVOS,
    ANDROID,
    UWP,
    WIN10,
    NETSTANDARD20,
    NET461,
    XAMARIN_MAC,
    NETCOREAPP31,
    NET50,
}

public static class common
{
    public const string ROOT_NAME = "SQLitePCLRaw";

    public static string AsString(this TFM e)
    {
        switch (e)
        {
            case TFM.NONE: throw new Exception("TFM.NONE.AsString()");
            case TFM.IOS: return "Xamarin.iOS10";
            case TFM.TVOS: return "net6-tvos10";
            case TFM.ANDROID: return "MonoAndroid80";
            case TFM.UWP: return "uap10.0";
            case TFM.WIN10: return "net6.0-windows10";
            case TFM.NETSTANDARD20: return "netstandard2.0";
            case TFM.XAMARIN_MAC: return "Xamarin.Mac20";
            case TFM.NET461: return "net461";
            case TFM.NETCOREAPP31: return "netcoreapp3.1";
            case TFM.NET50: return "net5.0";
            default:
                throw new NotImplementedException(string.Format("TFM.AsString for {0}", e));
        }
    }

    public static void write_nuspec_file_entry(string src, string target, XmlWriter f)
    {
        f.WriteStartElement("file");
        f.WriteAttributeString("src", src);
        f.WriteAttributeString("target", target);
        f.WriteEndElement(); // file
    }

    public static void write_empty(XmlWriter f, TFM tfm)
    {
        f.WriteComment("empty directory in lib to avoid nuget adding a reference");

        f.WriteStartElement("file");
        f.WriteAttributeString("src", "_._");
        f.WriteAttributeString("target", string.Format("lib/{0}/_._", tfm.AsString()));
        f.WriteEndElement(); // file
    }

    public static void add_dep_core(XmlWriter f)
    {
        f.WriteStartElement("dependency");
        f.WriteAttributeString("id", string.Format("{0}.core", ROOT_NAME));
        f.WriteAttributeString("version", "$version$");
        f.WriteEndElement(); // dependency
    }

    const string PACKAGE_TAGS = "sqlite;xamarin";

    public static void write_nuspec_common_metadata(
        string id,
        XmlWriter f
        )
    {
        f.WriteAttributeString("minClientVersion", "2.12"); // TODO not sure this is right

        f.WriteElementString("id", id);
        f.WriteElementString("title", id);
        f.WriteElementString("version", "$version$");
        f.WriteElementString("authors", "$authors$");
        f.WriteElementString("copyright", "$copyright$");
        f.WriteElementString("requireLicenseAcceptance", "false");
        write_license(f);
        f.WriteStartElement("repository");
        f.WriteAttributeString("type", "git");
        f.WriteAttributeString("url", "https://github.com/ericsink/SQLitePCL.raw");
        f.WriteEndElement(); // repository
        f.WriteElementString("summary", "$summary$");
        f.WriteElementString("tags", PACKAGE_TAGS);
    }

    public static XmlWriterSettings XmlWriterSettings_default()
    {
        var settings = new XmlWriterSettings();
        settings.NewLineChars = "\n";
        settings.Indent = true;
        return settings;
    }

    public static void gen_dummy_csproj(string dir_proj, string id)
    {
        var settings = XmlWriterSettings_default();
        settings.OmitXmlDeclaration = true;

        using (XmlWriter f = XmlWriter.Create(Path.Combine(dir_proj, $"{id}.csproj"), settings))
        {
            f.WriteStartDocument();
            f.WriteComment("Automatically generated");

            f.WriteStartElement("Project");
            f.WriteAttributeString("Sdk", "Microsoft.NET.Sdk");

            f.WriteStartElement("PropertyGroup");

            f.WriteElementString("TargetFramework", "netstandard2.0");
            f.WriteElementString("NoBuild", "true");
            f.WriteElementString("IncludeBuildOutput", "false");
            f.WriteElementString("NuspecFile", $"{id}.nuspec");
            f.WriteElementString("NuspecProperties", "version=$(version);src_path=$(src_path);cb_bin_path=$(cb_bin_path);authors=$(Authors);copyright=$(Copyright);summary=$(Description)");

            f.WriteEndElement(); // PropertyGroup

            f.WriteEndElement(); // Project

            f.WriteEndDocument();
        }
    }

    private static void write_license(XmlWriter f)
    {
        f.WriteStartElement("license");
        f.WriteAttributeString("type", "expression");
        f.WriteString("Apache-2.0");
        f.WriteEndElement();
    }

}

