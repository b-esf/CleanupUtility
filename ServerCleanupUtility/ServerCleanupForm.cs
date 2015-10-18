using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Xml;
//using System.Xml.XmlConfiguration;
using Microsoft.VisualBasic.FileIO;


namespace ServerCleanupUtility
{
    public partial class ServerCleanupForm : Form
    {   
        
        public ServerCleanupForm()
        {
            InitializeComponent();
            
            instBox.Text = "Click 'Archive' to backup files in directories specified in the configuration file." + Environment.NewLine +
                "Click 'Clean' to clean out backups older than 3 months.";
            
            string verified = VerifyDirs();

            if (verified.Length > 0)
            {
                Zip.Enabled = false;
                Delete.Enabled = false;
                instBox.Text = "Invalid or missing directories. Please recheck locations in configuration file and restart.";
                AddToLog(verified);
            }
            else
            {
                try
                {
                    SuspendLayout();
                    AddLinks();
                    //ButtonHandler(AddLinks(locs));
                }
                finally
                {
                    ResumeLayout();
                }
            }
        }

        #region Events

        /// <summary>
        /// Used for archiving the eligible files in each directory listed in app.config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zip_Click(object sender, EventArgs e)
        {
            AddToLog("ARCHIVING IN PROGRESS...");
            DirectoryInfo directory = null;
            
            List<CleanUpFolder> folders = GetFolders();
            
            if (folders.Count > 0)
            {
                for (int counter = 0; counter < folders.Count; counter++)
                {
                    if (folders[counter].Archive)
                    {
                        try
                        {
                            string errors = "";
                            directory = new DirectoryInfo(folders[counter].Location);

                            //File types/extensions that can be zipped.
                            string[] fileExtensions = new string[] { ".txt", ".xml", ".log", ".csv" };

                            if (GetFilesByExtensions(directory, fileExtensions).Length < 1)
                            {
                                AddToLog("Nothing to Compress in " + directory.FullName + ".");
                                continue;
                            }

                            FileInfo file = GetOldestFile(directory);
                            DateTime now = DateTime.Now;

                            int oldestMonth = file.LastWriteTime.Month;
                            int oldestYear = file.LastWriteTime.Year;
                            List<string> filesToZip;
                            FileInfo[] files = GetFilesByExtensions(directory, fileExtensions);

                            if (!folders[counter].Bimonthly)
                            {
                                #region RegularZip
                                for (int batchYear = oldestYear; batchYear <= now.Year; batchYear++)
                                {
                                    for (int batchMonth = 1; batchMonth <= 12; batchMonth++)
                                    {
                                        filesToZip = new List<string>();
                                        if (batchYear == now.Year && batchMonth == now.Month)
                                        { /*Do nothing if it's the current month.*/ }
                                        else
                                        {
                                            for (int i = 0; i < files.Length; i++)
                                            {
                                                if (files[i].LastWriteTime.Year == batchYear && files[i].LastWriteTime.Month == batchMonth)
                                                {
                                                    filesToZip.Add("\"" + files[i].FullName + "\"");
                                                }
                                            }
                                            if (filesToZip.Count > 0)
                                            {
                                                errors = "Compressing in " + directory.Parent + "\\" + directory.Name + " --> " +
                                                    RarFiles(directory.FullName + "\\rar files", filesToZip, batchMonth, batchYear, 0);

                                                if (errors.Length > 0)
                                                    AddToLog(errors);
                                                else
                                                    AddToLog("Nothing to Compress in " + directory.FullName + ".");
                                            }
                                        }
                                    }
                                }
                                #endregion RegularZip
                            }
                            else
                            {
                                #region BimonthlyZip
                                for (int batchYear = oldestYear; batchYear <= now.Year; batchYear++)
                                {
                                    for (int batchMonth = 1; batchMonth <= 12; batchMonth++)
                                    {
                                        filesToZip = new List<string>();
                                        if (batchYear == now.Year && batchMonth == now.Month && now.Day < 16)
                                        { /*Do nothing if it's the 1st half of the current month.*/ }
                                        else
                                        {
                                            //First half of the month.
                                            for (int i = 0; i < files.Length; i++)
                                            {
                                                if (files[i].LastWriteTime.Year == batchYear && files[i].LastWriteTime.Month == batchMonth &&
                                                    files[i].LastWriteTime.Day < 16)
                                                {
                                                    filesToZip.Add("\"" + files[i].FullName + "\"");
                                                }
                                            }
                                            if (filesToZip.Count > 0)
                                            {
                                                AddToLog("Compressing in " + directory.Parent + "\\" + directory.Name + " --> " +
                                                    RarFiles(directory.FullName + "\\rar files", filesToZip, batchMonth, batchYear, 1));
                                            }
                                            //Clear list before starting 2nd half of the month.
                                            filesToZip.Clear();

                                            //Second half of the month.
                                            for (int i = 0; i < files.Length; i++)
                                            {
                                                if (files[i].LastWriteTime.Year == batchYear && files[i].LastWriteTime.Month == batchMonth &&
                                                    files[i].LastWriteTime.Day > 15)
                                                {
                                                    filesToZip.Add("\"" + files[i].FullName + "\"");
                                                }
                                            }
                                            if (filesToZip.Count > 0)
                                            {
                                                errors = "Compressing in " + directory.Parent + "\\" + directory.Name + " --> " +
                                                    RarFiles(directory.FullName + "\\rar files", filesToZip, batchMonth, batchYear, 2);

                                                if (errors.Length > 0)
                                                    AddToLog(errors);
                                                else
                                                    AddToLog("Nothing to Compress in " + directory.FullName + ".");
                                            }
                                        }
                                    }
                                }
                                #endregion BimonthlyZip
                            }


                        }
                        catch (Exception ex)
                        {
                            AddToLog("Error in processing " + directory.FullName + ".");
                            AddToLog(ex.Message);
                        }
                    }
                }                
            }            
        }

