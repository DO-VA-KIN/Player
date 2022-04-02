using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Player
{
    public class Settings
    {
        private const string settingsFileName = "Settings.xml";

        public static bool messagesActive { get; set; }
        public static bool warningDelPlaylist { get; set; }
        public static bool warningDelTrack { get; set; }
        public static bool warningExistPlaylist { get; set; }
        public static bool musicAtTheStart { get; set; }
        public static bool resize { get; set; }

        public static Settings GetInstance { get; } = new Settings();

        public static void SetDefault()
        {
            messagesActive = true;
            warningDelPlaylist = true;
            warningDelTrack = true;
            warningExistPlaylist = true;
            musicAtTheStart = false;
        }



        public String exceptionMessage = "";

        public bool InitializeSettings()
        {
            if (!File.Exists(settingsFileName))
            {
                SetDefault();
                return SaveSettings();
            }
            else
            {
                try
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        ConformanceLevel = ConformanceLevel.Fragment,
                        IgnoreWhitespace = true,
                        IgnoreComments = true
                    };

                    using (XmlReader reader = XmlReader.Create("Settings.xml", settings))
                    {
                        reader.MoveToElement();

                        XDocument doc = XDocument.Load(reader);

                        messagesActive = bool.Parse(doc.Element("Player_Settings").Attribute("messagesActive").Value);
                        warningDelPlaylist = bool.Parse(doc.Element("Player_Settings").Element("warnings").Attribute("warningDelPlaylist").Value);
                        warningDelTrack = bool.Parse(doc.Element("Player_Settings").Element("warnings").Attribute("warningDelTrack").Value);
                        warningExistPlaylist = bool.Parse(doc.Element("Player_Settings").Element("warnings").Attribute("warningExistPlaylist").Value);
                        musicAtTheStart = bool.Parse(doc.Element("Player_Settings").Attribute("musicAtTheStart").Value);
                    }
                }
                catch
                {
                    SetDefault();
                    try { File.Delete(settingsFileName); } catch { return false; }
                    SaveSettings();
                    return false;
                }
            }
            return true;
        }



        public bool SaveSettings()
        {
            if (!File.Exists(settingsFileName))
            {
                try
                {
                    XDocument doc = new XDocument(
                        new XElement("Player_Settings",
                            new XAttribute("messagesActive", Settings.messagesActive),
                            new XElement("warnings",
                                new XAttribute("warningDelPlaylist", Settings.warningDelPlaylist),
                                new XAttribute("warningDelTrack", Settings.warningDelTrack),
                                new XAttribute("warningExistPlaylist", Settings.warningExistPlaylist)
                            ),
                            new XAttribute("musicAtTheStart", Settings.musicAtTheStart)
                        )
                    );
                    File.WriteAllText("Settings.xml", doc.ToString());
                }
                catch (Exception ex)
                {
                    exceptionMessage = ex.Message;
                    return false;
                }
            }
            else
            {
                try
                {
                    XDocument doc;
                    using (XmlReader xmlR = XmlReader.Create(settingsFileName))
                    {
                        doc = XDocument.Load(xmlR);
                        doc.Element("Player_Settings").Attribute("messagesActive").SetValue(messagesActive);
                        doc.Element("Player_Settings").Element("warnings").Attribute("warningDelPlaylist").SetValue(warningDelPlaylist);
                        doc.Element("Player_Settings").Element("warnings").Attribute("warningDelTrack").SetValue(warningDelTrack);
                        doc.Element("Player_Settings").Element("warnings").Attribute("warningExistPlaylist").SetValue(warningExistPlaylist);
                        doc.Element("Player_Settings").Attribute("musicAtTheStart").SetValue(musicAtTheStart);
                    }
                    using (XmlWriter writer = XmlWriter.Create(settingsFileName))
                    {
                        doc.Save(writer);
                    }
                }
                catch
                {
                    if(!DelSettingsFile())
                        return false;
                }
            }
            return true;
        }


        public bool DelSettingsFile()
        {
            try
            {
                File.Delete(settingsFileName);
                SaveSettings();
                return true;
            }
            catch (Exception ex)
            { exceptionMessage = ex.Message; return false; }
        }
    }
}