        /// <summary>
        /// Used to delete archives older than 3 months (moved to recycle bin).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, EventArgs e)
        {
            AddToLog("CLEANING IN PROGRESS...");
            FileInfo oldestFile;
            DirectoryInfo current;
            DateTime limitDate = new DateTime();
            limitDate = limitDate.AddMonths(DateTime.Now.AddMonths(-3).Month);
            limitDate = limitDate.AddYears(DateTime.Now.Year - 1);            
            bool nothingToDelete = true;
            
            List<CleanUpFolder> folders = GetFolders();
            
            try
            {
                for (int i = 0; i < folders.Count; i++)
                {
                    if (folders[i].Clean)
                    {
                        current = new DirectoryInfo(folders[i].Location + "\\rar files");

                        if (current.Exists && current.GetFiles().Length > 0)
                        {
                            oldestFile = GetOldestFile(current);

                            while (oldestFile.LastWriteTime < limitDate)
                            {
                                //oldestFile.Delete() can't sent to recycle bin
                                FileSystem.DeleteFile(oldestFile.FullName, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                                AddToLog("Moving to Recycle Bin --> " + oldestFile.FullName);
                                nothingToDelete = false;
                                oldestFile = GetOldestFile(current);
                                if (oldestFile == null)
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddToLog(ex.Message);
            }

            if (nothingToDelete)
                AddToLog("Nothing to Delete.");
        }

        private void Verify_Click(object sender, EventArgs e)
        {
            string verified = VerifyDirs();
            if (verified.Length > 0)
            {
                Zip.Enabled = false;
                Delete.Enabled = false;
                instBox.Text = "Invalid or missing directories. Please recheck locations in configuration file." + NewLines(2) +
                    verified + NewLines(1);
            }
            else
            {
                instBox.Text = "Click Zip to backup files in directories specified in configuration file." + Environment.NewLine +
                "Click Delete to clean out backups older than 3 months." + Environment.NewLine + Environment.NewLine +
                "Current List of backup directories :" + Environment.NewLine;
                Zip.Enabled = true;
                Delete.Enabled = true;                
            }
        }

        /// <summary>
        /// Allows for clicking folder links and opening them in explorer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkClick(object sender, EventArgs e)
        {            
            //Button clicked = (Button)sender;
            LinkLabel clicked = (LinkLabel)sender;
            System.Diagnostics.Process.Start(clicked.Text);
            clicked.LinkVisited = true;
        }

        /// <summary>
        /// Used to allow the links at the top to be clickable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinkBox_MouseHover(object sender, EventArgs e)
        {
            linkBox.Focus();
        }

        /// <summary>
        /// Used for updating the config file values for the checkboxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, EventArgs e)
        {
            CheckBox clicked = (CheckBox)sender;
            bool boxChecked = clicked.Checked;

            //string key = clicked.Name.IndexOf("Clean") > -1 ? "Clean" : "Bimonthly";
            string key = "";
            if (clicked.Name.IndexOf("Clean") > -1)
            { key = "clean"; }
            else if (clicked.Name.IndexOf("Bimonthly") > -1)
            { key = "bimonthly"; }
            else if (clicked.Name.IndexOf("Archive") > -1)
            { key = "archive"; }

            UpdateXMLSetting(clicked.Name.Substring(clicked.Name.Length - 1), key, boxChecked.ToString());                       
        }
        #endregion Events
        
        #region Helpers

        #region GetOldestFile
        private FileInfo GetOldestFile(DirectoryInfo directory)
        {
            string location = directory.FullName;
            FileInfo oldestFile = null;
            
            try
            {
                if (!directory.Exists)
                    throw new ArgumentException();

                FileInfo[] files = directory.GetFiles();
                if (files.Length == 0)
                    return null;

                oldestFile = files[0];
                foreach (FileInfo file in files)
                {
                    if (file.LastWriteTime < oldestFile.LastWriteTime)
                        oldestFile = file;
                }
                        
            }
            catch (Exception e)
            {
                AddToLog("Finding file unsuccessful in directory " + location + Environment.NewLine + e.Message + Environment.NewLine);
            }
            return oldestFile;
        }
        #endregion GetOldestFile

        #region RarFiles
        private string RarFiles(string rarPackagePath, List<string> accFiles, int month, int year, int HalfOfMonth)
        {
            string archiveName = "";
            switch (HalfOfMonth)
            {
                case 0:
                    archiveName = year + "_" + month.ToString("00") + ".rar";
                    break;
                case 1:
                    archiveName = year + "_" + month.ToString("00") + "_01-15.rar";
                    break;
                case 2:
                    archiveName = year + "_" + month.ToString("00") + "_16-31.rar";
                    break;
            }
             
            string error = "";
            if (accFiles.Count > 0)
            {
                try
                {                                       
                    if (File.Exists(rarPackagePath + "\\" + archiveName))
                    {
                        throw new Exception(rarPackagePath + "\\" + archiveName + " already exists. Add files to existing archive or delete it and re-run this utility.");
                    }    

                    string[] files = new string[accFiles.Count];
                    int i = 0;
                    foreach (var fList_item in accFiles)
                    {
                        files[i] = fList_item.ToString();
                        i++;
                    }
                    string fileList = string.Join(Environment.NewLine, files);
                                        
                    StreamWriter cmdFileList = File.CreateText(rarPackagePath + "\\rarFileList.lst");                    
                    foreach (var cList_item in accFiles)
                    {
                        cmdFileList.WriteLine(cList_item.ToString());
                    }
                    cmdFileList.Flush();
                    cmdFileList.Dispose();
                    

                    //WinRAR <command> -<switch1> -<switchN> <archive> <files...> <@listfiles...> <path_to_extract\> 
                    //fileList += "\"";                    
                    System.Diagnostics.ProcessStartInfo sdp = new System.Diagnostics.ProcessStartInfo();

                    
                    //string cmdArgs = string.Format("A -ep -inul -dr {0} {1}",
                    //            String.Format("\"{0}\\" + year + "_" + month.ToString("00") + ".rar\"", rarPackagePath),
                    //            String.Format("\"@" + rarPackagePath + "\\rarFileList.lst\""));
                    string cmdArgs = string.Format("A -ep -inul -dr {0} {1}",
                                String.Format("\"{0}\\" + archiveName + "\"", rarPackagePath),
                                String.Format("\"@" + rarPackagePath + "\\rarFileList.lst\""));                    

                    //Winrar.exe path
                    if (File.Exists("C:\\Program Files\\WinRAR\\WinRAR.exe"))
                        sdp.FileName = "C:\\Program Files\\WinRAR\\WinRAR.exe";
                    else if (File.Exists("C:\\Program Files (x86)\\WinRAR"))
                        sdp.FileName = "C:\\Program Files (x86)\\WinRAR";                    
                    sdp.UseShellExecute = false;
                    sdp.Arguments = cmdArgs;
                    sdp.CreateNoWindow = false;
                    sdp.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    sdp.RedirectStandardOutput = true;
                    sdp.RedirectStandardError = true;
                    sdp.ErrorDialog = false;

                    System.Diagnostics.Process process = System.Diagnostics.Process.Start(sdp);
                                       
                    string errorOutput = process.StandardError.ReadToEnd();
                    string output = process.StandardOutput.ReadToEnd();

                    process.WaitForExit();
                    
                    if (process.ExitCode > 0)
                        throw new Exception(Environment.NewLine + output + " Exit Code:" + process.ExitCode);

                    process.Dispose();
                    error = archiveName + " successfully archived " + accFiles.Count + " file(s).";

                    File.Delete(rarPackagePath + "\\rarFileList.lst");
                }
                catch (Exception ex)
                {
                    File.Delete(rarPackagePath + "\\rarFileList.lst");
                    error = year + "_" + month.ToString("00") + ".rar - failed";
                    error += ex.Message;
                }
            }
            return error;
        }
        #endregion RarFiles

        #region NewLines
        private string NewLines(int enters)
        {
            string output = "";
            for (int i = 0; i < enters; i++)
                output += Environment.NewLine;
            return output;
        }
        #endregion NewLines

        /*
        #region GetLocs
        /// <summary>
        /// Used for all other needs other than the zip button event method, which only require location values.
        /// </summary>
        /// <returns></returns>
        private string[] GetLocs()
        {
            string[] temp = System.Configuration.ConfigurationManager.AppSettings.AllKeys;
            List<string> locs = new List<string>();
            string[] locations;

            try
            {
                foreach (string t in temp)
                {
                    if (t.StartsWith("Location"))
                        locs.Add(t);
                }
                locations = new string[locs.Count];
                for (int i = 0; i < locs.Count; i++)
                {
                    locations[i] = System.Configuration.ConfigurationManager.AppSettings[locs[i]];
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error in GetLocs().", ex);
            }

            return locations;
        }
        #endregion GetLocs
        #region GetLocs2
        /// <summary>
        /// Used for the zip button event method. Returns 2D array with both key and value.
        /// </summary>
        /// <returns></returns>
        private string[,] GetLocs2()
        {
            string[] temp = System.Configuration.ConfigurationManager.AppSettings.AllKeys;
            string[,] locKeyValue;
            try
            {
                IEnumerable<string> enumBM = temp.Where<string>(x => x.ToLower().IndexOf("locations") > -1);

                string[] arrLoc = enumBM.ToArray();
                locKeyValue = new string[arrLoc.Length, 2];
                for (int key = 0; key < arrLoc.Length; key++)
                {
                    locKeyValue[key, 0] = arrLoc[key];
                    locKeyValue[key, 1] = System.Configuration.ConfigurationManager.AppSettings[locKeyValue[key, 0]];
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetLocs2().", ex);
            }

            return locKeyValue;            
        }
        #endregion GetLocs2
        #region GetBimonthly
        /// <summary>
        /// 2D Array used for key/value pairs that help determine if the corresponding directory is backed up bimonthly.
        /// </summary>
        /// <returns></returns>
        private string[,] GetBimonthly()
        {
            string[] temp = System.Configuration.ConfigurationManager.AppSettings.AllKeys;
            string[,] biMonthlyKeyValue;

            try
            {
                IEnumerable<string> enumBM = temp.Where<string>(x => x.ToLower().IndexOf("bimonthly") > -1);

                string[] arrBM = enumBM.ToArray();
                biMonthlyKeyValue = new string[arrBM.Length, 2];
                for (int key = 0; key < arrBM.Length; key++)
                {
                    biMonthlyKeyValue[key, 0] = arrBM[key];
                    biMonthlyKeyValue[key, 1] = System.Configuration.ConfigurationManager.AppSettings[biMonthlyKeyValue[key, 0]];
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetBimonthly().", ex);
            }
            return biMonthlyKeyValue;
        }
        #endregion GetBimonthly        
        */
        
        #region GetFolders
        private List<CleanUpFolder> GetFolders()
        {
            XmlDocument xDoc = LoadXml();
            XmlNodeList tempLocs = xDoc.SelectNodes(@"//folder[starts-with(@name,'location')]");
            List<CleanUpFolder> cleanUpFolders = new List<CleanUpFolder>();

            try
            {
                foreach (XmlNode loc in tempLocs)
                {
                    string folderIndex = "";
                    Match match = Regex.Match(loc.Attributes["name"].Value, @"\d+");
                    if (match.Success)
                        folderIndex = (match.Value);
                            

                    if ( String.IsNullOrEmpty(GetXMLSetting(folderIndex, "path")) || String.IsNullOrEmpty(GetXMLSetting(folderIndex, "bimonthly")) ||
                        String.IsNullOrEmpty(GetXMLSetting(folderIndex, "clean")) || String.IsNullOrEmpty(GetXMLSetting(folderIndex, "archive")) )
                        throw new Exception("Missing/Invalid configuration for entry " + folderIndex + ".");

                    string location = GetXMLSetting(folderIndex, "path");
                    bool archive = Convert.ToBoolean(GetXMLSetting(folderIndex, "archive"));
                    bool bimonthly = Convert.ToBoolean(GetXMLSetting(folderIndex, "bimonthly"));
                    bool clean = Convert.ToBoolean(GetXMLSetting(folderIndex, "clean"));

                    CleanUpFolder temp = new CleanUpFolder(loc.Attributes["name"].Value, location, archive, bimonthly, clean);
                    cleanUpFolders.Add(temp);
                }
            }            
            catch (Exception ex)
            {
                AddToLog(ex.Message);
                throw new Exception(ex.Message);
            }
            return cleanUpFolders; 
        }
        #endregion GetFolders
        
        #region VerifyDirs
        private string VerifyDirs()
        {
            string verified = "";
            try
            {
                List<CleanUpFolder> folders = GetFolders();                
                foreach (CleanUpFolder folder in folders)
                {
                    if (!Directory.Exists(folder.Location))
                        verified += "Invalid directory - " + folder.Location + NewLines(1);
                }
            }
            catch (Exception ex)
            {
                verified = ex.Message;
            }
            return verified;
        }
        #endregion VerifyDirs

        #region AddLinks
        private void AddLinks()
        {
            List<CleanUpFolder> folders = GetFolders();
            for (int i = 0; i < folders.Count; i++)
            {
                //Get the correct index number.
                string index = "";
                Match match = Regex.Match(folders[i].Key, @"\d+");
                if (match.Success)
                    index = (match.Value);

                //Archive Checkbox
                CheckBox chkArchive = new CheckBox();
                chkArchive.Name = "chkArchive" + index;
                chkArchive.Text = "Archive";
                chkArchive.AutoSize = true;
                chkArchive.Checked = folders[i].Archive;
                chkArchive.Location = new Point(0, chkArchive.Height * i);
                chkArchive.Click += new System.EventHandler(CheckBox_Click);

                ToolTip archiveTooltip = new ToolTip();
                archiveTooltip.IsBalloon = true;
                archiveTooltip.AutomaticDelay = 0;
                archiveTooltip.InitialDelay = 0;
                archiveTooltip.AutoPopDelay = 32000;
                archiveTooltip.ShowAlways = true;
                archiveTooltip.SetToolTip(chkArchive, "If not checked, this entry will not be archived.");
                
                
                //BiMonthly Checkbox
                CheckBox chkBi = new CheckBox();
                chkBi.Name = "chkBimonthly" + index;
                chkBi.Text = "BiMonthly";                
                chkBi.AutoSize = true;                               
                chkBi.Checked = folders[i].Bimonthly;
                chkBi.Location = new Point(chkArchive.Location.X + chkArchive.Width, chkArchive.Height * i);
                chkBi.Click += new System.EventHandler(CheckBox_Click);

                ToolTip bmTooltip = new ToolTip();
                bmTooltip.IsBalloon = true;
                bmTooltip.AutomaticDelay = 0;
                bmTooltip.InitialDelay = 0;
                bmTooltip.AutoPopDelay = 32000;
                bmTooltip.ShowAlways = true;
                bmTooltip.SetToolTip(chkBi, "Archiving is done twice for each month, once for each of the 1st and 2nd halves.");

                //Archive Clean Up Checkbox
                CheckBox chkCl = new CheckBox();
                chkCl.Name = "chkClean" + index;
                chkCl.Text = "CleanArchives";
                chkCl.AutoSize = true;
                chkCl.Checked = folders[i].Clean;
                chkCl.Location = new Point(chkBi.Location.X + chkBi.Width, chkBi.Location.Y);
                chkCl.Click += new System.EventHandler(CheckBox_Click);

                ToolTip clTooltip = new ToolTip();
                clTooltip.IsBalloon = true;
                clTooltip.ShowAlways = true;
                clTooltip.AutomaticDelay = 0;
                clTooltip.InitialDelay = 0;
                clTooltip.AutoPopDelay = 32000;
                clTooltip.SetToolTip(chkCl, "Uncheck to not delete old archives for this directory during the cleaning process.");

                //Location LinkLabel
                var label = new LinkLabel();
                label.Text = folders[i].Location;
                label.Name = "llbDirectory" + index;
                label.AutoSize = true;
                label.Location = new Point(chkCl.Location.X + chkCl.Width + 20, chkBi.Location.Y);
                label.Click += new System.EventHandler(linkClick);
                label.VisitedLinkColor = Color.Purple;

                ToolTip lblTooltip = new ToolTip();
                lblTooltip.IsBalloon = true;
                lblTooltip.ShowAlways = true;
                lblTooltip.AutomaticDelay = 0;
                lblTooltip.InitialDelay = 0;
                lblTooltip.AutoPopDelay = 32000;
                lblTooltip.SetToolTip(label, label.Text);

                //Add Controls to Panel
                linkBox.Controls.Add(chkArchive);
                linkBox.Controls.Add(chkBi);
                linkBox.Controls.Add(chkCl);
                linkBox.Controls.Add(label);
            }
        }
        #endregion AddLinks

        #region GetFilesByExtension
        private FileInfo[] GetFilesByExtensions(DirectoryInfo dir, string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            IEnumerable<FileInfo> files = dir.EnumerateFiles();            

            files = files.Where(f => extensions.Contains(f.Extension));

            FileInfo[] returnFiles = files.ToArray<FileInfo>();

            return returnFiles;
        }
        #endregion GetFilesByExtension

        #region AddToLog
        /// <summary>
        /// Used for adding app messages to the logbox with timestamp and consistent formatting.
        /// </summary>
        /// <param name="msg"></param>
        private void AddToLog(string msg)
        {
            string timeStamp = DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00");
            logBox.AppendText(NewLines(1));
            logBox.AppendText(timeStamp + " - " + msg);
            logBox.AppendText(NewLines(1));
        }
        #endregion AddToLog

        #region AppConfig
        #region GetAppConfig
        private string GetAppConfig(string key)
        {
            var appSetting = ConfigurationManager.AppSettings;
            string result = appSetting[key] ?? "Not Found";
            return result;
        }
        #endregion GetAppConfig
        #region UpdateAppConfig
        private void UpdateAppConfig(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
        #endregion UpdateAppConfig
        #endregion AppConfig

        #region XML
        private XmlDocument LoadXml()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Directory.GetCurrentDirectory() + @"\folders.xml");
            return xDoc;
        }
        private string GetXMLSetting(string locIndex, string nodeName)
        {
            string value = "Not Found";
            XmlDocument xDoc = LoadXml();
            try
            {
                value = xDoc.SelectSingleNode(@"//folder[@name='location" + locIndex + "']/" + nodeName).InnerText ?? "Not Found";
            }
            catch (Exception ex)
            {
                AddToLog(ex.Message);
            }

            return value;
        }
        private void UpdateXMLSetting(string locIndex, string nodeName, string value)
        {
            XmlDocument xDoc = LoadXml();
            try
            {
                if (String.IsNullOrEmpty(xDoc.SelectSingleNode(@"folders/folder[@name='location" + locIndex + "']/" + nodeName).InnerText))
                {
                    XmlElement temp = xDoc.CreateElement(nodeName);
                    temp.InnerText = value;
                    xDoc.SelectSingleNode(@"folders/folder[@name=location" + locIndex + "]/" + nodeName).AppendChild(temp);
                }
                else
                {
                    xDoc.SelectSingleNode(@"folders/folder[@name='location" + locIndex + "']/" + nodeName).InnerText = value;
                }
                xDoc.Save(Directory.GetCurrentDirectory() + @"\folders.xml");
            }
            catch (Exception ex)
            {
                AddToLog(ex.Message); 
            }
        }
        #endregion XML

        #endregion Helpers

        #region CleanUpFolder Class
        private class CleanUpFolder
        {
            #region properties

            private string _key;
            public string Key
            {
                get { return _key; }
                set { this._key = value; }
            }

            private string _location;
            public string Location
            {
                get { return _location; }
                set { this._location = value; }
            }

            private bool _archive;
            public bool Archive
            {
                get { return _archive; }
                set { this._archive = value; }
            }

            private bool _bimonthly;
            public bool Bimonthly
            {
                get { return _bimonthly; }
                set { this._bimonthly = value; }
            }

            private bool _clean;
            public bool Clean
            {
                get { return _clean; }
                set { this._clean = value; }
            }
            #endregion properties
                        
            public CleanUpFolder(string keyVal, string locationVal, bool archiveVal, bool bimonthlyVal, bool cleanVal)
            {
                this.Key = keyVal;
                this.Location = locationVal;
                this.Archive = archiveVal;
                this.Bimonthly = bimonthlyVal;
                this.Clean = cleanVal;                
            }
        }
        #endregion CleanUpFolder Class

    }
}
